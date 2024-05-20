using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialManagementSystem.Services.PaymentLayout;
using FinancialManagementSystem.ViewModels.Helpers;
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
    
    private readonly IPaymentLayoutService _paymentLayoutService;
    private List<PaymentLayoutResponse> PaymentLayoutsListCopy { get; set; } = new();
    
    public ObservableCollection<PaymentLayoutResponse> PaymentLayoutsList { get; set; } = new();


    public PaymentLayoutGenerationViewModel()
    {
        _paymentLayoutService = new PaymentLayoutService("http://localhost:8080/api/v1/payment-layout");

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
            var result = await _paymentLayoutService.GetPaymentLayouts();
            PaymentLayoutsListCopy = result;
            
            FillObservableCollection(PaymentLayoutsList, result);
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
            FillObservableCollection(PaymentLayoutsList, PaymentLayoutsListCopy);
            return;
        }

        var filteredCredits = PaymentLayoutsListCopy.Where(p => p.clientRfc.Contains(Rfc, StringComparison.OrdinalIgnoreCase));

        var enumerable = filteredCredits.ToList();
        PaymentLayoutsList.Clear();

        if (enumerable.Count == 0)
        {
            DialogMessages.ShowMessage("", "Creditos no encontrados. Verifica el RFC ingresado.");
        }
        else
        {
            foreach (var client in enumerable.ToList())
            {
                PaymentLayoutsList.Add(client);
            }
        }
    }

    [RelayCommand]
    public async Task DownloadPaymentLayout(int paymentLayoutId)
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var topLevel = TopLevel.GetTopLevel(desktop.MainWindow);

            var file = await topLevel!.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Guardar Layout de Cobro"
            });

            if (file != null)
            {
                GeneratePaymentLayout(file.Path.ToString(), paymentLayoutId);
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
    
    private  void GeneratePaymentLayout(string destinationPath, int paymentLayoutId)
    {
        PaymentLayoutResponse paymentLayoutResponse = PaymentLayoutsList.FirstOrDefault(p => p.paymentLayoutId == paymentLayoutId)!;
        
        // Datos de ejemplo
        const string pdfTitle = "Layout de Cobros";
        const string financialName = "Financiera Independiente";
        var startDate = DateTime.Parse(paymentLayoutResponse.startDate);
        var clientName = paymentLayoutResponse.clientName;
        var creditType = paymentLayoutResponse.CreditType.Description;
        var amount = paymentLayoutResponse.CreditType.Amount.ToString(CultureInfo.InvariantCulture);
        var term = paymentLayoutResponse.CreditType.Term.ToString();
        var duration = paymentLayoutResponse.CreditType.Term;
        var termType = paymentLayoutResponse.CreditType.TermType;
        
        destinationPath = Uri.UnescapeDataString(new Uri(destinationPath).LocalPath) + ".pdf";

        using (var fs = new FileStream(destinationPath, FileMode.Create))
        {
            Document doc = new Document(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);

            doc.Open();

            PdfContentByte canvas = writer.DirectContent;

            Paragraph title = new Paragraph(pdfTitle, FontFactory.GetFont(FontFactory.HELVETICA, 16, Font.BOLD));
            title.Alignment = Element.ALIGN_CENTER;
            title.SpacingAfter = 20f;
            doc.Add(title);

            Paragraph info = new Paragraph();
            info.Add(new Phrase("Nombre de la financiera: " + financialName));
            info.Add(Chunk.NEWLINE);
            info.Add(new Phrase("Nombre del cliente: " + clientName));
            info.Add(Chunk.NEWLINE);
            info.Add(new Phrase("Tipo de crédito: " + creditType));
            info.Add(Chunk.NEWLINE);
            info.Add(new Phrase("Monto prestado: " + amount));
            info.Add(Chunk.NEWLINE);
            info.Add(new Phrase("Tipo de Plazo: " + termType));
            info.Add(Chunk.NEWLINE);
            info.Add(new Phrase("Plazo: " + term));
            info.Add(Chunk.NEWLINE);
            info.Add(new Phrase("Fecha de elaboración: " + DateTime.Now.Date));
            doc.Add(info);

            var table = new PdfPTable(4)
            {
                WidthPercentage = 100,
                HorizontalAlignment = Element.ALIGN_CENTER,
                SpacingBefore = 20f
            };

            string[] headers = { "Número", "Mes", "Saldo", "Monto a Cobrar" };
            foreach (var header in headers)
            {
                var headerCell = new PdfPCell(new Phrase(header, FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.WHITE)))
                    {
                        BackgroundColor = new BaseColor(51, 51, 51),
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                
                table.AddCell(headerCell);
            }

            var balance = paymentLayoutResponse.CreditType.Amount;
            var amountToPay = balance / duration;

            for (int i = 0; i < duration; i++)
            {
                var month = startDate.AddMonths(i).ToString("MMMM yyyy");
                table.AddCell((i + 1).ToString());
                table.AddCell(month);
                table.AddCell(balance.ToString(CultureInfo.InvariantCulture));
                table.AddCell(amountToPay.ToString(CultureInfo.InvariantCulture));
                balance -= amountToPay;
            }

            doc.Add(table);

            doc.Close();
        }
    }

    
}