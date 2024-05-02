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
using FinancialManagementSystem.Models.Helpers;
using FinancialManagementSystem.Services.Worker;
using FinancialManagementSystem.Services.Worker.Dto;
using Refit;

namespace FinancialManagementSystem.ViewModels;

public partial class EmployeeRegistrationViewModel: ViewModelBase
{
    public ObservableCollection<Role> Roles { get; set; }
    private readonly IWorkerService _workerService;
    
    private bool _validRoles;


    public EmployeeRegistrationViewModel()
    {
        _workerService = new WorkerService("http://localhost:8080/api/v1/worker");
        Roles = new ObservableCollection<Role>();
        
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            FillRoles();
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

    [RelayCommand]
    public void CleanFieldsCommand()
    {
        Email = "";
        UserNumber = "";
        Name = "";
        Lastname = "";
        SecondLastname = "";
        Rfc = "";
        
    }
    
    [RelayCommand]
    public async Task RegisterUserCommand()
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
            bool rfcExist = await _workerService.RfcExistAsync(Rfc);
            bool userNumberExist = await _workerService.UserNumberExistAsync(UserNumber);
            bool availableToCreate = true;
            
            if (rfcExist)
            {
                DialogMessages.ShowMessage("RFC", "El RFC que ingreso ya existe.");
                availableToCreate = false;
            }

            if (userNumberExist)
            {
                DialogMessages.ShowMessage("Número de usuario", "El número de usuario que ingreso ya existe.");
                availableToCreate = false;
            }

            if (availableToCreate)
            {
                var request = new RegisterRequest()
                {
                    Email = Email,
                    FirstName = Name,
                    LastName = Lastname + " " + SecondLastname,
                    Rfc = Rfc,
                    Role = role[0].Name,
                    UserNumber = UserNumber
                };

                await _workerService.RegisterWorkerAsync(request);
                
                DialogMessages.ShowMessage("Registro", "El registro fue exitoso!");
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
    
}