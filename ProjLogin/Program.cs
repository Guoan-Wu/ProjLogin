using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProjLogin.Controllers;
using ProjLogin.Encrypt;
using ProjLogin.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Jack config 
ConfigurationManager configuration = builder.Configuration; // allows both to access and to set up the config
IWebHostEnvironment environment = builder.Environment;
builder.Services.AddSingleton(new Appsettings(configuration));
//Jack config authentication.
//�����֤
builder.Services.AddAuthentication(x =>
{
    // ��ϸ��������� ��ͼ�д������ʾ����Ǹ�
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(o => {

    //��ȡ�����ļ�
    var audienceConfig = configuration.GetSection("Audience");
    var symmetricKeyAsBase64 = audienceConfig["Secret"]?? "";
    var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
    var signingKey = new SymmetricSecurityKey(keyByteArray);

    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signingKey,
        ValidateIssuer = true,
        ValidIssuer = audienceConfig["Issuer"],//������
        ValidateAudience = true,
        ValidAudience = audienceConfig["Audience"],//������
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,//����ǻ������ʱ�䣬Ҳ����˵����ʹ���������˹���ʱ�䣬����ҲҪ���ǽ�ȥ������ʱ��+���壬Ĭ�Ϻ�����7���ӣ������ֱ������Ϊ0
        RequireExpirationTime = true,
    };
});

builder.Services.AddControllers();

//Jack config myown authorisation service.
//�����Ȩ���Է���
builder.Services.AddAuthorization(options => {
    options.AddPolicy("Client", policy => policy.RequireRole("Client").Build());//������ɫ
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin").Build());
    options.AddPolicy("SystemOrAdmin", policy => policy.RequireRole("Admin", "System"));//��Ĺ�ϵ
    options.AddPolicy("SystemAndAdmin", policy => policy.RequireRole("Admin").RequireRole("System"));//�ҵĹ�ϵ
});

//Jack ,add token to header on Swageer.
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProjLogin", Version = "v1" });

    // ����С��
    c.OperationFilter<AddResponseHeadersFilter>();
    c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

    // ��header�����token�����ݵ���̨
    c.OperationFilter<SecurityRequirementsOperationFilter>();

    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {

        Description = "JWT��Ȩ(���ݽ�������ͷ�н��д���) ֱ�����¿�������Bearer {token}��ע������֮����һ���ո�\"",
        Name = "Authorization",//jwtĬ�ϵĲ�������
        In = ParameterLocation.Header,//jwtĬ�ϴ��Authorization��Ϣ��λ��(����ͷ��)
        Type = SecuritySchemeType.ApiKey
    });
});  // END AddSwaggerGen

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ProjLoginDBContext>(options =>
  options.UseMySql( builder.Configuration.GetConnectionString( "ProjLogin" ), new MySqlServerVersion( new Version( 8, 0, 31 ))));

//Jack for filter scope.
builder.Services.AddScoped<TokenFilter>();


var app = builder.Build();

//jack, query a specific service.
using (var serviceScope = app.Services.CreateScope())
{
    var myFilter = serviceScope.ServiceProvider.GetRequiredService<TokenFilter>();
    Console.WriteLine("qury TokenFilter instance.");
}

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Jack
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
