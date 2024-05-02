using System.Collections.Generic;
using System.Threading.Tasks;
using FinancialManagementSystem.Models;
using FinancialManagementSystem.Services.Worker.Dto;
using Refit;

namespace FinancialManagementSystem.Services.Worker;

public interface IWorkerService
{
    [Post("/register")]
    Task RegisterWorkerAsync([Body] RegisterRequest request);
    
    [Put("/modify-user")]
    Task ModifyAsync([Body] ModifyRequest request);
    
    [Get("/get-users")]
    Task<List<User>> GetAllUsersAsync();
    
    [Get("/")]
    Task<User> GetUserAsync([AliasAs("rfc")] string rfc);
    
    [Delete("/delete-user")]
    Task DeleteUserAsync([AliasAs("rfc")] string rfc);
    
    [Get("/rfc-exist")]
    Task<bool> RfcExistAsync([AliasAs("rfc")] string rfc);
    
    [Get("/user-number-exist")]
    Task<bool> UserNumberExistAsync([AliasAs("userNumber")] string userNumber);
    
}