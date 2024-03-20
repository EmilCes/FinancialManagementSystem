using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialManagementSystem.Services.CreditAplication;

namespace FinancialManagementSystem.ViewModels;

public partial class CreditAplicationViewModel: ViewModelBase
{
    private readonly ICreditAplicationService _creditAplicationService;


    public CreditAplicationViewModel()
    {
        _creditAplicationService = new CreditAplicationService("http://localhost:8080/api/v1/credit");
    }

    [RelayCommand]
    public async Task SearchClientCommand()
    {
        
    }
}