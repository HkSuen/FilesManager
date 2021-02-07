using FilesManager.Api.Comm.Config;
using FilesManager.Api.Model;
using FilesManager.Api.Model.ReqAndRepModel;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FilesManager.Api.Comm.Filters
{
    public class FileTypeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            /* todo */
            ResponseModel res = new ResponseModel() { code = ResultCode.BUSINESS_ERROR};
            try
            {
                var Form = context.HttpContext.Request.Form;
                    string[] allowType = AllowType();
                    var fileType = Path.GetExtension(Form["fileName"]);
                    fileType = fileType.ToLower();
                    if (!allowType.Contains(fileType))
                    {
                        res.code = ResultCode.INTERFACE_FORBIDDEN;
                        res.msg = "接口禁止访问:未授权文件类型";
                        context.Result = new JsonResult(new { status = false });
                        return;
                    }
            }
            catch (Exception ex)
            {
                res.code = ResultCode.INTERFACE_FORBIDDEN;
                res.msg = "接口禁止访问:未授权文件类型";
                context.Result = new JsonResult(new { status = false });
                return;
            }
        }


        private string[] AllowType()
        {
            string AllowType = Appsettings.Configuration.GetSection("ConfigSetting:AllowType").Value;
            if (!string.IsNullOrEmpty(AllowType))
            {
                AllowType = AllowType.ToLower();
                return AllowType.Split(";");
            }
            return null;
        }

    }
}
