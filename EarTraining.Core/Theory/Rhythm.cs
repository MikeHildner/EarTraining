namespace EarTraining.Core.Theory;

/// <summary>
/// Maps the web app's rhythm codes to a duration in seconds at a given tempo.
/// Codes: "1" whole, "2" half, "2." dotted half, "4" quarter, "4." dotted quarter, "8" eighth.
/// </summary>
public static class Rhythm
{
    public static double Seconds(string code, double bpm)
    {
        double quarter = 60.0 / bpm;
        return code switch
        {
            "1" => quarter * 4,
            "2" => quarter * 2,
            "2." => quarter * 3,
            "4" => quarter,
            "4." => quarter * 1.5,
            "8" => quarter / 2,
            _ => throw new NotSupportedException($"Duration '{code}' is not supported."),
        };
    }
}
