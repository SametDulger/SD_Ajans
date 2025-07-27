using Microsoft.AspNetCore.Http;

namespace SD_Ajans.Web.Services
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file, string folderName);
        bool DeleteFile(string filePath);
        string GetFileUrl(string filePath);
        bool IsValidFile(IFormFile file);
        string GetFileSizeInMB(long bytes);
    }
} 