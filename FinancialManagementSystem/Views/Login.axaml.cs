using Avalonia.Controls;
using Avalonia.Interactivity;

namespace FinancialManagementSystem.Views;

public partial class Login : Window
{
    public Login()
    {
        InitializeComponent();
    }

    private void Button_Active2FA(object? sender, RoutedEventArgs e)
    {
        Border? borderLogin = this.Find<Border>("BorderLogin");
        Border? borderActive2Fa = this.Find<Border>("BorderActive2Fa");
        StackPanel? stackPanelActive2Fa = this.Find<StackPanel>("StackPanelActive2Fa");


        borderLogin!.IsVisible = false;
        borderActive2Fa!.IsVisible = true;
        stackPanelActive2Fa!.IsVisible = true;

    }
    
    private void Button_Login(object? sender, RoutedEventArgs e)
    {
        Border? borderLogin = this.Find<Border>("BorderLogin");
        Border? borderActive2Fa = this.Find<Border>("BorderActive2Fa");
        StackPanel? stackPanelActive2Fa = this.Find<StackPanel>("StackPanelActive2Fa");
        StackPanel? stackPanelQrCode = this.Find<StackPanel>("StackPanelQr");


        borderLogin!.IsVisible = true;
        borderActive2Fa!.IsVisible = false;
        stackPanelActive2Fa!.IsVisible = false;
        stackPanelQrCode!.IsVisible = false;

    }
    
    private void Button_QrCode(object? sender, RoutedEventArgs e)
    {
        StackPanel? stackPanelActive2Fa = this.Find<StackPanel>("StackPanelActive2Fa");
        StackPanel? stackPanelQrCode = this.Find<StackPanel>("StackPanelQr");
        
        stackPanelActive2Fa!.IsVisible = false;
        stackPanelQrCode.IsVisible = true;

    }
}