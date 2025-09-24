using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
	[AddComponentMenu("More Mountains/Springs/MMSpringLightIntensity")]
	public class MMSpringLightIntensity : MMSpringFloatComponent<Light>
	{
		public override float TargetFloat
		{
			get => Target.intensity;
			set => Target.intensity = value; 
		}
	}
}
