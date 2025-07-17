using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DriveController : ControllerBase
{
    private readonly GoogleDriveService _driveService;

    public DriveController(GoogleDriveService driveService)
    {
        _driveService = driveService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload()
    {
        string localFile = "/Users/rajesh-sandhiya/Downloads/Testing.txt"; // Change this
        string mimeType = "text/plain";

        try
        {
            var fileId = await _driveService.UploadFileAsync(localFile, mimeType);
            return Ok(new { Message = "File uploaded successfully", FileId = fileId });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("save-path")]
    public IActionResult SavePath([FromBody] AdminSettings settings)
    {
        System.IO.File.WriteAllText("admin_path.txt", settings.LocalPath);
        return Ok(new { message = "Path saved successfully." });
    }

    [HttpPost("update-drive")]
    public async Task<IActionResult> UpdateDrive()
    {
        var path = System.IO.File.ReadAllText("admin_path.txt");

        if (!System.IO.File.Exists(path))
            return BadRequest("File does not exist.");

        string mimeType = "text/plain"; // You can infer this from extension if needed

        var fileId = await _driveService.UploadOrReplaceFileAsync(path, mimeType);

        return Ok(new { Message = "Drive updated", FileId = fileId });
    }


}
