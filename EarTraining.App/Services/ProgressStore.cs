using System.Text.Json;
using EarTraining.Core.Progress;
using Microsoft.Maui.Storage;

namespace EarTraining.App.Services;

/// <summary>
/// Persistent, on-device practice stats: per-drill all-time accuracy, a daily streak, and a daily
/// trend. The whole thing is one JSON blob in <see cref="Preferences"/> (key "progress.v1") — no
/// network, no account, no new dependencies. Recording is funnelled through <c>ScoreGauge.Record</c>
/// (keyed by the current Shell route), plus the two gauge-less ID pages. Static singleton to match the
/// app's no-DI service pattern; thread-safe via a single lock.
/// </summary>
public static class ProgressStore
{
    private const string Key = "progress.v1";
    private const int KeepDays = 90;   // cap the daily history

    public sealed class Stat { public int Correct { get; set; } public int Total { get; set; } }
    public sealed class DayStat { public int Answered { get; set; } public int Correct { get; set; } }

    private sealed class Data
    {
        public Dictionary<string, Stat> Drills { get; set; } = new();
        public Dictionary<string, DayStat> Days { get; set; } = new();   // key "yyyy-MM-dd"
        public string? LastDay { get; set; }
        public int CurrentStreak { get; set; }
        public int BestStreak { get; set; }
    }

    private static readonly object Gate = new();
    private static Data? _cache;

    private static string Today => DateTime.Now.ToString("yyyy-MM-dd");
    private static string Yesterday => DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

    private static Data Cache
    {
        get
        {
            if (_cache != null) return _cache;
            try
            {
                var json = Preferences.Get(Key, "");
                _cache = string.IsNullOrEmpty(json) ? new Data() : JsonSerializer.Deserialize<Data>(json) ?? new Data();
            }
            catch { _cache = new Data(); }
            return _cache;
        }
    }

    private static void Save()
    {
        try { Preferences.Set(Key, JsonSerializer.Serialize(_cache)); }
        catch { /* best-effort; stats are non-critical */ }
    }

    /// <summary>Record one answered drill. No-op for unknown/non-scoring routes (e.g. the Welcome gauge).</summary>
    public static void Record(string? route, bool correct)
    {
        if (string.IsNullOrWhiteSpace(route) || !DrillCatalog.IsDrill(route)) return;
        lock (Gate)
        {
            var d = Cache;

            if (!d.Drills.TryGetValue(route, out var s)) d.Drills[route] = s = new Stat();
            s.Total++;
            if (correct) s.Correct++;

            string today = Today;
            if (!d.Days.TryGetValue(today, out var day)) d.Days[today] = day = new DayStat();
            day.Answered++;
            if (correct) day.Correct++;

            d.CurrentStreak = StreakMath.NextStreak(d.LastDay, d.CurrentStreak, today, Yesterday);
            d.LastDay = today;
            if (d.CurrentStreak > d.BestStreak) d.BestStreak = d.CurrentStreak;

            Prune(d);
            Save();
        }
    }

    private static void Prune(Data d)
    {
        if (d.Days.Count <= KeepDays) return;
        foreach (var k in d.Days.Keys.OrderByDescending(k => k).Skip(KeepDays).ToList())
            d.Days.Remove(k);
    }

    // ── Read accessors (for ProgressPage) ──

    public static bool HasData() { lock (Gate) return Cache.Drills.Count > 0; }

    public static (int correct, int total) Overall()
    {
        lock (Gate)
        {
            int c = 0, t = 0;
            foreach (var s in Cache.Drills.Values) { c += s.Correct; t += s.Total; }
            return (c, t);
        }
    }

    /// <summary>Current streak, or 0 if it has lapsed (last practice before yesterday).</summary>
    public static int CurrentStreak()
    {
        lock (Gate)
        {
            var d = Cache;
            return StreakMath.IsActive(d.LastDay, Today, Yesterday) ? d.CurrentStreak : 0;
        }
    }

    public static int BestStreak() { lock (Gate) return Cache.BestStreak; }

    /// <summary>Per-drill stats (only drills with ≥1 answer), in catalog/display order.</summary>
    public static IReadOnlyList<(string Route, string Name, int Correct, int Total)> PerDrill()
    {
        lock (Gate)
        {
            var d = Cache;
            var list = new List<(string, string, int, int)>();
            foreach (var (route, name) in DrillCatalog.Ordered)
                if (d.Drills.TryGetValue(route, out var s) && s.Total > 0)
                    list.Add((route, name, s.Correct, s.Total));
            return list;
        }
    }

    /// <summary>Last <paramref name="days"/> days, oldest→newest, zero-filled for days with no practice.</summary>
    public static IReadOnlyList<(DateTime Date, int Answered, int Correct)> Trend(int days)
    {
        lock (Gate)
        {
            var d = Cache;
            var list = new List<(DateTime, int, int)>(days);
            var start = DateTime.Now.Date.AddDays(-(days - 1));
            for (int i = 0; i < days; i++)
            {
                var date = start.AddDays(i);
                if (d.Days.TryGetValue(date.ToString("yyyy-MM-dd"), out var ds)) list.Add((date, ds.Answered, ds.Correct));
                else list.Add((date, 0, 0));
            }
            return list;
        }
    }

    public static void Reset()
    {
        lock (Gate) { _cache = new Data(); Save(); }
    }
}
