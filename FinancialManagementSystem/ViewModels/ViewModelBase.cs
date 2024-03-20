using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FinancialManagementSystem.ViewModels;

public class ViewModelBase : ObservableValidator
{
    public void RaisePropertyChanging(PropertyChangingEventArgs args)
    {
        throw new System.NotImplementedException();
    }

    public void RaisePropertyChanged(PropertyChangedEventArgs args)
    {
        throw new System.NotImplementedException();
    }
}