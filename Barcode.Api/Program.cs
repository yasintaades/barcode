using Barcode.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Tambahkan CORS agar Next.js bisa akses
builder.Services.AddCors(options => {
    options.AddPolicy("AllowNextJS", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. Daftarkan Service kita
builder.Services.AddScoped<ISftpService, SftpService>();

var app = builder.Build();

app.UseCors("AllowNextJS");
app.UseAuthorization();
app.MapControllers();

app.Run();