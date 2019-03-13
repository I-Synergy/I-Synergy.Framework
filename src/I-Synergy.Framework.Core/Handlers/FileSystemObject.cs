using System.IO;
using System.Linq;

namespace ISynergy.Handlers
{
    public static class FileSystemObject
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
            var filedetails = new FileInfo(path);
            return filedetails.Length;
        }

        public static string GetExtention(string path)
        {
            var result = string.Empty;

            var fileparts = path.Split('.');

            if (fileparts.Count() > 0)
            {
                result = fileparts[fileparts.Count() - 1];
            }

            return result;
        }

        public static string GetFileDescription(string path)
        {
            var result = string.Empty;

            var fileparts = path.Split('.');

            if (fileparts.Count() > 0)
            {
                var folderparts = fileparts[0].Split('\\');

                if (folderparts.Count() > 0)
                {
                    result = folderparts[folderparts.Count() - 1];
                }
            }

            return result;
        }
    }
}