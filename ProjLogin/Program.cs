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
//添加认证
builder.Services.AddAuthentication(x =>
{
    // 仔细看这个单词 上图中错误的提示里的那个
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(o => {

    //读取配置文件
    var audienceConfig = configuration.GetSection("Audience");
    var symmetricKeyAsBase64 = audienceConfig["Secret"]?? "";
    var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
    var signingKey = new SymmetricSecurityKey(keyByteArray);

    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signingKey,
        ValidateIssuer = true,
        ValidIssuer = audienceConfig["Issuer"],//发行人
        ValidateAudience = true,
        ValidAudience = audienceConfig["Audience"],//订阅人
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,//这个是缓冲过期时间，也就是说，即使我们配置了过期时间，这里也要考虑进去，过期时间+缓冲，默认好像是7分钟，你可以直接设置为0
        RequireExpirationTime = true,
    };
});

builder.Services.AddControllers();

//Jack config myown authorisation service.
//添加授权策略服务
builder.Services.AddAuthorization(options => {
    options.AddPolicy("Client", policy => policy.RequireRole("Client").Build());//单独角色
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin").Build());
    options.AddPolicy("SystemOrAdmin", policy => policy.RequireRole("Admin", "System"));//或的关系
    options.AddPolicy("SystemAndAdmin", policy => policy.RequireRole("Admin").RequireRole("System"));//且的关系
});

//Jack ,add token to header on Swageer.
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProjLogin", Version = "v1" });

    // 开启小锁
    c.OperationFilter<AddResponseHeadersFilter>();
    c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

    // 在header中添加token，传递到后台
    c.OperationFilter<SecurityRequirementsOperationFilter>();

    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {

        Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
        Name = "Authorization",//jwt默认的参数名称
        In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
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
