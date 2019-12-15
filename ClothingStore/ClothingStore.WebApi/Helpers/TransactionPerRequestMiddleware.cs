using ClothingStore.WebApi.Models;
using ClothingStore.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClothingStore.WebApi.Helpers
{
    public class TransactionPerRequestMiddleware
    {
        private readonly RequestDelegate next_;

        public TransactionPerRequestMiddleware(RequestDelegate next)
        {
            next_ = next;
        }

        public async Task Invoke(HttpContext context, StoreContext dbContext,IBaseServise _baseServise)
        {
            var transaction = dbContext.Database.BeginTransaction(
                System.Data.IsolationLevel.ReadCommitted);

            await next_.Invoke(context);

            if (context.Response.StatusCode == 200 || context.Response.StatusCode == 204)
            {
                transaction.Commit();
            }
            else
            {
                transaction.Rollback();
                _baseServise.Logger.Error($"DB transaction was rollback during the request {context.Request.Path}");
            }
        }
    }
}
