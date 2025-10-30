using ECommerce.Core.Common.Constants;
using ECommerce.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.APIs.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = RolesConstants.Admin)]
public class ImageUploaderController(IImageUploader imageUploader) : ControllerBase
{
    [HttpPost("upload-image")]
    public async Task<IActionResult> UploadImage(IFormFile image, string folderName)
    {
        var result = await imageUploader.UploadImageAsync(image, folderName);
        return result.Uploaded
            ? Ok(new { result.FilePath })
            : BadRequest(new { result.ErrorMessage });
    }

    [HttpDelete("delete-image")]
    public IActionResult DeleteImage(string filePath)
    {
        imageUploader.DeleteFile(filePath);
        return Ok();
    }
}