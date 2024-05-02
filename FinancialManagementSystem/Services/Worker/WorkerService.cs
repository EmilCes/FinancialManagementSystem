using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using FinancialManagementSystem.Models;
using FinancialManagementSystem.Services.Worker;
using FinancialManagementSystem.Services.Worker.Dto;

public class WorkerService: IWorkerService
{
    private readonly IWorkerService _api;

    public WorkerService(string apiUrl)
    {
        _api = RestService.For<IWorkerService>(apiUrl);
    }
    
    public async Task RegisterWorkerAsync(RegisterRequest request)
    {
        await _api.RegisterWorkerAsync(request);
    }

    public async Task ModifyAsync(ModifyRequest request)
    {
        await _api.ModifyAsync(request);
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _api.GetAllUsersAsync();
    }

    public async Task<User> GetUserAsync(string rfc)
    {
        return await _api.GetUserAsync(rfc);
    }

    public async Task DeleteUserAsync(string rfc)
    {
        await _api.DeleteUserAsync(rfc);
    }

    public async Task<bool> RfcExistAsync(string rfc)
    {
        return await _api.RfcExistAsync(rfc);
    }

    public async Task<bool> UserNumberExistAsync(string userNumber)
    {
        return await _api.UserNumberExistAsync(userNumber);
    }
}

