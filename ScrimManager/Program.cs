using ScrimManagerApplication.Application;
using ScrimManagerApplication.Application.Interfaces;
using ScrimManagerDataAccess;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddScoped<TournamentService>();
builder.Services.AddScoped<ITournamentRepository, TournamentRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();

app.Run();