using FilesManager.Api.Model;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilesManager.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BaseController : ControllerBase
    {
        protected readonly IOptions<ConfigSettingModel> _ConfigSettings;
        protected readonly ILogger<Controller> _logger;
        protected static IWebHostEnvironment _hostingEvironment;
        public BaseController(IWebHostEnvironment hostingEnvironment, IOptions<ConfigSettingModel> configSettings,
            ILogger<Controller> logger)
        {
            _hostingEvironment = hostingEnvironment;
            _ConfigSettings = configSettings;
            _logger = logger;
        }


        protected string _webRootPath { 
            get
            {
                return (_hostingEvironment.WebRootPath ?? _hostingEvironment.ContentRootPath);
            } 
        }

        protected IEnumerable<IFormFile> _GetFiles
        {
            get
            {
                return Request.Form.Files;
            }
        }

        protected Dictionary<string, object> _GetFormData
        {
            get
            {
                Dictionary<string, object> formDatas = null;
                if (Request.Form.Keys.Count > 0)
                {
                    formDatas = new Dictionary<string, object>();
                    foreach (string _key in Request.Form.Keys)
                    {
                        formDatas.Add(_key, Request.Form[_key]);
                    }
                }
                return formDatas;
            }
        }

    }
}
