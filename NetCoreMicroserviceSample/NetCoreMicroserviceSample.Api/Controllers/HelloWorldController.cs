using Microsoft.AspNetCore.Mvc;

namespace NetCoreMicroserviceSample.Api.Controllers
{
    public interface IBusinessLogic
    {
        string GetNiceGreeting();
        string GetNotSoNiceGreeting();
    }

    public class BusinessLogic : IBusinessLogic
    {
        public string GetNiceGreeting() => "Hello" + " World!";
        public string GetNotSoNiceGreeting() => "What do you want???";
    }

    [Route("api/[controller]")]
    [ApiController]
    public class HelloWorldController : ControllerBase
    {
        private readonly IBusinessLogic bl;

        public HelloWorldController(IBusinessLogic bl)
        {
            this.bl = bl;
        }

        [HttpGet]
        public string HelloWorld([FromQuery] int? howNice)
        {
            howNice ??= 1;

            return howNice switch
            {
                1 => bl.GetNiceGreeting(),
                2 => bl.GetNotSoNiceGreeting(),
                _ => string.Empty,
            };
        }
    }
}
