using ImageGenerator.Dtos;

namespace ImageGenerator.Interface;

/// <summary>
/// Service for handling image generation requests.
/// </summary>
public interface IGenerateService
{
    /// <summary>
    /// Submits a new image generation task.
    /// </summary>
    /// <param name="request">The generation request.</param>
    /// <returns>The generation response with task ID and estimated cost.</returns>
    Task<GenerateResponseDto> GenerateAsync(GenerateRequestDto request);

    /// <summary>
    /// Gets the status of a generation task.
    /// </summary>
    /// <param name="taskId">The task ID.</param>
    /// <returns>The task status.</returns>
    Task<GenerateTaskStatusDto> GetTaskStatusAsync(Guid taskId);
}
