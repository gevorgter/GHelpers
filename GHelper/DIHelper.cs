using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GHelpers
{
    public interface IDIHelper
    {
    }
    public interface ISingleton : IDIHelper
    {

    }
    public interface IScoped : IDIHelper
    {

    }
    public interface ITransient : IDIHelper
    {

    }

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
            _ = mapTypes.TryGetValue(GetKey(t, key), out var resolvedType);
            resolvedType.ThrowExceptionIfNull("DI could not resolve type");
            var r = serviceProvider.GetRequiredService(resolvedType) as T;
            r.ThrowExceptionIfNull("DI could not resolve type");
            return r;
        }

        public static IServiceCollection UseDIHelper(this IServiceCollection serviceCollection, Type assemblyToScan)
        {
            Type baseType = typeof(IDIHelper);
            var assembly = assemblyToScan.Assembly;
            var allRepositories = assembly.GetTypes().Where(t => !t.IsAbstract && baseType.IsAssignableFrom(t));
            foreach (var t in allRepositories)
            {
                var useMethod = t.GetMethod("Use");
                useMethod?.Invoke(null, new object[] { serviceCollection });
                if (typeof(ISingleton).IsAssignableFrom(t))
                    serviceCollection.AddSingleton(t);
                if (typeof(IScoped).IsAssignableFrom(t))
                    serviceCollection.AddScoped(t);
                if (typeof(ITransient).IsAssignableFrom(t))
                    serviceCollection.AddTransient(t);
            }
            return serviceCollection;
        }
    }

}
