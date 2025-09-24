using ImageGenerator.Dtos;
using ImageGenerator.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImageGenerator.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UploadController(IImageStorageService storageService) : ControllerBase
{
    private readonly IImageStorageService _storage = storageService;

    [HttpPost]
    public async Task<ActionResult<ImageDto>> Upload([FromForm] UploadImageDto uploadDto)
    {
        try
        {
            var image = await _storage.UploadAsync(uploadDto);
            return Ok(image);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest($"上传图片失败: {ex.Message}");
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<ImageDto>>> List()
    {
        try
        {
            var images = await _storage.ListUserImagesAsync();
            return Ok(images);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            return BadRequest($"获取图片列表失败: {ex.Message}");
        }
    }
}
