using System.Threading.Tasks;
using Refit;

namespace FinancialManagementSystem.Services.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly IAuthenticationService _api;

    public AuthenticationService(string apiUrl)
    {
        _api = RestService.For<IAuthenticationService>(apiUrl);
    }
    
    public async Task<VerificationResponse> VerifyAsync(VerificationRequest request)
    {
        return await _api.VerifyAsync(request);
    }

    public async Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request)
    {
        return await _api.AuthenticateAsync(request);
    }

    public async Task<Enable2FaResponse> Enable2FaAsync(AuthenticationRequest request)
    {
        return await _api.Enable2FaAsync(request);
    }
}