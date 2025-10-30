namespace ECommerce.Core.Common;
public class UploadImageResult
{
	public bool Uploaded { get; set; }
	public string? ErrorMessage { get; set; }
	public string? FilePath { get; set; }
}
