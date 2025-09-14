using UnityEngine;
using UnityEngine.UI;
#if (MM_TEXTMESHPRO || MM_UGUI2)
using TMPro;

namespace MoreMountains.Feedbacks
{
	public class MMSpringTMPTextColor : MMSpringColorComponent<TMP_Text>
	{
		public override Color TargetColor
		{
			get => Target.color;
			set => Target.color = value;
		}
	}
}
#endif
