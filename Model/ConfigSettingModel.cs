using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilesManager.Api.Model
{
    public class ConfigSettingModel
    {
        public string FilesPath { get; set; }
        public string AllowType { get; set; }
        public long UploadSizeLimit { get; set; }
        public long DownloadSizeLimit { get; set; }
    }
}
