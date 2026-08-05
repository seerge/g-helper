using System.Diagnostics;
using System.Drawing.Text;
using System.IO.Compression;

namespace GHelper.AnimeMatrix
{
    public static class MatrixFont
    {
        const string PackageUrl = "https://dlcdnets.asus.com/pub/ASUS/GamingNB/AppforWin10/ROGFont/ROG_Font_V1.5.zip";

        static readonly string fontFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GHelper", "matrix.otf");
        static readonly PrivateFontCollection collection = new PrivateFontCollection();

        static FontFamily? family = FindInstalled() ?? LoadFile();

        public static FontFamily? Family => family;

        static FontFamily? FindInstalled()
        {
            foreach (var name in new[] { "AniMe Matrix Font", "ROG Fonts v1.5" })
                try { return new FontFamily(name); } catch { }

            return null;
        }

        static FontFamily? LoadFile()
        {
            if (!File.Exists(fontFile)) return null;

            try
            {
                collection.AddFontFile(fontFile);
                return collection.Families[0];
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Matrix font: " + ex.Message);
                return null;
            }
        }

        public static async Task<bool> Download()
        {
            if (family is not null) return true;

            string temp = Path.Combine(Path.GetTempPath(), "GHelperFont");

            try
            {
                Directory.CreateDirectory(temp);
                string zip = Path.Combine(temp, "font.zip");

                using (HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) })
                {
                    Logger.WriteLine("Getting: " + PackageUrl);
                    await File.WriteAllBytesAsync(zip, await client.GetByteArrayAsync(PackageUrl));
                }

                ZipFile.ExtractToDirectory(zip, temp, true);
                string msi = Directory.GetFiles(temp, "*.msi", SearchOption.AllDirectories)[0];

                // administrative install just unpacks the font, without registering it in the system
                using (var process = Process.Start(new ProcessStartInfo("msiexec", $"/a \"{msi}\" /qn TARGETDIR=\"{temp}\"") { CreateNoWindow = true }))
                    if (process is not null) await process.WaitForExitAsync();

                Directory.CreateDirectory(Path.GetDirectoryName(fontFile)!);
                File.Copy(Directory.GetFiles(temp, "*.otf", SearchOption.AllDirectories)[0], fontFile, true);

                family = LoadFile();
                Logger.WriteLine("Matrix font: " + family?.Name);
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Matrix font: " + ex.Message);
            }

            try { Directory.Delete(temp, true); } catch { }

            return family is not null;
        }
    }
}
