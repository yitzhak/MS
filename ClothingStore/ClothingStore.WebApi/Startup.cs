using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClothingStore.WebApi.Helpers;
using ClothingStore.WebApi.Middleware;
using ClothingStore.WebApi.Models;
using ClothingStore.WebApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
//using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NLog;
using Swashbuckle.AspNetCore.Swagger;

namespace ClothingStore.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            LogManager.LoadConfiguration(System.String.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(30);
            });
            services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("StoreConnection"));
            });

            // configure DI for application services
            services.AddScoped<IBaseServise, BaseServise>();
            services.AddScoped<ICanceledOrdersService, CanceledOrdersService>();
            services.AddScoped<ICheckoutService, CheckoutService>();
            services.AddScoped<ICouponsService,CouponsService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPaymentValidatorService, PaymentValidatorService>();
            services.AddScoped<IProductPiecesAvailableService, ProductPiecesAvailableService>();
            services.AddScoped<IProductsService, ProductsService>();
            services.AddScoped<IStatisticsService, StatisticsService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<Helpers.ILogger, LogNLog>();
            services.AddScoped<ITokenHandler, Helpers.TokenHandler>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ValidateModelStateAttribute));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            ConfigureJWTToken(services);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Api Docs", Version = "v1" });
            });
        }

        private void ConfigureJWTToken(IServiceCollection services)
        {
            string EncryptTokenKey = Configuration.GetValue<string>("EncryptTokenKey");
            //Provide a secret key to Encrypt and Decrypt the Token - JRozario
            var SecretKey = Encoding.ASCII.GetBytes(EncryptTokenKey);
            //Configure JWT Token Authentication - JRozario
            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(token =>
            {
                token.RequireHttpsMetadata = false;
                token.SaveToken = true;
                token.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(SecretKey),
                    ValidateIssuer = true,
                    //Usually this is your application base URL - JRozario
                    ValidIssuer = "http://localhost:45092/",
                    ValidateAudience = true,
                    //Here we are creating and using JWT within the same application. In this case base URL is fine - JRozario
                    //If the JWT is created using a web service then this could be the consumer URL - JRozario
                    ValidAudience = "http://localhost:45092/",
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //Addd User session - JRozario
            app.UseSession();

            //Add JWToken to all incoming HTTP Request Header - JRozario
            app.UseMiddleware<JWTokenMiddleware>();

            //Transaction Per Request https://stackoverflow.com/a/40616227/4871015
            app.UseMiddleware<HttpStatusCodeExceptionMiddleware>();
            app.UseMiddleware<TransactionPerRequestMiddleware>();

            //Add JWToken Authentication service - JRozario
            app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseMvc();

            #region swagger https://www.syncfusion.com/blogs/post/automatically-generate-api-docs-for-asp-net-core.aspx
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                c.RoutePrefix = string.Empty;
            }); 
            #endregion
        }
    }
}
