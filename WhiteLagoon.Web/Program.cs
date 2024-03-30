using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Net.Sockets;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Services.Implementation;
using WhiteLagoon.Application.Services.Interface;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(
    option => option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
 );

//rEPOSITORYpATTERN IMPLEMENTATION 
//this includes in IUnitOfWork 
//builder.Services.AddScoped<IVillaRepository, VillaRepository>();
//builder.Services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();

//addding Identity Framework
builder.Services.AddIdentity<AppUser,IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

                //AddEntityFrameworkStores<ApplicationDbContext>() 
                //specifies that Entity Framework Core should be used to store user and role information.

                //.AddDefaultTokenProviders()
                //adds default token providers for functionalities like email confirmation and password reset.

builder.Services.ConfigureApplicationCookie(option =>
{
    option.AccessDeniedPath = "/Account/AccessDenied";
    //option.AccessDeniedPath: This property sets the path to which the user will be redirected when they
    //try to access a resource that they are not authorized to access.
    option.LoginPath = "/Account/Login";
    //This property sets the path to which the user will be redirected when they try to access a resource that requires authentication,
    //but they are not logged in
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

var app = builder.Build();

//initiaiting stripe payment
StripeConfiguration.ApiKey = builder.Configuration.GetValue<string>("Stripe:SecretKey");

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

seedingDatabase();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

void seedingDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitilizer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitilizer.Initialize();
    }
}