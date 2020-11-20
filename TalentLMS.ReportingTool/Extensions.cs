using System;

namespace TalentLMSReporting
{
    public static class Extensions
    {
        public static void Then<T>(this T target, Action<T> toPerform)
        {
            toPerform(target);
        }
    }
}