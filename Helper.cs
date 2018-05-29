using System;
using System.Drawing;
using System.Windows.Forms;

namespace EmguObjDetect
{
   public static class Helper
    {
        public static void CrossThreadSafeCall(this Control ctl, Action action)
        {
            if (ctl.InvokeRequired)
            {
                ctl.BeginInvoke((MethodInvoker)delegate ()
                {
                    action();
                });
            }
            else
            {
                action();
            }
        }


        public static bool IsPointInPolygon(Point point, Point[] polygon)
        {
            int polygonLength = polygon.Length, i = 0;
            bool inside = false;
            // x, y for tested point.
            float pointX = point.X, pointY = point.Y;
            // start / end point for the current polygon segment.
            float startX, startY, endX, endY;
            Point endPoint = polygon[polygonLength - 1];
            endX = endPoint.X;
            endY = endPoint.Y;
            while (i < polygonLength)
            {
                startX = endX; startY = endY;
                endPoint = polygon[i++];
                endX = endPoint.X; endY = endPoint.Y;
                //
                inside ^= (endY > pointY ^ startY > pointY) /* ? pointY inside [startY;endY] segment ? */
                          && /* if so, test if it is under the segment */
                          ((pointX - endX) < (pointY - endY) * (startX - endX) / (startY - endY));
            }
            return inside;
        }
    }
}
