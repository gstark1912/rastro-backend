using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Rastro.Application.Abstractions;
using Rastro.Domain;

namespace RastroApi.Controllers;

[ApiController]
[Route("api/projects/{projectId}/[controller]")]
[Authorize]
public class MarkersController : RastroControllerBase
{
    private readonly IUserScopedCrudService<Marker> _service;

    public MarkersController(IUserScopedCrudService<Marker> service)
    {
        _service = service;
    }

    // GET: api/projects/{projectId}/markers
    [HttpGet]
    public async Task<IActionResult> GetByProject(string projectId, CancellationToken ct)
    {
        var result = await _service.ListAsync(this.UserId, ct: ct);
        var filtered = result.Where(m => m.ProjectId == projectId && m.IsActive);
        return Ok(filtered);
    }

    // GET: api/projects/{projectId}/markers/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string projectId, string id, CancellationToken ct)
    {
        var marker = await _service.GetAsync(id, this.UserId, ct);
        if (marker == null || marker.ProjectId != projectId)
            return NotFound();

        return Ok(marker);
    }

    // POST: api/projects/{projectId}/markers
    [HttpPost]
    public async Task<IActionResult> Create(string projectId, [FromBody] Marker marker, CancellationToken ct)
    {
        //if (!ModelState.IsValid)
        //return BadRequest(ModelState);

        marker.Id = ObjectId.GenerateNewId().ToString();
        marker.IsActive = true;
        marker.ProjectId = projectId;
        marker.UserId = this.UserId;
        var created = await _service.CreateAsync(marker, this.UserId, ct);
        return CreatedAtAction(nameof(GetById), new { projectId = projectId, id = created.Id }, created);
    }

    // PUT: api/projects/{projectId}/markers/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string projectId, string id, [FromBody] Marker marker, CancellationToken ct)
    {
        if (id != marker.Id)
            return BadRequest("Mismatched Ids");

        marker.ProjectId = projectId;

        var updated = await _service.UpdateAsync(marker, this.UserId, ct);
        if (!updated)
            return NotFound();

        return Ok(marker);
    }

    // DELETE: api/projects/{projectId}/markers/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string projectId, string id, CancellationToken ct)
    {
        var marker = await _service.GetAsync(id, this.UserId, ct);
        if (marker == null || marker.ProjectId != projectId)
            return NotFound();

        var deleted = await _service.DeleteAsync(id, this.UserId, ct);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}