using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RedZone.Data;
using RedZone.Data.Models.Entities;
using RedZone.Data.Models.Enums;
using RedZone.Services.Core;
using RedZone.Services.Core.Interfaces;
using RedZone.Web.Hubs;

namespace RedZone.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<RedZoneDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<RedZoneDbContext>();

            builder.Services.AddControllersWithViews();
            builder.Services.AddSignalR(); // ← ADD THIS

            builder.Services.AddScoped<ICompetitionService, CompetitionService>();
            builder.Services.AddScoped<IMatchService, MatchService>();
            builder.Services.AddScoped<IPredictionService, PredictionService>();
            builder.Services.AddScoped<ICommentService, CommentService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?statusCode={0}");
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.MapRazorPages()
                .WithStaticAssets();

            app.MapHub<LeaderboardHub>("/hubs/leaderboard"); // ← ADD THIS

            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var dbContext = scope.ServiceProvider.GetRequiredService<RedZoneDbContext>();

                string[] roles = { "Admin", "User" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                var adminEmail = "adminnew@redzone.com";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);

                if (adminUser == null)
                {
                    adminUser = new IdentityUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    await userManager.CreateAsync(adminUser, "Admin123!");
                }

                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }

                try
                {
                    if (!dbContext.Competitions.Any())
                    {
                        var competitions = new List<Competition>
                        {
                            new Competition { Name = "Premier League" },
                            new Competition { Name = "Champions League" },
                            new Competition { Name = "FA Cup" },
                            new Competition { Name = "EFL Cup" }
                        };

                        await dbContext.Competitions.AddRangeAsync(competitions);
                        await dbContext.SaveChangesAsync();
                    }

                    if (!dbContext.Matches.Any())
                    {
                        var premierLeagueId = dbContext.Competitions.First(c => c.Name == "Premier League").Id;
                        var championsLeagueId = dbContext.Competitions.First(c => c.Name == "Champions League").Id;
                        var faCupId = dbContext.Competitions.First(c => c.Name == "FA Cup").Id;
                        var eflCupId = dbContext.Competitions.First(c => c.Name == "EFL Cup").Id;

                        var matches = new List<Match>
                        {
                            new Match { HomeTeam = "Liverpool", AwayTeam = "Manchester City", MatchDate = new DateTime(2026, 4, 2, 20, 0, 0), CompetitionId = premierLeagueId, Status = MatchStatus.Upcoming },
                            new Match { HomeTeam = "Arsenal", AwayTeam = "Liverpool", MatchDate = new DateTime(2026, 4, 5, 19, 45, 0), CompetitionId = premierLeagueId, Status = MatchStatus.Upcoming },
                            new Match { HomeTeam = "Liverpool", AwayTeam = "Real Madrid", MatchDate = new DateTime(2026, 4, 8, 21, 0, 0), CompetitionId = championsLeagueId, Status = MatchStatus.Upcoming },
                            new Match { HomeTeam = "Liverpool", AwayTeam = "Chelsea", MatchDate = new DateTime(2026, 4, 11, 17, 30, 0), CompetitionId = faCupId, Status = MatchStatus.Upcoming },
                            new Match { HomeTeam = "Newcastle", AwayTeam = "Liverpool", MatchDate = new DateTime(2026, 4, 14, 15, 0, 0), CompetitionId = premierLeagueId, Status = MatchStatus.Upcoming },
                            new Match { HomeTeam = "Liverpool", AwayTeam = "PSG", MatchDate = new DateTime(2026, 4, 17, 21, 0, 0), CompetitionId = championsLeagueId, Status = MatchStatus.Upcoming },
                            new Match { HomeTeam = "Liverpool", AwayTeam = "Tottenham", MatchDate = new DateTime(2026, 4, 20, 16, 30, 0), CompetitionId = premierLeagueId, Status = MatchStatus.Upcoming },
                            new Match { HomeTeam = "Everton", AwayTeam = "Liverpool", MatchDate = new DateTime(2026, 4, 23, 14, 0, 0), CompetitionId = eflCupId, Status = MatchStatus.Upcoming },
                            new Match { HomeTeam = "Liverpool", AwayTeam = "Manchester United", MatchDate = new DateTime(2026, 4, 26, 18, 30, 0), CompetitionId = premierLeagueId, Status = MatchStatus.Upcoming },
                            new Match { HomeTeam = "Barcelona", AwayTeam = "Liverpool", MatchDate = new DateTime(2026, 4, 29, 21, 0, 0), CompetitionId = championsLeagueId, Status = MatchStatus.Upcoming },
                            new Match { HomeTeam = "Liverpool", AwayTeam = "AC Milan", MatchDate = new DateTime(2026, 5, 2, 20, 45, 0), CompetitionId = championsLeagueId, Status = MatchStatus.Upcoming },
                            new Match { HomeTeam = "Brighton", AwayTeam = "Liverpool", MatchDate = new DateTime(2026, 5, 5, 16, 0, 0), CompetitionId = premierLeagueId, Status = MatchStatus.Upcoming },
                            new Match { HomeTeam = "Liverpool", AwayTeam = "West Ham", MatchDate = new DateTime(2026, 5, 8, 19, 30, 0), CompetitionId = premierLeagueId, Status = MatchStatus.Upcoming },
                        };

                        await dbContext.Matches.AddRangeAsync(matches);
                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Seeding failed: {ex.Message}");
                }
            }

            app.Run();
        }
    }
}