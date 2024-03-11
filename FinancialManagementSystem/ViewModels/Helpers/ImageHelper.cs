using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;

namespace FinancialManagementSystem.Models.Helpers;

public static class ImageHelper
{
    public static async Task<Bitmap?>? LoadQrCode(string imageUri)
    {
        string base64Image = imageUri.Split(',')[1];
        byte[] imageBytes = Convert.FromBase64String(base64Image);
        Stream stream = new MemoryStream(imageBytes);
        
        return new Bitmap(stream);
    }
}