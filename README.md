# BeatSaberLagKiller

It's a rewritten [TheTrashMan](https://github.com/monkeymanboy/BeatSaberTrashMan): the original plugin by some reason assumes .NET garbage collection settings also work in Unity, but they don't. Unity relies on its own incremental GC implementation, *which still can be tuned*.

The plugin does a fairly simple job: it invokes `GarbageCollector.CollectIncremental` every frame (unless there is a lag) with the specified time budget. It's not fully clear whether something similar happens every frame anyway, because the parameters affecting this ( see [GarbageCollector.incrementalTimeSliceNanoseconds documentation](https://docs.unity3d.com/2019.1/Documentation/ScriptReference/Scripting.GarbageCollector-incrementalTimeSliceNanoseconds.html) --  `QualitySettings.vSyncCount` is 0 and `Application.targetFrameRate` is -1, so based on the description, it is at least it's unlikely that Unity uses the remaining time every frame for incremental GC; as a result, it might happen at arbitrary moments w/ higher (3ms) budget, which isn't good.

In addition, LagKiller adds performance statistics page, where it detects two kinds of issues:
- Dropped frames: basically, every frame taking > 1/70s is considered dropped
- Lags: every frame that takes > 1/10s
These settings are configurable via `UserData/LagKiller.json` ("FrameDropFpsBoundary", "LagFpsBoundary" - see [Settings.cs](https://github.com/alexyakunin/BeatSaberLagKiller/blob/master/src/LagKiller/Settings.cs)).

Moreover, it logs all the lags immediately, so if you launch Beat Saber with `--verbose` switch, you'll be able to see when exactly they happen. Note that you also need to set `ShowDebug` to `true` in `UserData/Beat Saber IPA.json`; see [this document](https://github.com/beat-saber-modding-group/BeatSaber-IPA-Reloaded/wiki/Developing#Debugging) for details.
