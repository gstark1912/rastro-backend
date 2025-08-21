// Api/Projects/ProjectDtos.cs
namespace RastroApi.Api.Projects;

public record ProjectCreateDto(string Title, string? Description);
public record ProjectUpdateDto(string Title, string? Description);
public record ProjectResponse(string Id, string Title, string? Description);
