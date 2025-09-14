using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
	public class MMSpringCameraFieldOfView : MMSpringFloatComponent<Camera>
	{
		public override float TargetFloat
		{
			get => Target.fieldOfView;
			set => Target.fieldOfView = value;
		}
	}
}
