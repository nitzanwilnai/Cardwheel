using UnityEngine;
using UnityEngine.UI;
#if (MM_TEXTMESHPRO || MM_UGUI2)
using TMPro;

namespace MoreMountains.Feedbacks
{
	public class MMSpringTMPCharacterSpacing : MMSpringFloatComponent<TMP_Text>
	{
		public override float TargetFloat
		{
			get => Target.characterSpacing;
			set => Target.characterSpacing = value;
		}
	}
}
#endif
