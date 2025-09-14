using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
	public class MMSpringSpriteColor : MMSpringColorComponent<SpriteRenderer>
	{
		public override Color TargetColor
		{
			get => Target.color;
			set => Target.color = value;
		}
	}
}
