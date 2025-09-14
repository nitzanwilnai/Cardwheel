#if MM_POSTPROCESSING
using UnityEngine.Rendering.PostProcessing;

namespace MoreMountains.Feedbacks
{
	public class MMSpringLensDistortionIntensity : MMSpringFloatComponent<PostProcessVolume>
	{
		protected LensDistortion _lensDistortion;
		
		protected override void Initialization()
		{
			if (Target == null)
			{
				Target = this.gameObject.GetComponent<PostProcessVolume>();
			}
			Target.profile.TryGetSettings(out _lensDistortion);
			base.Initialization();
		}
		
		public override float TargetFloat
		{
			get => _lensDistortion.intensity;
			set => _lensDistortion.intensity.Override(value);
		}
	}
}
#endif