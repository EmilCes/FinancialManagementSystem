using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FinancialManagementSystem.Messages;
using FinancialManagementSystem.Models;
using FinancialManagementSystem.Models.Helpers;
using FinancialManagementSystem.Services.Worker;
using FinancialManagementSystem.Services.Worker.Dto;
using Refit;

namespace FinancialManagementSystem.ViewModels;

public partial class EmployeeModificationPageViewModel: ViewModelBase
{
    public ObservableCollection<Role> Roles { get; set; }
    private readonly IWorkerService _workerService;
    private readonly IMessenger _messenger = Message.Instance;
    
    private bool _validRoles;


    public EmployeeModificationPageViewModel(string rfc)
    {
        _workerService = new WorkerService("http://localhost:8080/api/v1/worker");
        Roles = new ObservableCollection<Role>();
        
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            FillRoles();
            FillWorker(rfc);
        });
        
    }

    private void FillRoles()
    {
        Role user = new Role("USER", "Usuario");
        Role admin = new Role("ADMIN", "Administrador");
        Role moneyAnalist = new Role("ANALISTA_COBRO", "Analista de Cobro");
        Role creditAnalist = new Role("ANALISTA_CREDITO", "Analista de Crédito");
        Role creditAsesor = new Role("ASESOR_CREDITO", "Asesor de Crédito");
        Role manager = new Role("MANAGER", "Gerente");
        
        Roles.Add(user);
        Roles.Add(admin);
        Roles.Add(moneyAnalist);
        Roles.Add(creditAnalist);
        Roles.Add(creditAsesor);
        Roles.Add(manager);
    }

    private async void FillWorker(string rfc)
    {
        try
        {
            User user = await _workerService.GetUserAsync(rfc);
            Name = user.Firstname;
            int spaceIndex = user.Lastname.IndexOf(' ');

            if (spaceIndex != -1)
            {
                string firstName = user.Lastname.Substring(0, spaceIndex);
                string secondName = user.Lastname.Substring(spaceIndex + 1);
                Lastname = firstName;
                SecondLastname = secondName;
            }
            else
            {
                Lastname = user.Lastname;
            }
            
            Rfc = user.Rfc;
            UserNumber = user.UserNumber;
            Email = user.Email;
            RfcEnabled = false;
            WorkerNumberEnabled = false;
        }catch (ApiException)
        {
            DialogMessages.ShowApiExceptionMessage();
        }
        catch (HttpRequestException)
        {
            DialogMessages.ShowHttpRequestExceptionMessage();
        }
    }

    [RelayCommand]
    public void GoBack()
    {
        IMessenger messenger = Message.Instance;
        messenger.Send(new SearchWorkerMessage());
    }
    
    [RelayCommand]
    public void DeleteWorker()
    {
        try
        {
            _workerService.DeleteUserAsync(Rfc);
        }catch (ApiException)
        {
            DialogMessages.ShowApiExceptionMessage();
        }
        catch (HttpRequestException)
        {
            DialogMessages.ShowHttpRequestExceptionMessage();
        }
        
        DialogMessages.ShowMessage("Borrado", "El trabajador ha sido eliminado con éxito.");
        _messenger.Send(new SearchWorkerMessage());
    }
    
    [RelayCommand]
    public async Task ModifyUserCommand()
    {
        var role = GetSelectedRole();
        
        if (!Validations.ValidateFields(this))
        {
            DialogMessages.ShowInvalidFieldsMessage();
            return;
        }
        
        if (!_validRoles)
        {
            DialogMessages.ShowMessage("Datos incorrectos", "Se debe seleccionar solo un rol.");
            return;
        }

        try
        {
            bool availableToCreate = true;
            

            if (availableToCreate)
            {
                var request = new ModifyRequest()
                {
                    Email = Email,
                    Firstname = Name,
                    Lastname = Lastname + " " + SecondLastname,
                    Rfc = Rfc,
                    Role = role[0].Name,
                    UserNumber = UserNumber
                };

                await _workerService.ModifyAsync(request);
                
                DialogMessages.ShowMessage("Modificación", "La modificación del usuario fue exitosa!");
                _messenger.Send(new SearchWorkerMessage());
            }
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
    
    private List<Role> GetSelectedRole()
    {
        List<Role> role = Roles.Where(politic => politic.CbRoleState).ToList();
        
        _validRoles = (role.Count == 1);

        return role;
    }
    
    [ObservableProperty] 
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)] 
    [EmailAddress (ErrorMessage = ErrorMessages.EMAIL_MESSAGE)]
    private string _email;
    
    [ObservableProperty] 
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)] 
    private string _name;
    
    [ObservableProperty] 
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    private string _lastname;
    
    [ObservableProperty] 
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)] 
    private string _secondLastname;
    
    [ObservableProperty] 
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)] 
    [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = ErrorMessages.NUMERIC_FIELD_MESSAGE)]
    private string _userNumber;
    
    [ObservableProperty] 
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    [RegularExpression(@"^[A-Za-z]{4}\d{6}[A-Za-z\d]{3}$", ErrorMessage = ErrorMessages.RFC_MESSAGE)]
    private string _rfc;

    [ObservableProperty] 
    private bool _rfcEnabled;

    [ObservableProperty]
    private bool _workerNumberEnabled;

}