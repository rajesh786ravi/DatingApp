using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.StaticFiles;
using File = Google.Apis.Drive.v3.Data.File;

namespace API.Services;

public class GoogleDriveService
{
    private readonly DriveService _driveService;

    public GoogleDriveService()
    {
        UserCredential credential;
        using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
        {
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                new[] { DriveService.Scope.DriveFile },
                "user",
                CancellationToken.None,
                new FileDataStore("token.json", true)).Result;
        }

        _driveService = new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "Mac Drive Uploader"
        });
    }

    public async Task<string> UploadFileAsync_V0(string filePath, string mimeType)
    {
        var fileMetadata = new File
        {
            Name = Path.GetFileName(filePath)
        };

        using var stream = new FileStream(filePath, FileMode.Open);
        var request = _driveService.Files.Create(fileMetadata, stream, mimeType);
        request.Fields = "id";
        await request.UploadAsync();
        return request.ResponseBody?.Id ?? string.Empty;
    }

    public async Task<string> UploadFileAsync(string localFilePath, string mimeType)
    {
        var fileName = Path.GetFileName(localFilePath);

        // Step 1: Check if file already exists in Drive
        var listRequest = _driveService.Files.List();
        listRequest.Q = $"name = '{fileName}' and trashed = false";
        listRequest.Fields = "files(id, name)";
        var result = await listRequest.ExecuteAsync();

        var fileMetadata = new Google.Apis.Drive.v3.Data.File
        {
            Name = fileName
        };

        FilesResource.CreateMediaUpload uploadRequest;
        FilesResource.UpdateMediaUpload updateRequest;

        using var stream = new FileStream(localFilePath, FileMode.Open);
        if (result.Files != null && result.Files.Count > 0)
        {
            // File with same name exists → Update it
            var existingFile = result.Files.First();

            updateRequest = _driveService.Files.Update(fileMetadata, existingFile.Id, stream, mimeType);
            await updateRequest.UploadAsync();

            return existingFile.Id; // Return updated file ID
        }
        else
        {
            // File doesn't exist → Create new
            uploadRequest = _driveService.Files.Create(fileMetadata, stream, mimeType);
            uploadRequest.Fields = "id";
            await uploadRequest.UploadAsync();
            return uploadRequest.ResponseBody.Id; // Return new file ID
        }
    }
    public async Task<string> UploadOrReplaceFileAsync(string localFilePath)
    {
        // Detect MIME type (based on extension)
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(localFilePath, out var mimeType))
        {
            mimeType = "application/octet-stream"; // Fallback generic binary type
        }
        var fileName = Path.GetFileName(localFilePath);

        // Step 1: Check if file already exists in Drive
        var request = _driveService.Files.List();
        request.Q = $"name = '{fileName}' and trashed = false";
        request.Fields = "files(id, name)";
        var result = await request.ExecuteAsync();

        var existingFile = result.Files.FirstOrDefault();

        using var stream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read);

        if (existingFile != null)
        {
            // Step 2: Replace
            var updateRequest = _driveService.Files.Update(null, existingFile.Id, stream, mimeType);
            var updatedFile = await updateRequest.UploadAsync();

            return existingFile.Id;
        }
        else
        {
            // Step 3: Upload as new
            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = fileName
            };

            var uploadRequest = _driveService.Files.Create(fileMetadata, stream, mimeType);
            uploadRequest.Fields = "id";
            var file = await uploadRequest.UploadAsync();

            return uploadRequest.ResponseBody.Id;
        }
    }

    public async Task<List<string>> UploadAllFilesFromFolderAsync(string folderPath)
    {
        var uploadedFileIds = new List<string>();

        if (!Directory.Exists(folderPath))
            throw new DirectoryNotFoundException($"Folder does not exist: {folderPath}");

        var files = Directory.GetFiles(folderPath); // No subdirectories

        foreach (var filePath in files)
        {
            var fileId = await UploadOrReplaceFileAsync(filePath);
            uploadedFileIds.Add(fileId);
        }

        return uploadedFileIds;
    }


}