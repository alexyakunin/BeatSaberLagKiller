using System;
using System.Reflection;
using BeatSaberMarkupLanguage.Notify;

namespace LagKiller
{
    public static class Helpers
    {
        public static void NotifyChanged<T>(
            this T host,
            Action<T, string> propertyChangedNotifier,
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public)
            where T : INotifiableHost
        {
            foreach (var p in host.GetType().GetProperties(bindingFlags))
                propertyChangedNotifier(host, p.Name);
        }
    }
}
