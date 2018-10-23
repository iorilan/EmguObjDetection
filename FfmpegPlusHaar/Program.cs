using System;

namespace FFmpegPhotoGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            new VCAPhotoMover().StartAsync();
            FFmpegImageProducer.Startffmpeg();

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
        }
    }
}
