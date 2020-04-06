# LagKiller plugin for Beat Saber

LagKiller is a rewritten [TheTrashMan](https://github.com/monkeymanboy/BeatSaberTrashMan) that takes into account Unity-specific incremental GC mode (most of .NET GC settings don't work in Unity). It manually triggers incremental GC with the specified budget and provides a page showing useful statistics on GC and application performance (rate of frame drops, lags, "incomplete" incremental GCs, etc.).

It's a rewritten: the original plugin by some reason assumes .NET garbage collection settings also work in Unity, but they don't. Unity relies on its own incremental GC implementation, *which still can be tuned*.

The plugin does a fairly simple job: it invokes `GarbageCollector.CollectIncremental` every frame (unless there is a lag) with the specified time budget. It's not fully clear whether something similar happens every frame anyway, because the parameters affecting this are configured for incremental GC with 3ms by default, which happens when Unity decides to do so ( see [GarbageCollector.incrementalTimeSliceNanoseconds documentation](https://docs.unity3d.com/2019.1/Documentation/ScriptReference/Scripting.GarbageCollector-incrementalTimeSliceNanoseconds.html) --  `QualitySettings.vSyncCount` is 0 and `Application.targetFrameRate` is -1, so based on the description, it is at least it's unlikely that Unity uses the remaining time every frame for incremental GC); as a result, it might happen at arbitrary moments w/ a fairly high (3ms) budget. 

LagKiller doesn't change these settings, but instead triggers the incremental GC explicitly every frame with a configurable budget. The actual time spent during such calls is always less than budget. If it's too low, there is a chance Unity spends some extra time at a later moment, which may or may not cause frame drop. On a positive side, LagKiller tracks the % of cases when incremental GC completes with "incomplete GC" flag, so you'll actually see when it makes sense to adjust the budget.

In addition, LagKiller adds performance statistics page, where it detects two kinds of issues:
- Dropped frames: basically, every frame taking > 1/70s is considered dropped
- Lags: every frame that takes > 1/20s

These settings are configurable via `UserData/LagKiller.json` ("FrameDropFpsBoundary", "LagFpsBoundary" - see [Settings.cs](https://github.com/alexyakunin/BeatSaberLagKiller/blob/master/src/LagKiller/Settings.cs)).

And finally, all the lags are logged. If you launch Beat Saber with `--verbose` switch, you'll be able to see when exactly they happen. Note that you also need to set `ShowDebug` to `true` in `UserData/Beat Saber IPA.json`; see [this document](https://github.com/beat-saber-modding-group/BeatSaber-IPA-Reloaded/wiki/Developing#Debugging) for details.
