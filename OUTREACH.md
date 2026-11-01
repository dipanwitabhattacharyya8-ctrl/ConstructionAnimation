# Outreach drafts for Ti4goc/ConstructionAnimation

Drafts only; nothing here has been posted.

---

## 1. Issue: asking whether a PR would be welcome

**Title:** Would you welcome a PR with some performance fixes?

Hi @Ti4goc, thanks for Construction Animation. Watching buildings actually rise is the best thing in my cities right now.

In a busy city at 3x speed (about 52 active construction sites) my FPS dropped a lot, so I profiled the mod in a fork and made a few changes. In that test the mod's share of frame time fell from **30% to 8%**, and FPS rose from **about 25 to 44**.

What changed, in short:

- **Logging:** a "Log level" option (Off / PERF lines only / Full). Log strings on per-frame paths are only built at Full.
- **Nearby sites:** sites within 650 m run the heavy update (cut, scaffold, crane) about 10 times a second instead of every frame, and skip it when nothing has changed.
- **Re-cut per floor:** the rising building is re-cut only when it reaches the next floor, with the cut on the building's floor lines, instead of every 0.3 to 2.5 m.
- **Company banner:** each site checks its building's own renter list every 5 s. The city-wide renter scan still runs, but as one shared pass every 30 s for all waiting sites instead of one per site every second.

Each change has its own toggle in Options, so you can compare it with the current behaviour. A couple of them change how construction looks: the building rises floor by floor, and the scaffold updates less often. Which defaults to use, and how many of the toggles to keep, are entirely up to you.

Would you be open to a pull request? I can also split it into smaller PRs or adjust anything to fit how you like to work. The branch is here: https://github.com/dipanwitabhattacharyya8-ctrl/ConstructionAnimation/tree/performance-fixes

I also have a small, separate fix (the site search skips tool previews and destroyed buildings, like the game's own construction system does) that I can send as its own PR later if you're interested.

Thanks either way!

---

## 2. Pull request description

**Title:** Performance: lower the mod's frame-time cost in busy cities

Hi! As discussed in the issue, here are the performance changes. Thanks for taking a look.

### Result

Busy city, 3x speed, about 52 active construction sites:

| | Before | After |
|---|---|---|
| Mod's share of frame time | 30% | 8% |
| FPS | ~25 | ~44 |

### What's in it (one commit each)

1. **Company banner lookup:** each waiting site checks its building's own `Renter` buffer every 5 s instead of 1 s. The city-wide `PropertyRenter` reverse scan, which includes households, now runs as one shared pass at most every 30 s, and only while a site is waiting, instead of once per waiting site every second. Matches are re-checked before use.
2. **Log level option:** Off / PERF lines only / Full. Off never opens the diagnostic file and skips building the `[PERF]` report. Errors and the mod loaded/disposed lines still go to the game log at every level.
3. **Nearby-site throttle:** sites within 650 m run the heavy visual update (proxy, cut, scaffold, crane) about every 0.1 s instead of every frame. Progress is still smoothed every frame.
4. **Re-cut once per floor:** the structure and facade cuts snap to the building's floor levels from `FloorBoundaries`. Levels closer than 2 m are merged, gaps over 5 m are split, and buildings without detected floors use a level every 3 m. The meshes are rebuilt only when a cut reaches the next level, and a window-reveal step rebuilds just the window meshes. The cut heights and the rebuild decision now live in one place (`GetCutoffTargets`).
5. **Skip unchanged sites:** at each 0.1 s tick a nearby site updates only if its cut is due (the same `GetCutoffTargets` decision), its distance band, LOD, shadow range or cull range changed, it was hidden or beyond 650 m since its last near update, or a 1 s safety refresh is due.
6. **Cheaper logging calls:** per-frame log strings (progress, heartbeat, cut update, crane POI, facade colour) are only built at Full. This doesn't change behaviour, so it has no toggle.

### Options (Options > Mods > Construction Animation)

Each change except 6 has its own toggle, and switching one off restores the previous behaviour. The `[PERF]` report has a `TOGGLES` line, so every report says which options were active. I've left the defaults (all toggles on, log level Off) and the number of toggles for you to decide; they were mainly useful for A/B testing.

### Behaviour changes worth knowing

- With "Re-cut once per floor" on, the building rises floor by floor instead of near-continuously. The scaffold and crane advance with those steps, plus a refresh every second.
- With log level Off, the diagnostic log file isn't written. Set "Log level" to Full to get the old logs for bug reports.

Thanks again for the mod!
