using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialManagementSystem.Models.Helpers;
using FinancialManagementSystem.Services.Credit;
using FinancialManagementSystem.Services.Credit.Dto;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Refit;

namespace FinancialManagementSystem.ViewModels;

public partial class PaymentLayoutGenerationViewModel : ViewModelBase
{
    [ObservableProperty] 
    private ObservableCollection<string>? _layoutTypes;
    [ObservableProperty] 
    private string? _selectedItem;
    [ObservableProperty]
    private string _rfc = null!;
    
    private readonly ICreditService _creditService;
    private List<GetCreditResponse> CreditsListCopy { get; set; } = new();
    
    public ObservableCollection<GetCreditResponse> CreditsList { get; set; } = new();


    public PaymentLayoutGenerationViewModel()
    {
        _creditService = new CreditService("http://localhost:8080/api/v1/credit");

        LoadData();
        Initialize();
    }
    
    private async void Initialize()
    {
        await LoadCommand();
    }

    private void LoadData()
    {
        LayoutTypes = ["Mes", "Año", "Periodo Completo"];
    }
    
    private async Task LoadCommand()
    {
        try
        {
            var result = await _creditService.GetCreditsAsync();
            CreditsListCopy = result;
            
            FillObservableCollection(CreditsList, result);
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
    public void FilterCreditsByRfc()
    {
        if (string.IsNullOrEmpty(Rfc))
        {
            FillObservableCollection(CreditsList, CreditsListCopy);
            return;
        }

        var filteredCredits = CreditsListCopy.Where(credit => credit.ClientRfc.Contains(Rfc, StringComparison.OrdinalIgnoreCase));

        var enumerable = filteredCredits.ToList();
        CreditsList.Clear();

        if (enumerable.Count == 0)
        {
            DialogMessages.ShowMessage("", "Creditos no encontrados. Verifica el RFC ingresado.");
        }
        else
        {
            foreach (var client in enumerable.ToList())
            {
                CreditsList.Add(client);
            }
        }
    }

    [RelayCommand]
    public async void DownloadPaymentLayout()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var topLevel = TopLevel.GetTopLevel(desktop.MainWindow);

            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Guardar Layout de Cobro"
            });

            if (file != null)
            {
                GeneratePaymentLayout(file.Path.ToString());
                Console.WriteLine("PDF guardado exitosamente en: " + file.Path);
            }

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
    
    private static void GeneratePaymentLayout(string destinationPath)
    {
        // Datos de ejemplo
        int duracionPrestamoMeses = 6;
        DateTime fechaInicio = new DateTime(2024, 1, 1);
        string titulo = "Layout de Cobros";
        string nombreFinanciera = "Financiera Independiente";
        string nombreCliente = "Juan Pérez";
        string tipoCredito = "Automoviles";
        string montoPrestado = "80,000";
        string plazo = "12";
        string noCredito = "XYZ12121";
        
        destinationPath = Uri.UnescapeDataString(new Uri(destinationPath).LocalPath) + ".pdf";

        // Crear o abrir el documento PDF existente en modo de añadir
        using (var fs = new FileStream(destinationPath, FileMode.Create))
        {
            // Crear un documento PDF
            Document doc = new Document(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);

            // Abrir el documento
            doc.Open();

            // Agregar contenido al documento
            PdfContentByte canvas = writer.DirectContent;

            // Agregar título centrado
            Paragraph title = new Paragraph(titulo, FontFactory.GetFont(FontFactory.HELVETICA, 16, Font.BOLD));
            title.Alignment = Element.ALIGN_CENTER;
            title.SpacingAfter = 20f;
            doc.Add(title);

            // Agregar información
            Paragraph info = new Paragraph();
            info.Add(new Phrase("Nombre de la financiera: " + nombreFinanciera));
            info.Add(Chunk.NEWLINE);
            info.Add(new Phrase("Nombre del cliente: " + nombreCliente));
            info.Add(Chunk.NEWLINE);
            info.Add(new Phrase("Tipo de crédito: " + tipoCredito));
            info.Add(Chunk.NEWLINE);
            info.Add(new Phrase("Monto prestado: " + montoPrestado));
            info.Add(Chunk.NEWLINE);
            info.Add(new Phrase("Plazo: " + plazo + " meses"));
            info.Add(Chunk.NEWLINE);
            info.Add(new Phrase("Número de crédito: " + noCredito));
            info.Add(Chunk.NEWLINE);
            info.Add(new Phrase("Fecha de elaboración: " + DateTime.Now.Date));
            doc.Add(info);

            // Agregar tabla para los pagos mensuales
            PdfPTable table = new PdfPTable(4);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingBefore = 20f;

            // Agregar encabezados de la tabla
            string[] headers = { "Número", "Mes", "Saldo", "Monto a Cobrar" };
            foreach (var header in headers)
            {
                PdfPCell headerCell = new PdfPCell(new Phrase(header, FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.WHITE)));
                headerCell.BackgroundColor = new BaseColor(51, 51, 51);
                headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(headerCell);
            }

            // Calcular y agregar datos de pagos mensuales a la tabla
            for (int i = 0; i < duracionPrestamoMeses; i++)
            {
                string mes = fechaInicio.AddMonths(i).ToString("MMMM yyyy");
                decimal monto = 1000; // Ejemplo de monto fijo

                table.AddCell((i + 1).ToString());
                table.AddCell(mes);
                table.AddCell(""); // Este campo se puede llenar con los saldos
                table.AddCell(monto.ToString());
            }

            // Agregar tabla al documento
            doc.Add(table);

            // Cerrar el documento
            doc.Close();
        }
    }

    
}