using ApiIntento3.seguridad;

var builder = WebApplication.CreateBuilder(args);

// Configurar el servidor Kestrel
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5004); // Configura el servidor para que escuche en el puerto 5004
});

// Configuraci�n de CORS para permitir todos los or�genes
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

// Configuraci�n de Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registro de la clase hash para inyecci�n de dependencias
builder.Services.AddScoped<hash>(); // Esto asegura que 'hash' se inyecte correctamente en los controladores

var app = builder.Build();

// Usar la pol�tica CORS definida previamente
app.UseCors("AllowAllOrigins");

// Configuraci�n de Swagger en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Uso de autorizaci�n
app.UseAuthorization();

// Mapear los controladores a las rutas
app.MapControllers();

// Ejecutar la aplicaci�n
app.Run();
