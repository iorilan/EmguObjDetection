using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Windows.Forms;

namespace FFImgProducer
{
    public class MediaHelper
    {
        public static string FFmpegImgfolder()
        {
            var ffPath = ConfigurationManager.AppSettings["ffmpegFolder"];
            return ffPath;
        }

        public static string VcaImgfolder()
        {
            var vcaPath = ConfigurationManager.AppSettings["vcaFolder"];

            return vcaPath;
        }

        public static string FileName(string path)
        {
            var fileName = path.Split('\\').Last();
            return fileName;
        }

        public static string UniquePath(string path)
        {
            var fileName = FileName(path);
            var noExension = fileName.Split('.').First();
            return path.Replace(noExension, Guid.NewGuid().ToString());
        }

        public static void EnsureDirCreated(params string[] path)
        {
            foreach (var s in path)
            {
                if (!Directory.Exists(s))
                {
                    Directory.CreateDirectory(s);
                }
            }
        }
    }
}
