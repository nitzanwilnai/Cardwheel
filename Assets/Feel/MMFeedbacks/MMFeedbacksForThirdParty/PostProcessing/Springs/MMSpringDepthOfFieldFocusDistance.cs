#if MM_POSTPROCESSING
using UnityEngine.Rendering.PostProcessing;

namespace MoreMountains.Feedbacks
{
	public class MMSpringDepthOfFieldFocusDistance : MMSpringFloatComponent<PostProcessVolume>
	{
		protected DepthOfField _depthOfField;
		
		protected override void Initialization()
		{
			if (Target == null)
			{
				Target = this.gameObject.GetComponent<PostProcessVolume>();
			}
			Target.profile.TryGetSettings(out _depthOfField);
			base.Initialization();
		}
		
		public override float TargetFloat
		{
			get => _depthOfField.focusDistance;
			set => _depthOfField.focusDistance.Override(value);
		}
	}
}
#endif