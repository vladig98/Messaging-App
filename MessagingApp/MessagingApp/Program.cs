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

var builder = WebApplication.CreateBuilder(args);

var jwtIssuer = builder.Configuration["MessagingApp:JWT:Issuer"];
var jwtKey = builder.Configuration["MessagingApp:JWT:Key"];

// Service Registration
builder.Services.AddIdentityCore<User>().AddSignInManager().AddRoles<Role>().AddEntityFrameworkStores<MessageAppDbContext>().AddDefaultTokenProviders();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IChatService, ChatService>();

// Database Context Setup
var connectionString = builder.Configuration["ConnectionStrings:AZURE_MYSQL_CONNECTIONSTRING"] ??
        throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<MessageAppDbContext>(options => options.UseSqlServer(connectionString));

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

     //adds authentication to SignalR
     options.Events = new JwtBearerEvents
     {
         OnMessageReceived = context => {
             var accessToken = context.Request.Query["access_token"];

             // If the request is for our hub...
             var path = context.HttpContext.Request.Path;
             if (!string.IsNullOrEmpty(accessToken) &&
                 ((path.StartsWithSegments("/getuserinfo")) ||
                     (path.StartsWithSegments("/getchatinfo"))))
             {
                 // Read the token out of the query string
                 context.Token = accessToken;
             }
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
        //allows entering JWT without typing Bearer in front of it while ApiKey requires to write Bearer in fromt of the token
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    cfg.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
        new OpenApiSecurityScheme
            {
        Reference = new OpenApiReference
                {
        Type = ReferenceType.SecurityScheme,
        Id = "Bearer"
    }
            },
    new string[] {}
        }
    });
});

// Application Setup
var app = builder.Build();

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
app.MapHub<UserHub>("/getuserinfo");
app.MapHub<ChatHub>("/getchatinfo");

app.Run();