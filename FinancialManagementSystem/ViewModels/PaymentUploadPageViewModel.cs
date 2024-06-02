using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FinancialManagementSystem.Models.Helpers;
using CsvHelper;
using CsvHelper.Configuration;
using FinancialManagementSystem.Messages;
using FinancialManagementSystem.Services.Client;
using FinancialManagementSystem.Services.Client.Dto;
using FinancialManagementSystem.ViewModels.Helpers;

namespace FinancialManagementSystem.ViewModels;

public partial class PaymentUploadPageViewModel: ViewModelBase
{
    private readonly IClientService _clientService;
    private readonly IMessenger _messenger = Message.Instance;


    public PaymentUploadPageViewModel()
    {
        _clientService = new ClientService("http://localhost:8080/api/v1/client");
    }

    [RelayCommand]
    public async Task SearchClientCommand()
    {
        if (ValidateFields())
        {
            string clientRfc = Rfc;

            VerifyClientExistenceRequest verifyClientExistenceRequest = new VerifyClientExistenceRequest();
            verifyClientExistenceRequest.clientRfc = clientRfc;
            VerifyClientExistenceResponse response = await _clientService.VerifyClientExistenceAsync(verifyClientExistenceRequest);

            if (response.clientRegistered == true)
            {
                _messenger.Send(new ViewPaymentMessageWithoutPayment(clientRfc));
            }
            else
            {
                DialogMessages.ShowMessage("Cliente no encontrado", "El cliente no se encontro en el sistema, verifique el RFC");
            }
        }
        else
        {
            DialogMessages.ShowInvalidFieldsMessage();
        }
    }

    private bool ValidateFields()
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(this);
        return Validator.TryValidateObject(this, validationContext, validationResults, true);
    }
    
    [RelayCommand]
    public async Task UploadFileCommand()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();

        ConfigureFileDialog(openFileDialog);
        
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            string[] directory = (await openFileDialog.ShowAsync(desktop.MainWindow!))!;

            if (directory!.Length > 0)
            {
            
                FileInfo fileInfo = new FileInfo(directory[0]);
                long maxSizeInBytes = 10 * 1024 * 1024; 
                
                if (fileInfo.Length > maxSizeInBytes)
                {
                    DialogMessages.ShowMessage("Archivo muy grande", "El archivo excede el tamaño máximo de 10mb");
                }
                else
                {
                    try
                    {
                        using (var reader = new StreamReader(directory[0]))
                        using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                               {
                                   Delimiter = ",",
                                   HasHeaderRecord = true,
                               }))
                        {
                            var records = csv.GetRecords<PaymentRecord>();
                            foreach (var record in records)
                            {
                                VerifyClientExistenceRequest verifyClientExistenceRequest = new VerifyClientExistenceRequest();
                                verifyClientExistenceRequest.clientRfc = record.rfc;
                                VerifyClientExistenceResponse response = await _clientService.VerifyClientExistenceAsync(verifyClientExistenceRequest);

                                if (response.clientRegistered == true)
                                {
                                    _messenger.Send(new ViewPaymentMessage(record));
                                }
                                else
                                {
                                    DialogMessages.ShowMessage("Cliente no encontrado", "El cliente no se encontro en el sistema, verifique el RFC del archivo");
                                }
                                break;
                            }
                        }
                    }
                    catch (HeaderValidationException)
                    {
                        DialogMessages.ShowMessage("Error","El archivo CSV tiene un encabezado inválido. \nDeberia ser folio,rfc,amount.");
                    }
                    catch (Exception)
                    {
                        DialogMessages.ShowMessage("Error","Ocurrió un error al leer el archivo CSV");
                    }
                    
                }
                
            }
        }
    }
    
    private void ConfigureFileDialog(OpenFileDialog openFileDialog)
    {
        openFileDialog.AllowMultiple = false;
        openFileDialog.Filters.Add(new FileDialogFilter { Name = "Archivos .csv", Extensions = { "csv" } });
        openFileDialog.Title = "Seleccionar archivo CSV";
    }
    
    [ObservableProperty] 
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    [RegularExpression(@"^[A-Za-z]{4}\d{6}[A-Za-z\d]{3}$", ErrorMessage = ErrorMessages.RFC_MESSAGE)]
    private string? _rfc;
}