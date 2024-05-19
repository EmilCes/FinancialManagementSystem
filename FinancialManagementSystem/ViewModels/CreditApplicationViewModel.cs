using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FinancialManagementSystem.Messages;
using FinancialManagementSystem.Models;
using FinancialManagementSystem.Models.Helpers;
using FinancialManagementSystem.Services.Credit;
using FinancialManagementSystem.Services.CreditApplication;
using FinancialManagementSystem.Services.CreditType;
using FinancialManagementSystem.ViewModels.Helpers;
using Refit;
using HttpRequestException = System.Net.Http.HttpRequestException;

namespace FinancialManagementSystem.ViewModels;

public partial class CreditApplicationViewModel : ViewModelBase
{
    private readonly ICreditApplicationService _creditApplicationService;
    private readonly ICreditTypeService _creditTypeService;
    private readonly ICreditService _creditService;


    private const string FILE_SELECTED = "Seleccionado";

    private byte[] _identificationDocument = null;
    private byte[] _proofOfIncome = null;
    private byte[] _proofOfAddress = null;

    private Client clientToValidte;
    private int creditAplicationIdToValidate;
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
    
    public CreditApplicationViewModel(string clientRfc)
    {
        _creditApplicationService = new CreditApplicationService("http://localhost:8080/api/v1/creditApplication");
        _creditTypeService = new CreditTypeService("http://localhost:8080/api/v1/credit-type");
        SetLabels();
        SetCreditTypes();
        SetClient(clientRfc);
    }
    
    public CreditApplicationViewModel(CreditApplication creditApplication)
    {
        _creditApplicationService = new CreditApplicationService("http://localhost:8080/api/v1/creditApplication");
        _creditTypeService = new CreditTypeService("http://localhost:8080/api/v1/credit-type");
        _creditService = new CreditService("http://localhost:8080/api/v1/credit");

        creditAplicationValidation = creditApplication;
        _gridsAreEnabledValidation = false;
        _infoClienteVisibility = true;
        _btnRegisterVisibility = false;
        _isBorderVisible = true;
        _titleBinding = "Validar Solicitud de Crédito";

        Politics = new ObservableCollection<Politic>();

        foreach (Politic politic in creditApplication.SelectedCredit.Politics)
        {
            Politics.Add(politic);
        }

        creditAplicationIdToValidate = creditApplication.CreditApplicationId;
        
        SetCreditTypes();
        SelectedCredit = creditApplication.SelectedCredit;

        Rfc = creditApplication.CreditApplicant.Rfc;

        clientToValidte = creditApplication.CreditApplicant;
        _btnRegisterVisibility = false;
        NameReferenceOne = creditApplication.References[0].Name;
        FirstLastnameReferenceOne = creditApplication.References[0].FirstLastname;
        SecondLastnameReferenceOne = creditApplication.References[0].SecondLastname;
        PhoneReferenceOne = creditApplication.References[0].PhoneNumber;

        NameReferenceTwo = creditApplication.References[1].Name;
        FirstLastnameReferenceTwo = creditApplication.References[1].FirstLastname;
        SecondLastnameReferenceTwo  = creditApplication.References[1].SecondLastname;
        PhoneReferenceTwo = creditApplication.References[1].PhoneNumber;
        UploadDocs = false;

    }
    
