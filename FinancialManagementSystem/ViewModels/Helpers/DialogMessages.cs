using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Models;

namespace FinancialManagementSystem.ViewModels.Helpers;

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

    public static async Task<string?> ShowCustomMessage(string title, string message, List<ButtonDefinition> buttonDefinitions)
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // string title, string message, List<ButtonDefinition> buttonDefinitions
            var box = MessageBoxManager.GetMessageBoxCustom(
            new MessageBoxCustomParams
            {
                ButtonDefinitions = buttonDefinitions,
                ContentTitle = title,
                ContentMessage = message,
                Icon = MsBox.Avalonia.Enums.Icon.Question,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false,
                MaxWidth = 500,
                MaxHeight = 800,
                SizeToContent = SizeToContent.WidthAndHeight,
                ShowInCenter = true,
                Topmost = false
            });

            return await box.ShowWindowDialogAsync(desktop.MainWindow);
        }

        return null;
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
    
    public static void ShowInvalidRfc()
    {
        ShowMessage("No se encuentra el cliente.", "Cliente no encontrado. Por favor, registralo.");
    }
}