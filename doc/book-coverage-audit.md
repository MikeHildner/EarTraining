# Book-coverage audit — app vs. *Contemporary Eartraining* Levels 1 & 2

*Audited 2026-07-02 against Mark Harrison's Contemporary Eartraining Level 1 and Level 2
(source PDFs kept locally, gitignored). Compared with the mobile app at v1.0 (`dbed94e`);
the web app has the same drill coverage, so the findings apply to both.*

**Headline: nothing is wrong — every spot-checked drill table matches the book exactly.**
The app's deviations are deliberate supersets (random 12-key DO vs. the book's C/G/F/D/Bb;
inversions offered in triad ID where the book drills root position). The gaps are omissions,
concentrated in **dictation** (bass lines, later-chapter rhythms) and **Level 2**.

## Method

- Extracted both books' text (`pdftotext`): tables of contents, each chapter's concept +
  drill/workbook sections, and the answer keys for spot-checks.
- Inventoried every app drill page and its `EarTraining.Core` data table (prompt lists,
  interval sets, triad/inversion sets, progression lists), including in-app book references.
- Diffed chapter-by-chapter (coverage) and table-by-table (accuracy) on the highest-risk sets.

## Level 1 (book: 8 chapters + supplementary worksheets · app: C1–C7)

| Book chapter | App | Verdict |
|---|---|---|
| **C1** — scales/SOLFEG review, resolutions, melodic dictation (whole/half/quarter) | L1C1: Vocal Drills · Resolution ID · Pitch ID · Dictation | ✅ covered |
| **C2** — Maj 3rd + Min 6th; adds eighth-note dictation | L1C2: Vocal · Melodic · Harmonic · Dictation | ✅ covered |
| **C3** — Min 3rd + Maj 6th; D major; triads I/IV/V | L1C3: Vocal · Melodic · Harmonic · Triad Recognition · Dictation | ✅ covered |
| **C4** — melody harmonization (4-note melodies, I/IV/V); 2–3-chord progressions; *also* 4-way mixed interval ID (Ma3/Mi6/Mi3/Ma6) + continued dictation | L1C4: Melody Harmonization · Triad Progressions | ◑ harmonization quizzes single notes (captures the book's DO→I/IV and SO→I/V "ear decisions" exactly); ❌ no 4-way mixed-interval quiz; ❌ no C4 dictation |
| **C5** — P4/A4/P5/D5; triads add VI; **bass-line dictation** (half/quarter) | L1C5: Vocal · Melodic · Harmonic · Triad Recognition · Progressions | ◑ intervals/triads/progressions ✅ (A4/D5 asc+desc match the answer key); ❌ no bass-line dictation; ❌ no C5 melodic dictation. (App triad ID adds inversions — superset; book C5 drills root position.) |
| **C6** — Maj 2nd + Min 7th; **dotted quarter-eighth rhythms**; triads add III | L1C6 (5 pages) | ◑ drills ✅; ❌ no C6 dictation (dotted rhythms unsupported by the dictation engine); ❌ no bass-line dictation (the C6 workbook has a 5.5 bass-line section, p. 135, though the TOC doesn't advertise it) |
| **C7** — Min 2nd + Maj 7th; **eighth-note anticipations**; triads add II; bass-line dictation w/ eighths + anticipations | L1C7 (5 pages) | ◑ drills ✅; ❌ no C7 dictation, no anticipation rhythms, no bass-line dictation |
| **C8 — Review**: ALL-interval melodic ID + harmonic ID (16 Q each), melodic dictation, triad recognition, bass-line dictation, in G/F/D/Bb | — | ❌ missing. Closest is "Not in the Books → Interval ID", but that is chromatic root-based, not diatonic-solfeg *(2026-08 update: the L1C8 review chapter has since been built, and the chromatic Interval ID page removed — the section is now "Extras", holding only Blank Sheet Music)* |
| Supplementary worksheets (keys C/G/F/D/Bb) | — | N/A — written theory worksheets, not audio drills (deliberate skip) |

### Accuracy spot-checks (all PASSED — exact matches)

- **L1C4 harmonization option table** (book p. 59–60): DO→I *or* IV · RE→V · MI→I · FA→IV ·
  SO→I *or* V · LA→IV · TI→V — the app's 9 prompts match one-for-one.
- **L1C2 harmonic pairs**: Ma3 = DO-MI / FA-LA / SO-TI, Mi6 = their inversions — match.
- **L1C5 interval categories**: FA-TI Aug 4 and TI-FA Dim 5, both directions, per the answer key — match.
- **L2C4 movement types**: circle-of-5ths (V→I) / circle-of-4ths (IV→I) / half-step up / half-step down —
  the app's four categories match the book's list exactly.

## Level 2 (book: 10 chapters · app: C4 + C5 only)

| Book chapter | App | Verdict |
|---|---|---|
| Level 1 solfeg review (p. 2) | "Solfège Syllables" reference page | ✅ |
| **C2 — circle-of-5ths / circle-of-4ths concepts** | — | ❌ conceptual chapter; partially embodied in the L2C4 quiz — a reference page would cover it |
| **C3 — chromatic solfeg** (DI/RA/RI/ME/FI/SE/SI/LE/LI/TE; Fixed vs. Moveable DO) | — | ❌ missing entirely (no chromatic solfeg anywhere in the app) |
| **C4 — major triad progressions** (V→I / IV→I / half-step up / half-step down) | L2C4 movement-type quiz (2- & 3-chord) | ✅ type-level quiz is the right adaptation (exact chords re-randomize key); categories match |
| **C5 — diatonic triad progressions**: 4-chord, all 7 triads incl. VII° | L2C5 play-only explorer; L1C7 quiz covers 2–3 chords / 6 triads | ◑ no *scored* 4-chord quiz; VII° appears in no quiz |
| **C6 — modal scale recognition** (7 modes as solfeg alterations, tied to the circle) | — | ❌ missing |
| **C7 — II-V-I four-part progressions** (root-3-7 voicings; key-change recognition around the circle) | — | ❌ missing |
| **C8 — 7-3 melodic line dictation** over II-V-I ("7-3-7" vs. "3-7-3" lines) | — | ❌ missing |
| **C9 — vocal drills** (4 drills: circle-of-5ths & -4ths × Moveable/Fixed DO) | — | ❌ missing (Level 2 has no vocal drills page) |
| C10 — cassette contents | — | N/A |

## Prioritized roadmap (future releases)

### Tier 1 — complete Level 1 (natural v1.1)
1. **Dictation engine**: add dotted quarter-eighth rhythms (C6) and eighth-note anticipations (C7).
2. **Dictation pages for C4–C7**, widening the interval pool per chapter (reuse `IntervalDictationDrill`).
3. **Bass-line dictation** — new low-register generator; C5 rules (half/quarter) → C7 adds eighths/anticipations.
4. **C4 mixed-interval quiz** — Ma3/Mi6/Mi3/Ma6 4-way, reusing the existing L1C2/L1C3 tables.
5. **L1C8 review chapter** — all-interval melodic + harmonic ID over the combined C2–C7 tables
   (+ optionally a "review" Home section aggregating dictation/triads/bass lines).

### Tier 2 — finish Level 2 (the concrete "v1.2" list)
1. **Chromatic solfeg** reference page (+ optional ID quiz) — also a prerequisite for (6).
2. **Scored 4-chord diatonic progression quiz** incl. VII° (upgrade the L2C5 explorer).
3. **Modal scale recognition quiz** — 7 modes, solfeg-alteration framing.
4. **II-V-I quiz** — needs 4-part chords (root-3-7 voicings) + circle-based key-change prompts.
5. **7-3 line dictation** over II-V-I progressions ("7-3-7" vs. "3-7-3").
6. **L2 vocal drills page** — the four circle drills (Fixed-DO variants need chromatic syllables).

### Nice-to-have
- Circle-of-5ths/4ths reference visual (L2C2).
- Book-faithful 4-note-melody harmonization variant for L1C4.
- "Keys of the book" toggle (C/G/F/D/Bb) as an alternative to fully-random DO.
