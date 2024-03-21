using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using FinancialManagementSystem.Models;
using Refit;
using RestSharp;

namespace FinancialManagementSystem.Services.CRU_Politica;

public interface IPoliticsService
{
    [Post("/register")]
    Task RegisterAsync([Body] Politic request);
    
    [Post("/modify")]
    Task ModifyAsync([Body] Politic request);
    
    [Get("/")]
    Task<List<Politic>> GetPoliticsAsync();

}


