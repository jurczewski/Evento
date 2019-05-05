using Evento.Core.Repositories;
using Evento.Infrastructure;
using Evento.Infrastructure.Mappers;
using Evento.Infrastructure.Repositories;
using Evento.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NLog.Extensions.Logging;
using NLog.Web;
using System.Text;

namespace Evento
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddJsonOptions(x => x.SerializerSettings.Formatting = Formatting.Indented);
            services.AddAuthorization(x => x.AddPolicy("HasAdminRole", p => p.RequireRole("admin")));
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITicketService, TicketService>();
            services.AddSingleton(AutoMapperConfig.Initialize());
            services.AddSingleton<IJwtHandler, JwtHandler>();

            var jwtSettings = Configuration.GetSection("jwt");
            services.Configure<JwtSettings>(jwtSettings);

            services.AddAuthentication().AddJwtBearer(o =>
            {
                o.IncludeErrorDetails = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtSettings.GetValue<string>("issuer"),
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.GetValue<string>("key")))
                };
            });

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            //var level = Configuration.GetSection("Logging:LogLevel:Default").GetValue<LogLevel>("Default");
            //loggerFactory.AddConsole(level);
            //loggerFactory.AddDebug(level);
            //loggerFactory.AddEventSourceLogger();
            loggerFactory.AddNLog();
            app.AddNLogWeb();
            env.ConfigureNLog("nlog.config");

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();

        }
    }
}
