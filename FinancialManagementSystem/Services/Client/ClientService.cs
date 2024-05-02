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
    
    public async Task<Models.Client> GetClientByIdAsync(int id)
    {
        return await _api.GetClientByIdAsync(id);
    }

    public async Task<Models.Client> GetClientByRFCAsync(string rfc)
    {
        return await _api.GetClientByRFCAsync(rfc);
    }

    public async Task RegisterClientAsync(Models.Client request)
    {
        await _api.RegisterClientAsync(request);
    }

    public async Task<VerifyClientExistenceResponse> VerifyClientExistenceAsync(VerifyClientExistenceRequest request)
    {
        return await _api.VerifyClientExistenceAsync(request);
    }

    public async Task UpdateClientAsync(int id, Models.Client clientUpdated)
    {
        await _api.UpdateClientAsync(id, clientUpdated);
    }
}