using System.Collections.Generic;
using System.Threading.Tasks;
using FinancialManagementSystem.Services.Authentication;
using FinancialManagementSystem.Services.Client.Dto;
using Refit;

namespace FinancialManagementSystem.Services.Client;

public class ClientService : IClientService
{
    private readonly IClientService _api;
    
    public ClientService(string apiUrl)
    {
        _api = RestService.For<IClientService>(apiUrl);
    }
    
    public async Task<List<Models.Client>> GetClientsAsync()
    {
        return await _api.GetClientsAsync();
    }

    public async Task RegisterClientAsync(Models.Client request)
    {
        await _api.RegisterClientAsync(request);
    }

    public async Task<VerifyClientExistenceResponse> VerifyClientExistenceAsync(VerifyClientExistenceRequest request)
    {
        return await _api.VerifyClientExistenceAsync(request);
    }
}