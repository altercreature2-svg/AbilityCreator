using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace HiddenUnits
{
	// Token: 0x0200001A RID: 26
	public static class HUAddons
	{
		// Token: 0x060000A1 RID: 161 RVA: 0x0000780C File Offset: 0x00005A0C
		public static object GetField(Type type, object instance, string fieldName)
		{
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			FieldInfo field = type.GetField(fieldName, bindingAttr);
			return field.GetValue(instance);
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00007834 File Offset: 0x00005A34
		public static void SetField<T>(object originalObject, string fieldName, T newValue)
		{
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			FieldInfo field = originalObject.GetType().GetField(fieldName, bindingAttr);
			field.SetValue(originalObject, newValue);
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00007864 File Offset: 0x00005A64
		public static string DeepString(this GameObject self)
		{
			string str = "\nGameObject '" + self.name + "':\n{\n\tComponents:\n\t{\n";
			str += string.Concat(from Component component in self.GetComponents<Component>()
			select "\t\t" + component.GetType().Name + "\n");
			str += "\t}\n";
			bool flag = self.transform.childCount > 0;
			if (flag)
			{
				str += "\tChildren:\n\t{\n";
				str += string.Concat(from Transform child in self.transform
				select child.gameObject.DeepString().Replace("\n", "\n\t\t"));
				str += "\n\t}\n";
			}
			return str + "}\n";
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00007948 File Offset: 0x00005B48
		public static T DeepCopyOf<T>(this T self, T from) where T : class
		{
			foreach (FieldInfo fieldInfo in typeof(!!0).GetFields((BindingFlags)(-1)))
			{
				try
				{
					fieldInfo.SetValue(self, fieldInfo.GetValue(from));
				}
				catch
				{
				}
			}
			foreach (PropertyInfo propertyInfo in typeof(!!0).GetProperties((BindingFlags)(-1)))
			{
				bool flag = propertyInfo.CanWrite && propertyInfo.CanRead;
				if (flag)
				{
					try
					{
						propertyInfo.SetValue(self, propertyInfo.GetValue(from));
					}
					catch
					{
					}
				}
			}
			return self;
		}
	}
}
