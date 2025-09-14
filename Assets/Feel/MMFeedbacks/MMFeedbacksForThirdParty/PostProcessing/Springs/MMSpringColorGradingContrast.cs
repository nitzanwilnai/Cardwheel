#if MM_POSTPROCESSING
using UnityEngine.Rendering.PostProcessing;

namespace MoreMountains.Feedbacks
{
	public class MMSpringColorGradingContrast : MMSpringFloatComponent<PostProcessVolume>
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
			get => _colorGrading.contrast;
			set => _colorGrading.contrast.Override(value);
		}
	}
}
#endif