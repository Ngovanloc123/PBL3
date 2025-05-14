using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using StackBook.DAL.IRepository;
using StackBook.Data;
using StackBook.Models;
using StackBook.Services;
using StackBook.Utils;
using Microsoft.AspNetCore.Builder;
using StackBook.Configurations;
using StackBook.Interfaces;
using DocumentFormat.OpenXml.Bibliography;
using StackBook.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
using StackBook.Hubs;
using StackBook.Areas.Customer.Controllers;
using StackBook.DAL.Repository;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddControllersWithViews();
// Thêm services
builder.Services.AddSignalR();


builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddScoped<OAuthGoogleService>();
builder.Services.AddScoped<JwtUtils>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();


builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<AccountController>();


builder.Configuration.AddJsonFile("appsettings.json");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//builder.Services.AddAuthentication("Cookies")
//    .AddCookie("Cookies", options =>
//    {
//        options.Cookie.Name = "accessToken"; // hoặc tên bất kỳ, nhưng bạn cần xử lý tương ứng
//        options.Events.OnRedirectToLogin = context =>
//        {
//            context.Response.StatusCode = 401;
//            return Task.CompletedTask;
//        };
//    });
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Site/Account/Signin";
        options.AccessDeniedPath = "/AccessDenied";
        options.SlidingExpiration = true;
        //options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

        options.Cookie.Name = "accessToken";
        options.Events.OnValidatePrincipal = async context =>
        {
            await Task.CompletedTask;
        };
    });


builder.Services.AddAuthorization();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<EMailUtils>();

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddScoped<IEmailUtils, EMailUtils>();

builder.Services.Configure<GoogleOAuthConfig>(
    builder.Configuration.GetSection("GoogleOAuth"));

builder.Services.AddSingleton<JwtUtils>();

builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings"));

builder.Services.AddSingleton<CloudinaryUtils>();



builder.Services.AddHttpContextAccessor();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    //https
    app.UseHttpsRedirection();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
// Cấu hình endpoint
//builder.Services.AddHttpContextAccessor();
app.UseAuthentication(); //Middelware
app.UseMiddleware<Authentication>(); //Middleware custom của bạn
app.UseAuthorization(); //Để `[Authorize]` hoạt động
app.MapHub<NotificationHub>("/notificationHub");
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Site}/{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await AddData.InitializeAsync(services);
}



app.Run();