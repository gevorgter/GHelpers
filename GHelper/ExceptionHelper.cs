using System;
using System.Diagnostics.CodeAnalysis;

namespace GHelpers
{
    public static class ExceptionHelper
    {
        public static void ThrowApplicationExceptionIfNull([NotNull] this object? o, string message)
        {
            if (o == null)
                throw new ApplicationException(message);
        }

        public static void ThrowExceptionIfNull([NotNull] this object? o, string message)
        {
            if (o == null)
                throw new Exception(message);
        }
    }
}
