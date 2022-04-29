using Blog.Core.Domain.Contracts;
using Blog.Core.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using Notie.Contracts;
using Notie.Models;

namespace Blog.Api.Controllers.v1;

[ApiController]
[ApiVersion(Version)]
[Route("api/v{version:apiVersion}/[controller]")]
public class PostsController : Controller
{
    private const string Version = "1.0";

    private readonly INotifier _notifier;
    private readonly IPostService _service;

    public PostsController (IPostService service, INotifier notifier)
    {
        _service = service;
        _notifier = notifier;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<PostDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(List<Notification>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> All ([FromQuery] bool onlyResume)
    {
        return Ok(await _service.List(onlyResume));
    }

    [HttpPost]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(List<Notification>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(List<Notification>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Store ([FromBody] AddPostDto data)
    {
        var result = await _service.Add(data);
        return _notifier.HasNotifications()
            ? BadRequest(_notifier.All())
            : StatusCode(StatusCodes.Status201Created, result);
    }
}
