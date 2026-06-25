namespace EarTraining.Core.Drills;

/// <summary>
/// Interval qualities drilled in the Level 1 interval chapters: L1C2 (Major 3rd / Minor 6th)
/// and L1C3 (Minor 3rd / Major 6th). Each pair is an interval and its inversion.
/// </summary>
public enum IntervalQuality { Major3rd, Minor6th, Minor3rd, Major6th }

public static class IntervalQualityNames
{
    public static string Display(this IntervalQuality q) => q switch
    {
        IntervalQuality.Major3rd => "Maj 3rd",
        IntervalQuality.Minor6th => "Min 6th",
        IntervalQuality.Minor3rd => "Min 3rd",
        IntervalQuality.Major6th => "Maj 6th",
        _ => q.ToString(),
    };
}
