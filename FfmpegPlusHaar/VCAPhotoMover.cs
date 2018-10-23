using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FFmpegPhotoGenerator
{
    public class VCAPhotoMover
    {
        private static VCA _vca;

        public VCAPhotoMover()
        {
            _vca = VCA.SetAlgo(new AlgoHaarFullBody());
        }
        public void StartAsync()
        {
            Task.Run(() =>
            {
                var vcaPath = MediaHelper.VcaImgfolder();
                var ffPath = MediaHelper.FFmpegImgfolder();
                var resultPath = MediaHelper.OutputImgfolder();
                MediaHelper.EnsureDirCreated(vcaPath, ffPath, resultPath);
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

                             _vca.Analysis(vcaImgPath, resultPath);
                           // _vca.AnalyseTf(vcaImgPath, resultPath);
                        }
                        else
                        {
                            var vcaFolderFiles = Directory.GetFiles(vcaPath).OrderBy(x => x).ToList();
                            ////or else try take one image from vca folder
                            if (vcaFolderFiles.Count > 0)
                            {
                                first = vcaFolderFiles[0];
                                var fileName = MediaHelper.FileName(first);
                                var vcaImgPath = $"{vcaPath}\\{fileName}";

                                _vca.Analysis(vcaImgPath, resultPath);
                               // _vca.AnalyseTf(vcaImgPath, resultPath);
                            }
                            else
                            {
                                Thread.Sleep(1000);
                            }
                        }

                        


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