    [RelayCommand]
    public async Task DownloadDocsCommand()
    {
        if (creditAplicationValidation == null) return;

        var saveDialog = new SaveFileDialog
        {
            DefaultExtension = "pdf",
            Filters = new List<FileDialogFilter> { new FileDialogFilter { Name = "PDF Files", Extensions = { "pdf" } } },
            Title = "Save PDF Document"
        };

        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            saveDialog.InitialFileName = "Identificacion.pdf";
            var identificationFile = await saveDialog.ShowAsync(desktop.MainWindow);
            if (identificationFile != null)
            {
                await SaveFileAsync(identificationFile, creditAplicationValidation.IdentificationPdf);
            }

            saveDialog.InitialFileName = "Comprobante de Ingresos.pdf";
            var proofOfIncomeFile = await saveDialog.ShowAsync(desktop.MainWindow);
            if (proofOfIncomeFile != null)
            {
                await SaveFileAsync(proofOfIncomeFile, creditAplicationValidation.ProofOfIncomePdf);
            }

            saveDialog.InitialFileName = "Comprobante de Dirección.pdf";
            var proofOfAddressFile = await saveDialog.ShowAsync(desktop.MainWindow);
            if (proofOfAddressFile != null)
            {
                await SaveFileAsync(proofOfAddressFile, creditAplicationValidation.ProofOfAddressPdf);
            }
        }
    }

    private async Task SaveFileAsync(string filePath, byte[] fileData)
    {
        if (fileData == null) return;

        try
        {
            await File.WriteAllBytesAsync(filePath, fileData);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving file: {ex.Message}");
        }
    }
    
    [RelayCommand]
    public async void SeeInfoClientCommand()
    {
        ClientAndCredit clientAndCredit = new ClientAndCredit
        {
            CreditApplication = creditAplicationValidation,
            Client = clientToValidte
        };

        _messenger.Send(new ViewClientMessageFromValidation(clientAndCredit));
    }
    
    [RelayCommand]
    public async Task SendValidationCommand()
    {
        List<Politic> politics = Politics.Where(politic => politic.cbPoliticInvalid).ToList();
        List<Politic> politicsToValidate = Politics.ToList();
        bool validPolitics = true;
        
        foreach (Politic politic in politicsToValidate)
        {
            if (!politic.cbPoliticInvalid)
            {
                if (!politic.cbPoliticValid)
                {
                    validPolitics = false;
                }
            }
        }

        if (validPolitics)
        {
            List<String> comments = new List<string>();
        
            foreach (Politic politic in politics)
            {
                comments.Add(politic.comment);
                Console.WriteLine("Politica: " + politic.politicId);
            }
        
            Employee employee = Employee.Instance;
            Politic politic1 = new Politic();
            politic1.politicId = 1;
        
            Politic politic2 = new Politic();
            politic1.politicId = 2;
        
            List<Politic> politicsTo = new List<Politic>();
            politicsTo.Add(politic1);
            politicsTo.Add(politic2);
            string comment = String.Empty;

            foreach (string singleComment in comments)
            {
                comment = comment + singleComment;
            }
            try
            {
                ICreditService.ValidateCreditApplicationRequest request = new ICreditService.ValidateCreditApplicationRequest()
                {
                    RejectedPolicies = politics,
                    Comments = comment,
                    UserId = employee.EmployeeId,
                    CreditApplicationId = creditAplicationIdToValidate
                };
            
                await _creditService.ValidateCreditApplicationAsync(request);
            
                DialogMessages.ShowMessage("Validación Exitosa", "La solicitud fue validada correctamente.");

            }
            catch (ApiException e)
            {
                DialogMessages.ShowApiExceptionMessage();
                Console.WriteLine(e.Message);

                Console.WriteLine(e.StackTrace);
            }
            catch (HttpRequestException e)
            {
                DialogMessages.ShowHttpRequestExceptionMessage();
                Console.WriteLine(e.Message);

                Console.WriteLine(e.StackTrace);

            }
        }
        else
        {
            DialogMessages.ShowMessage("Faltan políticas por validar", "Faltan políticas por validar");
        }
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
        catch (ApiException)
        {
            DialogMessages.ShowApiExceptionMessage();
            GridsAreEnabled = false;
            DisableColor = new SolidColorBrush(Colors.DarkGray);
        }
        catch (HttpRequestException)
        {
            DialogMessages.ShowHttpRequestExceptionMessage();
            GridsAreEnabled = false;
            DisableColor = new SolidColorBrush(Colors.DarkGray);
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
        
        Console.WriteLine(SelectedCredit.CreditTypeId);
        
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
        creditApplicationRequest.SelectedCredit.CreditTypeId = creditType.CreditTypeId;
        
        

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
            catch (ApiException)
            {
                DialogMessages.ShowApiExceptionMessage();
            }
            catch (HttpRequestException)
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
        catch (ApiException)
        {
            DialogMessages.ShowApiExceptionMessage();
            GridsAreEnabled = false;
            DisableColor = new SolidColorBrush(Colors.DarkGray);
        }
        catch (HttpRequestException)
        {
            DialogMessages.ShowHttpRequestExceptionMessage();
            GridsAreEnabled = false;
            DisableColor = new SolidColorBrush(Colors.DarkGray);
        }
    }

    [RelayCommand]
    public async Task AddIdentificationDocumentCommand()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();

        ConfigureFileDialog(openFileDialog);
        
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            string[]? directory = await openFileDialog.ShowAsync(desktop.MainWindow!);

            if (directory!.Length > 0)
            {
                Console.WriteLine(directory[0]);
                _identificationDocument = ReadPdfFileToBytes(directory[0]);
                LblIdentificationDocument = FILE_SELECTED;
                
            }
        }
    }

    [RelayCommand]
    public async Task AddProofOfIncomeCommand()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();

        ConfigureFileDialog(openFileDialog);
        
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            string[] directory = await openFileDialog.ShowAsync(desktop.MainWindow);

            if (directory!.Length > 0)
            {
                Console.WriteLine(directory[0]);
                _proofOfIncome = ReadPdfFileToBytes(directory[0]);
                LblProofOfIncomeDocument = FILE_SELECTED;
            }
        }
    }

    [RelayCommand]
    public async Task AddProofOfAddressCommand()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();

        ConfigureFileDialog(openFileDialog);
        
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            string[] directory = (await openFileDialog.ShowAsync(desktop.MainWindow!))!;

            if (directory!.Length > 0)
            {
                Console.WriteLine(directory[0]);
                _proofOfAddress = ReadPdfFileToBytes(directory[0]);
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
    
    private byte[] ReadPdfFileToBytes(string filePath)
    {
        byte[] pdfBytes;
        try
        {
            pdfBytes = File.ReadAllBytes(filePath);
            return pdfBytes;
        }
        catch (IOException ex)
        {
            Console.WriteLine("Error al leer el archivo PDF: " + ex.Message);
            return null;
        }
    }
    
    [RelayCommand]
    public void CancelCommand()
    {
        
    }

    private async void SetClient(string clientRfc)
    {
        Rfc = clientRfc;
        await SearchClientCommand();
    }
    
    [ObservableProperty] private string _lblIdentificationDocument;
    [ObservableProperty] private string _lblProofOfAddressDocument;
    [ObservableProperty] private string _lblProofOfIncomeDocument;
    [ObservableProperty] private bool _gridsAreEnabled;
    [ObservableProperty] private bool _gridsAreEnabledValidation = true;
    [ObservableProperty] private bool _infoClienteVisibility = false;
    [ObservableProperty] private bool _btnRegisterVisibility = true;
    [ObservableProperty] private bool _isBorderVisible = false;
    [ObservableProperty] private bool _uploadDocs = true; 
    [ObservableProperty] private string _titleBinding = "Crear Solicitud de Crédito";

    [ObservableProperty] 
    private SolidColorBrush _rfcBrush;
    [ObservableProperty] 
    private SolidColorBrush _disableColor;

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