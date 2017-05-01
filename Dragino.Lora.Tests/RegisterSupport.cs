using System;
using System.Reflection;
using Dragino.Radio;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Dragino.Lora.Tests
{
	public static class RegisterSupport
	{
		public static Register GetDefaultRegister<T>() where T : Register
		{
			return GetDefaultRegister(typeof(T));
		}

		public static Register GetDefaultRegister(Type registerType)
		{
			try
			{
				return (Register)registerType
					.GetField("Default", BindingFlags.Public | BindingFlags.Static)
					.GetValue(null);
			}
			catch
			{
				Assert.Fail($"Failed getting the 'Default' static field of the register {registerType.Name}");
				return null;
			}
		}
	}
}