using ASC.DataAccess;
using ASC.DataAccess.Interfaces;
using ASC.Solution.Services;
using ASC.Web.Configuration;
using ASC.Web.Data;
using ASC.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCongfig(builder.Configuration).AddMyDependencyGroup(); //Add revise
//builder.Services.AddScoped<INavigationCacheOperations, NavigationCacheOperations>();

//// Add services to the container.
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(connectionString));

////builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ASCWebContext>();

////builder.Services.AddIdentity<IdentityUser, IdentityRole>((options) =>
////{
////    options.User.RequireUniqueEmail = true;
////}).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
////them thu
//builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
//{
//    options.SignIn.RequireConfirmedAccount = true;
//    options.User.RequireUniqueEmail = true;
//})
//.AddEntityFrameworkStores<ApplicationDbContext>()
//.AddDefaultTokenProviders().AddDefaultUI();
//// -->
//builder.Services.AddScoped<DbContext, ApplicationDbContext>();

//builder.Services.AddAuthentication()
//    .AddGoogle(options =>
//    {
//        options.ClientId = "183357356135-alcgp0ln6sh74dlv84ncudchadivi2f6.apps.googleusercontent.//com";
//        options.ClientSecret = "GOCSPX-0EdcukHVnWAjJtljcTRUjbZlQD//99";
//    });
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();

///*builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();*/

//builder.Services.AddOptions();
//builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("AppSettings"));

//builder.Services.AddControllersWithViews();

//builder.Services.AddRazorPages();

//builder.Services.AddTransient<IEmailSender, AuthMessageSender>();
//builder.Services.AddTransient<ISmsSender, AuthMessageSender>();
////Addition lab4
//builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
////End lab4
//builder.Services.AddSingleton<IIdentitySeed, IdentitySeed>();
//builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//builder.Services.AddDistributedMemoryCache();
//builder.Services.AddSession();

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
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areaRoute",
    pattern: "{area:exists}/{controller=Home}/{action=Index}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var storageSeed = scope.ServiceProvider.GetRequiredService<IIdentitySeed>();
    await storageSeed.Seed(
        scope.ServiceProvider.GetService<UserManager<IdentityUser>>(),
        scope.ServiceProvider.GetService<RoleManager<IdentityRole>>(),
        scope.ServiceProvider.GetService<IOptions<ApplicationSettings>>()
    );
}

using (var scope = app.Services.CreateScope())
{
    var navigationCacheOperations = scope.ServiceProvider.GetRequiredService<INavigationCacheOperations>();
    await navigationCacheOperations.CreateNavigationCacheAsync();
}

app.Run();