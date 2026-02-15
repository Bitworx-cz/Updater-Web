using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Updater.Web;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.Services
    .AddRazorPages().Services
    .AddServerSideBlazor().Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie().Services
    .AddAuthentication().AddGoogle(options =>
    {
        options.ClientId = builder.Configuration.GetExpectedValue<string>("Google:ClientId");
        options.ClientSecret = builder.Configuration.GetExpectedValue<string>("Google:ClientSecret");
        options.ClaimActions.MapJsonKey("urn:google:profile", "link");
        options.ClaimActions.MapJsonKey("urn:google:image", "picture");
    })
    .Services.AddAuthorization();
builder.AddRedisOutputCache("cache");
builder.Services.Configure<RazorPagesOptions>(options => options.RootDirectory = "/Components");
//builder.Services.AddHttpClient("apiservice", client => client.BaseAddress = new("https://localhost:7501"));
builder.Services.AddHttpClient("apiservice", client => client.BaseAddress = new("https://updaterapi.premiumasp.net"));


var app = builder.Build();

if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Error")
        .UseHsts();

app.UseHttpsRedirection()
    .UseStaticFiles()
    .UseCookiePolicy()
    .UseAuthentication()
    .UseAuthorization()
    .UseRouting();
app.UseAntiforgery();

app.UseOutputCache();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();
