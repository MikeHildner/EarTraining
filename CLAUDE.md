# EarTraining — notes for Claude

ASP.NET MVC (.NET Framework 4.7.2) ear-training app. Razor views live under
`EarTraining/Views`. The site has two themes: **Human** (the original,
hand-written site) and **Vibe** (enhanced). New AI-built features live in
**Vibe mode only**.

## Conventions

**Mark AI-generated features with an AI badge — this is the desired default,
add it without asking.** When a page gains an AI-built feature (scoring,
automation, etc.), add an AI badge to its nav link in the **main layout**
(`EarTraining/Views/Shared/_Layout.cshtml`):

```html
<span class="ai-badge"><i class="fa fa-magic"></i> AI</span>
```

Pages with the scoring quiz also get a scoring icon right after it:

```html
<i class="fa fa-bullseye text-success ml-1" title="Supports scoring"></i>
```

Do **not** add these to the Human-mode layout
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

Per-page multiple-choice self-scoring quiz (Vibe only), used on several L1C1
and L1C3 drill pages:

- Quiz options derived from the page's answer set, plus an animated SVG
  **accuracy gauge** with tiers: **Good Start / Good Work / Very Good /
  Quite Good** (red / amber / green / gold-with-glow).
- Keep the gauge CSS in an **in-view `<style>` block**, not the CSS bundle —
  bundled CSS changes don't reliably cache-bust in this app.
- Optional **"Score me"** toggle integrates scoring with the Automation
  feature: a scored run quizzes every drill and counts an unanswered drill
  as incorrect (with a slightly longer reveal/guess window).
