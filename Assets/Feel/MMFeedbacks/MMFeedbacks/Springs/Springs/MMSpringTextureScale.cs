using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.Feedbacks
{
	public class MMSpringTextureScale : MMSpringVector2Component<Renderer>
	{
		public override Vector2 TargetVector2
		{
			get => Target.material.mainTextureScale;
			set => Target.material.mainTextureScale = value;
		}
	}
}
