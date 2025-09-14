using UnityEngine;
using UnityEngine.UI;
#if (MM_TEXTMESHPRO || MM_UGUI2)
using TMPro;

namespace MoreMountains.Feedbacks
{
	public class MMSpringTMPWordSpacing : MMSpringFloatComponent<TMP_Text>
	{
		public override float TargetFloat
		{
			get => Target.wordSpacing;
			set => Target.wordSpacing = value;
		}
	}
}
#endif
