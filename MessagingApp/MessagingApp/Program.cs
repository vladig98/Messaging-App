using MessagingApp.Data;
using MessagingApp.Hubs;
using MessagingApp.Middleware.ExtensionMethods;
using MessagingApp.Models;
using MessagingApp.Services.Contracts;
using MessagingApp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Primitives;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string[] signalRPaths = builder.Configuration.GetSection("SignalRPaths").Get<string[]>() ?? [];

if (signalRPaths.Length == 0)
{
    throw new InvalidOperationException("No SignalR URLs configured.");
}

string jwtIssuer = builder.Configuration["MessagingApp:JWT:Issuer"] ?? throw new InvalidOperationException("No JWT issuer provided!");
string jwtKey = builder.Configuration["MessagingApp:JWT:Key"] ?? throw new InvalidOperationException("No JWT Key provided!");

// Service Registration
builder.Services.AddIdentityCore<User>().AddSignInManager().AddRoles<Role>().AddEntityFrameworkStores<MessageAppDbContext>().AddDefaultTokenProviders();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IChatService, ChatService>();

// Database Context Setup
string connectionString = builder.Configuration["ConnectionStrings:MainDb"] ?? throw new InvalidOperationException("Connection string 'MainDb' not found.");

builder.Services.AddDbContext<MessageAppDbContext>(options => options.UseSqlServer(connectionString, b =>
{
    b.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
}));

// Authentication and Authorization Setup
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
 {
     options.IncludeErrorDetails = true;
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = jwtIssuer,
         ValidAudience = jwtIssuer,
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
         ClockSkew = TimeSpan.Zero
     };

     // SignalR Authentication
     options.Events = new JwtBearerEvents
     {
         OnMessageReceived = context => {
             StringValues accessToken = context.Request.Query["access_token"];

             // If the request is for our hub...
             PathString path = context.HttpContext.Request.Path;

             if (StringValues.IsNullOrEmpty(accessToken))
             {
                 return Task.CompletedTask;
             }

             if (!signalRPaths.Any(x => path.StartsWithSegments(x)))
             {
                 return Task.CompletedTask;
             }

             context.Token = accessToken;

             return Task.CompletedTask;
         }
     };
 });

builder.Services.AddAuthorization();

// Middleware Setup
builder.Services.AddCors();
builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(cfg =>
{
    cfg.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT as Bearer",
        Name = "Authorization",
        In = ParameterLocation.Header,
        // Allows entering JWT token without 'Bearer' keyword
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    OpenApiSecurityScheme scheme = new()
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    cfg.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { scheme, [] }
    });
});

// Application Setup
WebApplication app = builder.Build();

// Development specific configurations
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware registration
app.UseErrorHandlingMiddleware();
app.UseHttpsRedirection();
app.UseCors(x => x.AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin => true).AllowCredentials());
app.UseAuthentication();
app.UseAuthorization();

// Endpoint mapping
app.MapControllers();
app.MapHub<UserHub>(builder.Configuration["SignalRPaths:UserHub"] ?? throw new InvalidOperationException("No SignalR URL for Users."));
app.MapHub<ChatHub>(builder.Configuration["SignalRPaths:ChatHub"] ?? throw new InvalidOperationException("No SignalR URL for Chat."));

app.Run();