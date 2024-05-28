using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Refit;

namespace FinancialManagementSystem.Services.Efficiencies;

public interface IEfficienciesService
{
    [Get("/year")]
    Task<int> GetYear();

    [Get("/")]
    Task<List<MonthResponse>> GetMonthlyEfficiencies([Query] int year);

}

public class MonthResponse
{
    public string Month { get; set; }
    public double ExpectedAmount { get; set; }
    public double ActualAmount { get; set; }

    public string ActualAmountString
    {
        get
        {
            return "$" + ActualAmount.ToString("N2", CultureInfo.InvariantCulture);
        }
    }

    public string ExpectedAmountString
    {
        get
        {
            return "$" +ExpectedAmount.ToString("N2", CultureInfo.InvariantCulture);
        }
    }

    public double Efficiencies { get; set; }

    public string EfficienciesString
    {
        get
        {
            return Efficiencies.ToString("N2", CultureInfo.InvariantCulture) + "%";
        }
    }
}