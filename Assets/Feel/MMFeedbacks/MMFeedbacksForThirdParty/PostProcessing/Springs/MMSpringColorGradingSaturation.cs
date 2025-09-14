#if MM_POSTPROCESSING
using UnityEngine.Rendering.PostProcessing;

namespace MoreMountains.Feedbacks
{
	public class MMSpringColorGradingSaturation : MMSpringFloatComponent<PostProcessVolume>
	{
		protected ColorGrading _colorGrading;
		
		protected override void Initialization()
		{
			if (Target == null)
			{
				Target = this.gameObject.GetComponent<PostProcessVolume>();
			}
			Target.profile.TryGetSettings(out _colorGrading);
			base.Initialization();
		}
		
		public override float TargetFloat
		{
			get => _colorGrading.saturation;
			set => _colorGrading.saturation.Override(value);
		}
	}
}
#endif