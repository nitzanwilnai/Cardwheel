#if MM_POSTPROCESSING
using UnityEngine.Rendering.PostProcessing;

namespace MoreMountains.Feedbacks
{
	public class MMSpringChromaticAberrationIntensity : MMSpringFloatComponent<PostProcessVolume>
	{
		protected ChromaticAberration _chromaticAberration;
		
		protected override void Initialization()
		{
			if (Target == null)
			{
				Target = this.gameObject.GetComponent<PostProcessVolume>();
			}
			Target.profile.TryGetSettings(out _chromaticAberration);
			base.Initialization();
		}
		
		public override float TargetFloat
		{
			get => _chromaticAberration.intensity;
			set => _chromaticAberration.intensity.Override(value);
		}
	}
}
#endif