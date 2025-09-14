#if MM_HDRP
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace MoreMountains.Feedbacks
{
	public class MMSpringColorAdjustmentsContrast_HDRP : MMSpringFloatComponent<Volume>
	{
		protected ColorAdjustments _colorAdjustments;
		
		protected override void Initialization()
		{
			if (Target == null)
			{
				Target = this.gameObject.GetComponent<Volume>();
			}
			Target.profile.TryGet(out _colorAdjustments);
			base.Initialization();
		}
		
		public override float TargetFloat
		{
			get => _colorAdjustments.contrast.value;
			set => _colorAdjustments.contrast.Override(value);
		}
	}
}
#endif