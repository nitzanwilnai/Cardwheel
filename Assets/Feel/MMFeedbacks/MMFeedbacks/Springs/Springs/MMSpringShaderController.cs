using MoreMountains.Tools;
using UnityEngine;

#if MM_UI
namespace MoreMountains.Feedbacks
{
	[AddComponentMenu("More Mountains/Springs/MMSpringShaderController")]
	public class MMSpringShaderController : MMSpringFloatComponent<ShaderController>
	{
		public override float TargetFloat
		{
			get => Target.DrivenLevel;
			set => Target.DrivenLevel = value;
		}
	}
}
#endif