using Microsoft.AspNetCore.Mvc;
using System.Web.Http.Cors;
namespace Etape1.Controllers;

[DisableCors]
[Route("/[controller]")]
[ApiController]
public class HomeController : ControllerBase
{
    
    [Route("hello")]
    [HttpGet]
    public object Hello()
    {
    return new { 
        etna = "Hello World"
    };
    }
}