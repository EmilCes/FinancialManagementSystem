using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FinancialManagementSystem.Models;
using FinancialManagementSystem.Services.CRU_Politica;
using Refit;

namespace FinancialManagementSystem.Services.Authentication;

public class PoliticsService : IPoliticsService
{
    private readonly IPoliticsService _api;
    public PoliticsService(string apiUrl)
    {
        _api = RestService.For<IPoliticsService>(apiUrl);
    }
    
    public async Task RegisterAsync(Politic request)
    {
        await _api.RegisterAsync(request);
    }
    
    public async Task ModifyAsync(Politic request)
    {
        await _api.ModifyAsync(request);
    }

    public async Task<List<Politic>> GetPoliticsAsync(Object none)
    {
        return await _api.GetPoliticsAsync(none);
    }
}
