using Microsoft.Extensions.DependencyInjection;
using System;

namespace GHelpers
{
    public enum DIScope { Singleton = 0, Scoped = 1, Transient = 2 }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class UseHelperAttribute : Attribute
    {

    }

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
        public static AttributeMap diHelperMapper = new AttributeMap(typeof(DIHelperAttribute), RegisterWithDI);
        public static AttributeMap useHelperMapper = new AttributeMap(typeof(UseHelperAttribute), CallUseMethod);

        public static void RegisterWithDI(IServiceCollection serviceCollection, Attribute attr, Type t)
        {
            RegisterTypeWithDi(serviceCollection, t, (DIHelperAttribute)attr);
        }

        public static void CallUseMethod(IServiceCollection serviceCollection, Attribute attr, Type t)
        {
            var useMethod = t.GetMethod("Use");
            useMethod?.Invoke(null, new object[] { serviceCollection });
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
