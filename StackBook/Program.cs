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
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<AuthorService>();
builder.Services.AddScoped<SearchService>();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<EMailUtils>();

var app = builder.Build();

//builder.Services.AddHttpContextAccessor();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Site}/{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await AddData.InitializeAsync(services);
}


app.Run();
