using MoreMountains.Tools;
using UnityEngine;
#if MM_UI
using UnityEngine.UI;

namespace MoreMountains.Feedbacks
{
	[AddComponentMenu("More Mountains/Springs/MMSpringImageColor")]
	public class MMSpringImageColor : MMSpringColorComponent<Image>
	{
		public override Color TargetColor
		{
			get => Target.color;
			set => Target.color = value;
		}
	}
}
#endif