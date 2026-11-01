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
- **Re-cut per floor:** the rising building is re-cut once per floor height instead of every 0.3 to 2.5 m.
- **Company banner:** the lookup uses only the building's own renter list and retries every 5 s, with no more city-wide scan.
- **Site query:** it skips Temp, Deleted and Destroyed buildings, like the game's own construction system does.

Each change has its own toggle in Options, so you can compare it with the current behaviour. A couple of them change how construction looks: the building rises in floor-sized steps, and the scaffold updates less often. I'm happy to make those off by default, or to drop anything you'd rather not have.

Would you be open to a pull request? I can also split it into smaller PRs or adjust anything to fit how you like to work. The branch is here: https://github.com/dipanwitabhattacharyya8-ctrl/ConstructionAnimation/tree/performance-fixes

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

1. **Company banner lookup:** uses only the building's own `Renter` buffer and retries every 5 s instead of 1 s. The city-wide `PropertyRenter` scan, which includes households, only runs with the option off.
2. **Log level option:** Off (default) / PERF lines only / Full. Off never opens the diagnostic file. Errors still go to the game log.
3. **Nearby-site throttle:** sites within 650 m run the heavy visual update (proxy, cut, scaffold, crane) about every 0.1 s instead of every frame. Progress is still smoothed every frame.
4. **Re-cut once per floor:** the cut-mesh rebuild threshold becomes the building's floor height (BuildingHeight / detected floors, clamped to 2–5 m), never finer than the current adaptive value.
5. **Skip unchanged sites:** at each 0.1 s tick a nearby site updates only if its cut is due, its distance band, LOD, shadow range or cull range changed, or a 1 s safety refresh is due.
6. **Cheaper logging calls:** per-frame log strings (progress, heartbeat, cut update, crane POI, facade colour) are only built at Full. This doesn't change behaviour, so it has no toggle.
7. **Site query:** excludes `Temp`, `Deleted` and `Destroyed`, matching the game's `BuildingConstructionSystem` query.

### Options (Options > Mods > Construction Animation)

All performance toggles default to **on**, and switching one off restores the current behaviour. The `[PERF]` report now has a `TOGGLES` line, so every report says which options were active.

### Behaviour changes worth knowing

- The building rises in floor-sized steps instead of near-continuously. The scaffold and crane advance with those steps, plus a refresh every second.
- The diagnostic log file is no longer written by default. Set "Log level" to Full to get the old logs for bug reports.
- Tool previews and destroyed or deleted buildings no longer get a construction visual.

If you'd rather have the visual changes off by default, or would like this split into smaller PRs, just say and I'll change it.

Thanks again for the mod!
