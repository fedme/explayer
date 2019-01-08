using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Android.Content;
using Explayer.Droid.Services;
using Explayer.Services;
using System.IO.Compression;
using Java.Util.Zip;
using Xamarin.Forms;

[assembly: Dependency(typeof(HandleStaticFilesService))]
namespace Explayer.Droid.Services
{
    public class HandleStaticFilesService : AbstractHandleStaticFilesService
    {
        private static Context _context;

        public static void Init(Context context)
        {
            _context = context;
        }

        public override string DirectoryPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Personal), "static");

        protected override Task LoadHtmlFromResource()
        {
            var tcs = new TaskCompletionSource<object>();

            Task.Factory.StartNew(() =>
            {
                //delete old folder if exists
                if (Directory.Exists(DirectoryPath))
                {
                    Directory.Delete(DirectoryPath, true);
                }

                //load all files in Assets to an HTML folder
                try
                {
                    SyncAssets("static", Environment.GetFolderPath(
                        Environment.SpecialFolder.Personal) + "/");
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                    return;
                }

                tcs.SetResult(null);
            });

            return tcs.Task;
        }

        /*protected override async Task<string> DownloadZipFile(string zipName)
        {
            string zipName = "stimuli-v1.0.0.zip";
            var zipUrl = "https://static.isearchlab.org/explayer/apps/" + zipName;
            using (var client = new WebClient())
            {
                var zipPath = Path.Combine(DirectoryPath, zipName);
                await client.DownloadFileTaskAsync(zipUrl, zipPath);
                System.IO.Compression.
            }
        }*/

        private static void SyncAssets(string assetFolder, string targetDir)
        {
            if (_context == null)
                throw new NullReferenceException("Class has not been initialized");

            var slash = (assetFolder == "" ? "" : "/");

            var assets = _context.Assets.List(assetFolder);

            foreach (var asset in assets)
            {
                var subAssets = _context.Assets.List(assetFolder + slash + asset);

                // if it has a length, it's a folder
                if (subAssets.Length > 0)
                {
                    SyncAssets(assetFolder + slash + asset, targetDir);
                }
                else
                {
                    // it's a file
                    using (var source = _context.Assets.Open(assetFolder + slash + asset))
                    {
                        if (!Directory.Exists(targetDir + assetFolder))
                        {
                            Directory.CreateDirectory(targetDir + assetFolder);
                        }

                        using (var dest = File.Create(targetDir + assetFolder + slash + asset))
                        {
                            Console.WriteLine("Copying '" + assetFolder + slash + asset + "' to '" 
                                + targetDir + assetFolder + slash + asset + "'");
                            source.CopyTo(dest);
                        }
                    }
                }

            }
        }
    }
}