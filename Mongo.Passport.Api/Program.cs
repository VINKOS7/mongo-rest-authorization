using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using Dotseed.EventBus.RabbitMQ.Options;
using Dotseed.IntegrationEventLog.InMemory.Setup;
using Dotseed.EventBus.RabbitMQ.Setup;

using Mongo.Passport.Api.Extensions;
using Mongo.Passport.Api.Options;
using Mongo.Passport.Infrastructure.Options.HttpClientYandexCaptchaOptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureMongoDriver(builder.Configuration);
builder.Services.ConfigureApplicationServices(builder.Configuration);
builder.Services.ConfigureInfrastructureServices();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection("JWTOptions"));
builder.Services.Configure<ImportantEndpontsOptions>(builder.Configuration.GetSection("ImportantEndponts"));
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("EmailOptions"));
builder.Services.Configure<HttpClientYandexCaptchaOptions>(builder.Configuration.GetSection("HttpClientYandexCaptchaOptions"));
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMqSettings"));

//If you want use RabbitMQ
//builder.Services.ConfigureInMemoryEventLog();
//builder.Services.ConfigureEventBus();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                // укзывает, будет ли валидироваться издатель при валидации токена
                ValidateIssuer = true,

                // строка, представляющая издателя
                ValidIssuer = builder.Configuration["JWTOptions:Issuer"],

                // будет ли валидироваться потребитель токена
                ValidateAudience = false,

                // установка потребителя токена
                //ValidAudience = AuthOptions.AUDIENCE, // ----------------------------------

                // будет ли валидироваться время существования
                ValidateLifetime = true,

                // установка ключа безопасности
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTOptions:SecretKey"])),

                // валидация ключа безопасности
                ValidateIssuerSigningKey = false,
            };
        });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseStatusCodePages();

app.Run();
