using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Sc3Hosted.Server.Data;
using Sc3Hosted.Server.Entities;
using Sc3Hosted.Server.Extensions;
using Sc3Hosted.Shared.Helpers;
var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var jwtIssuer = builder.Configuration["JwtIssuer"];
var jwtAudience = builder.Configuration["JwtAudience"];
var jwtSecurityKey = builder.Configuration["JwtSecurityKey"];

builder.Services.AddDbContextFactory<Sc3HostedDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<Sc3HostedDbContext>();
builder.Services.AddSwaggerGen();
builder.Services.ServiceWrapper();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecurityKey)),
            ClockSkew = TimeSpan.FromSeconds(0)
        };
    });
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization(config => {
    config.AddPolicy(Policies.IsAdmin, Policies.IsAdminPolicy());
    config.AddPolicy(Policies.IsUser, Policies.IsUserPolicy());
    config.AddPolicy(Policies.IsManager, Policies.IsManagerPolicy());
});

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseMigrationsEndPoint();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blazor API V1");
});
app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints => {
    endpoints.MapControllers();
    endpoints.MapRazorPages();
    endpoints.MapFallbackToFile("index.html");
});

app.Run();
