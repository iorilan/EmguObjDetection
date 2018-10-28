using System;
using System.Configuration;
using System.Diagnostics;

namespace FFImgProducer
{
    /// <summary>
    /// A class manage ffmpeg process to run command chop images.
    /// Also maintain a certain number of images under folder
    /// </summary>
    public class FFmpegImageProducer
    {
        public static void Startffmpeg()
        {
            var rtsp = ConfigurationManager.AppSettings["rtsp"];

            var pathFF = MediaHelper.FFmpegImgfolder();
            MediaHelper.EnsureDirCreated(pathFF);

            var fps = ConfigurationManager.AppSettings["fps"];

            var cmd = $"-i {rtsp} -vf fps={fps} {pathFF}\\ffout%d.png";
            var ffPath = ConfigurationManager.AppSettings["ffPath"];
            RunProcessBlock(cmd, ffPath);
        }

        private static void RunProcessBlock(string arguments, string ffmpeg)
        {
            try
            {
                var process = new Process
                {
                    StartInfo =
                    {
                        Arguments = arguments,
                        FileName = ffmpeg,
                        WindowStyle = ProcessWindowStyle.Normal
                    }
                };

                process.Exited += (sender, eventArgs) => { System("ffmpeg Existed."); };
                process.ErrorDataReceived += (sender, eventArgs) => { System("ffmpeg Error"); };

                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void System(string message)
        {
            Console.WriteLine("[System] :" + message);
        }
    }
}
