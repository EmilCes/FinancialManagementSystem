using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FinancialManagementSystem.Messages;
using FinancialManagementSystem.Models;
using FinancialManagementSystem.Models.Helpers;
using FinancialManagementSystem.Services.Client;
using FinancialManagementSystem.Services.Client.Dto;
using FinancialManagementSystem.ViewModels.Helpers;
using MsBox.Avalonia.Models;
using Refit;

namespace FinancialManagementSystem.ViewModels;

public partial class ClientPageViewModel : ViewModelBase
{
    private readonly IClientService _clientService;
    private bool _isCheckboxActive;
    private Client _currentClient;
    private CreditApplication _creditApplication;
    private IMessenger _messenger = Message.Instance;

    [ObservableProperty] private bool _cancelReadOnly = true;
    [ObservableProperty] private bool _cancelNotReadOnly = false;
    [ObservableProperty] private bool _updateNotReadOnly = false;
    [ObservableProperty] private bool _modifyReadOnly = true;
    [ObservableProperty] private bool _registerReadOnly = true;
    [ObservableProperty] private bool _registerAplicationReadOnly = true;
    [ObservableProperty] private bool _btnReturnToValid;
    
    
    public ClientPageViewModel()
    {
        _clientService = new ClientService("http://localhost:8080/api/v1/client");
        BtnRegisterClient = true;
    }
    
    public ClientPageViewModel(Client client)
    {
        _clientService = new ClientService("http://localhost:8080/api/v1/client");
        _currentClient = client;
        LoadClientData();
        SetFormAsReadOnlyCommand();
    }
    
    public ClientPageViewModel(Client client, CreditApplication creditApplication)
    {
        _btnRegisterClient = false;
        _btnReturnToValid = true;
        _isFormReadOnly = true;
        _clientService = new ClientService("http://localhost:8080/api/v1/client");
        _currentClient = client;
        _creditApplication = creditApplication;
        HideAllButtons();
        BtnReturnToValid = true;
        LoadClientData();
        SetFormAsReadOnlyCommand();
        _cancelReadOnly = false;
        _cancelNotReadOnly = false;
        _updateNotReadOnly = false;
        _modifyReadOnly = false;
        _registerReadOnly = false;
        _registerAplicationReadOnly = false;
    }

    private void HideAllButtons()
    {
        BtnReturnToValid = false;
        IsFormReadOnly = false;
        ModificationModeEnable = true;

    }
    
