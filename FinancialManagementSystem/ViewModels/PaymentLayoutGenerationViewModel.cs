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
                Title = "Guardar Layout de Cobro",
                SuggestedFileName = "Layout"
            });

            if (file != null)
            {
                GeneratePaymentLayout(file.Path.ToString(), paymentLayoutId);
                Console.WriteLine("PDF guardado exitosamente en: " + file.Path);
            }

        }
    }
    
    private static void FillObservableCollection<T>(ICollection<T> observableCollection, List<T> listToCopy)
    {
        observableCollection.Clear();
        
        foreach (var item in listToCopy)
        {
            observableCollection.Add(item);
        }
    }
    
   private void GeneratePaymentLayout(string destinationPath, int paymentLayoutId)
    {
        var paymentLayoutResponse = PaymentLayoutsList.FirstOrDefault(p => p.paymentLayoutId == paymentLayoutId)!;

        var interestRate = paymentLayoutResponse.CreditType.Iva / 100;
        var vatRate = paymentLayoutResponse.CreditType.Iva / 100;
        var amountValue = paymentLayoutResponse.CreditType.Amount;
        var vatValue = amountValue * vatRate;
        var interestValue = amountValue * interestRate;
        
        const string pdfTitle = "Layout de Cobros";
        const string financialName = "Financiera Independiente";
        var clientName = paymentLayoutResponse.clientName;
        var startDate = DateTime.Parse(paymentLayoutResponse.startDate);
        var creditType = paymentLayoutResponse.CreditType.Description;
        var amount = amountValue.ToString("C", CultureInfo.CurrentCulture);
        var term = paymentLayoutResponse.CreditType.Term.ToString();
        var duration = paymentLayoutResponse.CreditType.Term;
        var termType = paymentLayoutResponse.CreditType.TermType;
        var interest = interestRate.ToString("P1", CultureInfo.CurrentCulture);
        var vat = vatRate.ToString("P1", CultureInfo.CurrentCulture);
        var totalAmount = (amountValue + vatValue + interestValue).ToString("C", CultureInfo.CurrentCulture);
        
        destinationPath = Uri.UnescapeDataString(new Uri(destinationPath).LocalPath) + ".pdf";

        using (var fs = new FileStream(destinationPath, FileMode.Create))
        {
            var doc = new Document(PageSize.A4);
            var writer = PdfWriter.GetInstance(doc, fs);

            doc.Open();

            var title = new Paragraph(pdfTitle, FontFactory.GetFont(FontFactory.HELVETICA, 16, Font.BOLD))
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 20f
            };
            
            doc.Add(title);

            var tableInfo = new PdfPTable(2)
            {
                WidthPercentage = 100,
                SpacingBefore = 20f,
                SpacingAfter = 20f
            };
            
            AddCellToTable(tableInfo, "Financiera:", financialName, true);
            AddCellToTable(tableInfo, "Tipo de crédito:", creditType, true);
            AddCellToTable(tableInfo, "Cliente:", clientName, true);
            AddCellToTable(tableInfo, "Tipo de Plazo:", termType, true);
            AddCellToTable(tableInfo, "Monto prestado:", amount, true);
            AddCellToTable(tableInfo, "Fecha de elaboración:", DateTime.Now.Date.ToString("d"), true);
            AddCellToTable(tableInfo, "Monto Total:", totalAmount, true);
            AddCellToTable(tableInfo, "Plazo:", term, true);
            AddCellToTable(tableInfo, "", "", true);
            AddCellToTable(tableInfo, "Interes:", interest, true);
            AddCellToTable(tableInfo, "", "", true);
            AddCellToTable(tableInfo, "Iva:", vat, true);
            AddCellToTable(tableInfo, "", "", true);
            
            doc.Add(tableInfo);

            var table = new PdfPTable(5)
            {
                WidthPercentage = 100,
                HorizontalAlignment = Element.ALIGN_CENTER,
                SpacingBefore = 20f
            };

            string[] headers = { "Número", "Mes", "Saldo Capital", "Saldo Intereses", "Monto a Cobrar" };
            foreach (var header in headers)
            {
                var headerCell = new PdfPCell(new Phrase(header, FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.WHITE)))
                {
                    BackgroundColor = new BaseColor(51, 51, 51),
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                
                table.AddCell(headerCell);
            }

            var capital = amountValue;
            var interests = interestValue + vatValue;
            
            var capitalToPay = capital / duration;
            var interestsToPay = interests / duration;

            var amountToPay = capitalToPay + interestsToPay;
            
            for (var i = 0; i < duration; i++)
            {
                var month = startDate.AddMonths(i).ToString("MMMM yyyy");
                table.AddCell((i + 1).ToString());
                table.AddCell(month);
                table.AddCell(capital.ToString("C", CultureInfo.CurrentCulture));
                table.AddCell(interests.ToString("C", CultureInfo.CurrentCulture));
                table.AddCell(amountToPay.ToString("C", CultureInfo.CurrentCulture));
                
                capital -= capitalToPay;
                interests -= interestsToPay;
            }

            doc.Add(table);

            doc.Close();
        }
    }

    private static void AddCellToTable(PdfPTable table, string label, string value, bool noBorder = false)
    {
        var labelCell = new PdfPCell(new Phrase(label + " " + value))
        {
            Border = noBorder ? Rectangle.NO_BORDER : Rectangle.BOX // Eliminar bordes si noBorder es true
        };
        table.AddCell(labelCell);
    }

    
}