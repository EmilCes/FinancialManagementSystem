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
    
    public static void ShowApiExceptionMessage()
    {
        ShowMessage("Error", "Hubo un error con el servidor. Intentálo mas tarde.");
    }
    
    public static void ShowHttpRequestExceptionMessage()
    {
        ShowMessage("Error de Conexión", "Hubo un error al conectar con el servidor. Intentálo mas tarde.");
    }
}