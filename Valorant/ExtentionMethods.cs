using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

public static class ExtentionMethods
{
    public static BitmapImage DownloadImage(this string Url)
    {
        using (HttpClient httpClient = new HttpClient())
        {
            try
            {
                byte[] imageData = httpClient.GetByteArrayAsync(Url).Result;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(imageData);
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            } catch { return null;  }
        }
    }
}

