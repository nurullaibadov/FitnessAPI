namespace FitnessAPI.Application.DTOs.BodyMeasurement;

public class BodyMeasurementDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal? WeightKg { get; set; }
    public decimal? HeightCm { get; set; }
    public decimal? BodyFatPercentage { get; set; }
    public decimal? MuscleMassKg { get; set; }
    public decimal? ChestCm { get; set; }
    public decimal? WaistCm { get; set; }
    public decimal? HipCm { get; set; }
    public decimal? ArmCm { get; set; }
    public decimal? LegCm { get; set; }
    public decimal? NeckCm { get; set; }
    public DateTime MeasuredAt { get; set; }
    public string? Notes { get; set; }
    public decimal? Bmi => (WeightKg.HasValue && HeightCm.HasValue && HeightCm > 0)
        ? Math.Round(WeightKg.Value / (decimal)Math.Pow((double)(HeightCm.Value / 100), 2), 1)
        : null;
}

public class CreateBodyMeasurementDto
{
    public decimal? WeightKg { get; set; }
    public decimal? HeightCm { get; set; }
    public decimal? BodyFatPercentage { get; set; }
    public decimal? MuscleMassKg { get; set; }
    public decimal? ChestCm { get; set; }
    public decimal? WaistCm { get; set; }
    public decimal? HipCm { get; set; }
    public decimal? ArmCm { get; set; }
    public decimal? LegCm { get; set; }
    public decimal? NeckCm { get; set; }
    public DateTime MeasuredAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
}
