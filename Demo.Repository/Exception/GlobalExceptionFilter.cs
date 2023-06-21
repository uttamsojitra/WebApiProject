using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Business.Exception
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "An unhandled exception occurred.");

            // Handle the exception and generate an appropriate response
            var errorResponse = new ErrorResponse
            {
                Message = context.Exception.Message.ToString(),
                ErrorCode = 500
            };

            switch (context.Exception)
            {
                case ArgumentException :
                    errorResponse.Message = MessageHelper.InvalidUser;
                    errorResponse.ErrorCode = 400;
                    break;
                case InvalidOperationException :
                    errorResponse.Message = MessageHelper.UserNotRemove;
                    errorResponse.ErrorCode = 401;
                    break;
                default:
                    break;
            }

            context.Result = new ObjectResult(errorResponse)
            {
                StatusCode = 500
            };

            // Prevent the exception from being re-thrown
            context.ExceptionHandled = true;
        }
    }
}
