using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.Feedbacks
{
	public class MMSpringTextureOffset : MMSpringVector2Component<Renderer>
	{
		public override Vector2 TargetVector2
		{
			get => Target.material.mainTextureOffset;
			set => Target.material.mainTextureOffset = value;
		}
	}
}
