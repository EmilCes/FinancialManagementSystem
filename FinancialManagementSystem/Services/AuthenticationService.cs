using System;
using System.Net;
using System.Threading.Tasks;
using FinancialManagementSystem.Models;
using Newtonsoft.Json;
using RestSharp;

namespace FinancialManagementSystem.Services;

public static class AuthenticationService
{
    private static readonly string BaseUrl = "http://localhost:8080/api/v1/auth";
    
    public static async Task<bool> AuthenticateAsync(string? email, string? password)
    {
        bool isMfaEnabled = false;
        var client = new RestClient(BaseUrl);
        var request = new RestRequest("/authenticate", Method.Post);
        
        request.RequestFormat = DataFormat.Json;
        request.AddJsonBody(new
        {
            email,
            password
        });

        try
        {
            var response = await client.PostAsync(request);
            Response2Fa? response2Fa = JsonConvert.DeserializeObject<Response2Fa>(response.Content!);
            isMfaEnabled = response2Fa!.MfaEnabled;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
        }
        
        return isMfaEnabled;
    }
    
    public static async Task<string?> GetQrCode(string? email, string? password)
    {
        var client = new RestClient(BaseUrl);
        var request = new RestRequest("/enable-2fa", Method.Post);
        request.RequestFormat = DataFormat.Json;
        
        request.AddJsonBody(new
        {
            email,
            password
        });

        try
        {
            var response = await client.PostAsync(request);
            Response2Fa? response2Fa = JsonConvert.DeserializeObject<Response2Fa>(response.Content!);
            return response2Fa!.SecretImageUri;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
        }

        return null;
    }
    
    public static async Task<AuthenticationServiceResponse> VerifyCode2FaAsync(string? email, string? code)
    {
        AuthenticationServiceResponse authenticationServiceResponse = new AuthenticationServiceResponse();
        
        var client = new RestClient(BaseUrl);
        var request = new RestRequest("/verify", Method.Post);
        
        request.RequestFormat = DataFormat.Json;
        
        request.AddJsonBody(new
        {
            email,
            code
        });

        try
        {
            var response = await client.PostAsync(request);
            authenticationServiceResponse.RespondeCode = response.StatusCode;
        }
        catch (Exception e)
        {
            authenticationServiceResponse.Exception = e;
        }
        
        return authenticationServiceResponse;
    }
}