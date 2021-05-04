using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Queue.Consumer.Services;
using Queue.Helper;
using Queue.QueryBuilder.Data;
using Queue.QueryBuilder.Helper;
using Queue.QueryBuilder.Service;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Queue.Consumer
{
    public class Program
    {
        public static Action<IConfigurationBuilder> BuildConfiguration =
           builder => builder
               .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
               .AddEnvironmentVariables();

        public static int Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            BuildConfiguration(builder);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .CreateLogger();

            try
            {
                Log.Information("Starting host");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseWindowsService()
                .ConfigureHostConfiguration(BuildConfiguration)

                .UseServiceProviderFactory(new AutofacServiceProviderFactory())

                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    builder.RegisterType<SecurityHelper>();
                    builder.RegisterType<SqlConnectionHelper>().As<ISqlConnectionHelper>();
                    builder.RegisterType<QueryBuilderService>().As<IQueryBuilderService>();
                    builder.RegisterType<DapperQueryService>().As<IDapperQueryService>();
                    builder.RegisterType<RabbitMqListenerService>().As<IRabbitMqListenerService>();
                })

                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;

                    WorkerOptions workerOptions = configuration.GetSection("WorkerOptions").Get<WorkerOptions>();
                    services.AddSingleton(workerOptions);

                    services.AddHostedService<Worker>();

                    services.AddDbContext<QueryBuilderDbContext>(options => options
                                             .UseSqlServer(configuration.GetValue<string>("ConnectionString"),
                                              providerOptions => providerOptions.CommandTimeout(configuration.GetValue<int>("ConnectionStringTimeOut")))
                                             .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll));
                })
                .UseWindowsService()
                .UseSerilog();
    }
}
