using Microsoft.AspNetCore.Identity;
using SD_Ajans.Core.Entities;

namespace SD_Ajans.Data
{
    public static class SeedData
    {
        public static async Task Initialize(AppDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (context.Users.Any())
                return;

            var roles = new[] { "Admin", "Manager", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminUser = new User
            {
                UserName = "admin@sdajans.com",
                Email = "admin@sdajans.com",
                FirstName = "Admin",
                LastName = "User",
                Role = UserRole.Admin,
                EmailConfirmed = true,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            if (await userManager.FindByEmailAsync(adminUser.Email) == null)
            {
                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            await context.SaveChangesAsync();

            // Örnek Mankenler
            if (!context.Mankens.Any())
            {
                var mankens = new List<Manken>
                {
                    new Manken
                    {
                        FirstName = "Ayşe",
                        LastName = "Yılmaz",
                        Email = "ayse.yilmaz@email.com",
                        Phone = "0532 123 4567",
                        BirthDate = new DateTime(1995, 5, 15),
                        Gender = Gender.Female,
                        Height = 170,
                        Weight = 55,
                        HairColor = "Kahverengi",
                        EyeColor = "Kahverengi",
                        City = "İstanbul",
                        Category = MankenCategory.Category1,
                        Description = "Kozmetik ürün tanıtımında deneyimli",
                        IsAvailable = true,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        CreatedById = adminUser.Id
                    },
                    new Manken
                    {
                        FirstName = "Mehmet",
                        LastName = "Demir",
                        Email = "mehmet.demir@email.com",
                        Phone = "0533 234 5678",
                        BirthDate = new DateTime(1992, 8, 22),
                        Gender = Gender.Male,
                        Height = 185,
                        Weight = 75,
                        HairColor = "Siyah",
                        EyeColor = "Mavi",
                        City = "Ankara",
                        Category = MankenCategory.Category2,
                        Description = "Tören ve etkinliklerde hosteslik yapar",
                        IsAvailable = true,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        CreatedById = adminUser.Id
                    },
                    new Manken
                    {
                        FirstName = "Zeynep",
                        LastName = "Kaya",
                        Email = "zeynep.kaya@email.com",
                        Phone = "0534 345 6789",
                        BirthDate = new DateTime(1998, 3, 10),
                        Gender = Gender.Female,
                        Height = 168,
                        Weight = 52,
                        HairColor = "Sarı",
                        EyeColor = "Yeşil",
                        City = "İzmir",
                        Category = MankenCategory.Category3,
                        Description = "Animatör ve eğlence etkinlikleri",
                        IsAvailable = true,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        CreatedById = adminUser.Id
                    },
                    new Manken
                    {
                        FirstName = "Ali",
                        LastName = "Özkan",
                        Email = "ali.ozkan@email.com",
                        Phone = "0535 456 7890",
                        BirthDate = new DateTime(1990, 12, 5),
                        Gender = Gender.Male,
                        Height = 180,
                        Weight = 80,
                        HairColor = "Kahverengi",
                        EyeColor = "Kahverengi",
                        City = "Bursa",
                        Category = MankenCategory.Category1,
                        Description = "Müzik sistemi kiralama ve teknik destek",
                        IsAvailable = true,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        CreatedById = adminUser.Id
                    }
                };

                context.Mankens.AddRange(mankens);
                await context.SaveChangesAsync();

                // Örnek Organizasyonlar
                var organizations = new List<Organization>
                {
                    new Organization
                    {
                        Name = "L'Oréal Kozmetik Tanıtımı",
                        Date = DateTime.Now.AddDays(7),
                        Location = "İstanbul, Kadıköy",
                        Type = OrganizationType.CosmeticPromotion,
                        TotalBudget = 5000,
                        Status = OrganizationStatus.Planned,
                        Description = "Yeni kozmetik ürünlerinin tanıtımı için promotör gerekiyor",
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        CreatedById = adminUser.Id
                    },
                    new Organization
                    {
                        Name = "Düğün Töreni Hostesi",
                        Date = DateTime.Now.AddDays(14),
                        Location = "Ankara, Çankaya",
                        Type = OrganizationType.CeremonyHostess,
                        TotalBudget = 3000,
                        Status = OrganizationStatus.Planned,
                        Description = "Düğün töreninde İngilizce konuşan hostes gerekiyor",
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        CreatedById = adminUser.Id
                    },
                    new Organization
                    {
                        Name = "Tatil Köyü Animatör",
                        Date = DateTime.Now.AddDays(21),
                        Location = "Antalya, Kemer",
                        Type = OrganizationType.HolidayAnimation,
                        TotalBudget = 8000,
                        Status = OrganizationStatus.Planned,
                        Description = "Tatil köyünde çocuk animatörü ve eğlence etkinlikleri",
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        CreatedById = adminUser.Id
                    },
                    new Organization
                    {
                        Name = "Konser Müzik Sistemi",
                        Date = DateTime.Now.AddDays(3),
                        Location = "İzmir, Konak",
                        Type = OrganizationType.ConcertEvent,
                        TotalBudget = 15000,
                        Status = OrganizationStatus.Planned,
                        Description = "Konser için profesyonel müzik sistemi kiralama",
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        CreatedById = adminUser.Id
                    }
                };

                context.Organizations.AddRange(organizations);
                await context.SaveChangesAsync();

                // Örnek Görevlendirmeler
                var assignments = new List<Assignment>
                {
                    new Assignment
                    {
                        MankenId = mankens[0].Id, // Ayşe Yılmaz
                        OrganizationId = organizations[0].Id, // L'Oréal
                        StartTime = organizations[0].Date,
                        EndTime = organizations[0].Date.AddDays(1),
                        NumberOfDays = 1,
                        DailyRate = 400, // Günlük ücret
                        TotalPayment = 400, // Günlük ücret * gün sayısı
                        Status = AssignmentStatus.Scheduled,
                        IncludesMeal = true,
                        IncludesAccommodation = false,
                        MealCost = 100, // Günlük yemek maliyeti
                        AccommodationCost = 0,
                        Fee = 500, // Toplam ücret (günlük ücret + yemek)
                        Notes = "Kozmetik ürün tanıtımı için günlük çalışma",
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        CreatedById = adminUser.Id
                    },
                    new Assignment
                    {
                        MankenId = mankens[1].Id, // Mehmet Demir
                        OrganizationId = organizations[1].Id, // Düğün
                        StartTime = organizations[1].Date,
                        EndTime = organizations[1].Date.AddDays(1),
                        NumberOfDays = 1,
                        DailyRate = 1000, // Günlük ücret
                        TotalPayment = 1000, // Günlük ücret * gün sayısı
                        Status = AssignmentStatus.Scheduled,
                        IncludesMeal = true,
                        IncludesAccommodation = true,
                        MealCost = 200, // Günlük yemek maliyeti
                        AccommodationCost = 500, // Günlük konaklama maliyeti
                        Fee = 1700, // Toplam ücret (günlük ücret + yemek + konaklama)
                        Notes = "Düğün töreninde İngilizce hosteslik",
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        CreatedById = adminUser.Id
                    },
                    new Assignment
                    {
                        MankenId = mankens[2].Id, // Zeynep Kaya
                        OrganizationId = organizations[2].Id, // Tatil Köyü
                        StartTime = organizations[2].Date,
                        EndTime = organizations[2].Date.AddDays(7),
                        NumberOfDays = 7,
                        DailyRate = 800, // Günlük ücret
                        TotalPayment = 5600, // Günlük ücret * gün sayısı
                        Status = AssignmentStatus.Scheduled,
                        IncludesMeal = true,
                        IncludesAccommodation = true,
                        MealCost = 1400, // 7 gün * 200 TL
                        AccommodationCost = 3500, // 7 gün * 500 TL
                        Fee = 10500, // Toplam ücret (günlük ücret + yemek + konaklama)
                        Notes = "Tatil köyünde animatör olarak 1 hafta",
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        CreatedById = adminUser.Id
                    }
                };

                context.Assignments.AddRange(assignments);
                await context.SaveChangesAsync();

                // Örnek Ödemeler
                var payments = new List<Payment>
                {
                    new Payment
                    {
                        AssignmentId = assignments[0].Id,
                        OrganizationId = organizations[0].Id,
                        Amount = 500,
                        PaymentType = PaymentType.Cash,
                        PaymentDate = DateTime.Now.AddDays(-5),
                        Status = PaymentStatus.Completed,
                        Description = "L'Oréal kozmetik tanıtımı ödemesi",
                        Notes = "Günlük ücret + yemek",
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        CreatedById = adminUser.Id,
                        ProcessedById = adminUser.Id
                    },
                    new Payment
                    {
                        AssignmentId = assignments[0].Id,
                        OrganizationId = organizations[0].Id,
                        Amount = 400,
                        PaymentType = PaymentType.BankTransfer,
                        PaymentDate = DateTime.Now.AddDays(-4),
                        Status = PaymentStatus.Completed,
                        Description = "Ayşe Yılmaz ücret ödemesi",
                        Notes = "Manken ücreti ödendi",
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        CreatedById = adminUser.Id,
                        ProcessedById = adminUser.Id
                    },
                    new Payment
                    {
                        AssignmentId = assignments[1].Id,
                        OrganizationId = organizations[1].Id,
                        Amount = 1700,
                        PaymentType = PaymentType.Cash,
                        PaymentDate = DateTime.Now.AddDays(-3),
                        Status = PaymentStatus.Completed,
                        Description = "Düğün hostesi ödemesi",
                        Notes = "Günlük ücret + yemek + konaklama",
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        CreatedById = adminUser.Id,
                        ProcessedById = adminUser.Id
                    },
                    new Payment
                    {
                        AssignmentId = assignments[1].Id,
                        OrganizationId = organizations[1].Id,
                        Amount = 1000,
                        PaymentType = PaymentType.BankTransfer,
                        PaymentDate = DateTime.Now.AddDays(-2),
                        Status = PaymentStatus.Completed,
                        Description = "Mehmet Demir ücret ödemesi",
                        Notes = "Manken ücreti ödendi",
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        CreatedById = adminUser.Id,
                        ProcessedById = adminUser.Id
                    }
                };

                context.Payments.AddRange(payments);
                await context.SaveChangesAsync();
            }
        }
    }
} 