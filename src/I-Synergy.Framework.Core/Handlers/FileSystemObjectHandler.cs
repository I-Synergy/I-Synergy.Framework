using System.IO;
using System.Linq;

namespace ISynergy.Common.Handlers
{
    public static class FileSystemObjectHandler
    {
        public static string EnsureDirectoryExists(string path)
        {
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        public static MemoryStream ConvertByteArray2Stream(byte[] bytes)
        {
            MemoryStream result = null;

            if (bytes != null)
            {
                result = new MemoryStream();
                result.Write(bytes, 0, bytes.Length);
                result.Position = 0;
            }

            return result;
        }

        public static long GetFileSize(string path)
        {
            //1MB = 128Kb = 1024Kbits = 131072Bytes = 1048576Bits
            //0,5Mb = 524288Bytes
            long iBytes = 0;

            FileInfo filedetails = new FileInfo(path);
            iBytes = filedetails.Length;

            return iBytes;
        }

        public static string GetExtention(string path)
        {
            string result = string.Empty;

            string[] fileparts = path.Split('.');

            if (fileparts.Count() > 0)
            {
                result = fileparts[fileparts.Count() - 1];
            }

            return result;
        }

        public static string GetFileDescription(string path)
        {
            string result = string.Empty;

            string[] fileparts = path.Split('.');

            if (fileparts.Count() > 0)
            {
                string[] folderparts = fileparts[0].Split('\\');

                if (folderparts.Count() > 0)
                {
                    result = folderparts[folderparts.Count() - 1];
                }
            }

            return result;
        }
    }
}