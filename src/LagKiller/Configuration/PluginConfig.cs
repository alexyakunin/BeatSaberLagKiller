using IPA.Config.Stores;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace LagKiller.Configuration
{
    public class PluginConfig
    {
        public static PluginConfig Instance { get; set; }
        public PluginConfig()
        {
        }

        static PluginConfig()
        {
            Instance = new PluginConfig();
        }

        public static readonly float DefaultGCBudget = 1f;
        public static readonly float MinGCBudget = 0.25f;
        public static readonly float MaxGCBudget = 10;
        public static readonly float MinFps = 1;
        public static readonly float MaxFps = 1000;
        public static readonly float MinDuration = 0.01f;
        public static readonly float MaxDuration = 3600 * 24 * 366;

        public virtual bool IsEnabled { get; set; } = false;
        public virtual float GCBudget { get; set; } = DefaultGCBudget;
        public virtual float FrameDropFpsBoundary { get; set; } = 1f;
        public virtual float LagFpsBoundary { get; set; } = 1f;
        public virtual float ApplyGCModePeriod { get; set; } = 1f;
        public virtual float GameStartupDuration { get; set; } = MinDuration;


        public event Action<PluginConfig> OnChangedEvent;

        /// <summary>
        /// This is called whenever BSIPA reads the config from disk (including when file changes are detected).
        /// </summary>
        public virtual void OnReload()
        {
            // Do stuff after config is read from disk.
            this.OnChangedEvent?.Invoke(this);
        }

        /// <summary>
        /// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
        /// </summary>
        public virtual void Changed()
        {
            // Do stuff when the config is changed.
            this.OnChangedEvent?.Invoke(this);
        }

        /// <summary>
        /// Call this to have BSIPA copy the values from <paramref name="other"/> into this config.
        /// </summary>
        public virtual void CopyFrom(PluginConfig other)
        {
            // This instance's members populated from other
            this.IsEnabled = other.IsEnabled;
            this.GCBudget = other.GCBudget;
        }
    }
}
