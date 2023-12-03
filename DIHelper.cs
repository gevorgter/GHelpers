using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace GHelpers
{
    public static class DIHelper
    {
        static readonly Dictionary<string, Type> mapTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        static string GetKey(Type askType, string key) => $"{key}_{askType.FullName}";

        public static void RegisterSingleton(this IServiceCollection serviceCollection, string key, Type askType, Type resolvedType)
        {
            mapTypes[GetKey(askType, key)] = resolvedType;
            serviceCollection.AddSingleton(resolvedType);
        }

        public static void RegisterScoped(this IServiceCollection serviceCollection, string key, Type askType, Type resolvedType)
        {
            mapTypes[GetKey(askType, key)] = resolvedType;
            serviceCollection.AddScoped(resolvedType);
        }

        public static void RegisterTransient(this IServiceCollection serviceCollection, string key, Type askType, Type resolvedType)
        {
            mapTypes[GetKey(askType, key)] = resolvedType;
            serviceCollection.AddTransient(resolvedType);
        }

        public static T GetService<T>(this IServiceProvider serviceProvider, string key)
            where T : class
        {
            var t = typeof(T);
            if (!mapTypes.TryGetValue(GetKey(t, key), out var resolvedType))
                return null;
            return serviceProvider.GetService(resolvedType) as T;
        }
    }

}
