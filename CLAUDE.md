# EarTraining — notes for Claude

ASP.NET MVC (.NET Framework 4.7.2) ear-training app. Razor views live under
`EarTraining/Views`. The site has two themes: **Human** (the original,
hand-written site) and **Vibe** (enhanced). New AI-built features live in
**Vibe mode only**.

## Conventions

**Do not add per-page "AI" badges.** Vibe mode already implies AI
involvement, so the nav no longer carries `.ai-badge` chips — they were
removed to reduce clutter, along with their now-dead CSS in `Site.css` and
`vibe-theme.css`. Don't reintroduce them.

Pages with the scoring quiz still get a scoring icon on their nav link in the
**main layout** (`EarTraining/Views/Shared/_Layout.cshtml`), right after the
link text:

```html
<i class="fa fa-bullseye text-success ml-1" title="Supports scoring"></i>
```

Do **not** add this to the Human-mode layout
(`EarTraining/Areas/Human/Views/Shared/_Layout.cshtml`) — Human mode is the
original, unmarked site.

**AI features are Vibe-mode only.** Wrap their markup in
`<div class="vibe-only">…</div>` (hidden in Human via `Site.css`, shown in
Vibe via `vibe-theme.css`) and gate the JS with:

```js
var isVibe = document.body.classList.contains('theme-vibe');
```

Add a Vibe-only `.ai-note` banner near the top of such pages noting the AI
work.

## Scoring / self-quiz feature

Per-page multiple-choice self-scoring quiz (Vibe only), used across the
L1C1–L1C7 drill pages:

- Quiz options derived from the page's answer set, plus an animated SVG
  **accuracy gauge** with tiers: **Good Start / Good Work / Very Good /
  Quite Good** (red / amber / green / gold-with-glow).
- Keep the gauge CSS in an **in-view `<style>` block**, not the CSS bundle —
  bundled CSS changes don't reliably cache-bust in this app.
- Optional **"Score me"** toggle integrates scoring with the Automation
  feature: a scored run quizzes every drill and counts an unanswered drill
  as incorrect (with a slightly longer reveal/guess window).
- **Quiz buttons track the user's "Include in Drills" selections** (and any
  drill-type toggle): `renderQuizOptions()` offers only the patterns that can
  actually play and re-runs on each checkbox/toggle `change`. Dedupe where
  several drills share a label. (Most relevant where buttons map to specific
  drills, e.g. `SolfegResolutionsDO`, `VocalDrills`.)
- **The gauge re-animates on every answer** — even when the % is unchanged
  (e.g. consecutive correct answers at 100%):
  - *Fill sweep replay:* in `updateGauge()`, set the fill `transition:none`,
    reset `stroke-dashoffset` to empty (`GAUGE_CIRCUMFERENCE`), force one
    reflow, then restore the transition and set the target offset so the 0.7s
    sweep replays from empty.
  - *Scale pop:* `@keyframes gauge-pulse` (write `@@keyframes` in the Razor
    view) on `.score-gauge`, re-triggered by remove-class → reflow → add-class.

### Rollout status — COMPLETE

Both features now ship on **all 11 scoring-gauge pages** (find them via the
`id="gaugeFill"` / `function updateGauge` markup):

- **Gauge re-animation (sweep + pop)** — on every gauge page. (L1C4/Index has
  two gauges, so its `updateGauge(q)` scopes the pulse to the answered tab via
  `fill.closest('.score-gauge')` instead of `document.querySelector`.)
- **Button-tracking** — `renderQuizOptions()` offers only patterns that can
  actually play, re-running on each `cb-include` / drill-type-toggle change
  (and after L1C3/Index's "Invert Selections", via the `invertDrillSelections`
  wrapper). Dedupes shared labels where needed — `PitchIdentification` (DO/TI
  appear twice). The Min3rd/Maj6th interval pages (`L1C3/HarmonicMin3rdMaj6th`,
  `L1C3/MelodicMin3rdMaj6thNoDO`) quiz the **specific prompt** heard — one
  button per playable prompt, gated by the interval-type toggle + includes,
  like the L1C2 interval pages — not a Min-3rd-vs-Maj-6th category choice.

