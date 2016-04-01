using System;
using System.Threading.Tasks;

namespace SpecEasy
{
    internal static class ActionExtensions
    {
        public static Func<Task> Wrap(this Action action)
        {
            return async () => action();
        }
    }
}
