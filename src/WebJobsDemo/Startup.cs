using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UkrGuru.SqlJson;

namespace WebJobsDemo
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
            services.AddWebJobs(connString: Configuration.GetConnectionString("SqlJsonConnection"),
                logLevel: Configuration.GetValue<LogLevel>("WJbSettings:LogLevel"),
                nThreads: Configuration.GetValue<int>("WJbSettings:NThreads"));

            services.AddRazorPages();

            InitDemoDb();

            static void InitDemoDb()
            {
                var assembly = Assembly.GetExecutingAssembly();
                var product_name = assembly.GetName().Name;
                var product_version = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;

                var db_version = "0.0.0";
                try { db_version = DbHelper.FromProcAsync($"WJbSettings_Get", new { Name = product_name }).Result; } catch { }

                if (db_version.CompareTo(product_version) < 0)
                {
                    var version_file = $"{product_name}.Resources.{db_version ?? "0.0.0"}.sql";

                    var files = assembly.GetManifestResourceNames().Where(s => s.EndsWith(".sql")).OrderBy(s => s);
                    foreach (var file in files)
                    {
                        if (file.CompareTo(version_file) >= 0) assembly.ExecScript(file);
                    }

                    try { DbHelper.ExecProcAsync($"WJbSettings_Set", new { Name = product_name, Value = product_version }).Wait(); } catch { }
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
