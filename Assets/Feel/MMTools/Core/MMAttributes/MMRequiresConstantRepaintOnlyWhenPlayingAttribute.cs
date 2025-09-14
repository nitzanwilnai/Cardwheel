using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace MoreMountains.Tools
{
	[AttributeUsage(AttributeTargets.Class)]
	public class MMRequiresConstantRepaintOnlyWhenPlayingAttribute : Attribute
	{

	}
}