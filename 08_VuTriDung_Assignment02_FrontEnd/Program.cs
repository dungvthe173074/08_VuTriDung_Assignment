using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication.Cookies;
using _08_VuTriDung_Assignment02_FrontEnd.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "temp-keys")));

// Add services to the container.
builder.Services.AddControllersWithViews();

// 1. Configure HttpContextAccessor & DelegatingHandler for JWT forwarding
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<JwtAuthorizationHeaderHandler>();

// 2. Register HttpClient with JWT handler
builder.Services.AddHttpClient("BackendApi", client =>
{
    var baseUrl = builder.Configuration["BackendApiUrl"];
    if (!baseUrl.EndsWith("/")) baseUrl += "/";
    client.BaseAddress = new Uri(baseUrl);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
})
.AddHttpMessageHandler<JwtAuthorizationHeaderHandler>();

builder.Services.AddHttpClient();

// 3. Register API Services
builder.Services.AddScoped<IAuthApiService, AuthApiService>();
builder.Services.AddScoped<IAccountApiService, AccountApiService>();
builder.Services.AddScoped<ICategoryApiService, CategoryApiService>();
builder.Services.AddScoped<ITagApiService, TagApiService>();
builder.Services.AddScoped<INewsArticleApiService, NewsArticleApiService>();

// 4. Configure Session & Cookie Authentication
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
