using ClothingStore.WebApi.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClothingStore.WebApi.Services
{
    public interface IBaseServise
    {
        ILogger Logger { get; }
        IConfiguration Configuration { get; }
        //IExceptionHandler ExceptionHandler { get; }
        string UserName { get; }
        int UserId { get; }
    }

    public class BaseServise : IBaseServise
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BaseServise(ILogger logger,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            Logger = logger;
            Configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public ILogger Logger { get; }
        public IConfiguration Configuration { get; }

        string IBaseServise.UserName
        {
            get
            {
                string ClaimsUserName = _httpContextAccessor.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.Name).Value;
                return ClaimsUserName;
            }
        }

        int IBaseServise.UserId
        {
            get
            {
                string ClaimsUserId = _httpContextAccessor.HttpContext.User.FindFirst("USERID").Value;
                int.TryParse(ClaimsUserId, out int _id);
                return _id;
            }
        }
    }
}
