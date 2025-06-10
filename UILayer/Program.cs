// Program.cs
using BusinessLayer; // נחוץ עבור IDashboardService, ISimulationService (ממשקים), וגם SimulationService (מחלקה)
using DataLayer.Repositories; // נחוץ עבור ICurrencyPairRepository, CurrencyPairRepository
using UILayer.Hubs; // נחוץ עבור TradeHub
using UILayer.Services; // נחוץ עבור TradeNotifier
using Microsoft.AspNetCore.SignalR;
using SharedModels; // וודא שזה מכיל את ה-Models המשותפים (כמו CurrencyPair, DashboardData וכו')
using DataLayer.Data; // נחוץ עבור ApplicationDbContext
using Microsoft.EntityFrameworkCore; // נחוץ עבור UseSqlServer
using Microsoft.Extensions.Logging; // נחוץ עבור AddLogging, LoggerFilterOptions

var builder = WebApplication.CreateBuilder(args);

// הוספת שירותים ל-Dependency Injection container

// ************ תיקון קריטי: רישום ה-DbContext ************
// קבלת מחרוזת החיבור מתוך קובץ appsettings.json
// וודא שמחרוזת החיבור "DefaultConnection" מוגדרת ב-appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// רישום ה-DbContext: ApplicationDbContext
// משתמשים ב-UseSqlServer בהתאם למחרוזת החיבור שצוינה
// וודא שחבילת ה-NuGet Microsoft.EntityFrameworkCore.SqlServer מותקנת בפרויקט DataLayer.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// **********************************************************

// רישום שירותי לוגינג (ILogger)
// הגדרת רמת לוג מינימלית ל-Debug כדי לראות מידע מפורט יותר
builder.Services.AddLogging(configure => configure.AddConsole())
                .Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Debug);

// רישום רפוזיטורים (Repositories)
// רפוזיטורים הם בדרך כלל Scoped עבור בקשות ווב, כלומר נוצרים פעם אחת לכל בקשת HTTP.
builder.Services.AddScoped<ICurrencyPairRepository, CurrencyPairRepository>();

// רישום שירותי השכבה העסקית (Business Layer Services)
// גם אלה בדרך כלל Scoped או Transient.
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ITradeNotifier, TradeHubNotifier>(); // TradeNotifier נמצא ב-UILayer.Services

// ************ תיקון קריטי: אופן רישום וטיפול ב-SimulationService ************
// SimulationService הוא IHostedService, ולכן הוא צריך להיות Singleton.
// מכיוון שהוא מוזרק גם ל-Controller (כ-ISimulationService) וגם פועל כ-HostedService,
// אנו רושמים אותו בשלושה שלבים:

// 1. רשום את המחלקה SimulationService כמופע Singleton אחד.
builder.Services.AddSingleton<SimulationService>();

// 2. רשום את הממשק ISimulationService כך שיפנה לאותו מופע Singleton של SimulationService.
// זה מאפשר להזריק את ISimulationService לקונטרולרים ולשירותים אחרים.
builder.Services.AddSingleton<ISimulationService>(provider => provider.GetRequiredService<SimulationService>());

// 3. רשום את ה-Singleton של SimulationService כ-IHostedService.
// זה מבטיח שהשיטות StartAsync ו-StopAsync יופעלו אוטומטית על ידי מערכת ASP.NET Core.
builder.Services.AddHostedService(provider => provider.GetRequiredService<SimulationService>());

// **************************************************************************

// רישום SignalR לטיפול בתקשורת בזמן אמת
builder.Services.AddSignalR();

// רישום Controllers (עבור API ו-Views) ו-Razor Pages
builder.Services.AddControllersWithViews(); // נחוץ עבור Controllers המשתמשים ב-Views (כמו HomeController)
builder.Services.AddRazorPages(); // נחוץ אם יש לך Razor Pages (לדוגמה, קובץ Index.cshtml)

var app = builder.Build();

// קונפיגורציה של pipeline בקשות ה-HTTP

// הגדרת טיפול בשגיאות בסביבת פיתוח
if (!app.Environment.IsDevelopment())
{
    // עבור סביבות Production, מפנה לדף שגיאות גנרי
    app.UseExceptionHandler("/Home/Error");
    // מפעיל HSTS (HTTP Strict Transport Security) עבור אבטחה
    app.UseHsts();
}

// הפניה מ-HTTP ל-HTTPS
app.UseHttpsRedirection();
// משרת קבצים סטטיים (CSS, JavaScript, תמונות) מתיקיית wwwroot
app.UseStaticFiles();

// מאפשר ניתוב בקשות ל-Endpoints
app.UseRouting();

// מאפשר אוטוריזציה (אם יש לך מנגנון אימות/הרשאות)
app.UseAuthorization();

// מיפוי SignalR Hubs לניתוב ספציפי
app.MapHub<TradeHub>("/tradehub"); // תואם ל-'.withUrl("/tradehub")' ב-JavaScript

// הגדרת ניתוב ברירת המחדל עבור Controllers ו-Views
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// מפעיל את האפליקציה ומתחיל להאזין לבקשות HTTP
app.Run();