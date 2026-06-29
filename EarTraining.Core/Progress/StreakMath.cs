namespace EarTraining.Core.Progress;

/// <summary>
/// Pure, deterministic daily-streak math (no clock, no I/O) so it can be unit-tested offline.
/// Dates are passed as "yyyy-MM-dd" strings; the caller supplies today/yesterday from the device clock.
/// A "practice day" is any day with at least one answered drill.
/// </summary>
public static class StreakMath
{
    /// <summary>
    /// The streak value after recording an answer on <paramref name="today"/>, given the previous
    /// <paramref name="lastDay"/> and <paramref name="current"/> streak:
    /// same day → unchanged; the day after → +1; any gap (or first ever) → 1.
    /// </summary>
    public static int NextStreak(string? lastDay, int current, string today, string yesterday)
    {
        if (lastDay == today) return current;          // already counted today
        if (lastDay == yesterday) return current + 1;  // consecutive day
        return 1;                                       // gap or first practice
    }

    /// <summary>
    /// Whether a stored streak is still "live" for display — true only if the last practice day was
    /// today or yesterday; otherwise the streak has lapsed and should display as 0.
    /// </summary>
    public static bool IsActive(string? lastDay, string today, string yesterday)
        => lastDay == today || lastDay == yesterday;
}
