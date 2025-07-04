using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TS.EasyStockManager.Core.Repository;
using TS.EasyStockManager.Core.Service;
using TS.EasyStockManager.Core.UnitOfWorks;
using TS.EasyStockManager.Data.Context;
using TS.EasyStockManager.Repository.Base;
using TS.EasyStockManager.Service.Category;
using TS.EasyStockManager.Service.Product;
using TS.EasyStockManager.Service.Store;
using TS.EasyStockManager.Service.StoreStock;
using TS.EasyStockManager.Service.Transaction;
using TS.EasyStockManager.Service.UnitOfMeasure;
using TS.EasyStockManager.Service.User;

namespace TS.EasyStockManager.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Configure services
        public void ConfigureServices(IServiceCollection services)
        {
            // ?? DB context
            services.AddDbContext<EasyStockManagerDbContext>(options =>
            {
                options.UseSqlServer(Configuration["ConnectionStrings:SqlConStr"].ToString(), o =>
                {
                    o.MigrationsAssembly("TS.EasyStockManager.Data");
                });
            });

            // ?? AutoMapper & Dependency Injection
            services.AddAutoMapper(c => c.AddProfile<TS.EasyStockManager.Mapper.MapProfile>(), typeof(Startup));
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IUnitOfMeasureService, UnitOfMeasureService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IStoreStockService, StoreStockService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IUnitOfWorks, TS.EasyStockManager.UnitOfWork.UnitOfWork>();

            // ?? MVC & JSON config
            services.AddControllersWithViews()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                        options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    })
                    .AddRazorRuntimeCompilation();

            // ? Session support
            services.AddHttpContextAccessor();         // allows injecting IHttpContextAccessor
            services.AddSession();                     // adds session services

            // ?? Authentication config
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = new PathString("/auth/login");
                    options.Cookie.HttpOnly = true;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(120);
                    options.SlidingExpiration = true;
                });
        }

        // Configure middleware pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/Home/Error");

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            // ? Enable Session Middleware
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                ).RequireAuthorization();
            });
        }
    }
}
