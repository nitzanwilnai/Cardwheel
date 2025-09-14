using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
	public class MMSpringAudioSourceVolume : MMSpringFloatComponent<AudioSource>
	{
		public override float TargetFloat
		{
			get => Target.volume;
			set => Target.volume = value; 
		}
	}
}
