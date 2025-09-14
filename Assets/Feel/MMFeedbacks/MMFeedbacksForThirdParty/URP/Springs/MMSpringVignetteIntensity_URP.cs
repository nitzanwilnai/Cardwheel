#if MM_URP
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MoreMountains.Feedbacks
{
	public class MMSpringVignetteIntensity_URP : MMSpringFloatComponent<Volume>
	{
		protected Vignette _vignette;
		
		protected override void Initialization()
		{
			if (Target == null)
			{
				Target = this.gameObject.GetComponent<Volume>();
			}
			Target.profile.TryGet(out _vignette);
			base.Initialization();
		}
		
		public override float TargetFloat
		{
			get => _vignette.intensity.value;
			set => _vignette.intensity.Override(value);
		}
	}
}
#endif