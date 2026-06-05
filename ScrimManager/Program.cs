using ScrimManagerApplication.Application;
using ScrimManagerApplication.Application.Interfaces;
using ScrimManagerDataAccess;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddSession();

builder.Services.AddScoped<TournamentService>();

builder.Services.AddScoped<ITournamentRepository>(provider =>
{
    string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
    return new TournamentRepository(connectionString);
});

builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<IUserRepository>(provider =>
{
    string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
    return new UserRepository(connectionString);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapRazorPages();

app.Run();