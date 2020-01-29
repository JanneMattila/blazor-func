using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using SharedLibrary;

namespace Func
{
    public static class WeatherFunction
    {
        [FunctionName("Weather")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Requesting weather data.");

            var random = new Random();
            var list = new List<WeatherForecast>();
            var summaries = new List<string>() { "Cold", "Warm", "Freezing", "Sunny" };
            for (int i = 0; i < 10; i++)
            {
                list.Add(new WeatherForecast()
                {
                    Date = DateTime.Now.AddDays(-i),
                    Summary = summaries[random.Next(summaries.Count)],
                    TemperatureC = random.Next(-15, 35)
                });
            }

            return new OkObjectResult(list);
        }
    }
}
