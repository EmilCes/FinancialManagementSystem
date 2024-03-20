using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialManagementSystem.Models;
using FinancialManagementSystem.Services.Authentication;
using FinancialManagementSystem.Services.Client;
using FinancialManagementSystem.Services.Client.Dto;
using Refit;

namespace FinancialManagementSystem.ViewModels;

public partial class ClientPageViewModel : ViewModelBase
{
    private readonly IClientService _clientService;
    private bool _isCheckboxActive = false;
    
    public ClientPageViewModel()
    {
        _clientService = new ClientService("http://localhost:8080/api/v1/client");
    }
    
    [RelayCommand]
    public async Task RegisterClientCommand()
    {
        var address = new Address()
        {
            Street = Street!,
            Neighborhood = Neighborhood,
            ExteriorNumber = ExteriorNumber,
            InteriorNumber = InteriorNumber,
            PostalCode = PostalCode,
            Municipality = Municipality,
            State = State
        };

        var workplace = new Workplace()
        {
            Name = WorkPlaceName,
            Email = WorkPlaceEmail,
            PhoneNumber = WorkPlacePhoneNumber,
            Rfc = WorkPlaceRfc
        };

        var paymentBankAccount = new BankAccount()
        {
            Clabe = PaymentClabe,
            BankName = PaymentBankName
        };
        
        var depositBankAccount = new BankAccount()
        {
            Clabe = DepositClabe,
            BankName = DepositBankName
        };

        var client = new Client()
        {
            Name = Name!,
            Lastname = Lastname,
            Surname = Surname,
            PhoneNumber = PhoneNumber,
            Email = Email,
            Rfc = Rfc,
            MonthlySalary = MonthlySalary,
            Address = address,
            Workplace = workplace,
            BankAccounts = [paymentBankAccount, depositBankAccount]
        };

        try
        {
            await _clientService.RegisterClientAsync(client);
            //TODO: Confirmation Message
        }
        catch (ApiException ex)
        {
            Console.WriteLine(ex.StatusCode);
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
    }
    
    [RelayCommand]
    public async Task VerifyClientExistenceCommand()
    {
        Console.WriteLine(Rfc);
        var request = new VerifyClientExistenceRequest()
        {
            clientRfc = Rfc
        };

        try
        {
            var response = await _clientService.VerifyClientExistenceAsync(request);
            
            //TODO: SHOW MESSAGE
            if (response.clientRegistered)
            {
                Console.WriteLine("Cliente ya registrado");
            }
            else
            {
                Console.WriteLine("Cliente aún no registrado");
            }
        }
        catch (ApiException ex)
        {
            Console.WriteLine(ex.StatusCode);
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
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
    
    [ObservableProperty] 
    [NotifyDataErrorInfo]
    [Required]
    private string? _name = string.Empty;
    [ObservableProperty] 
    private string _lastname;
    [ObservableProperty] 
    private string _surname;
    [ObservableProperty] 
    private string _dateOfBirth;
    [ObservableProperty] 
    private string _phoneNumber;
    [ObservableProperty] 
    private string _email;
    [ObservableProperty] 
    private string _rfc;
    [ObservableProperty] 
    private float _monthlySalary;
    
    [ObservableProperty] 
    [NotifyDataErrorInfo]
    [Required]
    private string? _street = string.Empty;
    [ObservableProperty] 
    private string _neighborhood;
    [ObservableProperty] 
    private int _exteriorNumber;
    [ObservableProperty] 
    private int _interiorNumber;
    [ObservableProperty] 
    private string _postalCode;
    [ObservableProperty] 
    private string _municipality;
    [ObservableProperty] 
    private string _state;
    
    [ObservableProperty] 
    private string _workPlaceName;
    [ObservableProperty] 
    private string _workPlacePhoneNumber;
    [ObservableProperty] 
    private string _workPlaceEmail;
    [ObservableProperty] 
    private string _workPlaceRfc;
    
    [ObservableProperty] 
    private string _paymentClabe;
    [ObservableProperty] 
    private string _paymentBankName;
    [ObservableProperty] 
    private string _depositClabe;
    [ObservableProperty] 
    private string _depositBankName;

}