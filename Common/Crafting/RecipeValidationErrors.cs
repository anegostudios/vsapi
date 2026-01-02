using System;
using System.Collections.Generic;

namespace Vintagestory.API.Common
{
    public static class RecipeValidationErrors
    {
        static readonly List<string> Errors = new List<string>();

        public static void Add(string message)
        {
            if (string.IsNullOrEmpty(message)) return;
            Errors.Add(message);
        }

        public static bool HasErrors => Errors.Count > 0;

        public static void ThrowIfAny()
        {
            if (Errors.Count == 0) return;

            string message = "Invalid recipes found:" + Environment.NewLine + string.Join(Environment.NewLine, Errors);
            throw new InvalidOperationException(message);
        }

        public static string[] GetAll()
        {
            return Errors.ToArray();
        }

        public static void Clear()
        {
            Errors.Clear();
        }
    }
}
