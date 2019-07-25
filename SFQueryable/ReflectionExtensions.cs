using System;
using System.Linq;

namespace SFQuerable
{
    public static class ReflectionExtensions
    {
        /// <summary>
		/// Determines if the object instance implements the generic interface type.
		/// </summary>
		/// <param name="instance">Object instance to verify.</param>
		/// <param name="genericType">Generic interface type to check for.</param>
		/// <returns>True if the type of instance implements the given generic interface type.</returns>
		public static bool ImplementsGenericType(this object instance, Type genericType)
        {
            if (!genericType.IsGenericType || !genericType.IsInterface || genericType.GenericTypeArguments?.Length > 0)
                throw new ArgumentException(nameof(genericType));

            return instance.GetType().GetInterfaces().Where(i => i.IsGenericType).Any(i => i.GetGenericTypeDefinition() == genericType);
        }

        public static TReturn CallMethod<TReturn>(this object instance, string methodName, params object[] parameters)
        {
            return (TReturn)instance.GetType().GetMethod(methodName).Invoke(instance, parameters);
        }

        public static TReturn CallMethod<TReturn>(this object instance, string methodName, Type[] parameterTypes, params object[] parameters)
        {
            return (TReturn)instance.GetType().GetMethod(methodName, parameterTypes).Invoke(instance, parameters);
        }

        public static TReturn GetPropertyValue<TReturn>(this object instance, string propertyName)
        {
            var property = instance.GetType().GetProperty(propertyName);
            return (TReturn)property?.GetValue(instance);
        }

        public static void SetPropertyValue(this object instance, string propertyName, object value)
        {
            var property = instance.GetType().GetProperty(propertyName);
            property?.SetValue(instance, value);
        }
    }
}
