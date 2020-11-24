using System;
using System.Collections.Generic;
using System.Linq;

namespace TalentLMSReporting
{
    public static class Extensions
    {
        public static void Then<T>(this T target, Action<T> toPerform)
        {
            toPerform(target);
        }

        public static TReturn Then<T, TReturn>(this T target, Func<T, TReturn> toPerform)
        {
            return toPerform(target);
        }

        public static IEnumerable<KeyValuePair<string, object>> ToNamePairs<T>(this T target)
        {
            return target.GetType()
               .GetProperties()
               .Select(p => KeyValuePair.Create(p.Name, p.GetValue(target)));
        }
    }
}