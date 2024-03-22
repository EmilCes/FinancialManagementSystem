using Avalonia.Controls.ApplicationLifetimes;
using MsBox.Avalonia;

namespace FinancialManagementSystem.Models.Helpers;

public static class DialogMessages
{
    public static async void ShowMessage(string title, string message)
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var box = MessageBoxManager.GetMessageBoxStandard(title, message);
            await box.ShowWindowDialogAsync(desktop.MainWindow);
        }
    }

    public static void ShowInvalidFieldsMessage()
    {
        ShowMessage("Campos Inválidos", "Por favor, verifica los campos.");
    }
    
    public static void ShowApiExceptionMessage()
    {
        ShowMessage("Error", "Hubo un error con el servidor. Intentálo mas tarde.");
    }
    
    public static void ShowHttpRequestExceptionMessage()
    {
        ShowMessage("Error de Conexión", "No se pudo contactar con el servidor. Revise su conexión a internet.");
    }
}