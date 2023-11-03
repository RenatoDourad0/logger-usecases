using AeC.digitalJourney.Lib.Monitor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var env = new Env { ENV = "production", LOG_MIN_LEVEL = "debug", OUTPUT = "file", APPLICATION="minimal-api" };
var logger = LoggerConfigurationHelper.ConfigureLogger(env);
builder.Services.AddSingleton<CustomLogger>(logger);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// midleware deve adicionar id passado na requisicao OK
// middleware deve criar id caso nao exista? SIM

app.UseMiddleware<HttpLoggingMiddleware>();


// logger da rota vai pegar o id? OK
// objeto secundario OK
app.MapPost("/", async (HttpContext context) => {
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();

    context.Response.StatusCode = 200;
    var teste = new Dictionary<string, object>
    {
        { "chave1", 123 },
        { "chave2", new int[] { 1, 2, 3 } },
        { "nome", new int[] { 1, 2, 3 } },
        { "chave4", new {
            foo = "bar",
            nome = "renato",
            numbers = new int[] { 1, 2, 3 },
            info = new object[] { new { cpf = "123" }, new { foo = "bar" }},
        }},
    };

    // logger.Information("Log 1", "200", teste);

    logger.Information("log2", "200", body);
    return;
});

app.Run();
