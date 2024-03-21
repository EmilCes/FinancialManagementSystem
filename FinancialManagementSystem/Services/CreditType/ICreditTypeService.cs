using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace FinancialManagementSystem.Services.CreditType;

public interface ICreditTypeService
{
    [Get("/")]
    Task<List<Models.CreditType>> GetCreditsTypeAsync();

    [Post("/register")]
    Task RegisterCreditTypeAsync([Body] Models.CreditType request);
}