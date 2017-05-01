using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Dragino.Radio
{
    internal class RegisterConstructorInfo
    {
        private static readonly ConcurrentDictionary<Type, RegisterConstructorInfo> _registerConstructorInfos = new ConcurrentDictionary<Type, RegisterConstructorInfo>();

        private RegisterConstructorInfo(byte address, byte length, ConstructorInfo constructorInfo)
        {
            Address = address;
            Length = length;
            ConstructorInfo = constructorInfo;
        }

        public byte Address { get; }

        public byte Length { get; }

        public ConstructorInfo ConstructorInfo { get; }

        public static RegisterConstructorInfo Get(Type type)
        {
            return _registerConstructorInfos.GetOrAdd(
                type,
                x =>
                {
                    RegisterAttribute registerAttribute = type.GetTypeInfo().GetCustomAttribute<RegisterAttribute>();
                    ConstructorInfo constructor = type.GetConstructors(BindingFlags.Instance|BindingFlags.NonPublic).First(ConstructorHasByteArrayParameters);
                    return new RegisterConstructorInfo(registerAttribute.Address, registerAttribute.Length, constructor);
                });
        }

        private static bool ConstructorHasByteArrayParameters(ConstructorInfo constructorInfo)
        {
            ParameterInfo[] parameterInfos = constructorInfo.GetParameters();
            if (parameterInfos.Length != 1)
            {
                return false;
            }

            return parameterInfos[0].ParameterType == typeof(byte[]);
        }
    }
}