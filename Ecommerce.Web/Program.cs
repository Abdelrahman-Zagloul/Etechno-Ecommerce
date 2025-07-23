using Stripe;
namespace Ecommerce.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<AppDbContext>(options =>
               options.UseSqlServer(builder.Configuration.GetConnectionString("RemoteConnection")));

            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

            builder.Services.AddScoped<IDbInitializer, DbInitializer>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
               .AddEntityFrameworkStores<AppDbContext>()
               .AddDefaultTokenProviders();

            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;

            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddGitHub(options =>
            {
                options.ClientId = builder.Configuration["Authentication:GitHub:ClientId"] ?? "";
                options.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"] ?? "";
                options.Scope.Add("user:email");
            })
            .AddGoogle(options =>
            {
                options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "";
                options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "";
            })
            .AddLinkedIn(options =>
             {
                 options.ClientId = builder.Configuration["Authentication:LinkedIn:ClientId"] ?? "";
                 options.ClientSecret = builder.Configuration["Authentication:LinkedIn:ClientSecret"] ?? "";
                 options.Scope.Add("openid");
                 options.Scope.Add("profile");
                 options.Scope.Add("email");
             })
            .AddMicrosoftAccount(options =>
            {
                options.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"] ?? "";
                options.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"] ?? "";
                options.SaveTokens = true;
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("email");
                options.Scope.Add("profile");
                options.Scope.Add("User.Read");
            })
             .AddFacebook(options =>
             {

                 options.ClientId = builder.Configuration["Authentication:Facebook:AppId"] ?? "";
                 options.ClientSecret = builder.Configuration["Authentication:Facebook:AppSecret"] ?? "";
                 options.Fields.Add("name");
             })
            ;

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Authentication/Account/Login";
                options.AccessDeniedPath = "/Authentication/Account/AccessDenied";
            });
            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseExceptionHandler("/Customer/Home/Error");
            app.UseSession();
            StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:Secretkey").Get<string>();
            app.UseRouting();
            SeedDatabase();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
               name: "default",
               pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

            app.Run();

            void SeedDatabase()
            {
                using (var scope = app.Services.CreateScope())
                {
                    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                    dbInitializer.InitializeAsync().Wait();
                }
            }
        }
    }
}