using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers;

[Route("api/v{version:ApiVersion}/teste")]
[ApiController]
[ApiVersion("1.0", Deprecated = true)]
public class TesteV1Controler : ControllerBase
{
    [HttpGet]
    public string GetVersion()
    {
        return "Teste V1 - GET - Api Versão 1.0";
    }
}
