using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers;

[Route("api/teste")]
[ApiController]
[ApiVersion(3)]
[ApiVersion(4)]
public class TesteV3Controler : ControllerBase
{

    [MapToApiVersion(3)]
    [HttpGet]
    public string GetVersion3()
    {
        return "Version3 - Get - Api Versão 3";
    }

    [MapToApiVersion(4)]
    [HttpGet]
    public string GetVersion4()
    {
        return "Version4 - Get - Api Versão 4";
    }
}
