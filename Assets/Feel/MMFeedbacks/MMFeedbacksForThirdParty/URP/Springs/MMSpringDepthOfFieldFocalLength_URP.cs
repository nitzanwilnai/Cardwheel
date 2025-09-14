#if MM_URP
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MoreMountains.Feedbacks
{
	public class MMSpringDepthOfFieldFocalLength_URP : MMSpringFloatComponent<Volume>
	{
		protected DepthOfField _depthOfField;
		
		protected override void Initialization()
		{
			if (Target == null)
			{
				Target = this.gameObject.GetComponent<Volume>();
			}
			Target.profile.TryGet(out _depthOfField);
			base.Initialization();
		}
		
		public override float TargetFloat
		{
			get => _depthOfField.focalLength.value;
			set => _depthOfField.focalLength.Override(value);
		}
	}
}
#endif