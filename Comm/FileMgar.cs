using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FilesManager.Api.Comm
{
    public class FileMgar : IFileMagr
    {
        public int GetChunkCount(string Path)
        {
            var dirInfo = new System.IO.DirectoryInfo(Path);
            int totalFile = 0;
            totalFile += dirInfo.GetFiles().Length;
            return totalFile;
        }

        public async Task<bool> ChunkUpload(Stream stream, string path)
        {
            try
            {
                // Stream convert to byte
                byte[] fileBuf = new byte[stream.Length];
                stream.Read(fileBuf, 0, fileBuf.Length);
                //set current stream for begin start;
                stream.Seek(0, SeekOrigin.Begin);
                if (!File.Exists(path))
                {
                    using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(fileBuf, 0, fileBuf.Length);
                        fs.Close();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("写入文件出错：消息={0},堆栈={1}", ex.Message, ex.StackTrace));
            }
        }

        public  async Task<bool> MergeFiles(string[] fileName, string outFileName)
        {
            int b;
            int n = fileName.Length;
            FileStream[] fileIn = new FileStream[n];
            using (FileStream fileOut = new FileStream(outFileName, FileMode.Create))
            {
                for (int i = 0; i < n; i++)
                {
                    try
                    {
                        fileIn[i] = new FileStream(fileName[i], FileMode.Open);
                        while ((b = fileIn[i].ReadByte()) != -1)
                            fileOut.WriteByte((byte)b);
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return false;
                    }
                    finally
                    {
                        fileIn[i].Close();
                    }

                }
            }
            return true;
        }

        public void CheckFolder(string fileUrl)
        {
            if (!Directory.Exists(fileUrl))
            {
                Directory.CreateDirectory(fileUrl);
            }
        }


        public  void DeleteDirectoryOrFile(string directoryPath, string fileName)
        {
            string[] Directories = Directory.GetDirectories(directoryPath);
            for (int i = 0; i < Directories.Length; i++)
            {
                string directories = Directories[i].Replace(directoryPath, "");
                if (directories == fileName)
                {
                    Directory.Delete(Directories[i], true);
                }
            }

            string[] Files = Directory.GetFiles(directoryPath);
            for (int i = 0; i < Files.Length; i++)
            {
                string files = Files[i].Replace(directoryPath, "");
                if (files == fileName)
                {
                    File.Delete(Files[i]);
                }
            }
        }
    }
}
