using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace HiddenUnits.Properties
{
	// Token: 0x02000029 RID: 41
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		// Token: 0x060000F0 RID: 240 RVA: 0x0000B145 File Offset: 0x00009345
		internal Resources()
		{
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x060000F1 RID: 241 RVA: 0x0000B150 File Offset: 0x00009350
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				bool flag = Resources.resourceMan == null;
				if (flag)
				{
					ResourceManager resourceManager = new ResourceManager("HiddenUnits.Properties.Resources", typeof(Resources).Assembly);
					Resources.resourceMan = resourceManager;
				}
				return Resources.resourceMan;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x060000F2 RID: 242 RVA: 0x0000B198 File Offset: 0x00009398
		// (set) Token: 0x060000F3 RID: 243 RVA: 0x0000B1AF File Offset: 0x000093AF
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x060000F4 RID: 244 RVA: 0x0000B1B8 File Offset: 0x000093B8
		internal static byte[] hiddenunits
		{
			get
			{
				object @object = Resources.ResourceManager.GetObject("hiddenunits", Resources.resourceCulture);
				return (byte[])@object;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x060000F5 RID: 245 RVA: 0x0000B1E8 File Offset: 0x000093E8
		internal static byte[] humaps
		{
			get
			{
				object @object = Resources.ResourceManager.GetObject("humaps", Resources.resourceCulture);
				return (byte[])@object;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x060000F6 RID: 246 RVA: 0x0000B218 File Offset: 0x00009418
		internal static byte[] egyptmap
		{
			get
			{
				object @object = Resources.ResourceManager.GetObject("egyptmap", Resources.resourceCulture);
				return (byte[])@object;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x060000F7 RID: 247 RVA: 0x0000B248 File Offset: 0x00009448
		internal static byte[] egyptmap2
		{
			get
			{
				object @object = Resources.ResourceManager.GetObject("egyptmap2", Resources.resourceCulture);
				return (byte[])@object;
			}
		}

		// Token: 0x04000174 RID: 372
		private static ResourceManager resourceMan;

		// Token: 0x04000175 RID: 373
		private static CultureInfo resourceCulture;
	}
}
