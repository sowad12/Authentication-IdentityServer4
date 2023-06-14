using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Test;
using IdServer;
using IdServer.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();


//identity ApplicationIdDbContext configuration
var assembly = typeof(Program).Assembly.GetName().Name;
var defaultConnString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationIdDbContext>(options =>
    options.UseSqlServer(defaultConnString,
         b => b.MigrationsAssembly(assembly)));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationIdDbContext>();

//Registering IdentityServer4 in ASP.NET Core without DB in memory configuration
//builder.Services.AddIdentityServer()
//              .AddInMemoryClients(Config.Clients)
//              .AddInMemoryApiScopes(Config.ApiScopes)
//              .AddInMemoryIdentityResources(Config.IdentityResources)
//              .AddTestUsers(Config.TestUsers)
//              .AddDeveloperSigningCredential();


//Registering IdentityServer4 in ASP.NET Core with DB 
builder.Services.AddIdentityServer()
    .AddAspNetIdentity<IdentityUser>()
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = b => b.UseSqlServer(defaultConnString, opt => opt.MigrationsAssembly(assembly));
    })
    .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = b => b.UseSqlServer(defaultConnString, opt => opt.MigrationsAssembly(assembly));
    })
    .AddDeveloperSigningCredential();



//google authentication
var configuration = builder.Configuration;

builder.Services.Configure<GoogleOptions>(configuration.GetSection("Authentication:Google"));

builder.Services.AddAuthentication()
    .AddGoogle(options =>

    {
        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
        //options.SignInScheme = "Identity.External";
        options.ClientId = configuration["Authentication:Google:ClientId"];
        options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
    });



//builder.services.configure<googleoptions>(configuration.getsection("authentication:google"));
//builder.services.addauthentication()
//                .addgoogle("google", options =>
//                {
//                    options.signinscheme = identityserverconstants.externalcookieauthenticationscheme;

//                    //options.clientid = "<insert here>";
//                    //options.clientsecret = "<insert here>";

//                    options.clientid = configuration["authentication:google:clientid"];
//                    options.clientsecret = configuration["authentication:google:clientsecret"];
//                })
//                .addopenidconnect("oidc", "demo identityserver", options =>
//                {
//                    options.signinscheme = identityserverconstants.externalcookieauthenticationscheme;
//                    options.signoutscheme = identityserverconstants.signoutscheme;
//                    options.savetokens = true;

//                    options.authority = "https://demo.identityserver.io/";
//                    options.clientid = "interactive.confidential";
//                    options.clientsecret = "secret";
//                    options.responsetype = "hybrid";

//                    options.tokenvalidationparameters = new tokenvalidationparameters
//                    {
//                        nameclaimtype = "name",
//                        roleclaimtype = "role"
//                    };
//                });


//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//})
//                .AddCookie(options =>
//                {
//                    options.LoginPath = "/account/google-login";
//                })
//                .AddGoogle(options =>
//                {
//                    options.ClientId = configuration["authentication:google:clientid"];
//                    options.ClientSecret = configuration["authentication:google:clientsecret"];
//                });



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

#region Initialized Database
using (var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
{
    serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

    var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
    context.Database.Migrate();
    if (!context.Clients.Any())
    {
        foreach (var client in Config.Clients)
        {
            context.Clients.Add(client.ToEntity());
        }
        context.SaveChanges();
    }

    if (!context.IdentityResources.Any())
    {
        foreach (var resource in Config.IdentityResources)
        {
            context.IdentityResources.Add(resource.ToEntity());
        }
        context.SaveChanges();
    }

    if (!context.ApiScopes.Any())
    {
        foreach (var resource in Config.ApiScopes.ToList())
        {
            context.ApiScopes.Add(resource.ToEntity());
        }

        context.SaveChanges();
    }

    if (!context.ApiResources.Any())
    {
        foreach (var resource in Config.ApiResources)
        {
            context.ApiResources.Add(resource.ToEntity());
        }
        context.SaveChanges();
    }
}
#endregion


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();
app.UseAuthentication();

app.UseAuthorization();

//app.MapRazorPages();
app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

app.Run();