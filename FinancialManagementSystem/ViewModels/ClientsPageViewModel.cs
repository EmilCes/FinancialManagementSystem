using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FinancialManagementSystem.Messages;
using FinancialManagementSystem.Models;
using FinancialManagementSystem.Models.Helpers;
using FinancialManagementSystem.Services.Client;
using Refit;

namespace FinancialManagementSystem.ViewModels;

public partial class ClientsPageViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _name;
    [ObservableProperty]
    private string _rfc;
    [ObservableProperty]
    private string _state;
    
    private readonly IClientService _clientService;
    private readonly IMessenger _messenger = Message.Instance;
    
    public ObservableCollection<Client> ClientsList { get; set; } = new();
    private List<Client> ClientsListCopy { get; set; } = new();
    

    
    public ClientsPageViewModel()
    {
        _clientService = new ClientService("http://localhost:8080/api/v1/client");
        Initialize();
    }

    private async void Initialize()
    {
        await LoadCommand();
    }

    private async Task LoadCommand()
    {
        try
        {
            List<Client> result = await _clientService.GetClientsAsync();
            ClientsListCopy = result;
            
            FillObservableCollection(ClientsList, result);
        }
        catch (ApiException ex)
        {
            Console.WriteLine(ex.StatusCode);
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
    }

    [RelayCommand]
    public void FilterClientsByRfc()
    {
        if (string.IsNullOrEmpty(Rfc))
        {
            FillObservableCollection(ClientsList, ClientsListCopy);
            return;
        }

        var filteredClients = ClientsListCopy.Where(client => client.Rfc.Contains(Rfc, StringComparison.OrdinalIgnoreCase));

        var enumerable = filteredClients.ToList();
        ClientsList.Clear();

        if (enumerable.Count == 0)
        {
            DialogMessages.ShowMessage("", "Cliente no encontrado. Verifica el RFC ingresado.");
        }
        else
        {
            foreach (var client in enumerable.ToList())
            {
                ClientsList.Add(client);
            }
        }
    }

    [RelayCommand]
    public async Task ShowClientData(string clientId)
    {
        try
        {
            var id = int.Parse(clientId);
            var response = await _clientService.GetClientByIdAsync(id);
            response.ClientId = id;
            _messenger.Send(new ViewClientMessage(response));
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

    private void FillObservableCollection<T>(ObservableCollection<T> observableCollection, List<T> listToCopy)
    {
        observableCollection.Clear();
        
        foreach (var item in listToCopy)
        {
            observableCollection.Add(item);
        }
    }
    
}