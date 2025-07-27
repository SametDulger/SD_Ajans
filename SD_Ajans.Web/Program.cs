using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using SD_Ajans.Business.Services;
using SD_Ajans.Core.Entities;
using SD_Ajans.Core.Repositories;
using SD_Ajans.Data;
using SD_Ajans.Data.Repositories;
using SD_Ajans.Web.Services;

// Serilog konfigürasyonu
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File("logs/sdajans-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30)
    .CreateLogger();

try
{
    Log.Information("SD Ajans uygulaması başlatılıyor...");

    var builder = WebApplication.CreateBuilder(args);

    // Serilog'u builder'a ekle
    builder.Host.UseSerilog();

    // Add services to the container.
    builder.Services.AddControllersWithViews();

    // Database context
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Identity configuration
    builder.Services.AddIdentity<User, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
        
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = false;
        
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.MaxFailedAccessAttempts = 5;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

    // Cookie configuration
    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

    // Repository and Unit of Work
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

    // Business Services
    builder.Services.AddScoped<IMankenService, MankenService>();
    builder.Services.AddScoped<IOrganizationService, OrganizationService>();
    builder.Services.AddScoped<IAssignmentService, AssignmentService>();
    builder.Services.AddScoped<IPaymentService, PaymentService>();
    builder.Services.AddScoped<IAccountingService, AccountingService>();
    builder.Services.AddScoped<IReportService, ReportService>();

    // Web Services
    builder.Services.AddScoped<IFileService, FileService>();

    // Memory Cache
    builder.Services.AddMemoryCache();

    // HTTP Context Accessor
    builder.Services.AddHttpContextAccessor();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }
    else
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    // Database initialization and seeding
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        try
        {
            context.Database.EnsureCreated();
            logger.LogInformation("Veritabanı başarıyla oluşturuldu.");
            
            await SeedData.Initialize(context, userManager, roleManager);
            logger.LogInformation("Seed data başarıyla yüklendi.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Veritabanı başlatılırken hata oluştu.");
            throw;
        }
    }

    Log.Information("SD Ajans uygulaması başarıyla başlatıldı.");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "SD Ajans uygulaması başlatılırken kritik hata oluştu.");
}
finally
{
    Log.CloseAndFlush();
}
