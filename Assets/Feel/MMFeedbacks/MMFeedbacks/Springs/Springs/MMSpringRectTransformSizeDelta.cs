using UnityEngine;

namespace MoreMountains.Feedbacks
{
	[AddComponentMenu("More Mountains/Springs/MMSpringRectTransformSizeDelta")]
	public class MMSpringRectTransformSizeDelta : MMSpringVector2Component<RectTransform>
	{
		public override Vector2 TargetVector2
		{
			get => Target.sizeDelta;
			set => Target.sizeDelta = value;
		}
	}
}
