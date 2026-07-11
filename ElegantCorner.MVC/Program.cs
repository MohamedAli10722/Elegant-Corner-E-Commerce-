using ElegantCorner.Application.Interfaces;
using ElegantCorner.Application.Interfaces.Admin;
using ElegantCorner.Application.Services;
using ElegantCorner.Application.Services.Admin;
using ElegantCorner.Domain.Entities;
using ElegantCorner.Domain.Interfaces;
using ElegantCorner.Infrastructure.Data;
using ElegantCorner.Infrastructure.Repositories;
using ElegantCorner.Infrastructure.Services;
using ElegantCorner.MVC.Interfaces;
using ElegantCorner.MVC.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ── Database ─────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Identity ──────────────────────────────────────────────────────────────────
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// ── Authentication: Cookie (MVC default) + JWT (API) ─────────────────────────
// IMPORTANT: Cookie must be the default scheme so MVC sessions work correctly.
// The API project (or api/* routes) uses JWT separately.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;

    options.Events.OnRedirectToLogin = ctx =>
    {
        if (ctx.Request.Headers.Accept.ToString().Contains("application/json") ||
            ctx.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            ctx.Response.StatusCode = 401;
            return Task.CompletedTask;
        }

        ctx.Response.Redirect(ctx.RedirectUri);
        return Task.CompletedTask;
    };
});

builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        // JWT Configuration
    });

// ── Repositories & Services ───────────────────────────────────────────────────
builder.Services.AddScoped<IUnitOfWork,      UnitOfWork>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService,  ProductService>();
builder.Services.AddScoped<ICartService,     CartService>();
builder.Services.AddScoped<IOrderService,    OrderService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IProductAdminService, ProductAdminService>();
builder.Services.AddScoped<IProductAdminService, ProductAdminService>();
builder.Services.AddScoped<ICategoryAdminService, CategoryAdminService>();

// ── MVC + API ─────────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();
builder.Services.AddAntiforgery(o => o.HeaderName = "RequestVerificationToken");

// ── CORS (for standalone API consumers if needed) ─────────────────────────────
builder.Services.AddCors(options =>
    options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

// ── Middleware pipeline ───────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// API routes (JWT-protected, handled by controllers with [ApiController])
app.MapControllers();

// MVC routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
