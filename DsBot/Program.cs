using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DsBot.Commands;
using DsBot.Data;
using DsBot.Services;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// discord
builder.Services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
{
    GatewayIntents = GatewayIntents.AllUnprivileged |
                     GatewayIntents.GuildVoiceStates |
                     GatewayIntents.Guilds |
                     GatewayIntents.GuildMessages
}));
builder.Services.AddSingleton(x =>
{
    var client = x.GetRequiredService<DiscordSocketClient>();
    return new InteractionService(client.Rest, new InteractionServiceConfig
    {
        DefaultRunMode = RunMode.Async,
        UseCompiledLambda = true
    });
});

//services and bot
builder.Services.AddHostedService<BotService>();
builder.Services.AddDbContext<BotDbContext>(options => options.UseSqlite("Data Source=botdata.db"));
builder.Services.AddSingleton<InteractionHandler>();
builder.Services.AddSingleton<VoiceTrackingService>();
builder.Services.AddScoped<RewardService>();
builder.Services.AddScoped<VoiceRewardSettingsService>();
builder.Services.AddScoped<HouseService>();

// autoreg commands and handlers

var assembly = Assembly.GetExecutingAssembly();
builder.Services.AddHandlers(assembly);
builder.Services.AddBotCommands(assembly);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

