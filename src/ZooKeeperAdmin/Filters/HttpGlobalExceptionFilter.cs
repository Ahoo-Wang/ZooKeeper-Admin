using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using ZooKeeperAdmin.Message;

namespace ZooKeeperAdmin.Filters
{
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IHostingEnvironment env;
        private readonly ILogger<HttpGlobalExceptionFilter> logger;

        public HttpGlobalExceptionFilter(IHostingEnvironment env, ILogger<HttpGlobalExceptionFilter> logger)
        {
            this.env = env;
            this.logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            logger.LogError(
                new EventId(exception.HResult),
                exception,
                exception.Message);
            var errorResp = new ResponseMessage
            {
                Message = exception.Message,
                ErrorCode = "0001",
                IsSuccess = false
            };
            var result = new JsonResult(errorResp);
            result.StatusCode = (int)HttpStatusCode.OK;
            context.Result = result;
            context.ExceptionHandled = true;
        }
    }
}
