using ClothingStore.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothingStore.WebApi.Helpers
{
    public class HttpStatusCodeExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public HttpStatusCodeExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IBaseServise baseService)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                string errorMessage = GetErrorMessage(ex);
                LogError(context, baseService, ex, errorMessage);

                context.Response.Clear();
                context.Response.ContentType = @"application/json";
                if (ex is BadRequestException)
                {
                    BadRequestException custom = ex as BadRequestException;
                    context.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                    await context.Response.WriteAsync(custom.ExceptionMessage);
                }
                else
                {
                    context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                    await context.Response.WriteAsync("An error occurred in this action, please try again.");
                }
                return;
            }

        }

        private static void LogError(HttpContext context, IBaseServise baseService, Exception ex, string errorMessage)
        {
            Dictionary<string, string> errorDetails = new Dictionary<string, string>
                {
                    {"Path",context.Request.Path},
                    {"Message",errorMessage },
                    {"StackTrace",ex.StackTrace }
                };
            string errorInJson = JsonConvert.SerializeObject(errorDetails);
            baseService.Logger.Error(errorInJson);
        }

        private static string GetErrorMessage(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(ex.Message);
            Exception InnerException = ex.InnerException;
            while (InnerException != null)
            {
                sb.AppendLine(InnerException.Message);
                InnerException = InnerException.InnerException;
            }
            return sb.ToString();
        }
    }
}