    [RelayCommand]
    public async Task RegisterClientCommand()
    {
        if (!Validations.ValidateFields(this))
        {
            DialogMessages.ShowInvalidFieldsMessage();
            return;
        }
        
        var address = new Address
        {
            Street = Street!,
            Neighborhood = Neighborhood,
            ExteriorNumber = int.Parse(ExteriorNumber),
            InteriorNumber = !string.IsNullOrEmpty(InteriorNumber) ? int.Parse(InteriorNumber) : 0,
            PostalCode = PostalCode,
            Municipality = Municipality,
            State = State
        };

        var workplace = new Workplace
        {
            Name = WorkPlaceName,
            Email = WorkPlaceEmail,
            PhoneNumber = WorkPlacePhoneNumber,
            Rfc = WorkPlaceRfc
        };

        var paymentBankAccount = new BankAccount
        {
            Clabe = PaymentClabe,
            BankName = PaymentBankName
        };

        var depositBankAccount = new BankAccount
        {
            Clabe = DepositClabe,
            BankName = DepositBankName
        };

        var client = new Client
        {
            Name = Name!,
            Lastname = Lastname,
            Surname = Surname,
            PhoneNumber = PhoneNumber,
            DateOfBirth = ConvertToMySqlDateFormat(DateOfBirth),
            Email = Email,
            Rfc = Rfc,
            MonthlySalary = float.Parse(MonthlySalary),
            Address = address,
            Workplace = workplace,
            BankAccounts = [paymentBankAccount, depositBankAccount]
        };

        try
        {
            await _clientService.RegisterClientAsync(client);
            
            var response = await DialogMessages.ShowCustomMessage("Registro Exitoso", "El Cliente fue registrado correctamente.", [
                new() { Name = "Aceptar", },
                new() { Name = "Crear Solicitud", }
            ]);

            if (response!.Equals("Crear Solicitud"))
            {
                _messenger.Send(new CreateCreditApplication(Rfc));
            }

            if (response!.Equals("Aceptar"))
            {
                _messenger.Send(new ViewClientsMessage());
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
    
    [RelayCommand]
    public async Task UpdateClientCommand()
    {
        if (!Validations.ValidateFields(this))
        {
            DialogMessages.ShowInvalidFieldsMessage();
            return;
        }
        
        var address = new Address
        {
            AddressId = _currentClient.Address.AddressId,
            Street = Street!,
            Neighborhood = Neighborhood,
            ExteriorNumber = int.Parse(ExteriorNumber),
            InteriorNumber = !string.IsNullOrEmpty(InteriorNumber) ? int.Parse(InteriorNumber) : 0,
            PostalCode = PostalCode,
            Municipality = Municipality,
            State = State
        };

        var workplace = new Workplace
        {
            WorkplaceId =_currentClient.Workplace.WorkplaceId,
            Name = WorkPlaceName,
            Email = WorkPlaceEmail,
            PhoneNumber = WorkPlacePhoneNumber,
            Rfc = WorkPlaceRfc
        };

        var paymentBankAccount = new BankAccount
        {
            BankAccountId = _currentClient.BankAccounts[0].BankAccountId,
            Clabe = PaymentClabe,
            BankName = PaymentBankName
        };

        var depositBankAccount = new BankAccount
        {
            BankAccountId = _currentClient.BankAccounts[1].BankAccountId,
            Clabe = DepositClabe,
            BankName = DepositBankName
        };

        var client = new Client
        {
            ClientId = _currentClient.ClientId,
            Name = Name!,
            Lastname = Lastname,
            Surname = Surname,
            PhoneNumber = PhoneNumber,
            DateOfBirth = ConvertToMySqlDateFormat(DateOfBirth),
            Email = Email,
            Rfc = Rfc,
            MonthlySalary = float.Parse(MonthlySalary),
            Address = address,
            Workplace = workplace,
            BankAccounts = [paymentBankAccount, depositBankAccount]
        };

        try
        {
            await _clientService.UpdateClientAsync(_currentClient.ClientId, client);
            await DialogMessages.ShowCustomMessage("Modificación Exitosa", "Cliente modificado con éxito.", [
                new() { Name = "Aceptar", }
            ]);
            
            SetFormAsReadOnlyCommand();
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
    
    [RelayCommand]
    public async Task VerifyClientExistenceCommand()
    {
        var request = new VerifyClientExistenceRequest()
        {
            clientRfc = Rfc
        };

        try
        {
            var response = await _clientService.VerifyClientExistenceAsync(request);

            if (response.clientRegistered)
            {
                RegisterClientIsEnable = false;
                ClientRfcBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                RegisterClientIsEnable = true;
                ClientRfcBrush = new SolidColorBrush(Colors.Aqua);
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

    [RelayCommand]
    public void SameBankAccountCommand()
    {
        if (!_isCheckboxActive)
        {
            DepositBankName = PaymentBankName;
            DepositClabe = PaymentClabe;
            _isCheckboxActive = true;
        }
        else
        {
            DepositBankName = string.Empty;
            DepositClabe = string.Empty;
            _isCheckboxActive = false;
        }
    }

    [RelayCommand]
    public void SetFormAsModifiableCommand()
    {
        IsFormReadOnly = false;
        _cancelReadOnly = false;
        _cancelNotReadOnly = true;
        _updateNotReadOnly = false;
        _modifyReadOnly = false;
        _registerAplicationReadOnly = false;
        _registerReadOnly = true;
    }
    
    [RelayCommand]
    public void ReturnToValidationCommand()
    {
        _messenger.Send(new ViewCreditAplicationMessage(_creditApplication));
    }
    
    [RelayCommand]
    public void SetFormAsReadOnlyCommand()
    {
        IsFormReadOnly = true;
        _cancelReadOnly = true;
        _cancelNotReadOnly = false;
        _updateNotReadOnly = true;
        _modifyReadOnly = true;
        _registerReadOnly = false;
        ModificationModeEnable = true;
        _registerAplicationReadOnly = true;
    }
    
    [RelayCommand]
    public void CancelCommand()
    {
        _messenger.Send(new ViewClientsMessage());
    }

    [RelayCommand]
    public void RegisterCreditApplicationCommand()
    {
        _messenger.Send(new CreateCreditApplication(Rfc));
    }

    public void CancelRegistrationCommand()
    {
        _messenger.Send(new ViewClientsMessage());
    }
    
    private void LoadClientData()
    {
        var client = _currentClient;
        
        // Client Information
        Rfc = client.Rfc;
        Name = client.Name;
        Lastname = client.Lastname;
        Surname = client.Surname;
        PhoneNumber = client.PhoneNumber;
        Email = client.Email;
        MonthlySalary = client.MonthlySalary.ToString(CultureInfo.InvariantCulture);
        DateOfBirth = client.DateOfBirth;
        
        // Address Information
        Address address = client.Address;
        Street = address.Street;
        Neighborhood = address.Neighborhood;
        Municipality = address.Municipality;
        State = address.State;
        PostalCode = address.PostalCode;
        InteriorNumber = address.InteriorNumber.ToString();
        ExteriorNumber = address.ExteriorNumber.ToString();
        
        // Work Place
        Workplace workplace = client.Workplace;
        WorkPlaceName = workplace.Name;
        WorkPlaceEmail = workplace.Email;
        WorkPlaceRfc = workplace.Rfc;
        WorkPlacePhoneNumber = workplace.PhoneNumber;
        
        // Bank Account 1
        BankAccount bankAccount1 = client.BankAccounts[0];
        PaymentBankName = bankAccount1.BankName;
        PaymentClabe = bankAccount1.Clabe;
        
        // Bank Account 2
        BankAccount bankAccount2 = client.BankAccounts[1];
        DepositBankName = bankAccount2.BankName;
        DepositClabe = bankAccount2.Clabe;
    }
    
    private string ConvertToMySqlDateFormat(string dateString)
    {
        try
        {
            char[] separator = { ' ' };
            string[] parts = dateString.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            string datePart = parts[0];

            DateTime date = DateTime.ParseExact(datePart, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            string mysqlDateFormat = date.ToString("yyyy-MM-dd");

            return mysqlDateFormat;
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }

    [ObservableProperty] 
    private SolidColorBrush _clientRfcBrush;

    [ObservableProperty] 
    private bool _registerClientIsEnable;

    [ObservableProperty] 
    private bool _isFormReadOnly;
    
    [ObservableProperty] 
    private bool _btnRegisterClient
        ;
    
    [ObservableProperty] 
    private bool _modificationModeEnable;
    
    [ObservableProperty] 
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    private string? _name;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    private string _lastname;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    private string _surname;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    private string _dateOfBirth;
    
    [ObservableProperty] 
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    [Phone (ErrorMessage = ErrorMessages.PHONE_MESSAGE)]
    private string _phoneNumber;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    [EmailAddress (ErrorMessage = ErrorMessages.EMAIL_MESSAGE)]
    private string _email;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    [RegularExpression(@"^[A-Za-z]{4}\d{6}[A-Za-z\d]{3}$", ErrorMessage = ErrorMessages.RFC_MESSAGE)]
    private string _rfc;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = ErrorMessages.NUMERIC_FIELD_MESSAGE)]
    private string _monthlySalary;
    
    [ObservableProperty] 
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    private string? _street = string.Empty;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    private string _neighborhood;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    [RegularExpression(@"^\d+$", ErrorMessage = ErrorMessages.NUMERIC_FIELD_MESSAGE)]
    private string _exteriorNumber;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RegularExpression(@"^\d+$", ErrorMessage = ErrorMessages.NUMERIC_FIELD_MESSAGE)]
    private string _interiorNumber = String.Empty;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    [RegularExpression(@"^\d+$", ErrorMessage = ErrorMessages.NUMERIC_FIELD_MESSAGE)]
    private string _postalCode;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    private string _municipality;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    private string _state;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    private string _workPlaceName;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    [Phone (ErrorMessage = ErrorMessages.PHONE_MESSAGE)]
    private string _workPlacePhoneNumber;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    [EmailAddress (ErrorMessage = ErrorMessages.EMAIL_MESSAGE)]
    private string _workPlaceEmail;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    [RegularExpression(@"^[A-Za-z]{4}\d{6}[A-Za-z\d]{3}$", ErrorMessage = ErrorMessages.RFC_MESSAGE)]
    private string _workPlaceRfc;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    [RegularExpression(@"^\d+$", ErrorMessage = ErrorMessages.NUMERIC_FIELD_MESSAGE)]
    private string _paymentClabe;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    private string _paymentBankName;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    [RegularExpression(@"^\d+$", ErrorMessage = ErrorMessages.NUMERIC_FIELD_MESSAGE)]
    private string _depositClabe;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    private string _depositBankName;

}