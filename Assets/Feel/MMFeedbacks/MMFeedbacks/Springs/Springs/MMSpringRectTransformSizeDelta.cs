using System;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.Feedbacks
{
	public class MMSpringRectTransformSizeDelta : MMSpringVector2Component<RectTransform>
	{
		public override Vector2 TargetVector2
		{
			get => Target.sizeDelta;
			set => Target.sizeDelta = value;
		}
	}
}
