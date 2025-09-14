#if MM_HDRP
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace MoreMountains.Feedbacks
{
	public class MMSpringChromaticAberrationIntensity_HDRP : MMSpringFloatComponent<Volume>
	{
		protected ChromaticAberration _chromaticAberration;
		
		protected override void Initialization()
		{
			if (Target == null)
			{
				Target = this.gameObject.GetComponent<Volume>();
			}
			Target.profile.TryGet(out _chromaticAberration);
			base.Initialization();
		}
		
		public override float TargetFloat
		{
			get => _chromaticAberration.intensity.value;
			set => _chromaticAberration.intensity.Override(value);
		}
	}
}
#endif