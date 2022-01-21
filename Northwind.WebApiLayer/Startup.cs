using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Northwind.BusinessLogicLayer.Concrete.BusinessLogicManagers;
using Northwind.DataAccessLayer.Abstract.IRepository;
using Northwind.DataAccessLayer.Abstract.UnitOfWorkRepository;
using Northwind.DataAccessLayer.Concrete.EntityFramework.Context;
using Northwind.DataAccessLayer.Concrete.EntityFramework.EfRepository;
using Northwind.DataAccessLayer.Concrete.EntityFramework.UnitOfWorkRepository;
using Northwind.InterfaceLayer.Abstract.ModelService;

namespace Northwind.WebApiLayer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {



            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(cfiguration =>
            {
                cfiguration.SaveToken = true;
                cfiguration.RequireHttpsMetadata = false;

                cfiguration.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    RoleClaimType = "Roles",
                    ClockSkew = TimeSpan.FromMinutes(5),
                    ValidateLifetime = true,
                    ValidIssuer = Configuration["Tokens:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = Configuration["Tokens:Issuer"],
                    RequireSignedTokens = true,
                    RequireExpirationTime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"]))
                };
            });



            services.AddScoped<DbContext, NorthwindContext>();
            services.AddDbContext<NorthwindContext>(DbContextOptionsBuilder =>
            {
                DbContextOptionsBuilder.UseSqlServer(Configuration.GetConnectionString("SqlServer"),
                    SqlServerDbContextOptionsBuilder =>
                    {
                        SqlServerDbContextOptionsBuilder.MigrationsAssembly("Northwind.DataAccessLayer");
                    });
            });

   


            services.AddScoped<ICustomerService, CustomerManager>();
            services.AddScoped<ICategoryService, CategoryManager>();
            services.AddScoped<ICustomerDemographicService, CustomerDemographicManager>();
            services.AddScoped<IEmployeeService, EmployeeManager>();
            services.AddScoped<IEmployeeTerritoryService, EmployeeTerritoryManager>();
            services.AddScoped<IOperationClaimService, OperationClaimManager>();
            services.AddScoped<IOrderService, OrderManager>();
            services.AddScoped<IOrderDetailService, OrderDetailManager>();
            services.AddScoped<IProductService, ProductManager>();
            services.AddScoped<IRegionService, RegionManager>();
            services.AddScoped<IShipperService, ShipperManager>();
            services.AddScoped<ISupplierService, SupplierManager>();
            services.AddScoped<ITerritoryService, TerritoryManager>();
            services.AddScoped<IUserService, UserManager>();
            services.AddScoped<IUserOperationClaimService, UserOperationClaimManager>();



            services.AddScoped<ICustomerRepository, EfCustomerRepository>();
            services.AddScoped<ICategoryRepository, EfCategoryRepository>();
            services.AddScoped<ICustomerDemographicRepository, EfCustomerDemographicRepository>();
            services.AddScoped<IEmployeeRepository, EfEmployeeRepository>();
            services.AddScoped<IEmployeeTerritoryRepository, EfEmployeeTerritoryRepository>();
            services.AddScoped<IOperationClaimRepository, EfOperationClaimRepository>();
            services.AddScoped<IOrderRepository, EfOrderRepository>();
            services.AddScoped<IOrderDetailRepository, EfOrderDetailRepository>();
            services.AddScoped<IProductRepository, EfProductRepository>();
            services.AddScoped<IRegionRepository, EfRegionRepository>();
            services.AddScoped<IShipperRepository, EfShipperRepository>();
            services.AddScoped<ITerritoryRepository, EfTerritoryRepository>();
            services.AddScoped<IUserRepository, EfUserRepository>();
            services.AddScoped<IUserOperationClaimRepository, EfUserOperationClaimRepository>();
            


            services.AddScoped<IUnitOfWorkRepository, UnitOfWorkRepository>();



            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy",
                    builder => builder
                    .WithOrigins("www.api.com")
                    .AllowAnyOrigin()
                    .WithMethods("GET","POST")
                    .AllowAnyMethod()
                    .WithHeaders("accept","content-type")
                    .AllowAnyHeader()
                    .AllowCredentials()
                );
            });


            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Northwind.WebApiLayer", Version = "v1" });
            });
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Northwind.WebApiLayer v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
