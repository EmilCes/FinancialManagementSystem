using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FinancialManagementSystem.Messages;
using FinancialManagementSystem.Models;
using FinancialManagementSystem.Models.Helpers;
using FinancialManagementSystem.Services.Client;
using FinancialManagementSystem.Services.CreditApplication;
using FinancialManagementSystem.Services.CreditType;
using Refit;
using HttpRequestException = System.Net.Http.HttpRequestException;

namespace FinancialManagementSystem.ViewModels;

public partial class CreditApplicationViewModel : ViewModelBase
{
    private readonly ICreditApplicationService _creditApplicationService;
    private readonly ICreditTypeService _creditTypeService;
    private readonly IClientService _clientService;

    private const string FILE_SELECTED = "Seleccionado";

    private byte[] _identificationDocument = null;
    private byte[] _proofOfIncome = null;
    private byte[] _proofOfAddress = null;

    private Client clientToValidte;
    
    public ObservableCollection<Politic> Politics { get; }
    
    private CreditApplication creditAplicationValidation;
    
    private readonly IMessenger _messenger = Message.Instance;

    public CreditApplicationViewModel()
    {
        _creditApplicationService = new CreditApplicationService("http://localhost:8080/api/v1/creditApplication");
        _creditTypeService = new CreditTypeService("http://localhost:8080/api/v1/credit-type");
        SetLabels();
        SetCreditTypes();
    }
    
    public CreditApplicationViewModel(CreditApplication creditApplication)
    {
        _creditApplicationService = new CreditApplicationService("http://localhost:8080/api/v1/creditApplication");
        _creditTypeService = new CreditTypeService("http://localhost:8080/api/v1/credit-type");
        
        _gridsAreEnabledValidation = false;
        _infoClienteVisibility = true;
        _btnRegisterVisibility = false;

        Politics = new ObservableCollection<Politic>();

        foreach (Politic politic in creditApplication.SelectedCredit.Politics)
        {
            Politics.Add(politic);
        }
        
        SetCreditTypes();
        SelectedCredit = creditApplication.SelectedCredit;

        Rfc = creditApplication.CreditApplicant.Rfc;

        clientToValidte = creditApplication.CreditApplicant;

    }
    

    [RelayCommand]
    public async void SeeInfoClientCommand()
    {
        _messenger.Send(new ViewClientMessage(clientToValidte));
    }

    private void SetLabels()
    {
        string noFile = "Sin archivo";
        LblIdentificationDocument = noFile;
        LblProofOfAddressDocument = noFile;
        LblProofOfIncomeDocument = noFile;
        GridsAreEnabled = false;
        DisableColor = new SolidColorBrush(Colors.DarkGray);
    }

