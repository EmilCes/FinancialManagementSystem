using System;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialManagementSystem.Models;
using FinancialManagementSystem.Models.Helpers;
using FinancialManagementSystem.Services.Authentication;
using ReactiveUI;
using Refit;
using AuthenticationResponse = FinancialManagementSystem.Services.Authentication.AuthenticationResponse;

namespace FinancialManagementSystem.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    //To make a login we need: Access Token, Name, Role, Refresh Token?

    private readonly IAuthenticationService _authenticationService;


    [ObservableProperty]
    private string _email;
    [ObservableProperty]
    private string _password;
    [ObservableProperty]
    private string _code2Fa;
    [ObservableProperty] 
    private bool _showLoginView = true;
    [ObservableProperty] 
    private bool _showCodeView;
    [ObservableProperty] 
    private Task<Bitmap> _qrCodeImage;
    
    public LoginViewModel()
    {
        _authenticationService = new AuthenticationService("http://localhost:8080/api/v1/auth");
    }

    [RelayCommand]
    public async Task LoginCommand()
    {
        var request = new AuthenticationRequest()
        {
            email = Email,
            password = Password
        };

        try
        {
            AuthenticationResponse response = await _authenticationService.AuthenticateAsync(request);
            //TODO: Save tokens
            //TODO: Verify MFA is enabled
            

            ShowCodeView = true;
            ShowLoginView = false;
        }
        catch (ApiException ex)
        {
            Console.WriteLine(ex.StatusCode);
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
    }

    [RelayCommand]
    public async Task VerifyCommand()
    {
        //TODO: Redirect if MFA is not enabled
        var request = new VerificationRequest()
        {
            email = Email,
            code = Code2Fa
        };

        try
        {
            VerificationResponse response = await _authenticationService.VerifyAsync(request);

            Employee.Instance.FirstName = response.firstName;
            Employee.Instance.LastName = response.lastName;
            Employee.Instance.AccessToken = response.accessToken;
            Employee.Instance.Role = response.role;
            
            Console.WriteLine(Employee.Instance.ToString());
            
            ChangeWindowToMainMenu();
        }
        catch (ApiException ex)
        {
            Console.WriteLine(ex.StatusCode);
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
    }

    public async Task Enable2Fa()
    {
        //TODO: Validate MFA is not enabled
        
        var request = new AuthenticationRequest()
        {
            email = Email,
            password = Password
        };

        try
        {
            Enable2FaResponse response = await _authenticationService.Enable2FaAsync(request);

            if (response.mfaEnabled)
            {
                Console.WriteLine("Ya activaste MFA");
            }
            
            QrCodeImage =  ImageHelper.LoadQrCode(response.secretImageUri);
        }
        catch (ApiException ex)
        {
            Console.WriteLine(ex.StatusCode);
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
    }
    
    private void ChangeWindowToMainMenu()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var currentWindow = desktop.MainWindow;
            
            desktop.MainWindow = new Views.MainMenu()
            {
                DataContext = new MainMenuViewModel(),
            };
            
            desktop.MainWindow.Show();
            currentWindow?.Close();
        }
    }
    
}