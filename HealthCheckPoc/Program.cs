using Grpc.Core;
using Grpc.Health.V1;
using Grpc.HealthCheck;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using static Grpc.Health.V1.HealthCheckResponse.Types;

namespace HealthCheckPoc
{
    public class Program
    {
        //these would be parameterised - just for POC here
        public static readonly string GrpcHost = "127.0.0.1";
        public static readonly int GrpcPort = 23456;

        public static readonly string NotServingService = "notservingservice";
        public static readonly string AvailableService = "availableservice";
        public static readonly string UnknownStatusService = "unknownstatusservice";

        public static void Main(string[] args)
        {
            var healthService = new HealthServiceImpl();

            //set the status for all services 
            healthService.SetStatus(NotServingService, ServingStatus.NotServing);
            healthService.SetStatus(AvailableService, ServingStatus.Serving);
            healthService.SetStatus(UnknownStatusService, ServingStatus.Unknown);

            //set a status for the overall server health using empty string as the service name
            healthService.SetStatus(string.Empty, ServingStatus.Serving);

            //configure and lanch the grpc service
            var server = new Server
            {
                Services =
                {
                    Health.BindService(healthService)
                },
                Ports = { { GrpcHost, GrpcPort, ServerCredentials.Insecure } }
            };
            server.Start();

            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();


    }
}
