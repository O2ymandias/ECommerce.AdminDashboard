using ECommerce.Core.Common;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Core.Interfaces.Services;
public interface IImageUploader
{
	Task<UploadImageResult> UploadImageAsync(IFormFile file, string folderName);
	void DeleteFile(string? filePath);
}
