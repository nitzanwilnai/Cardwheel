using UnityEngine;
using UnityEngine.UI;
#if (MM_TEXTMESHPRO || MM_UGUI2)
using TMPro;

namespace MoreMountains.Feedbacks
{
	public class MMSpringTMPFontSize : MMSpringFloatComponent<TMP_Text>
	{
		public override float TargetFloat
		{
			get => Target.fontSize;
			set => Target.fontSize = (int)value;
		}
	}
}
#endif
