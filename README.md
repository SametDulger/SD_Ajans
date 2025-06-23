# SD Ajans — Promotör ve Manken Ajansı Yönetim Sistemi

SD Ajans, promotör ve manken ajanslarının operasyonel süreçlerini yönetmek için geliştirilmiş, modern ve çok katmanlı bir web uygulamasıdır. Manken, organizasyon, görevlendirme, ödeme ve muhasebe işlemlerini tek bir platformda birleştirir.

## 🚀 Özellikler

### 📋 Temel Modüller
- **Manken Yönetimi:** Kayıt, arama, profil güncelleme, fotoğraf yükleme, kategori ve uygunluk takibi
- **Organizasyon Yönetimi:** Organizasyon oluşturma, planlama, tür ve durum takibi
- **Görevlendirme:** Manken-organizasyon eşleştirme, otomatik ücret ve maliyet hesaplama
- **Ödeme ve Muhasebe:** Kategoriye göre ücretlendirme, gelir-gider ve kar/zarar raporları, Excel çıktısı

### 🔐 Güvenlik ve Yetkilendirme
- **Rol Tabanlı Yetkilendirme:** Sadece Admin rolü (güvenlik odaklı)
- **Kimlik Doğrulama:** Güvenli giriş sistemi
- **Yetkilendirme:** Tüm sayfalar için login zorunlu

### 🎨 Kullanıcı Arayüzü
- **Modern Tasarım:** Bootstrap 5.3.2, responsive tasarım
- **Kullanıcı Dostu:** Intuitive navigation ve modern UI/UX
- **Mobil Uyumlu:** Tüm cihazlarda mükemmel görünüm

## 🏗️ Katmanlı Mimari

```
SD_Ajans/
├── SD_Ajans.Core/          # Temel entity ve interface'ler
│   ├── Entities/           # Veritabanı entity'leri
│   └── Repositories/       # Repository interface'leri
├── SD_Ajans.Data/          # Veri erişim katmanı
│   ├── AppDbContext.cs     # Entity Framework context
│   ├── Repositories/       # Repository implementasyonları
│   └── SeedData.cs         # Başlangıç verileri
├── SD_Ajans.Business/      # İş mantığı katmanı
│   └── Services/           # İş servisleri
└── SD_Ajans.Web/           # Web uygulaması
    ├── Controllers/        # MVC controller'ları
    ├── Views/              # Razor view'ları
    ├── Models/             # View model'leri
    ├── Services/           # Web servisleri
    └── wwwroot/            # Statik dosyalar
```

## 🛠️ Teknoloji Stack

- **Backend:** ASP.NET Core 9.0 MVC
- **Veritabanı:** SQLite (Development)
- **ORM:** Entity Framework Core
- **Frontend:** Bootstrap 5.3.2, jQuery
- **Raporlama:** ClosedXML (Excel export)
- **Dosya İşlemleri:** Custom FileService

## 📦 Kurulum

### Gereksinimler

- .NET 9.0 SDK
- Visual Studio 2022 veya VS Code
- Git

### Adımlar

1. **Projeyi klonlayın:**
   ```bash
   git clone https://github.com/SametDulger/SD_Ajans.git
   cd SD_Ajans
   ```

2. **Bağımlılıkları yükleyin:**
   ```bash
   dotnet restore
   ```

3. **Projeyi derleyin:**
   ```bash
   dotnet build
   ```

4. **Veritabanı migrasyonlarını oluşturun:**
   ```bash
   dotnet ef migrations add InitialCreate -p SD_Ajans.Data -s SD_Ajans.Web
   dotnet ef database update -p SD_Ajans.Data -s SD_Ajans.Web
   ```

5. **Projeyi başlatın:**
   ```bash
   dotnet run --project SD_Ajans.Web
   ```

6. **Tarayıcıda açın:**
   ```
   https://localhost:5001
   ```

## 👤 Kullanıcı Girişi

### Admin Kullanıcısı
- **E-posta:** admin@sdajans.com
- **Şifre:** Admin123!
- **Yetkiler:** Tüm sistem işlemleri

> **Not:** Sistem sadece Admin rolünü destekler. Kullanıcı kaydı bulunmamaktadır.

## 📊 Sistem Özellikleri

### Ana Sayfa
- **Public Erişim:** Ajans tanıtım sayfası
- **Dinamik İstatistikler:** Model, organizasyon ve görevlendirme sayıları
- **Modern Tasarım:** Hero section, hizmetler, iletişim bilgileri

### Manken Yönetimi
- **Fotoğraf Yükleme:** Otomatik klasör oluşturma ve güvenli dosya adlandırma
- **Detaylı Arama:** İsim, kategori ve uygunluk bazlı filtreleme
- **Profil Yönetimi:** Kapsamlı bilgi güncelleme

### Muhasebe ve Raporlama
- **Ücret Hesaplama:** Kategori bazlı otomatik hesaplama
- **Excel Raporları:** Detaylı finansal raporlar
- **Gelir-Gider Takibi:** Kapsamlı mali analiz

## 🔧 Geliştirme

### Proje Yapısı
- **Clean Architecture:** Katmanlı mimari prensipleri
- **Repository Pattern:** Veri erişim soyutlaması
- **Service Layer:** İş mantığı ayrımı
- **Dependency Injection:** Loose coupling

### Kod Standartları
- **C# 12:** Modern C# özellikleri
- **Async/Await:** Asenkron programlama
- **Nullable Reference Types:** Tip güvenliği
- **LINQ:** Veri sorgulama


## 🤝 Katkı

1. Fork yapın
2. Feature branch oluşturun (`git checkout -b feature/AmazingFeature`)
3. Değişikliklerinizi commit edin (`git commit -m 'Add some AmazingFeature'`)
4. Branch'inizi push edin (`git push origin feature/AmazingFeature`)
5. Pull Request oluşturun

