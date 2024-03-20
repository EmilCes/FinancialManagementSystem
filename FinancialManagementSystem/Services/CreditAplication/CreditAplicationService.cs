using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace FinancialManagementSystem.Services.CreditAplication;

public class CreditAplicationService: ICreditAplicationService
{
    private readonly ICreditAplicationService _api;


    public CreditAplicationService(string apiUrl)
    {
        _api = RestService.For<ICreditAplicationService>(apiUrl);
    }
    
    public async Task<VerifyClientExistenceResponse> VerifyExistanceAsync(VerifyClientRequest request)
    {
        return await _api.VerifyExistanceAsync(request);
    }

    public async Task<VerifyRegularClientResponse> VerifyRegularAsync(VerifyClientRequest request)
    {
        return await _api.VerifyRegularAsync(request);
    }

    public async Task<List<CreditType>> GetCreditTypesAsync(object dummy = null)
    {
        return await _api.GetCreditTypesAsync(dummy);
    }

    public async Task<CreateAplicationResponse> CreateAplicationAsync(CreditAplicationRequest request)
    {
        return await _api.CreateAplicationAsync(request);
    }
}