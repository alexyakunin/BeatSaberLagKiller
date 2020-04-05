using System;
using System.Reflection;
using BeatSaberMarkupLanguage.Notify;
using BS_Utils.Utilities;

namespace LagKiller
{
    public static class Helpers
    {
        public static void NotifyChanged<T>(
            this T host,
            Action<T, string> propertyChangedNotifier = null,
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public)
            where T : INotifiableHost
        {
            propertyChangedNotifier ??= (host, name) =>
                host.InvokeMethod("NotifyPropertyChanged", name);
            foreach (var p in host.GetType().GetProperties(bindingFlags))
                propertyChangedNotifier(host, p.Name);
        }
    }
}
