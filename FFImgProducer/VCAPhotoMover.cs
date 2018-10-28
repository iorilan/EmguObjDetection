using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FFImgProducer
{
    /// <summary>
    /// Move ff images into VA folder
    /// </summary>
    public class VcaPhotoMover
    {
        public void StartAsync()
        {
            Task.Run(() =>
            {
                var vcaPath = MediaHelper.VcaImgfolder();
                var ffPath = MediaHelper.FFmpegImgfolder();

                MediaHelper.EnsureDirCreated(vcaPath, ffPath);
                while (true)
                {
                    try
                    {
                        var allImgs = Directory.GetFiles(ffPath).OrderBy(x => x).ToList();
                        string first = string.Empty;

                        ////try take one image from ffmpeg folder
                        if (allImgs.Count > 0)
                        {
                            first = allImgs[0];

                            var fileName = MediaHelper.FileName(first);
                            var vcaImgPath = $"{vcaPath}\\{fileName}";
                            if (File.Exists(vcaImgPath))
                            {
                                vcaImgPath = MediaHelper.UniquePath(vcaImgPath);
                            }
                            File.Move(first, vcaImgPath);
                        }
                        Thread.Sleep(1000);
                    }
                    catch (Exception ex)
                    {
                        // locked by ffmpeg, skip, later will try again
                    }
                }
            });


        }
    }
}