    private async void SetCreditTypes()
    {
        try
        {
            List<CreditType> creditTypes =  await _creditTypeService.GetCreditsTypeAsync();
            
            foreach (CreditType creditType in creditTypes)
            {
                CreditTypes.Add(creditType);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message + e.StackTrace);
        }
    }

    
    private bool ValidateFields()
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(this);
        return Validator.TryValidateObject(this, validationContext, validationResults, true);
    }
    
    [RelayCommand]
    public async Task ClientApplicateForCreditCommand()
    {

        if (!ValidateFields())
        {
            DialogMessages.ShowInvalidFieldsMessage();
            return;
        }
        
        CreditApplicationRequest creditApplicationRequest = new CreditApplicationRequest();
        Reference referenceOne = new Reference();
        Reference referenceTwo = new Reference();
        CreditType creditType = SelectedCredit;
        
        referenceOne.Name = NameReferenceOne;
        referenceOne.FirstLastname = FirstLastnameReferenceOne;
        referenceOne.SecondLastname = SecondLastnameReferenceOne;
        referenceOne.Telephone = PhoneReferenceOne;

        referenceTwo.Name = NameReferenceTwo;
        referenceTwo.FirstLastname = FirstLastnameReferenceTwo;
        referenceTwo.SecondLastname = SecondLastnameReferenceTwo;
        referenceTwo.Telephone = PhoneReferenceTwo;

        creditApplicationRequest.ClientRfc = Rfc;
        creditApplicationRequest.FirstReference = referenceOne;
        creditApplicationRequest.SecondReference = referenceTwo;
        creditApplicationRequest.SelectedCredit = creditType;

        if (_identificationDocument != null && _proofOfAddress != null && _proofOfIncome != null)
        {
            creditApplicationRequest.IdentificationPdf = _identificationDocument;
            creditApplicationRequest.ProofOfAddressPdf = _proofOfAddress;
            creditApplicationRequest.ProofOfIncomePdf = _proofOfIncome;
            
            try
            {
                await _creditApplicationService.CreateAplicationAsync(creditApplicationRequest);
                DialogMessages.ShowMessage("Registro Exitoso", "El Cliente fue registrado correctamente.");
            }
            catch (ApiException e)
            {
                DialogMessages.ShowApiExceptionMessage();
            }
            catch (HttpRequestException e)
            {
                DialogMessages.ShowHttpRequestExceptionMessage();
            }
        }
        else
        {
            DialogMessages.ShowMessage("Faltan archivos.", "Seleccione todos los archivos a subir.");
        }
    }
    
    [RelayCommand]
    public async Task SearchClientCommand()
    {
        string rfc = Rfc;
        
        try
        {
            VerifyRegularClientResponse response = await _creditApplicationService.VerifyRegularAsync(rfc);
            
            if (response.clientIsRegular)
            {
                RfcBrush = new SolidColorBrush(Colors.Green);
                GridsAreEnabled = true;
                DisableColor = new SolidColorBrush(Colors.Transparent);
            }
            else
            {
                RfcBrush = new SolidColorBrush(Colors.MediumOrchid);
                GridsAreEnabled = false;
                DisableColor = new SolidColorBrush(Colors.DarkGray);
                DialogMessages.ShowMessage("Cliente invalido", "El cliente ya tiene un crédito activo.");
            }
        }
        catch (ApiException e)
        {
            DialogMessages.ShowApiExceptionMessage();
            GridsAreEnabled = false;
            DisableColor = new SolidColorBrush(Colors.DarkGray);
        }
        catch (HttpRequestException e)
        {
            DialogMessages.ShowHttpRequestExceptionMessage();
            GridsAreEnabled = false;
            DisableColor = new SolidColorBrush(Colors.DarkGray);
        }
    }

    [RelayCommand]
    public async void AddIdentificationDocumentCommand()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();

        ConfigureFileDialog(openFileDialog);
        
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            string[] directory = await openFileDialog.ShowAsync(desktop.MainWindow);

            if (directory.Length > 0)
            {
                Console.WriteLine(directory[0]);
                _identificationDocument = ReadPDFFileToBytes(directory[0]);
                LblIdentificationDocument = FILE_SELECTED;
                
            }
        }
    }

    [RelayCommand]
    public async void AddProofOfIncomeCommand()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();

        ConfigureFileDialog(openFileDialog);
        
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            string[] directory = await openFileDialog.ShowAsync(desktop.MainWindow);

            if (directory.Length > 0)
            {
                Console.WriteLine(directory[0]);
                _proofOfIncome = ReadPDFFileToBytes(directory[0]);
                LblProofOfIncomeDocument = FILE_SELECTED;
            }
        }
    }

    [RelayCommand]
    public async void AddProofOfAddressCommand()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();

        ConfigureFileDialog(openFileDialog);
        
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            string[] directory = await openFileDialog.ShowAsync(desktop.MainWindow);

            if (directory.Length > 0)
            {
                Console.WriteLine(directory[0]);
                _proofOfAddress = ReadPDFFileToBytes(directory[0]);
                LblProofOfAddressDocument = FILE_SELECTED;
            }
        }
    }
    
    private void ConfigureFileDialog(OpenFileDialog openFileDialog)
    {
        openFileDialog.AllowMultiple = false;
        openFileDialog.Filters.Add(new FileDialogFilter { Name = "Archivos PDF", Extensions = { "pdf" } });
        openFileDialog.Title = "Seleccionar archivo PDF";
    }
    
    public byte[] ReadPDFFileToBytes(string filePath)
    {
        byte[] pdfBytes;
        try
        {
            pdfBytes = File.ReadAllBytes(filePath);
            return pdfBytes;
        }
        catch (IOException ex)
        {
            //TODO usar logger y quitar el WriteLine.
            Console.WriteLine("Error al leer el archivo PDF: " + ex.Message);
            return null;
        }
    }
    
    [RelayCommand]
    public void CancelCommand()
    {
        
    }
    
    [ObservableProperty] private string _lblIdentificationDocument;
    [ObservableProperty] private string _lblProofOfAddressDocument;
    [ObservableProperty] private string _lblProofOfIncomeDocument;
    [ObservableProperty] private bool _gridsAreEnabled;
    [ObservableProperty] private bool _gridsAreEnabledValidation = true;
    [ObservableProperty] private bool _infoClienteVisibility = false;
    [ObservableProperty] private bool _btnRegisterVisibility = true;


    [ObservableProperty] private SolidColorBrush _rfcBrush;
    [ObservableProperty] private SolidColorBrush _disableColor;

    [ObservableProperty] 
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    [RegularExpression(@"^[A-Za-z]{4}\d{6}[A-Za-z\d]{3}$", ErrorMessage = ErrorMessages.RFC_MESSAGE)]
    private string _rfc;
    
    [ObservableProperty] 
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)] 
    private string _nameReferenceOne;
    
    [ObservableProperty] 
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    private string _firstLastnameReferenceOne;
    
    [ObservableProperty] 
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    private string _secondLastnameReferenceOne;
    
    [ObservableProperty] 
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)] 
    private string _phoneReferenceOne;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)] 
    private string _nameReferenceTwo;
    
    [ObservableProperty] 
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)] 
    private string _firstLastnameReferenceTwo;
    
    [ObservableProperty] 
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)] 
    private string _secondLastnameReferenceTwo;
    
    [ObservableProperty] 
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)] 
    private string _phoneReferenceTwo;
    
    [ObservableProperty] 
    private ObservableCollection<CreditType> _creditTypes = new ObservableCollection<Models.CreditType>();

    [ObservableProperty] 
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)] 
    private CreditType _selectedCredit;
    
}