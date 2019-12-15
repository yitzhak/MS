using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClothingStore.WebApi.Middleware
{
    public class JWTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public JWTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var JWToken = context.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(JWToken))
            {
                context.Request.Headers.Add("Authorization", "Bearer " + JWToken);
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
