using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
	public class MMSpringLightColor : MMSpringColorComponent<Light>
	{
		public override Color TargetColor
		{
			get => Target.color;
			set => Target.color = value;
		}
	}
}
