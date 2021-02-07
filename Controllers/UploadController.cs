using FilesManager.Api.Comm;
using FilesManager.Api.Comm.Filters;
using FilesManager.Api.Model;
using FilesManager.Api.Model.ReqAndRepModel;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FilesManager.Api.Controllers
{
    public class UploadController : BaseController
    {
        private static IFileMagr _fileManager;
        private static string _uploadDirectory = "/UploadFiles/";
        public UploadController(IFileMagr fileMagr, IWebHostEnvironment hostingEnvironment, IOptions<ConfigSettingModel> configSettings, ILogger<Controller> logger)
            : base(hostingEnvironment, configSettings, logger)
        {
            _fileManager = fileMagr;
        }

        /// <summary>
        /// 此接口需要的参数
        /// {
        ///     fileKey: new Date().getTime(),
        ///     sliceSize: 1024 * 1024 * 1,//2M
        ///     size: file.size,
        ///     fileName: file.name,
        ///     fileType: file.type,
        ///     chunkIndex: 0,
        ///     chunk: null
        /// }
        /// </summary>
        /// <returns></returns>
        /// 
        [FileType]
        [HttpPost("filesAsync")]
        public async Task<IActionResult> FilesAsync()
        {
            ResponseModel response = new ResponseModel() { code = ResultCode.SCCUESS};
            bool End = false;
            int chunkCount = 0, chunkIndex = 0;
            string FileName = string.Empty;
            try
            {
                Dictionary<string, object> paramDatas = _GetFormData;
                string fileKey = paramDatas["fileKey"]?.ToString(),
                    fileName = paramDatas["fileName"].ToString();
                string fileUrl = _uploadDirectory + fileKey+"/";
                string Path = _webRootPath + fileUrl;
                int size = Convert.ToInt32(paramDatas["size"].ToString());
                chunkCount = Convert.ToInt32(paramDatas["chunkCount"].ToString());
                chunkIndex = Convert.ToInt32(paramDatas["chunkIndex"].ToString());
                bool IsEnd = chunkIndex == chunkCount;
                var Files = _GetFiles;
                if (Files != null && Files.Count() > 0)
                {
                    foreach (var file in Files)
                    {
                        Stream fileStream = file.OpenReadStream();
                        _fileManager.CheckFolder(Path);
                        if (!IsEnd)
                        {
                            _fileManager.ChunkUpload(fileStream, (Path + fileKey + chunkIndex));
                        }
                        else
                        {
                            await _fileManager.ChunkUpload(fileStream, (Path + fileKey + chunkIndex));
                        }
                    }
                    if (IsEnd)
                    {
                        var fileCount = _fileManager.GetChunkCount(Path);
                        while (chunkCount >= fileCount)
                        {
                            if (chunkCount == _fileManager.GetChunkCount(Path))
                            {
                                List<string> FileChunkNames = new List<string>();
                                for (int i = 1; i <= chunkCount; i++)
                                {
                                    FileChunkNames.Add((Path + fileKey + i));
                                }
                                string PathFile = _ConfigSettings.Value.FilesPath;
                                _fileManager.CheckFolder(PathFile);
                                FileName = fileKey + "_" + fileName;
                                End = await _fileManager.MergeFiles(FileChunkNames.ToArray(), PathFile + FileName);
                                if (End)
                                {
                                    _fileManager.DeleteDirectoryOrFile(_webRootPath + _uploadDirectory, fileKey);
                                }
                                break;
                            }
                            fileCount = _fileManager.GetChunkCount(Path);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.code = ResultCode.SYSTEM_INNER_ERROR;
                response.msg = ex.ToString();
                return Ok(response);
            }
            response.data = new { status = true, end = End, chunkIndex, chunkCount, fileName = FileName };
            return Ok(response);
        }


        [HttpGet("DownloadBigFile/{fileName}")]
        public IActionResult DownloadFile(string fileName = "")
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return Ok(new { Error = "File Not Found !" });
            }
            var filePath = _ConfigSettings.Value.FilesPath + fileName;
            int bufferSize = 1024;
            Response.ContentType = MimeMapping.GetMimeMapping(fileName);
            var contentDisposition = "attachment;" + "filename=" + System.Web.HttpUtility.UrlEncode(fileName);
            Response.Headers.Add("Content-Disposition", new string[] { contentDisposition });
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (Response.Body)
                {
                    long contentLength = fs.Length;
                    Response.ContentLength = contentLength;
                    byte[] buffer;
                    long hasRead = 0;
                    while (hasRead < contentLength)
                    {
                        if (HttpContext.RequestAborted.IsCancellationRequested)
                        {
                            break;
                        }
                        buffer = new byte[bufferSize];
                        int currentRead = fs.Read(buffer, 0, bufferSize);
                        Response.Body.WriteAsync(buffer, 0, currentRead);
                        Response.Body.Flush();//注意每次Write后，要及时调用Flush方法，及时释放服务器内存空间
                        hasRead += currentRead;//更新已经发送到客户端浏览器的字节数
                    }
                }
            }

            return new EmptyResult();
        }
    }
}