Done: L1C1 (`SolfegResolutionsDO`, `SolfegResolutionsNoDO`,
`PitchIdentification`), L1C2 (`VocalDrills`, `HarmonicMaj3rdMin6th`,
`MelodicMaj3rdMin6thNoDO`), L1C3 (`Index`, `VocalDrills`,
`HarmonicMin3rdMaj6th`, `MelodicMin3rdMaj6thNoDO`), L1C4 (`Index`).

(At the time, L1C7 and L2C4 had **no** scoring gauge. L1C7 has since gained
scoring — see below; L2C4 still has none.)

### Scoring expansion — L1C5–L1C7 interval ID + triad recognition

Scoring (gauge + quiz, same conventions as above) now also ships on the nine
L1C5/L1C6/L1C7 drill pages below. These were the original, un-AI'd pages, so
each also gained a Vibe `.ai-note` banner and a nav scoring icon:

- **Interval ID** (`MelodicIntervals`, `HarmonicIntervals` in L1C5/6/7) — the
  quiz identifies the **interval quality**, *not* the specific solfège pair
  (several pairs share a quality, so the pair is unguessable by ear and would
  be 14–28 buttons). A `drillCategory` map sends each drill type → a quality
  label; `getPlayableCategories()` offers one button per category that has a
  checked prompt in the toggle's range. Melodic pages add direction
  (`Perfect 4th Asc` / `Desc`, …); harmonic don't. On harmonic L1C6/L1C7 each
  toggle side is a single quality, so a one-category toggle shows a single
  button — the real discrimination quiz is the **Both** view.
- **Triad Recognition** (`DiatonicTriadRecognition` in L1C5/6/7) — mirrors
  `L1C3/Index`: Triad vs Triad+inversion `scoreMode`, includes-filtered,
  composite `cb-include-{triad}-{inv}` ids. L1C7 also has an Invert Selections
  button (→ `invertDrillSelections` wrapper) and Automation wired to a working
  **"Score me"** toggle.

Fixed in passing: a wrong checkbox id on `L1C5/MelodicIntervals`
(`cb-include-42` → `41`).

### Scoring expansion — progressions, vocal drills, harmonization

Scoring now also ships on the chord-progression pages, the Vocal Drills, and
Melody Harmonization (same gauge + quiz conventions):

- **Progression ID** — L1C5/6/7 `DiatonicTriadProgressions` mirror `L1C4/Index`
  (a per-tab quiz + gauge for the 2- and 3-chord tabs; per-prompt labels, since
  the key is fixed). `L2C4/MajorTriadProgressions2` & `3` instead quiz the
  progression **type** (Circle of 5th / 4th / ½-step up / down) — those pages
  re-randomize the key every drill, so the exact inversions aren't guessable by
  ear (same reasoning as the interval-quality model).
- **Vocal Drills** — L1C5/6/7 `VocalDrills` identify the sung pattern. L1C5 &
  L1C7 use unique full labels; L1C6 reuses the same 10 patterns across both
  intervals, so it dedupes by syllable (like `L1C3/VocalDrills`).
- **Melody Harmonization** (`L1C4`) — quizzes the harmonizing triad (I / IV /
  V), gated by which (note, triad) prompts are included.

**Not scorable** (no finite answer set — they build a *random* progression):
`L2C5/DiatonicTriadProgressions4` and `L2/MajorTriadProgressions`. The
Dictation/Transcription pages and the standalone (nav-unlinked)
`PitchIdentification` page also remain out of scope.

Automation-bearing pages currently get **standalone** scoring (no "Score me"
auto-run): L1C7 `DiatonicTriadProgressions`, `L2C4/MajorTriadProgressions2`/`3`,
and `L1C4/MelodyHarmonization` — a quick follow-up to wire in if wanted.
