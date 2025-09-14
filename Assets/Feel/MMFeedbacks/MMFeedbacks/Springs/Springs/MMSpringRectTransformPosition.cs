using System;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.Feedbacks
{
	public class MMSpringRectTransformPosition : MMSpringVector3Component<RectTransform>
	{
		public override Vector3 TargetVector3
		{
			get => Target.anchoredPosition3D;
			set => Target.anchoredPosition3D = value;
		}
	}
}
