using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace GHelpers
{
    public static class ExceptionHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowApplicationExceptionIfNull([NotNull] this object? o, string message)
        {
            if (o == null)
                throw new ApplicationException(message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowApplicationExceptionIfNull([NotNull] this object? o, string message, object p1)
        {
            if (o == null)
                throw new ApplicationException(string.Format(message, p1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowApplicationExceptionIfNull([NotNull] this object? o, string message, object p1, object p2)
        {
            if (o == null)
                throw new ApplicationException(string.Format(message, p1, p2));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowApplicationExceptionIfNull([NotNull] this object? o, string message, object p1, object p2, object p3)
        {
            if (o == null)
                throw new ApplicationException(string.Format(message, p1, p2, p3));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowExceptionIfNull([NotNull] this object? o, string message)
        {
            if (o == null)
                throw new Exception(message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowExceptionIfNull([NotNull] this object? o, string message, object p1)
        {
            if (o == null)
                throw new Exception(string.Format(message, p1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowExceptionIfNull([NotNull] this object? o, string message, object p1, object p2)
        {
            if (o == null)
                throw new Exception(string.Format(message, p1, p2));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowExceptionIfNull([NotNull] this object? o, string message, object p1, object p2, object p3)
        {
            if (o == null)
                throw new Exception(string.Format(message, p1, p2, p3));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfArrayIsShort([NotNull] this Array? array, int lenght, string message)
        {
            if (array == null || array.Length < lenght)
                throw new ArgumentException(message);
        }

    }
}
