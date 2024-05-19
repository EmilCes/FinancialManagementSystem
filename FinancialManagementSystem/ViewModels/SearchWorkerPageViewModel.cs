using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FinancialManagementSystem.Messages;
using FinancialManagementSystem.Models;
using FinancialManagementSystem.Models.Helpers;
using FinancialManagementSystem.Services.Worker;
using FinancialManagementSystem.ViewModels.Helpers;
using Refit;

namespace FinancialManagementSystem.ViewModels;

public partial class SearchWorkerPageViewModel: ViewModelBase
{
    private readonly IWorkerService _workerService;
    private readonly IMessenger _messenger = Message.Instance;
    private List<User> UserListCopy { get; set; } = new();

    public ObservableCollection<User> UserList { get; set; } = new();

    public SearchWorkerPageViewModel()
    {
        _workerService = new WorkerService("http://localhost:8080/api/v1/worker");
        LoadUsers();
    }

    private async Task LoadUsers()
    {
        try
        {
            List<User> result = await _workerService.GetAllUsersAsync();
            UserListCopy = result;
            
            FillObservableCollection(UserList, UserListCopy);
        }
        catch (ApiException e)
        {
            Console.WriteLine(e.ToString());
            DialogMessages.ShowApiExceptionMessage();
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e.Message);
            DialogMessages.ShowHttpRequestExceptionMessage();
        }
    }
    
    private void FillObservableCollection<T>(ObservableCollection<T> observableCollection, List<T> listToCopy)
    {
        observableCollection.Clear();
        
        foreach (var item in listToCopy)
        {
            observableCollection.Add(item);
        }
    }
    
    [RelayCommand]
    public void FilterUsersByRfc()
    {
        if (string.IsNullOrEmpty(Rfc))
        {
            FillObservableCollection(UserList, UserListCopy);
            return;
        }

        var filteredUsers = UserListCopy.Where(user => user.Rfc.Contains(Rfc, StringComparison.OrdinalIgnoreCase));

        var enumerable = filteredUsers.ToList();
        UserList.Clear();

        if (enumerable.Count == 0)
        {
            DialogMessages.ShowMessage("", "Trabajador no encontrado. Verifica el RFC ingresado.");
        }
        else
        {
            foreach (var client in enumerable.ToList())
            {
                UserList.Add(client);
            }
        }
    }
    
    [RelayCommand]
    public async Task ShowWorkerData(string rfc)
    {
        try
        {
            var response = await _workerService.GetUserAsync(rfc);
            response.Rfc = rfc;
            _messenger.Send(new ViewWorkerMessage(response));
        }
        catch (ApiException)
        {
            DialogMessages.ShowApiExceptionMessage();
        }
        catch (HttpRequestException)
        {
            DialogMessages.ShowHttpRequestExceptionMessage();
        }
    }
    
    [ObservableProperty] 
    private string _rfc;
}