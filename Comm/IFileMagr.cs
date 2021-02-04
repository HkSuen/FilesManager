using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FilesManager.Api.Comm
{
    public interface IFileMagr
    {
        int GetChunkCount(string Path);
        Task<bool> ChunkUpload(Stream stream, string path);
        Task<bool> MergeFiles(string[] fileName, string outFileName);
        void CheckFolder(string fileUrl);
        void DeleteDirectoryOrFile(string directoryPath, string fileName);
    }
}
