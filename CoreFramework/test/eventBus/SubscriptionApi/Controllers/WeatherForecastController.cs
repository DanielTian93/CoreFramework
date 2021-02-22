using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.EventBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SubscriptionApi.Event;
using EventHandler = SubscriptionApi.Event.EventHandler;

namespace SubscriptionApi.Controllers
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
        private readonly IMessageSubscribe _messageSubscribe;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,IMessageSubscribe messageSubscribe)
        {
            _logger = logger;
            _messageSubscribe = messageSubscribe;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            _messageSubscribe.UnSubscribe<CustomerEvent,EventHandler>();
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
