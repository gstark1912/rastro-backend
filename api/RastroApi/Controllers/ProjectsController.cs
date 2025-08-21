using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rastro.Domain;
using RastroApi.Api.Projects;
using Rastro.Application.Abstractions;

namespace RastroApi.Controllers;

/// <summary>
/// API controller for managing user-scoped <see cref="Project"/> resources.
/// Provides endpoints for listing, retrieving, creating, updating, and deleting projects.
/// All actions require authentication.
/// </summary>
[ApiController]
[Route("api/projects")]
[Authorize]
public class ProjectsController : RastroControllerBase
{
    private readonly IUserScopedCrudService<Project> _service;

    public ProjectsController(IUserScopedCrudService<Project> service)
    {
        _service = service;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectResponse>>> List(int skip = 0, int take = 100, CancellationToken ct = default)
    {
        if (!IsUserAuthenticated) return Unauthorized();

        var items = await _service.ListAsync(UserId!, skip, Math.Clamp(take, 1, 200), ct);
        var result = items.Select(p => new ProjectResponse(p.Id, p.Title, p.Description));
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectResponse>> GetById(string id, CancellationToken ct = default)
    {
        if (!IsUserAuthenticated) return Unauthorized();

        var p = await _service.GetAsync(id, UserId!, ct);
        if (p is null) return NotFound();

        return Ok(new ProjectResponse(p.Id, p.Title, p.Description));
    }

    [HttpPost]
    public async Task<ActionResult<ProjectResponse>> Create([FromBody] ProjectCreateDto dto, CancellationToken ct = default)
    {
        if (!IsUserAuthenticated) return Unauthorized();

        var entity = new Project
        {
            Title = dto.Title,
            Description = dto.Description
        };

        var created = await _service.CreateAsync(entity, UserId!, ct);
        var res = new ProjectResponse(created.Id, created.Title, created.Description);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, res);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] ProjectUpdateDto dto, CancellationToken ct = default)
    {
        if (!IsUserAuthenticated) return Unauthorized();

        var entity = new Project
        {
            Id = id,
            Title = dto.Title,
            Description = dto.Description,
            UserId = UserId! // enforced again in service
        };

        var ok = await _service.UpdateAsync(entity, UserId!, ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken ct = default)
    {
        if (!IsUserAuthenticated) return Unauthorized();

        var ok = await _service.DeleteAsync(id, UserId!, ct);
        return ok ? NoContent() : NotFound();
    }
}
