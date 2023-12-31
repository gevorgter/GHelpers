﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace GHelpers
{
    public class AttributeMap
    {
        public Type attributeClass;
        public Action<IServiceCollection, Attribute, Type> attributeHandler;
        public AttributeMap(Type attributeClass, Action<IServiceCollection, Attribute, Type> attributeHandler)
        {
            if (!typeof(Attribute).IsAssignableFrom(attributeClass))
                throw new Exception($"{attributeClass.Name} must inherit Attribute");
            this.attributeClass = attributeClass;
            this.attributeHandler = attributeHandler;
        }
    }

    public static class AssemblyScanner
    {
        public static IServiceCollection AddAttributeDefinedServices(this IServiceCollection serviceCollection, Type assemblyToScan, params AttributeMap[] attributeMap)
        {
            var assembly = assemblyToScan.Assembly;
            var allClasses = assembly.GetTypes();
            foreach (var t in allClasses)
            {
                var attributes = t.GetCustomAttributes(false);
                foreach (var attr in attributes)
                {
                    var mapper = attributeMap.FirstOrDefault(x => x.attributeClass == attr.GetType());
                    if (mapper == null)
                        continue;
                    mapper.attributeHandler(serviceCollection, (Attribute)attr, t);
                }
            }
            return serviceCollection;
        }
    }
}
