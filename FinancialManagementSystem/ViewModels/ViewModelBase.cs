using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FinancialManagementSystem.ViewModels;
using ReactiveUI;

public class ViewModelBase : ObservableObject, IReactiveObject
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