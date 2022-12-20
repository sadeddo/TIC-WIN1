using Microsoft.AspNetCore.Mvc;

namespace Etape1.Controllers;

[Route("Api/[controller]")]
[ApiController]
public class WeatherForecastController : ControllerBase
{
    [Route("Get")]
    [HttpGet]
    public string Get()
    {
        return "ihello word";
    }
}

