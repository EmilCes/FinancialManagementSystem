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
using FinancialManagementSystem.Models;
using FinancialManagementSystem.Models.Helpers;
using FinancialManagementSystem.Services.CreditApplication;
using FinancialManagementSystem.Services.CreditType;

namespace FinancialManagementSystem.ViewModels;

public partial class CreditApplicationViewModel : ViewModelBase
{
    private readonly ICreditApplicationService _creditApplicationService;
    private readonly ICreditTypeService _creditTypeService;
    private const string FILE_SELECTED = "Seleccionado";

    private byte[] _identificationDocument = null;
    private byte[] _proofOfIncome = null;
    private byte[] _proofOfAddress = null;

    [ObservableProperty] private string _lblIdentificationDocument;
    [ObservableProperty] private string _lblProofOfAddressDocument;
    [ObservableProperty] private string _lblProofOfIncomeDocument;

    [ObservableProperty] private SolidColorBrush _rfcBrush;

    [ObservableProperty] [Required] private string _rfc;
    
    [ObservableProperty] [Required] private string _nameReferenceOne;
    [ObservableProperty] [Required] private string _firstLastnameReferenceOne;
    [ObservableProperty] [Required] private string _secondLastnameReferenceOne;
    [ObservableProperty] [Required] private string _phoneReferenceOne;

    [ObservableProperty] [Required] private string _nameReferenceTwo;
    [ObservableProperty] [Required] private string _firstLastnameReferenceTwo;
    [ObservableProperty] [Required] private string _secondLastnameReferenceTwo;
    [ObservableProperty] [Required] private string _phoneReferenceTwo;
    [ObservableProperty] private ObservableCollection<CreditType> _creditTypes = new ObservableCollection<Models.CreditType>();
    public CreditApplicationViewModel()
    {
        _creditApplicationService = new CreditApplicationService("http://localhost:8080/api/v1/creditApplication");
        _creditTypeService = new CreditTypeService("http://localhost:8080/api/v1/credit-type");
        SetLabels();
        SetCreditTypes();
    }

    private void SetLabels()
    {
        string noFile = "Sin archivo";
        LblIdentificationDocument = noFile;
        LblProofOfAddressDocument = noFile;
        LblProofOfIncomeDocument = noFile;
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

    [RelayCommand]
    public async Task ClientApplicateForCreditCommand()
    {
        CreditApplicationRequest creditApplicationRequest = new CreditApplicationRequest();
        Reference referenceOne = new Reference();
        Reference referenceTwo = new Reference();
        Models.CreditType creditType = new Models.CreditType(); //A borrar

        creditType.Description = "Crédito 1"; //A borrar
        
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
        creditApplicationRequest.SelectedCredit = creditType; //A borrar

        if (_identificationDocument != null && _proofOfAddress != null && _proofOfIncome != null)
        {
            creditApplicationRequest.IdentificationPdf = _identificationDocument;
            creditApplicationRequest.ProofOfAddressPdf = _proofOfAddress;
            creditApplicationRequest.ProofOfIncomePdf = _proofOfIncome;
            
            try
            {
                await _creditApplicationService.CreateAplicationAsync(creditApplicationRequest);
            }
            catch (Exception e)
            {
                //TODO Logger
                Console.WriteLine(e.StackTrace);
            }
        }
        else
        {
            DialogMessages.ShowMessage("Faltan datos.", "Seleccione todos los archivos a subir.");
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
                RfcBrush = new SolidColorBrush(Colors.Aqua);
            }
            else
            {
                RfcBrush = new SolidColorBrush(Colors.MediumOrchid);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            RfcBrush = new SolidColorBrush(Colors.Red);
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
                
                //TO DELETE
                Console.WriteLine("");
                for (int i = 0; i < _identificationDocument.Length; i++)
                {
                    Console.Write(_identificationDocument[i]);
                }
                //TO DELETE
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
                
                //TO DELETE
                Console.WriteLine("");
                for (int i = 0; i < _proofOfIncome.Length; i++)
                {
                    Console.Write(_proofOfIncome[i]);
                }
                //TO DELETE
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
                
                //TO DELETE
                Console.WriteLine("");
                for (int i = 0; i < _proofOfAddress.Length; i++)
                {
                    Console.Write(_proofOfAddress[i]);
                }
                //TO DELETE
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
}