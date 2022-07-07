using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using PingOne.Core.Configuration;
using PingOne.Core.Configuration.Extensions;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddControllersWithViews();

builder.Services.AddPingOneManagement(configuration.GetSection("PingOne:Management")
              .Get<PingOneConfigurationManagement>());

// Allow sign in via an OpenId Connect provider like OneLogin
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login/";
})
.AddOpenIdConnect(options =>
{
    options.ClientId = configuration.GetValue<string>("PingOne:Authentication:ClientId");

    var authBaseUrl = configuration.GetValue<string>("PingOne:Authentication:AuthBaseUrl");
    var environmentId = configuration.GetValue<string>("PingOne:Authentication:EnvironmentId");

    options.Authority = $"{authBaseUrl}/{environmentId}/as";

    options.CallbackPath = "/oidc/signin-ping";

    options.ResponseType = "code";

    options.GetClaimsFromUserInfoEndpoint = true;

    options.Events.OnMessageReceived = ctx =>
    {
        return Task.CompletedTask;
    };

    options.Events.OnTokenValidated = ctx =>
    {

        return Task.CompletedTask;
    };

});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapRazorPages();
app.MapDefaultControllerRoute();

app.Run();
