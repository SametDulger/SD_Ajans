using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SD_Ajans.Web.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Dosya boş olamaz.");

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", folderName);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Güvenli dosya adı oluştur
            var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            // Türkçe karakterleri ve özel karakterleri temizle
            var safeFileName = RemoveSpecialCharacters(originalFileName);
            if (string.IsNullOrEmpty(safeFileName))
                safeFileName = "photo";
            
            var fileName = $"{Guid.NewGuid()}_{safeFileName}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Path.Combine("uploads", folderName, fileName).Replace('\\', '/');
        }

        private string RemoveSpecialCharacters(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Türkçe karakterleri değiştir
            var turkishChars = new Dictionary<char, char>
            {
                {'ç', 'c'}, {'Ç', 'C'},
                {'ğ', 'g'}, {'Ğ', 'G'},
                {'ı', 'i'}, {'I', 'I'},
                {'ö', 'o'}, {'Ö', 'O'},
                {'ş', 's'}, {'Ş', 'S'},
                {'ü', 'u'}, {'Ü', 'U'}
            };

            var result = input;
            foreach (var kvp in turkishChars)
            {
                result = result.Replace(kvp.Key, kvp.Value);
            }

            // Sadece harf, rakam, tire ve alt çizgi bırak
            result = new string(result.Where(c => char.IsLetterOrDigit(c) || c == '-' || c == '_').ToArray());
            
            // Birden fazla tire veya alt çizgiyi tekli yap
            result = System.Text.RegularExpressions.Regex.Replace(result, @"[-_]+", "_");
            
            return result.Trim('_', '-');
        }

        public bool DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            // Remove leading slash if present and convert to system path
            var cleanPath = filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.Combine(_environment.WebRootPath, cleanPath);
            
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }

            return false;
        }

        public string GetFileUrl(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return string.Empty;

            return $"/{filePath.Replace('\\', '/')}";
        }
    }
} 