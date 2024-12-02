using ApiIntento3.seguridad;

var builder = WebApplication.CreateBuilder(args);

// Configurar el servidor Kestrel
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5004); // Configura el servidor para que escuche en el puerto 5004
});

// Configuración de CORS para permitir todos los orígenes
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Registro de servicios
builder.Services.AddControllers();

// Configuración de Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registro de la clase hash para inyección de dependencias
builder.Services.AddScoped<hash>(); // Esto asegura que 'hash' se inyecte correctamente en los controladores

var app = builder.Build();

// Usar la política CORS definida previamente
app.UseCors("AllowAllOrigins");

// Configuración de Swagger en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Uso de autorización
app.UseAuthorization();

// Mapear los controladores a las rutas
app.MapControllers();

// Ejecutar la aplicación
app.Run();
