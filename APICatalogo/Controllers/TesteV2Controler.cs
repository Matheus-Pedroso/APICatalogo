using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers;

[Route("api/v{version:ApiVersion}/teste")]
[ApiController]
[ApiVersion("2.0")]
public class TesteV2Controler : ControllerBase
{
    [HttpGet]
    public string GetVersion()
    {
        return "Teste V1 - GET - Api Versão 2.0";
    }
}
