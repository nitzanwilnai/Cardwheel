using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
	public class MMSpringCameraOrthographicSize : MMSpringFloatComponent<Camera>
	{
		public override float TargetFloat
		{
			get => Target.orthographicSize;
			set => Target.orthographicSize = value; 
		}
	}
}
