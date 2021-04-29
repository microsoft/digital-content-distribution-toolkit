using blendnet.api.proxy;
using blendnet.api.proxy.Cms;
using blendnet.common.dto.Oms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.oms.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private ContentProxy _contentProxy;
        private SubscriptionProxy _subscriptionProxy;
        private IOptionsMonitor<OmsAppSettings> _optionsMonitor;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,
            ContentProxy contentProxy, SubscriptionProxy subscriptionProxy, IOptionsMonitor<OmsAppSettings> optionsMonitor)
        {
            _logger = logger;
            _contentProxy = contentProxy;
            _subscriptionProxy = subscriptionProxy;
            _optionsMonitor = optionsMonitor;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.LogInformation("Test");


            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
