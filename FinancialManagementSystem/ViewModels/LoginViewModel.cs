using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialManagementSystem.Messages;
using FinancialManagementSystem.Models;
using FinancialManagementSystem.Models.Helpers;
using FinancialManagementSystem.Services.Authentication;
using FinancialManagementSystem.ViewModels.Helpers;
using Refit;
using AuthenticationResponse = FinancialManagementSystem.Services.Authentication.AuthenticationResponse;

namespace FinancialManagementSystem.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly IAuthenticationService _authenticationService;

    [ObservableProperty] 
    private bool _showVerifyEmailView;
    [ObservableProperty] 
    private bool _showVerifyCodeView;
    [ObservableProperty] 
    private bool _showChangePasswordView;
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
            if (e.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                DialogMessages.ShowMessage("Credenciales Inválidas", "Por favor, verifica tus credenciales.");
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
            if (e.StatusCode == System.Net.HttpStatusCode.Forbidden)
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

    [RelayCommand]
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
    
    [RelayCommand]
    public async Task VerifyEmailCommand()
    {
        if (!Validations.ValidateFields(this, GetType(), new List<string> { "Email" }))
        {
            DialogMessages.ShowInvalidFieldsMessage();
            return;
        }
        
        try
        {
            bool response = await _authenticationService.VerifyEmailAsync(Email);

            if (response)
            {
                ShowVerifyEmailView = false;
                ShowVerifyCodeView = true;
            }
            else
            {
                DialogMessages.ShowMessage("Correo Inválido", "Verifica que el correo sea correcto.");
            }
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
    
    [RelayCommand]
    public async Task VerifyCodeCommand()
    {
        if (!Validations.ValidateFields(this, GetType(), new List<string> { "ChangePasswordCode" }))
        {
            DialogMessages.ShowInvalidFieldsMessage();
            return;
        }

        VerificationRequest request = new VerificationRequest
        {
            email = Email,
            code = ChangePasswordCode
        };
        
        try
        {
            bool response = await _authenticationService.VerifyPasswordCodeAsync(request);

            if (response)
            {
                ShowVerifyCodeView = false;
                ShowChangePasswordView = true;
            }
            else
            {
                DialogMessages.ShowMessage("Código Inválido", "Verifica que el correo sea correcto.");
            }
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
    
    [RelayCommand]
    public async Task ChangePasswordCommand()
    {
        if (!Validations.ValidateFields(this, GetType(), new List<string> { "ChangedPassword", "ChangedPasswordConfirmation" }))
        {
            DialogMessages.ShowInvalidFieldsMessage();
            return;
        }

        if (ChangedPassword != ChangedPasswordConfirmation)
        {
            DialogMessages.ShowMessage("Contraseñas Inválidas", "Las contraseñas no son iguales.");
            return;
        }

        AuthenticationRequest request = new AuthenticationRequest
        {
            email = Email,
            password = ChangedPassword
        };
        
        try
        {
            await _authenticationService.ChangePasswordAsync(request);
            
            ShowChangePasswordView = false;
            ShowLoginView = true;

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
    
    [RelayCommand]
    public void ForgottenPasswordCommand()
    {
        ShowLoginView = false;
        ShowVerifyEmailView = true;
    }
    
    [RelayCommand]
    public void BackFromChangePasswordCommand()
    {
        ShowLoginView = true;
        ShowVerifyEmailView = false;
    }
    
    private void ChangeWindowToMainMenu()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var currentWindow = desktop.MainWindow;
            
            desktop.MainWindow = new Views.MainMenu
            {
                DataContext = new MainMenuViewModel(Message.Instance),
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
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    private string _changePasswordCode;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    private string _changedPassword;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    private string _changedPasswordConfirmation;
    
}