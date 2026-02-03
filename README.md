# SD Ajans - Manken ve Organizasyon Yönetim Sistemi

## 📋 Proje Hakkında

SD Ajans, manken ve organizasyon yönetimi için geliştirilmiş kapsamlı bir web uygulamasıdır. Bu sistem, manken kayıtları, organizasyon planlaması, görevlendirme yönetimi ve ödeme takibi gibi temel işlevleri sağlar.

## 🏗️ Mimari Yapı

Proje Clean Architecture prensiplerine uygun olarak geliştirilmiştir:

- **SD_Ajans.Core**: Domain entities, interfaces ve business rules
- **SD_Ajans.Data**: Data access layer, DbContext ve repositories
- **SD_Ajans.Business**: Business logic ve services
- **SD_Ajans.Web**: Presentation layer (MVC)

## 🚀 Özellikler

### Manken Yönetimi
- Manken kayıt ve profil yönetimi
- Fotoğraf yükleme ve yönetimi
- Kategori bazlı sınıflandırma
- Müsaitlik durumu takibi
- Detaylı arama ve filtreleme

### Organizasyon Yönetimi
- Organizasyon planlama ve kayıt
- Bütçe yönetimi
- Durum takibi (Planlandı, Devam Ediyor, Tamamlandı, İptal)
- Farklı organizasyon tipleri

### Görevlendirme Sistemi
- Manken-organizasyon eşleştirme
- Tarih ve saat yönetimi
- Ücret hesaplama
- Yemek ve konaklama maliyetleri
- Çakışma kontrolü

### Ödeme Yönetimi
- Ödeme kayıtları
- Farklı ödeme tipleri (Nakit, Havale, Kredi Kartı)
- Gelir-gider takibi
- Raporlama

### Güvenlik
- ASP.NET Core Identity ile kullanıcı yönetimi
- Role-based authorization
- Session yönetimi
- CSRF koruması

## 🛠️ Teknolojiler

- **Backend**: ASP.NET Core 9.0
- **Database**: SQLite
- **ORM**: Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **Logging**: Serilog
- **Frontend**: Razor Pages, Bootstrap, jQuery
- **File Upload**: Custom File Service

## 📦 Kurulum

### Gereksinimler
- .NET 9.0 SDK
- Visual Studio 2022 veya VS Code

### Adımlar

1. **Repository'yi klonlayın**
   ```bash
   git clone https://github.com/your-username/SD_Ajans.git
   cd SD_Ajans
   ```

2. **Bağımlılıkları yükleyin**
   ```bash
   dotnet restore
   ```

3. **Veritabanını oluşturun**
   ```bash
   dotnet ef database update --project SD_Ajans.Data --startup-project SD_Ajans.Web
   ```

4. **Uygulamayı çalıştırın**
   ```bash
   dotnet run --project SD_Ajans.Web
   ```

5. **Tarayıcıda açın**
   ```
   https://localhost:5001
   ```

### Varsayılan Kullanıcı
- **Email**: admin@sdajans.com
- **Şifre**: Admin123!

## 🔧 Konfigürasyon

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=SD_Ajans.db"
  },
  "FileUpload": {
    "MaxFileSizeInMB": 5,
    "AllowedExtensions": [".jpg", ".jpeg", ".png", ".gif", ".webp"]
  },
  "Security": {
    "SessionTimeoutInMinutes": 480,
    "MaxLoginAttempts": 5
  }
}
```

## 📊 Veritabanı Şeması

### Ana Tablolar
- **Users**: Kullanıcı bilgileri
- **Mankens**: Manken profilleri
- **Organizations**: Organizasyon bilgileri
- **Assignments**: Görevlendirmeler
- **Payments**: Ödeme kayıtları

### İlişkiler
- Manken ↔ Assignment (1:N)
- Organization ↔ Assignment (1:N)
- Organization ↔ Payment (1:N)
- User ↔ Organization (1:N) [CreatedBy]
- User ↔ Payment (1:N) [ProcessedBy]

## 🔒 Güvenlik Özellikleri

- **Authentication**: JWT token tabanlı
- **Authorization**: Role-based access control
- **Input Validation**: Server-side validation
- **File Upload Security**: Dosya tipi ve boyut kontrolü
- **SQL Injection Protection**: Entity Framework ile parametrized queries
- **XSS Protection**: HTML encoding
- **CSRF Protection**: Anti-forgery tokens

## 📝 Logging

Serilog kullanılarak yapılandırılmış logging sistemi:
- Console logging
- File logging (günlük rotasyon)
- Structured logging
- Error tracking

## 🧪 Test

```bash
# Unit testleri çalıştır
dotnet test

# Coverage raporu
dotnet test --collect:"XPlat Code Coverage"
```

## 📈 Performans

- **Caching**: Memory cache kullanımı
- **Database Indexing**: Optimized indexes
- **Lazy Loading**: Navigation properties
- **Async/Await**: Asynchronous operations

## 🚀 Deployment

### Production
```bash
dotnet publish -c Release -o ./publish
```

### Docker
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0
COPY ./publish /app
WORKDIR /app
EXPOSE 80
ENTRYPOINT ["dotnet", "SD_Ajans.Web.dll"]
```

## 🤝 Katkıda Bulunma

1. Fork yapın
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Commit yapın (`git commit -m 'Add amazing feature'`)
4. Push yapın (`git push origin feature/amazing-feature`)
5. Pull Request oluşturun

## 📄 Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için [LICENSE](LICENSE) dosyasına bakın.

## 🔄 Changelog

### v1.0.0 (2024-01-XX)
- İlk sürüm
- Temel CRUD işlemleri
- Manken ve organizasyon yönetimi
- Ödeme sistemi
- Güvenlik özellikleri

---

**Not**: Bu proje geliştirme aşamasındadır. Production kullanımı için ek güvenlik ve performans optimizasyonları gerekebilir.

