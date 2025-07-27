using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SD_Ajans.Web.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FileService> _logger;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private const int MaxFileSizeInMB = 5;
        private const int MaxFileSizeInBytes = MaxFileSizeInMB * 1024 * 1024;

        public FileService(IWebHostEnvironment environment, ILogger<FileService> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderName)
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("Dosya boş olamaz.");

                // Dosya boyutu kontrolü
                if (file.Length > MaxFileSizeInBytes)
                    throw new ArgumentException($"Dosya boyutu {MaxFileSizeInMB}MB'dan büyük olamaz.");

                // Dosya uzantısı kontrolü
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_allowedExtensions.Contains(fileExtension))
                    throw new ArgumentException($"Desteklenmeyen dosya formatı. İzin verilen formatlar: {string.Join(", ", _allowedExtensions)}");

                // Güvenli klasör adı oluştur
                var safeFolderName = RemoveSpecialCharacters(folderName);
                if (string.IsNullOrEmpty(safeFolderName))
                    safeFolderName = "uploads";

                var uploadsFolder = Path.Combine(_environment.WebRootPath, safeFolderName);
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    _logger.LogInformation("Upload klasörü oluşturuldu: {FolderPath}", uploadsFolder);
                }

                // Güvenli dosya adı oluştur
                var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                var safeFileName = RemoveSpecialCharacters(originalFileName);
                if (string.IsNullOrEmpty(safeFileName))
                    safeFileName = "file";
                
                var fileName = $"{Guid.NewGuid()}_{safeFileName}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Dosya yükleme
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var relativePath = Path.Combine(safeFolderName, fileName).Replace('\\', '/');
                
                _logger.LogInformation("Dosya başarıyla yüklendi: {FilePath}", relativePath);
                
                return relativePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dosya yükleme sırasında hata oluştu. FileName: {FileName}, FolderName: {FolderName}", 
                    file?.FileName, folderName);
                throw;
            }
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
            result = Regex.Replace(result, @"[-_]+", "_");
            
            return result.Trim('_', '-');
        }

        public bool DeleteFile(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    return false;

                // Güvenlik kontrolü - sadece uploads klasöründeki dosyaları sil
                if (!filePath.StartsWith("uploads/", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("Güvenlik ihlali: Uploads klasörü dışındaki dosya silinmeye çalışıldı: {FilePath}", filePath);
                    return false;
                }

                // Path traversal saldırısını önle
                var normalizedPath = Path.GetFullPath(Path.Combine(_environment.WebRootPath, filePath));
                var webRootPath = Path.GetFullPath(_environment.WebRootPath);
                
                if (!normalizedPath.StartsWith(webRootPath, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("Path traversal saldırısı tespit edildi: {FilePath}", filePath);
                    return false;
                }

                if (File.Exists(normalizedPath))
                {
                    File.Delete(normalizedPath);
                    _logger.LogInformation("Dosya başarıyla silindi: {FilePath}", filePath);
                    return true;
                }

                _logger.LogWarning("Silinecek dosya bulunamadı: {FilePath}", filePath);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dosya silme sırasında hata oluştu: {FilePath}", filePath);
                return false;
            }
        }

        public string GetFileUrl(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return string.Empty;

            // Güvenlik kontrolü - sadece uploads klasöründeki dosyalara erişim
            if (!filePath.StartsWith("uploads/", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Güvenlik ihlali: Uploads klasörü dışındaki dosyaya erişim denemesi: {FilePath}", filePath);
                return string.Empty;
            }

            return $"/{filePath.Replace('\\', '/')}";
        }

        public bool IsValidFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            if (file.Length > MaxFileSizeInBytes)
                return false;

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return _allowedExtensions.Contains(fileExtension);
        }

        public string GetFileSizeInMB(long bytes)
        {
            return Math.Round((double)bytes / (1024 * 1024), 2).ToString();
        }
    }
} 