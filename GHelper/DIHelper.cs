using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace GHelpers
{
    public enum DIScope { Singleton = 0, Scoped = 1, Transient = 2 }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class DIHelperAttribute : Attribute
    {
        public DIScope scope;
        public string? key;
        public Type? askType;

        public DIHelperAttribute(DIScope scope, Type? askType = null, string? key = null)
        {
            this.scope = scope;
            this.key = key;
            this.askType = askType;
        }
    }

    public static class DIHelper
    {
        public static IServiceCollection UseDIHelper(this IServiceCollection serviceCollection, Type assemblyToScan)
        {
            var assembly = assemblyToScan.Assembly;
            var allClasses = assembly.GetTypes().Where(t => !t.IsAbstract);
            foreach (var t in allClasses)
            {
                var attr = t.GetCustomAttribute<DIHelperAttribute>(false);
                if (attr == null)
                    continue;

                var useMethod = t.GetMethod("Use");
                useMethod?.Invoke(null, new object[] { serviceCollection });
                RegisterTypeWithDi(serviceCollection, t, attr);
            }
            return serviceCollection;
        }

        static void RegisterTypeWithDi(IServiceCollection serviceCollection, Type implementationType, DIHelperAttribute diKey)
        {
            switch (diKey.scope)
            {
                case DIScope.Singleton:
                    RegisterTypeWithDi(implementationType, diKey.key, diKey.askType, serviceCollection.AddSingleton, serviceCollection.AddSingleton, serviceCollection.AddKeyedSingleton);
                    break;
                case DIScope.Scoped:
                    RegisterTypeWithDi(implementationType, diKey.key, diKey.askType, serviceCollection.AddScoped, serviceCollection.AddScoped, serviceCollection.AddKeyedScoped);
                    break;
                case DIScope.Transient:
                    RegisterTypeWithDi(implementationType, diKey.key, diKey.askType, serviceCollection.AddTransient, serviceCollection.AddTransient, serviceCollection.AddKeyedTransient);
                    break;
            }
        }

        static bool RegisterTypeWithDi(Type t, string? key, Type? askType, Func<Type, IServiceCollection> AddToDi, Func<Type, Type, IServiceCollection> AddToDi2, Func<Type, object?, Type, IServiceCollection> AddToDiKeyed)
        {
            if (key == null)
            {
                if (askType == null)
                    AddToDi(t);
                else
                    AddToDi2(askType, t);
            }
            else
            {
                askType.ThrowExceptionIfNull("You must provide askType if you are using keys registration with DI, type:{0}", t.Name);
                AddToDiKeyed(askType, key, t);
            }
            return true;
        }
    }

}
