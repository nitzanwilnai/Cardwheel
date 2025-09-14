using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
	public class MMSpringShaderController : MMSpringFloatComponent<ShaderController>
	{
		public override float TargetFloat
		{
			get => Target.DrivenLevel;
			set => Target.DrivenLevel = value;
		}
	}
}
