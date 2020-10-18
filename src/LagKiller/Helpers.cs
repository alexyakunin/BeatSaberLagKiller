using System;
using System.ComponentModel;
using System.Reflection;

namespace LagKiller
{
    public static class Helpers
    {
        public static void NotifyChanged<T>(
            this T host,
            Action<T, string> propertyChangedNotifier,
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public)
            where T : INotifyPropertyChanged
        {
            foreach (var p in host.GetType().GetProperties(bindingFlags))
                propertyChangedNotifier(host, p.Name);
        }
    }
}
