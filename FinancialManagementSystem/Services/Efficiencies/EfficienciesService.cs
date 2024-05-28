using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace FinancialManagementSystem.Services.Efficiencies;

public class EfficienciesService: IEfficienciesService
{
    private readonly IEfficienciesService _api;

    public EfficienciesService(string apiUrl)
    {
        _api = RestService.For<IEfficienciesService>(apiUrl);
    }
    
    public async Task<int> GetYear()
    {
        return await _api.GetYear();
    }

    public async Task<List<MonthResponse>> GetMonthlyEfficiencies(int year)
    {
        return await _api.GetMonthlyEfficiencies(year);
    }
}