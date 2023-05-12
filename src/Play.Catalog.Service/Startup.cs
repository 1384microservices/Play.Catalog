using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Play.Catalog.Service.Entities;
using Play.Common.Configuration;
using Play.Common.HealthChecks;
using Play.Common.Identity;
using Play.Common.Logging;
using Play.Common.MassTransit;
using Play.Common.MongoDB;
using Play.Common.Settings;

namespace Play.Catalog.Service;

public class Startup
{
    private const string AllowedOriginSetting = "AllowedOrigin";

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddMongo()
            .AddMongoRepository<Item>("Items")
            .AddMassTransitWithMessageBroker(Configuration)
            .AddJwtBearerAuthentication();

        services.AddAuthorization(opt =>
        {
            opt.AddPolicy(Policies.Read, policy =>
            {
                policy.RequireRole("Admin");
                policy.RequireClaim("scope", "catalog.readaccess", "catalog.fullaccess");
            });

            opt.AddPolicy(Policies.Write, policy =>
            {
                policy.RequireRole("Admin");
                policy.RequireClaim("scope", "catalog.writeaccess", "catalog.fullaccess");
            });
        });

        services.AddControllers(opt => opt.SuppressAsyncSuffixInActionNames = false);

        services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "Play.Catalog.Service", Version = "v1" }));

        services.AddHealthChecks().AddMongoDb();

        services.AddSeqLogging(Configuration.GetSeqSettings());
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Play.Catalog.Service v1"));
            app.UseCors(cfg =>
            {
                cfg.WithOrigins(Configuration[AllowedOriginSetting])
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
        }

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapPlayEconomyHealthChecks();
        });
    }
}
