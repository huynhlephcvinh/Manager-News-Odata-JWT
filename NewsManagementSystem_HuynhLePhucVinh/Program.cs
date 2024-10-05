using DataAccessLayer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using NewsManagementSystem_HuynhLePhucVinh;
using ProductManagementAPI.Mapping;
using Repositories;
using Services;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
// Add services to the container.
builder.Services.AddScoped(typeof(FunewsManagementFall2024Context));

builder.Services.AddControllers().AddOData(
o => o.Select().Filter().OrderBy().Expand().Count().SetMaxTop(null).AddRouteComponents("odata", ModelBuilder.GetEDMModel())
);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

});
builder.Services.AddSwaggerGen(w =>
{
    w.AddSecurityRequirement(new OpenApiSecurityRequirement
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
builder.Services
   .AddSwaggerGen(c =>
   {
       c.SwaggerDoc("odata", new OpenApiInfo { Title = "Assignment", Version = "odata" });
 //      c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
   });
builder.Services.AddSwaggerGen(c =>
{
    c.ResolveConflictingActions(apiDescriptions =>
    {
        var descriptions = apiDescriptions as ApiDescription[] ?? apiDescriptions.ToArray();
        var first = descriptions.First(); // build relative to the 1st method
        var parameters = descriptions.SelectMany(d => d.ParameterDescriptions).ToList();

        first.ParameterDescriptions.Clear();
        // add parameters and make them optional
        foreach (var parameter in parameters)
            if (first.ParameterDescriptions.All(x => x.Name != parameter.Name))
            {
                first.ParameterDescriptions.Add(new ApiParameterDescription
                {
                    ModelMetadata = parameter.ModelMetadata,
                    Name = parameter.Name,
                    ParameterDescriptor = parameter.ParameterDescriptor,
                    Source = parameter.Source,
                    IsRequired = false,
                    DefaultValue = null
                });
            }
        return first;
    });
});
AddFormatters(builder.Services);

 void AddFormatters(IServiceCollection services)
{
    services.AddMvcCore(options =>
    {
        foreach (var outputFormatter in options.OutputFormatters.OfType<ODataOutputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
        {
            outputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
        }
        foreach (var inputFormatter in options.InputFormatters.OfType <ODataInputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
        {
            inputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
        }
    });
}

builder.Services.AddScoped<ISystemAccountService, SystemAccountService>();
builder.Services.AddScoped<ISystemAccountRepository, SystemAccountRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<INewsArticleService, NewsArticleService>();
builder.Services.AddScoped<INewsArticleRepository, NewsArticleRepository>();

var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var token = context.SecurityToken as JwtSecurityToken;
                if (token != null && !JwtGenerator.IsTokenValid(token.RawData))
                {
                    context.Fail("Token is invalid.");
                }

                return Task.CompletedTask;
            }
        };
    });
var app = builder.Build();

app.UseCors(builder =>
       builder.WithOrigins("*")
           .AllowAnyMethod()
           .AllowAnyHeader());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(
        c=> {
            c.SwaggerEndpoint("/swagger/odata/swagger.json", "Assignment V1");
            }
        );
}

app.UseODataBatching();

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
