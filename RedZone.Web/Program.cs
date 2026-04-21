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
                options.UseSqlServer(connectionString, sqlOptions =>
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null)));

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
            builder.Services.AddSignalR();

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

            app.MapHub<LeaderboardHub>("/hubs/leaderboard");

            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var dbContext = scope.ServiceProvider.GetRequiredService<RedZoneDbContext>();

                // Seed roles
                string[] roles = { "Admin", "User" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                // Seed Admin account
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

                // Seed Demo account 1
                var demo1Email = "demo1@gmail.com";
                var demo1User = await userManager.FindByEmailAsync(demo1Email);

                if (demo1User == null)
                {
                    demo1User = new IdentityUser
                    {
                        UserName = demo1Email,
                        Email = demo1Email,
                        EmailConfirmed = true
                    };

                    await userManager.CreateAsync(demo1User, "demo123");
                }

                if (!await userManager.IsInRoleAsync(demo1User, "User"))
                {
                    await userManager.AddToRoleAsync(demo1User, "User");
                }

                // Seed Demo account 2
                var demo2Email = "demo2@gmail.com";
                var demo2User = await userManager.FindByEmailAsync(demo2Email);

                if (demo2User == null)
                {
                    demo2User = new IdentityUser
                    {
                        UserName = demo2Email,
                        Email = demo2Email,
                        EmailConfirmed = true
                    };

                    await userManager.CreateAsync(demo2User, "demo1234");
                }

                if (!await userManager.IsInRoleAsync(demo2User, "User"))
                {
                    await userManager.AddToRoleAsync(demo2User, "User");
                }

                try
                {
                    // Seed Competitions
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

                    // Seed Matches
                    if (!dbContext.Matches.Any())
                    {
                        var premierLeagueId = dbContext.Competitions.First(c => c.Name == "Premier League").Id;
                        var championsLeagueId = dbContext.Competitions.First(c => c.Name == "Champions League").Id;
                        var faCupId = dbContext.Competitions.First(c => c.Name == "FA Cup").Id;
                        var eflCupId = dbContext.Competitions.First(c => c.Name == "EFL Cup").Id;

                        var matches = new List<Match>
                        {
                            new Match { HomeTeam = "Liverpool", AwayTeam = "Manchester City", MatchDate = new DateTime(2026, 5, 3, 15, 0, 0), CompetitionId = premierLeagueId, Status = MatchStatus.Upcoming },
                            new Match { HomeTeam = "Arsenal", AwayTeam = "Liverpool", MatchDate = new DateTime(2026, 5, 6, 19, 45, 0), CompetitionId = premierLeagueId, Status = MatchStatus.Upcoming },
                            new Match { HomeTeam = "Liverpool", AwayTeam = "Real Madrid", MatchDate = new DateTime(2026, 5, 9, 21, 0, 0), CompetitionId = championsLeagueId, Status = MatchStatus.Upcoming },
                            new Match { HomeTeam = "Liverpool", AwayTeam = "Chelsea", MatchDate = new DateTime(2026, 5, 12, 17, 30, 0), CompetitionId = faCupId, Status = MatchStatus.Upcoming },
                            new Match { HomeTeam = "Newcastle", AwayTeam = "Liverpool", MatchDate = new DateTime(2026, 5, 15, 15, 0, 0), CompetitionId = premierLeagueId, Status = MatchStatus.Upcoming },
                            new Match { HomeTeam = "Liverpool", AwayTeam = "PSG", MatchDate = new DateTime(2026, 5, 18, 21, 0, 0), CompetitionId = championsLeagueId, Status = MatchStatus.Upcoming },
                            new Match { HomeTeam = "Liverpool", AwayTeam = "Tottenham", MatchDate = new DateTime(2026, 5, 21, 16, 30, 0), CompetitionId = premierLeagueId, Status = MatchStatus.Upcoming },
                            new Match { HomeTeam = "Everton", AwayTeam = "Liverpool", MatchDate = new DateTime(2026, 5, 24, 14, 0, 0), CompetitionId = eflCupId, Status = MatchStatus.Upcoming },
                            new Match { HomeTeam = "Liverpool", AwayTeam = "Manchester United", MatchDate = new DateTime(2026, 5, 27, 18, 30, 0), CompetitionId = premierLeagueId, Status = MatchStatus.Upcoming },
                            new Match { HomeTeam = "Barcelona", AwayTeam = "Liverpool", MatchDate = new DateTime(2026, 5, 30, 21, 0, 0), CompetitionId = championsLeagueId, Status = MatchStatus.Upcoming },
                            new Match { HomeTeam = "Liverpool", AwayTeam = "AC Milan", MatchDate = new DateTime(2026, 6, 2, 20, 45, 0), CompetitionId = championsLeagueId, Status = MatchStatus.Upcoming },
                            new Match { HomeTeam = "Brighton", AwayTeam = "Liverpool", MatchDate = new DateTime(2026, 6, 5, 16, 0, 0), CompetitionId = premierLeagueId, Status = MatchStatus.Upcoming },
                            new Match { HomeTeam = "Liverpool", AwayTeam = "West Ham", MatchDate = new DateTime(2026, 6, 8, 19, 30, 0), CompetitionId = premierLeagueId, Status = MatchStatus.Upcoming },
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