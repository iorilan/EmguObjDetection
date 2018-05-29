using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace EmguObjDetect
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Capture _capture;
        private Timer _timer_drawing = new Timer();
        private IList<Point> _points;
        private bool _done = false;
        private Size _frameSize;
        Timer _timerDetect = new Timer();
        private ObjTest _objTest;
        private BackgroundWorker _bgWorker;
        private void RenderPolygon()
        {
            using (var imageFrame = _capture.QueryFrame().ToImage<Bgr, Byte>())
            {
                if (imageFrame != null)
                {
                    _frameSize = imageFrame.Size;


                    if (_done)
                    {
                        imageFrame.DrawPolyline(_points.ToArray(), true, new Bgr(Color.Green), 1);
                    }
                    else
                    {
                        for (int i = 0; i < _points.Count; i++)
                        {
                            imageFrame.Draw(new CircleF(_points[i], 1), new Bgr(Color.Orange),
                                1);
                        }

                    }
                    var bmp = EmguHelper.ResizeImage(imageFrame.ToBitmap(), new Size(pictureBox1.Width, pictureBox1.Height));
                    pictureBox1.Image = bmp;
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            _capture = new Capture();
            _points = new List<Point>();

            _timer_drawing = new Timer();
            _timer_drawing.Tick += (e1, o) =>
            {
                RenderPolygon();
            };
            _timer_drawing.Interval = 200;
            _timer_drawing.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_points == null || _points.Count == 0)
            {
                MessageBox.Show("Must draw a polygon first!!!");
            }

            _timer_drawing.Stop();


            if (_objTest == null)
            {
                _objTest = new ObjTest(_points.ToArray());
            }
            else
            {
                _objTest.ResetPoly(_points.ToArray());
            }


            if (_bgWorker != null)
            {
                _bgWorker.Dispose();
                _bgWorker = null;
            }
            _bgWorker = new BackgroundWorker();
            _bgWorker.DoWork += (o, args) =>
            {
                _objTest.Do(pictureBox1, () =>
                {
                    lblAlarm.CrossThreadSafeCall(() =>
                    {
                        lblAlarm.Tag = DateTime.Now;
                        lblAlarm.Text = DateTime.Now + " Intrusion";
                    });
                });


                if (lblAlarm.Tag != null)
                {
                    if (((DateTime)lblAlarm.Tag).AddSeconds(3) < DateTime.Now)
                    {
                        lblAlarm.Tag = null;
                        lblAlarm.CrossThreadSafeCall(() => { lblAlarm.Text = ""; });
                    }
                }
            };


            _timerDetect.Interval = 100;
            _timerDetect.Tick += (o, args) =>
            {
                if (!_bgWorker.IsBusy)
                {
                    _bgWorker.RunWorkerAsync();
                }
            };
            _timerDetect.Start();
        }



        private void button2_Click(object sender, EventArgs e)
        {
            _timerDetect.Stop();
            _points.Clear();
            _done = false;
            _timer_drawing.Start();
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            _done = true;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            var ratioX = pictureBox1.Width * 1.0 / _frameSize.Width;
            var ratioY = pictureBox1.Height * 1.0 / _frameSize.Height;
            var newX = (int)(e.X / ratioX);
            var newY = (int)(e.Y / ratioY);
            var p = new Point(newX, newY);

            if (_done)
            {
                if (Helper.IsPointInPolygon(p, _points.ToArray()))
                {
                    MessageBox.Show("clicked location in polygon");
                }
                return;
            }
            else
            {
                _points.Add(p);
            }

        }
    }
}
