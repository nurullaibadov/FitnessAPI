using AutoMapper;
using FitnessAPI.Application.DTOs.Auth;
using FitnessAPI.Application.DTOs.BodyMeasurement;
using FitnessAPI.Application.DTOs.Exercise;
using FitnessAPI.Application.DTOs.User;
using FitnessAPI.Application.DTOs.WorkoutLog;
using FitnessAPI.Application.DTOs.WorkoutPlan;
using FitnessAPI.Domain.Entities;

namespace FitnessAPI.Application.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.FullName))
            .ForMember(d => d.Roles, o => o.Ignore());

        CreateMap<User, UserProfileDto>()
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.FullName))
            .ForMember(d => d.Roles, o => o.Ignore())
            .ForMember(d => d.TotalWorkouts, o => o.Ignore())
            .ForMember(d => d.TotalCaloriesBurned, o => o.Ignore())
            .ForMember(d => d.CurrentStreak, o => o.Ignore())
            .ForMember(d => d.TotalWorkoutMinutes, o => o.Ignore())
            .ForMember(d => d.LastWorkoutDate, o => o.Ignore());

        CreateMap<User, UserInfoDto>()
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.FullName))
            .ForMember(d => d.Roles, o => o.Ignore());

        CreateMap<CreateUserDto, User>()
            .ForMember(d => d.PasswordHash, o => o.Ignore())
            .ForMember(d => d.Id, o => o.Ignore());
    }
}

public class WorkoutPlanMappingProfile : Profile
{
    public WorkoutPlanMappingProfile()
    {
        CreateMap<WorkoutPlan, WorkoutPlanDto>()
            .ForMember(d => d.UserFullName, o => o.MapFrom(s => s.User != null ? s.User.FullName : string.Empty));

        CreateMap<WorkoutDay, WorkoutDayDto>()
            .ForMember(d => d.Exercises, o => o.MapFrom(s => s.WorkoutDayExercises));

        CreateMap<WorkoutDayExercise, WorkoutDayExerciseDto>()
            .ForMember(d => d.ExerciseName, o => o.MapFrom(s => s.Exercise != null ? s.Exercise.Name : string.Empty))
            .ForMember(d => d.ExerciseImageUrl, o => o.MapFrom(s => s.Exercise != null ? s.Exercise.ImageUrl : null));

        CreateMap<CreateWorkoutPlanDto, WorkoutPlan>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.UserId, o => o.Ignore())
            .ForMember(d => d.User, o => o.Ignore());

        CreateMap<CreateWorkoutDayDto, WorkoutDay>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.WorkoutPlanId, o => o.Ignore());

        CreateMap<CreateWorkoutDayExerciseDto, WorkoutDayExercise>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.WorkoutDayId, o => o.Ignore());
    }
}

public class ExerciseMappingProfile : Profile
{
    public ExerciseMappingProfile()
    {
        CreateMap<Exercise, ExerciseDto>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category != null ? s.Category.Name : string.Empty))
            .ForMember(d => d.MuscleGroupName, o => o.MapFrom(s => s.MuscleGroup != null ? s.MuscleGroup.Name : null));

        CreateMap<CreateExerciseDto, Exercise>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.IsApproved, o => o.Ignore())
            .ForMember(d => d.CreatedByUserId, o => o.Ignore());

        CreateMap<WorkoutLog, WorkoutLogDto>()
            .ForMember(d => d.WorkoutPlanName, o => o.MapFrom(s => s.WorkoutPlan != null ? s.WorkoutPlan.Name : null))
            .ForMember(d => d.WorkoutDayName, o => o.MapFrom(s => s.WorkoutDay != null ? s.WorkoutDay.Name : null))
            .ForMember(d => d.Details, o => o.MapFrom(s => s.WorkoutLogDetails));

        CreateMap<WorkoutLogDetail, WorkoutLogDetailDto>()
            .ForMember(d => d.ExerciseName, o => o.MapFrom(s => s.Exercise != null ? s.Exercise.Name : string.Empty));

        CreateMap<BodyMeasurement, BodyMeasurementDto>();

        CreateMap<CreateBodyMeasurementDto, BodyMeasurement>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.UserId, o => o.Ignore());
    }
}
