using UnityEngine;
using UnityEngine.UI;
#if (MM_TEXTMESHPRO || MM_UGUI2)
using TMPro;

namespace MoreMountains.Feedbacks
{
	public class MMSpringTMPDilate : MMSpringFloatComponent<TMP_Text>
	{
		protected override void ApplyValue(float newValue)
		{
			base.ApplyValue(newValue);
			Target.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, newValue);
		}
		
		protected override void GrabCurrentValue()
		{
			FloatSpring.CurrentValue = Target.fontMaterial.GetFloat(ShaderUtilities.ID_FaceDilate);
		}
	}
}
#endif
