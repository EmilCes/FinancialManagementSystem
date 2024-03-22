using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialManagementSystem.Models;
using FinancialManagementSystem.Models.Helpers;
using FinancialManagementSystem.Services.Authentication;
using Refit;
using AuthenticationResponse = FinancialManagementSystem.Services.Authentication.AuthenticationResponse;

namespace FinancialManagementSystem.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly IAuthenticationService _authenticationService;
    
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
        if (!Validations.ValidateFields(this, GetType(),new List<string> { "Email", "Password" }))
        {
            DialogMessages.ShowInvalidFieldsMessage();
            return;
        }
        
        var request = new AuthenticationRequest()
        {
            email = Email,
            password = Password
        };

        try
        {
            AuthenticationResponse response = await _authenticationService.AuthenticateAsync(request);

            if (response.mfaEnabled)
            {
                ShowCodeView = true;
                ShowLoginView = false;
            }
            else
            {
                DialogMessages.ShowMessage("MFA no activado",
                    "La autenticación de dos factores no esta activada. Activala antes de continuar.");
            }
        }
        catch (ApiException e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            DialogMessages.ShowApiExceptionMessage();
        }
        catch (HttpRequestException)
        {
            DialogMessages.ShowHttpRequestExceptionMessage();
        }
    }

    [RelayCommand]
    public async Task VerifyCommand()
    {
        if (!Validations.ValidateFields(this, GetType(),new List<string> { "Code2Fa" }))
        {
            DialogMessages.ShowInvalidFieldsMessage();
            return;
        }
        
        var request = new VerificationRequest
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
            
            ChangeWindowToMainMenu();
        }
        catch (ApiException e)
        {
            if (e.StatusCode == System.Net.HttpStatusCode.Forbidden) // 403 Forbidden
            {
                DialogMessages.ShowMessage("Código incorrecto", "Verifica el código que ingresaste");
            }
            else
            {
                DialogMessages.ShowApiExceptionMessage();
            }
        }
        catch (HttpRequestException)
        {
            DialogMessages.ShowHttpRequestExceptionMessage();
        }
    }

    public async Task Enable2Fa()
    {
        if (!Validations.ValidateFields(this, GetType(), new List<string> { "Email", "Password" }))
        {
            DialogMessages.ShowInvalidFieldsMessage();
            return;
        }
        
        var request = new AuthenticationRequest
        {
            email = Email,
            password = Password
        };

        try
        {
            Enable2FaResponse response = await _authenticationService.Enable2FaAsync(request);
            
            QrCodeImage =  ImageHelper.LoadQrCode(response.secretImageUri);
        }
        catch (ApiException)
        {
            DialogMessages.ShowApiExceptionMessage();
        }
        catch (HttpRequestException)
        {
            DialogMessages.ShowHttpRequestExceptionMessage();
        }
    }
    
    private void ChangeWindowToMainMenu()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var currentWindow = desktop.MainWindow;
            
            desktop.MainWindow = new Views.MainMenu
            {
                DataContext = new MainMenuViewModel(),
            };
            
            desktop.MainWindow.Show();
            currentWindow?.Close();
        }
    }
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    [EmailAddress (ErrorMessage = ErrorMessages.EMAIL_MESSAGE)]
    private string _email;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    private string _password;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    private string _code2Fa;
    
}