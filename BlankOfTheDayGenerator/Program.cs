using BlankOfTheDayGenerator.Components;
using Data.Models;
using Data.Services;
using DictionaryApiClient;
using Domain.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<BlankOfTheDayContext>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IEmotionRepoistory, EmotionRepository>();
builder.Services.AddScoped<IEmotionService, EmotionService>();
builder.Services.AddTransient<IDefiner, FreeDictionaryApiClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
