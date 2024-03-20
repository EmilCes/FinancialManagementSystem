using System.Collections.Generic;
using System.Threading.Tasks;
using FinancialManagementSystem.Services.Authentication;
using FinancialManagementSystem.Services.Client.Dto;
using Refit;

namespace FinancialManagementSystem.Services.Client;

public interface IClientService
{
    [Get("/")]
    Task<List<Models.Client>> GetClientsAsync();

    [Post("/register")]
    Task RegisterClientAsync([Body] Models.Client request);

    [Post("/verify-existence")]
    Task<VerifyClientExistenceResponse> VerifyClientExistenceAsync([Body] VerifyClientExistenceRequest request);
}