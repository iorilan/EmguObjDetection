using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using EmguObjDetect;
using OpenTK;

namespace VCAOpenCV
{
    public class VCA
    {
        private static IDetect Algo;

        private VCA() { }
        public static VCA SelectAlgo(IDetect algo)
        {
            Algo = algo;
            return new VCA();
        }
        public void Analyse()
        {
            Task.Run(() =>
            {
                var vcaFolder = ConfigurationManager.AppSettings["vcaFolder"];
                var outputFolder = ConfigurationManager.AppSettings["outputFolder"];
                MediaHelper.EnsureDirCreated(vcaFolder);
                while (true)
                {
                    var files = Directory.GetFiles(vcaFolder);
                    if (files.Length > 0)
                    {
                        var file = files[0];
                        Do(file, outputFolder);
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            });
           
        }

        private void Do(string path, string outputFolder)
        {
            bool detected = false;
            using (var fs = new FileStream(path, FileMode.Open))
            {
                var bmp = new Bitmap(fs);
                using (var imageFrame = new Image<Bgr, Byte>(bmp))
                {
                   // EmguHelper.ImproveContrast(imageFrame);
                    var grayframe = imageFrame.Convert<Gray, byte>();
                    var targets = Algo.Detect(grayframe);

                    if (targets.Length > 0)
                    {
                        detected = true;
                    }

                    if (detected)
                    {
                        foreach (var t in targets)
                        {
                            //var c = MathHelper.CenterRect(t);
                            // if (Helper.IsPointInPolygon(c, _points))
                            // {
                            imageFrame.Draw(t, new Bgr(Color.Red), 5);
                            //the detected face(s) is highlighted here using a box that is drawn around it/them


                            // }
                        }

                        var newBmp = imageFrame.ToBitmap();
                        var oldFileName = MediaHelper.FileName(path);
                        var newFolderName = outputFolder;
                        var newPath = Path.Combine(newFolderName, oldFileName);
                        try
                        {
                            if (File.Exists(newPath))
                            {
                                newPath = MediaHelper.UniquePath(newPath);
                            }
                            newBmp.Save(newPath);

                            Console.WriteLine("target detected");
                        }
                        catch (Exception ex)
                        {

                        }
                    }


                }
            }

            TryDelete(path);
        }

        private void TryDelete(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception e)
            {
                //System.GC.Collect();
            }
        }
    }
}
