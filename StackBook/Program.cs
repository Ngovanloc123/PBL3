using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using StackBook.DAL;
using StackBook.DAL.IRepository;
using StackBook.Data;
using StackBook.Models;
using StackBook.Services;
using StackBook.Utils;
using Microsoft.AspNetCore.Builder;
using StackBook.Configurations;
using StackBook.Interfaces;
using StackBook.Controllers;
using DocumentFormat.OpenXml.Bibliography;
using StackBook.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<AuthorService>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddScoped<OAuthGoogleService>();
builder.Services.AddScoped<JwtUtils>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<AccountController>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<EMailUtils>();
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailUtils, EMailUtils>();
builder.Services.Configure<GoogleOAuthConfig>(
    builder.Configuration.GetSection("GoogleOAuthConfig"));
builder.Services.AddSingleton<StackBook.Utils.JwtUtils>();
var app = builder.Build();

//builder.Services.AddHttpContextAccessor();

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
app.UseMiddleware<Authentication>();
// app.UseAuthorization();
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
