using UnityEngine;

namespace MoreMountains.Feedbacks
{
	[AddComponentMenu("More Mountains/Springs/MMSpringTextureScale")]
	public class MMSpringTextureScale : MMSpringVector2Component<Renderer>
	{
		public override Vector2 TargetVector2
		{
			get => Target.material.mainTextureScale;
			set => Target.material.mainTextureScale = value;
		}
	}
}
