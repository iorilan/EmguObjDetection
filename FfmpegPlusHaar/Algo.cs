using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Cuda;
using Emgu.CV.Cvb;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.TF;
using Emgu.TF.Models;

namespace FFmpegPhotoGenerator
{
    public interface IDetect
    {
        Rectangle[] Detect(Image<Gray, byte> grayframe);
    }

    public class AlgoHog : IDetect
    {
        public Rectangle[] Detect(Image<Gray, byte> grayframe)
        {
            using (HOGDescriptor des = new HOGDescriptor())
            {
                des.SetSVMDetector(HOGDescriptor.GetDefaultPeopleDetector());

                MCvObjectDetection[] results = des.DetectMultiScale(grayframe);
                var rects = new Rectangle[results.Length];
                if (results.Length > 0)
                {
                    for (int i = 0; i < results.Length; i++)
                    {
                        rects[i] = results[i].Rect;
                    }
                }

                return rects;
            }
        }
    }

    [Obsolete("Have bug")]
    public class AlgoCudaHog : IDetect
    {
        public Rectangle[] Detect(Image<Gray, byte> grayframe)
        {
            using (CudaHOG des = new CudaHOG(new Size(64, 128), new Size(16, 16), new Size(8, 8), new Size(8, 8)))
            {
                des.SetSVMDetector(des.GetDefaultPeopleDetector());

                using (GpuMat cudaBgra = new GpuMat())
                using (VectorOfRect vr = new VectorOfRect())
                {
                    CudaInvoke.CvtColor(grayframe, cudaBgra, ColorConversion.Bgr2Bgra);
                    des.DetectMultiScale(cudaBgra, vr);
                    var regions = vr.ToArray();

                    return regions;
                }
            }
        }
    }
    public class AlgoHaar : IDetect
    {
        private static CascadeClassifier _haarClassifier = new CascadeClassifier(Application.StartupPath + ConfigurationManager.AppSettings["haarPath"]);

        public Rectangle[] Detect(Image<Gray, byte> grayframe)
        {
            var targets = _haarClassifier.DetectMultiScale(grayframe, 1.1, 10,
                Size.Empty);
            return targets;
        }
    }

    public class AlgoHaarFullBody : IDetect
    {
        private static CascadeClassifier _haarClassifier = new CascadeClassifier(Application.StartupPath + ConfigurationManager.AppSettings["haarFullBody"]);

        public Rectangle[] Detect(Image<Gray, byte> grayframe)
        {
            var targets = _haarClassifier.DetectMultiScale(grayframe, 1.1, 10,
                Size.Empty);
            return targets;
        }
    }

    public class AlgoBodyCascade : IDetect
    {
        //https://drive.google.com/file/d/0B_kNUWF69Zs2N2JpZWkyLXNfSnc/view
        private static CascadeClassifier _haarClassifier = new CascadeClassifier(Application.StartupPath + ConfigurationManager.AppSettings["cascadePath"]);
        //detectorBody.detectMultiScale(img, human, 1.04, 4, 0 | 1, Size(30, 80), Size(80,200));

        public Rectangle[] Detect(Image<Gray, byte> grayframe)
        {
            var targets = _haarClassifier.DetectMultiScale(grayframe, 1.1, 10,
                Size.Empty);
            return targets;
        }
    }
    public class AlgoBodyCascade2 : IDetect
    {
        //https://drive.google.com/file/d/0B_kNUWF69Zs2N2JpZWkyLXNfSnc/view
        CascadeClassifier _detectorBody = new CascadeClassifier(Application.StartupPath + ConfigurationManager.AppSettings["cascadePath"]);

        //detectorBody.detectMultiScale(img, human, 1.04, 4, 0 | 1, Size(30, 80), Size(80,200));
        public Rectangle[] Detect(Image<Gray, byte> grayframe)
        {
            var targets = _detectorBody.DetectMultiScale(grayframe, 1.04, 10, new Size(30, 80), new Size(80, 200));
            return targets;
        }
    }

    public class AlgoCascadeHead : IDetect
    {
        //https://drive.google.com/file/d/0B_kNUWF69Zs2N2JpZWkyLXNfSnc/view
        private static CascadeClassifier _haarClassifier = new CascadeClassifier(Application.StartupPath + ConfigurationManager.AppSettings["cascadeHeadPath"]);
        //detectorHead.detectMultiScale(img, head, 1.1, 4, 0 | 1, Size(40, 40), Size(100, 100));
        public Rectangle[] Detect(Image<Gray, byte> grayframe)
        {
            var targets = _haarClassifier.DetectMultiScale(grayframe, 1.1, 10,
                Size.Empty);
            return targets;
        }
    }


    public class AlgoBodyTrainedByMe : IDetect
    {
        private static CascadeClassifier _haarClassifier = new CascadeClassifier(Application.StartupPath + ConfigurationManager.AppSettings["cascadeTrainedByMe"]);
        public Rectangle[] Detect(Image<Gray, byte> grayframe)
        {
            var targets = _haarClassifier.DetectMultiScale(grayframe, 1.1, 10,
                Size.Empty);
            return targets;
        }
    }
    //public class AlgoTf
    //{
    //    private MultiboxGraph graph;
    //    private bool _coldSession = true;
    //    public AlgoTf()
    //    {
    //        graph = new MultiboxGraph();
    //        graph.OnDownloadProgressChanged += (sender, args) => { };
    //        graph.OnDownloadCompleted += (sender, args) => { };
    //    }
    //    public void DetectAndSave(string fileName, string outputPath)
    //    {
    //        Tensor imageTensor = ImageIO.ReadTensorFromImageFile(fileName, 224, 224, 128.0f, 1.0f / 128.0f);

    //        var result = graph.Detect(imageTensor);

    //        //Here we are trying to time the execution of the graph after it is loaded
    //        //If we are not interest in the performance, we can skip the 3 lines that follows
    //        result = graph.Detect(imageTensor);
    //        //if (result.Scores.Length > 0)
    //        //{
    //        Bitmap bmp = new Bitmap(fileName);
    //        MultiboxGraph.DrawResults(bmp, result, 0.1f);
    //        var savingFile = MediaHelper.FileName(fileName);
    //        var newPAth = Path.Combine(outputPath, savingFile);
    //        bmp.Save(newPAth);
    //        //}
    //    }
    //}
}
