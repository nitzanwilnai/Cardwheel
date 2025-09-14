#if MM_URP
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MoreMountains.Feedbacks
{
	public class MMSpringLensDistortionIntensity_URP : MMSpringFloatComponent<Volume>
	{
		protected LensDistortion _lensDistortion;
		
		protected override void Initialization()
		{
			if (Target == null)
			{
				Target = this.gameObject.GetComponent<Volume>();
			}
			Target.profile.TryGet(out _lensDistortion);
			base.Initialization();
		}
		
		public override float TargetFloat
		{
			get => _lensDistortion.intensity.value;
			set => _lensDistortion.intensity.Override(value);
		}
	}
}
#endif