using Grpc.Core;
using Grpc.Health.V1;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Grpc.Health.V1.Health;
using static Grpc.Health.V1.HealthCheckResponse.Types;

namespace HealthCheckPoc.Controllers
{
    public class HealthController : Controller
    {

        public IActionResult Info()
        {
            return View();
        }
        public string Check(string id = "")
        {
            var channel = new Channel(Program.GrpcHost, Program.GrpcPort, ChannelCredentials.Insecure);
            channel.ConnectAsync();

            var healthClient = new HealthClient(channel);
            var request = new HealthCheckRequest();

            request.Service = id;

            var responseText = string.Empty;

            try
            {
                var result = healthClient.Check(request);

                switch (result.Status)
                {
                    case ServingStatus.NotServing:
                        Response.StatusCode = 503;
                        break;
                    case ServingStatus.Unknown:
                        Response.StatusCode = 503;
                        break;
                    case ServingStatus.Serving:
                        Response.StatusCode = 200; //just to be clear
                        break;
                }
                
                responseText = result.Status.ToString();
            }
            catch (RpcException ex)
            {
                responseText = ex.Status.StatusCode.ToString();
                Response.StatusCode = 404;
            }

            return responseText 
                //adding http status code for POC
                + " |  HttpStatusCode: " + Response.StatusCode;
        }
    }

}
