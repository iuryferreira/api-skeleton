using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Controllers;

[Consumes("application/json")]
[Produces("application/json")]
public abstract class Controller : ControllerBase
{
}
