using Amazon.DynamoDBv2.Model;
using ecomm_project_clothes.Dataaccess.Data;
using ecomm_project_clothes.Dataaccess.Repository;
using ecomm_project_clothes.Dataaccess.Repository.IRepository;
using ecomm_project_clothes.Utility;
using Laptop_Ecommerce.Twilio;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("Constr");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<emailSender, emailSender>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.Configure<StripeSetting>(builder.Configuration.GetSection("Stripe"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<TwilioSettings>();
builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("Twilio"));
builder.Services.AddScoped<ISMSService, SMSService>();
//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//  .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddIdentity<IdentityUser,IdentityRole>().AddDefaultTokenProviders().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddScoped<IUnitofwork, Unitofwork>();
builder.Services.AddRazorPages();
builder.Services.AddScoped<IEmailSender,emailSender>();
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
// To Add Error messages while unauthorized Access
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
    options.LogoutPath = $"/Identity/Account/Logout";
});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = "725041659836761";
    options.AppSecret = "4b210b8acef6339cc38c22d31079e736";
});
builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = "558131963224-fbk9dd2ba09q4p9ijprskcfd69neef51.apps.googleusercontent.com";
    options.ClientSecret = "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";
});
builder.Services.AddAuthentication().AddTwitter(options =>
{
    options.ConsumerKey = "a4CZ2v41FkFvpa09Qax8D86O7";
    options.ConsumerSecret = "hCAYK1SmFBs9TnqJh2o0GntJcxUXn9ydNwiWuYXbC45I73u4Mf";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe")["SecretKey"];
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
