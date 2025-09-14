using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
	public class MMSpringLightRange : MMSpringFloatComponent<Light>
	{
		public override float TargetFloat
		{
			get => Target.range;
			set => Target.range = value; 
		}
	}
}
