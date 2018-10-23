using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFmpegPhotoGenerator
{
    public class MediaHelper
    {
        public static string FFmpegImgfolder()
        {
            var appPath = Application.StartupPath;
            var ffImgsFolder = ConfigurationManager.AppSettings["ffmpegFolder"];
            var ffPath = Path.Combine(appPath, ffImgsFolder);
            return ffPath;
        }

        public static string VcaImgfolder()
        {
            var appPath = Application.StartupPath;
            var vcaFolder = ConfigurationManager.AppSettings["vcaFolder"];
            var vcaPath = Path.Combine(appPath, vcaFolder);

            return vcaPath;
        }

        public static string OutputImgfolder()
        {
            var appPath = Application.StartupPath;
            var resultFolder = ConfigurationManager.AppSettings["outputFolder"];
            var outputPath = Path.Combine(appPath, resultFolder);

            return outputPath;
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
