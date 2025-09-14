using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
	public class MMSpringAudioSourcePitch : MMSpringFloatComponent<AudioSource>
	{
		public override float TargetFloat
		{
			get => Target.pitch;
			set => Target.pitch = value; 
		}
	}
}
