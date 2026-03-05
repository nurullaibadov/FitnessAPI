using FitnessAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessAPI.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FirstName).IsRequired().HasMaxLength(50);
        builder.Property(x => x.LastName).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(256);
        builder.HasIndex(x => x.Email).IsUnique();
        builder.Property(x => x.PasswordHash).IsRequired();
        builder.Property(x => x.PhoneNumber).HasMaxLength(20);
        builder.Property(x => x.ProfileImageUrl).HasMaxLength(500);
        builder.Property(x => x.Height).HasColumnType("decimal(5,2)");
        builder.Property(x => x.Weight).HasColumnType("decimal(5,2)");
        builder.Property(x => x.EmailVerificationToken).HasMaxLength(500);
        builder.Property(x => x.PasswordResetToken).HasMaxLength(500);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
        builder.HasIndex(x => x.Name).IsUnique();
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasKey(x => new { x.UserId, x.RoleId });
        builder.HasOne(x => x.User).WithMany(x => x.UserRoles).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Role).WithMany(x => x.UserRoles).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Token).IsRequired().HasMaxLength(500);
        builder.HasIndex(x => x.Token).IsUnique();
        builder.HasOne(x => x.User).WithMany(x => x.RefreshTokens).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.Instructions).HasMaxLength(5000);
        builder.Property(x => x.VideoUrl).HasMaxLength(500);
        builder.Property(x => x.ImageUrl).HasMaxLength(500);
        builder.Property(x => x.CaloriePerMinute).HasColumnType("decimal(5,2)");
        builder.HasOne(x => x.Category).WithMany(x => x.Exercises).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.MuscleGroup).WithMany(x => x.Exercises).HasForeignKey(x => x.MuscleGroupId).OnDelete(DeleteBehavior.SetNull);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class WorkoutPlanConfiguration : IEntityTypeConfiguration<WorkoutPlan>
{
    public void Configure(EntityTypeBuilder<WorkoutPlan> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.ImageUrl).HasMaxLength(500);
        builder.HasOne(x => x.User).WithMany(x => x.WorkoutPlans).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class WorkoutDayConfiguration : IEntityTypeConfiguration<WorkoutDay>
{
    public void Configure(EntityTypeBuilder<WorkoutDay> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.HasOne(x => x.WorkoutPlan).WithMany(x => x.WorkoutDays).HasForeignKey(x => x.WorkoutPlanId).OnDelete(DeleteBehavior.Cascade);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class WorkoutDayExerciseConfiguration : IEntityTypeConfiguration<WorkoutDayExercise>
{
    public void Configure(EntityTypeBuilder<WorkoutDayExercise> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.WeightKg).HasColumnType("decimal(6,2)");
        builder.HasOne(x => x.WorkoutDay).WithMany(x => x.WorkoutDayExercises).HasForeignKey(x => x.WorkoutDayId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Exercise).WithMany(x => x.WorkoutDayExercises).HasForeignKey(x => x.ExerciseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class WorkoutLogConfiguration : IEntityTypeConfiguration<WorkoutLog>
{
    public void Configure(EntityTypeBuilder<WorkoutLog> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.User).WithMany(x => x.WorkoutLogs).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.WorkoutPlan).WithMany(x => x.WorkoutLogs).HasForeignKey(x => x.WorkoutPlanId).OnDelete(DeleteBehavior.SetNull);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class BodyMeasurementConfiguration : IEntityTypeConfiguration<BodyMeasurement>
{
    public void Configure(EntityTypeBuilder<BodyMeasurement> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.WeightKg).HasColumnType("decimal(5,2)");
        builder.Property(x => x.HeightCm).HasColumnType("decimal(5,2)");
        builder.Property(x => x.BodyFatPercentage).HasColumnType("decimal(4,1)");
        builder.Property(x => x.MuscleMassKg).HasColumnType("decimal(5,2)");
        builder.Property(x => x.ChestCm).HasColumnType("decimal(5,2)");
        builder.Property(x => x.WaistCm).HasColumnType("decimal(5,2)");
        builder.Property(x => x.HipCm).HasColumnType("decimal(5,2)");
        builder.Property(x => x.ArmCm).HasColumnType("decimal(5,2)");
        builder.Property(x => x.LegCm).HasColumnType("decimal(5,2)");
        builder.Property(x => x.NeckCm).HasColumnType("decimal(5,2)");
        builder.HasOne(x => x.User).WithMany(x => x.BodyMeasurements).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
