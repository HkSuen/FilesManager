using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilesManager.Api.Model.ReqAndRepModel
{
    public class ResponseModel
    {
        public ResultCode code { get; set; }
        public object data { get; set; }
        public string msg { get; set; }
    }


}
