using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Cuda;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace FFmpegPhotoGenerator
{
    public class VCA
    {
        private static IDetect Algo;
    
        private VCA() { }
        public static VCA SetAlgo(IDetect algo)
        {
            Algo = algo;
            return new VCA();
        }
        public void Analysis(string path, string outputFolder)
        {
            bool detected = false;
            using (var fs = new FileStream(path, FileMode.Open))
            {
                using (var imageFrame = new Image<Bgr, Byte>(new Bitmap(fs)))
                {
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


                        // imageFrame.DrawPolyline(_points, true, new Bgr(Color.Crimson), 1);
                        // var bmp = EmguHelper.ResizeImage(imageFrame.ToBitmap(), new Size(pictureBox.Width, pictureBox.Height));

                        //pictureBox.Image = bmp;

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
                        }
                        catch (Exception ex)
                        {

                        }
                    }


                }
            }

            TryDelete(path);
        }

       

        private static void TryDelete(string path)
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
