namespace EarTraining.Core.Theory;

/// <summary>
/// The DO (tonic) for L1C1 drills, chosen in a comfortable singing range — G3..Gb4 —
/// matching the web app's Pitches list. Every L1C1 drill is built relative to DO.
/// (Note numbers: 34 = G3 … 45 = Gb4.)
/// </summary>
public static class Tonic
{
    public const int Lowest = 34;   // G3
    public const int Highest = 45;  // Gb4

    public static int RandomDo(Random rng) => rng.Next(Lowest, Highest + 1);
}
