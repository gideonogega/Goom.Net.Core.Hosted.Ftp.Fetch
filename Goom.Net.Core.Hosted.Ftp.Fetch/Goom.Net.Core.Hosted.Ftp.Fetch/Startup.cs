using System;
using Goom.Net.Core.Hosted.Ftp.Fetch.Repos;
using Goom.Net.Core.Hosted.Ftp.Fetch.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Goom.Net.Core.Hosted.Ftp.Fetch
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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Goom.Net.Core.Hosted.Ftp.Fetch", Version = "v1" });
            });


            services.AddSingleton(new SftpEndpointInfo
            {
                Host = Environment.GetEnvironmentVariable("FTP_HOST"),
                Port = int.Parse(Environment.GetEnvironmentVariable("FTP_PORT")),
                InputDirectory = Environment.GetEnvironmentVariable("FTP_INPUT_DIR"),
                ArchiveDirectory = Environment.GetEnvironmentVariable("FTP_ARCHIVE_DIR"),
                Username = Environment.GetEnvironmentVariable("FTP_USER"),
                Password = Environment.GetEnvironmentVariable("FTP_PWD")
            });

            services.AddSingleton<IFtpRepo, FtpRepo>();
            services.AddSingleton<IFetchService, FetchService>();

            services.AddHostedService<ScheduledPollingService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Goom.Net.Core.Hosted.Ftp.Fetch v1"));
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
