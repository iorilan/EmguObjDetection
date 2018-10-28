using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFImgProducer
{
    class Program
    {
        static void Main(string[] args)
        {
            new VcaPhotoMover().StartAsync();
            FFmpegImageProducer.Startffmpeg();

            Console.ReadLine();
        }
    }
}
