using UnityEngine;
#if MM_POSTPROCESSING
using UnityEngine.Rendering.PostProcessing;

namespace MoreMountains.Feedbacks
{
	public class MMSpringVignetteColor : MMSpringColorComponent<PostProcessVolume>
	{
		protected Vignette _vignette;
		
		protected override void Initialization()
		{
			if (Target == null)
			{
				Target = this.gameObject.GetComponent<PostProcessVolume>();
			}
			Target.profile.TryGetSettings(out _vignette);
			base.Initialization();
		}
		
		public override Color TargetColor
		{
			get => _vignette.color;
			set => _vignette.color.Override(value);
		}
	}
}
#endif