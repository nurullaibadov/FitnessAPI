using FitnessAPI.Domain.Entities;
using FitnessAPI.Domain.Enums;
using FitnessAPI.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FitnessAPI.Persistence.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(FitnessDbContext context, ILogger logger)
    {
        try
        {
            await context.Database.MigrateAsync();
            await SeedRolesAsync(context);
            await SeedAdminUserAsync(context);
            await SeedCategoriesAsync(context);
            await SeedMuscleGroupsAsync(context);
            logger.LogInformation("Database seeded successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred seeding the database.");
        }
    }

    private static async Task SeedRolesAsync(FitnessDbContext context)
    {
        if (await context.Roles.AnyAsync()) return;

        var roles = new List<Role>
        {
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Admin", Description = "Administrator with full access" },
            new() { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "User", Description = "Regular user" },
            new() { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Trainer", Description = "Fitness trainer" }
        };

        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();
    }

    private static async Task SeedAdminUserAsync(FitnessDbContext context)
    {
        if (await context.Users.AnyAsync(x => x.Email == "admin@fitnessapp.com")) return;

        var adminUser = new User
        {
            Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@fitnessapp.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123456"),
            IsActive = true,
            IsEmailVerified = true,
            Gender = Gender.Other,
            GoalType = GoalType.GeneralFitness
        };

        await context.Users.AddAsync(adminUser);

        var adminRole = await context.Roles.FirstAsync(x => x.Name == "Admin");
        await context.UserRoles.AddAsync(new UserRole { UserId = adminUser.Id, RoleId = adminRole.Id });

        await context.SaveChangesAsync();
    }

    private static async Task SeedCategoriesAsync(FitnessDbContext context)
    {
        if (await context.Categories.AnyAsync()) return;

        var categories = new List<Category>
        {
            new() { Name = "Chest", Description = "Chest exercises" },
            new() { Name = "Back", Description = "Back exercises" },
            new() { Name = "Shoulders", Description = "Shoulder exercises" },
            new() { Name = "Arms", Description = "Biceps and triceps exercises" },
            new() { Name = "Legs", Description = "Leg exercises" },
            new() { Name = "Core", Description = "Core and abdominal exercises" },
            new() { Name = "Cardio", Description = "Cardiovascular exercises" },
            new() { Name = "Full Body", Description = "Full body exercises" }
        };

        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();
    }

    private static async Task SeedMuscleGroupsAsync(FitnessDbContext context)
    {
        if (await context.MuscleGroups.AnyAsync()) return;

        var muscleGroups = new List<MuscleGroup>
        {
            new() { Name = "Pectoralis Major", BodyPart = "Chest" },
            new() { Name = "Latissimus Dorsi", BodyPart = "Back" },
            new() { Name = "Deltoids", BodyPart = "Shoulders" },
            new() { Name = "Biceps Brachii", BodyPart = "Arms" },
            new() { Name = "Triceps Brachii", BodyPart = "Arms" },
            new() { Name = "Quadriceps", BodyPart = "Legs" },
            new() { Name = "Hamstrings", BodyPart = "Legs" },
            new() { Name = "Gluteus Maximus", BodyPart = "Glutes" },
            new() { Name = "Rectus Abdominis", BodyPart = "Core" },
            new() { Name = "Calves", BodyPart = "Legs" }
        };

        await context.MuscleGroups.AddRangeAsync(muscleGroups);
        await context.SaveChangesAsync();
    }
}
