using APICatalogo.Context;
using APICatalogo.DTOS.Mappings;
using APICatalogo.Extension;
using APICatalogo.Filters;
using APICatalogo.Logging;
using APICatalogo.Models;
using APICatalogo.RateLimitOptions;
using APICatalogo.Repositories;
using APICatalogo.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(
    options =>
    {
        options.Filters.Add(typeof(ApiExceptionFilter));
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    })
    .AddNewtonsoftJson();

builder.Services.AddCors(options =>
    options.AddPolicy("OrigensComAcessosPermitidos",
    policy =>
    {
        policy.WithOrigins("https://apirequest.io/")
                                    .WithMethods("GET", "POST")
                                    .AllowAnyHeader();
    })
);



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "apicatalogo", Version = "v1 " });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Bearer JWT",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] { }
        }
    });
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<APICatalogoContext>()
    .AddDefaultTokenProviders();

// authetication

builder.Services.AddAuthorization();
//builder.Services.AddAuthentication("Bearer").AddJwtBearer();


// String Connection with WorkBench for add services to the container

string? mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<APICatalogoContext>(options =>
    options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection)));

//var valor1 = builder.Configuration["chave1"];
//var valor2 = builder.Configuration["secao1:chave2"];


builder.Services.AddTransient<IMyServices, MyService>();

// desativando as chamadas implicitas de from services, exemplo no controlador produto onde tem duas requisições get, uma com from services e outra fromservices implicito
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.DisableImplicitFromServicesParameters = true;
});


var secretyKey = builder.Configuration["JWT:SecretKey"]
    ?? throw new ArgumentException("Invalid secret key!");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Audience = "http://localhost:7128";
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = false,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretyKey))
    };
});


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("SuperAdminOnly", policy => policy.RequireRole("Admin").RequireClaim("id", "Matheus"));
    options.AddPolicy("UserOnly", policy => policy.RequireClaim("Users"));
    options.AddPolicy("ExclusiveOnly", policy =>
    {
        policy.RequireAssertion(APICatalogoContext =>
            APICatalogoContext.User.HasClaim(claim => 
                claim.Type == "id" && claim.Value == "Matheus") 
                || APICatalogoContext.User.IsInRole("SuperAdmin"));
    });
});



// Rate Limite através do appseting. Essa é uma forma mais robusta de se criar Limitadores de Requisição

var myOptions = new MyRateLimitOptions();

builder.Configuration.GetSection(MyRateLimitOptions.MyRateLimit).Bind(myOptions);

// Rate Limiter para controllers e metodos
builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.AddFixedWindowLimiter("fixedwindow", options =>
    {
        options.PermitLimit = myOptions.PermitLimit; //1;
        options.Window = TimeSpan.FromSeconds(myOptions.Window);
        options.QueueLimit = myOptions.QueueLimit;//2;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
    rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// Rate Limiter Global
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                            RateLimitPartition.GetFixedWindowLimiter(
                                                partitionKey: httpContext.User.Identity?.Name ??
                                                              httpContext.Request.Headers.Host.ToString(),
                                                factory: partition => new FixedWindowRateLimiterOptions
                                                {
                                                    AutoReplenishment = true,
                                                    PermitLimit = 2,
                                                    QueueLimit = 0,
                                                    Window = TimeSpan.FromSeconds(10)
                                                }));
});




builder.Services.AddApiVersioning(o =>
{
    o.DefaultApiVersion = new ApiVersion(1, 0);
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.ReportApiVersions = true;
    o.ApiVersionReader = ApiVersionReader.Combine(
                                            new QueryStringApiVersionReader(),
                                            new UrlSegmentApiVersionReader());
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});



builder.Services.AddScoped<ApiLoggingFilter>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information
}));


builder.Services.AddAutoMapper(typeof(ProductDTOMappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ConfigureExceptionHandle();
    //app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseRateLimiter();
app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
