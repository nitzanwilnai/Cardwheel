using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
	[AddComponentMenu("More Mountains/Springs/MMSpringCameraOrthographicSize")]
	public class MMSpringCameraOrthographicSize : MMSpringFloatComponent<Camera>
	{
		public override float TargetFloat
		{
			get => Target.orthographicSize;
			set => Target.orthographicSize = value; 
		}
	}
}
