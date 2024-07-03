using Microsoft.AspNetCore.Mvc;

namespace web;

[Route("[controller]")]
[ApiController]
public class TestContoller : ControllerBase
{
    private readonly IConfiguration _config;
    public TestContoller(IConfiguration config)
    {
        this._config = config;
    }

    [HttpGet("checkResult/{key}")]
    public IActionResult Get(string key)
    {
        var obj = _config.GetSection(key).Get<SecretObject>();
        return Ok(obj);
    }
}

public class SecretObject
{
    public string Token { get; set; }
    public SecretToken SecretToken { get; set; }
}
public class SecretToken
{
    public string Token1 { get; set; }
    public string Token2 { get; set; }
}
