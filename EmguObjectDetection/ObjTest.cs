using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;

namespace EmguObjDetect
{
    public class ObjTest
    {
        private CascadeClassifier _cascadeClassifier;
        private Capture _capture;
        private Point[] _points;
        public ObjTest(Point[] points)
        {
            _points = points;
            _capture = new Capture();
            _cascadeClassifier = new CascadeClassifier(Application.StartupPath + "../../../haarcascades/haarcascade_upperbody.xml");
        }

        public void ResetPoly(Point[] points)
        {
            _points = points;
        }

        public void Do(PictureBox pictureBox,Action callback)
        {
            using (var imageFrame = _capture.QueryFrame().ToImage<Bgr, Byte>())
            {
                if (imageFrame != null)
                {
                    var grayframe = imageFrame.Convert<Gray, byte>();
                    var faces = _cascadeClassifier.DetectMultiScale(grayframe, 1.1, 10,
                        Size.Empty); //the actual face detection happens here

                    foreach (var face in faces)
                    {
                        var c = CenterRect(face);
                        if (Helper.IsPointInPolygon(c, _points))
                        {
                            imageFrame.Draw(face, new Bgr(Color.Chartreuse),
                                1); //the detected face(s) is highlighted here using a box that is drawn around it/them

                            if (callback != null)
                            {
                                callback();
                            }
                        }
                    }


                    imageFrame.DrawPolyline(_points, true, new Bgr(Color.Crimson), 1);
                    var bmp = EmguHelper.ResizeImage(imageFrame.ToBitmap(), new Size(pictureBox.Width, pictureBox.Height));

                    pictureBox.Image = bmp;

                 
                }


            }
        }

        private Point CenterRect(Rectangle r)
        {
            return new Point(r.Left + r.Width / 2, r.Top + r.Height / 2);
        }
    }
}
