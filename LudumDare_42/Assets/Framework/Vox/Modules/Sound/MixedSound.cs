#define SKIP_CODE_IN_DOCUMENTATION

using UnityEngine;
using UnityEngine.Audio;
using VoxInternal;

namespace Vox
{
    public enum AudioModelType
    {
        DEFAULT
    }
    /// <summary>
    /// Class used to create Vox.SoundNode and manage their outputs by using Unity <A href="http://docs.unity3d.com/Manual/AudioMixer.html"><STRONG>AudioMixers</STRONG></A>.
    /// </summary>
    /// <remarks>
    /// 
    /// Class used to create Vox.SoundNode and manage their outputs by using Unity  <A href="http://docs.unity3d.com/Manual/AudioMixer.html"><STRONG>AudioMixers</STRONG></A>
    /// 
    /// To understand AudioMixers check the Unity Official Documentation: <A href="http://docs.unity3d.com/Manual/AudioMixer.html"><STRONG>Unity Audio Mixer Reference</STRONG></A>
    /// 
    /// <STRONG>Attention:</STRONG> To insert a Mixer to be used in the project, go to main editor window and follow <I></c>"Vox/Modules/Sound"</I>, there you can drag any created audiomixers to be available to this class.
    /// 
    /// Small example of code used with MixedSound:
    /// 
    /// <code>
    /// 
    /// Vox.SoundNode _backgroundMusic;
    /// public AudioClip _clip;
    /// 
    /// public void PlayMusic()
    /// {
    /// 	_backgroundMusic = Vox.MixedSound.Play2DSound(_clip, true, "GameplayMixer", "Music");
    /// }
    /// 
    /// public float GetMusicVolume()
    /// {
    /// 	return Vox.MixedSound.GetMixerGroupVolume("GameplayMixer", "Music");
    /// }
    /// 
    /// </code>
    /// 
    /// </remarks>
    public class MixedSound: BaseSound
	{
		#region Private Internal Only

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @cond
		#endif

		/// <summary>
		/// Private function that actually creates SoundNode.
		/// </summary>
		private static SoundNode PlaySound(AudioClip p_clip, bool p_isLoop, float p_volume, string p_mixerName, string p_groupOutput, Transform p_target = null, AudioModelType p_model = AudioModelType.DEFAULT)
		{
            SoundNode __nodule = new SoundNode( SoundModule.instance.GetFreeVoxAudioSource( p_model ), p_clip, p_isLoop, p_volume, p_mixerName , p_groupOutput, SoundModule.instance.GetNextNodeID());

            if (p_target != null)
            {
                // 3D Sound
                __nodule.VoxVoxSource.audioSource.spatialBlend = 1;
                __nodule.VoxVoxSource.targetTransform = p_target;
            }
            else
            {
                // 2D Sound
                __nodule.VoxVoxSource.audioSource.spatialBlend = 0;
            }

            __nodule.Resume();
            
            SoundModule.instance.AddNode(__nodule);
            return __nodule;
        }

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @endcond
		#endif

		#endregion

		#region MixerMode
		/// <summary>
		/// Creates and plays SoundNode in 3Dspace using the  passed parameters.
		/// </summary>
		/// <remarks>
		/// Creates and plays SoundNode in 3Dspace using the  passed parameters.
		/// Use this if you want your sound to be associated to a gameobject in the game, making the sound increase or decrease depending on the camera distance to the object.
		/// The function requires that you passes the name of the destined mixer and outputmixergroup.
		/// <code>
		/// 
		/// Vox.SoundNode _monsterSnore;
		/// public GameObject _monster;
		/// public AudioClip _clip;
		/// 
		/// public void MonsterWentToSleep()
		/// {
		/// 	_monsterSnore = Vox.MixedSound.Play3DSound(_clip, _monster, true, "GameplayMixer", "EnemiesFX");
		/// }
		/// 
		/// </code>
		/// 
		/// </remarks> 
		public static SoundNode Play3DSound(AudioClip p_clip, Transform p_target, bool p_isLoop = false, float p_volume = 1, string p_mixerName = "", string p_groupOutput = "", AudioModelType p_model = AudioModelType.DEFAULT)
		{
		    return PlaySound(p_clip, p_isLoop, p_volume, p_mixerName , p_groupOutput, p_target, p_model);
		}

		/// <summary>
		/// Creates and plays SoundNode that ignores camera distance.
		/// </summary>
		/// <remarks>
		/// Creates and plays SoundNode that ignores camera distance. Useful for music or other sounds you always want to listen with the same volume.
		/// The function requires that you passes the name of the destined mixer and outputmixergroup.
		/// 
		/// <code>
		/// 
		/// Vox.SoundNode _backgroundMusic;
		/// public AudioClip _clip;
		/// 
		/// public void PlayMusic()
		/// {
		/// 	_backgroundMusic = Vox.MixedSound.Play2DSound(_clip, true, "GameplayMixer", "Music");
		/// }
		/// 
		/// </code>
		/// 
		/// </remarks> 
		public static SoundNode Play2DSound(AudioClip p_clip, bool p_isLoop = false, float p_volume = 1, string p_mixerName = "", string p_groupOutput = "" , AudioModelType p_model = AudioModelType.DEFAULT)
        {
		    return PlaySound(p_clip, p_isLoop, p_volume, p_mixerName , p_groupOutput, null, p_model );
        }

        /// <summary>
        /// Get volume of the desired  OutputMixerGroup.
        /// </summary>
        /// <remarks>
        /// Get the current volume which is used to control all the sounds of a OutputMixerGroup.
        /// 
        /// <code>
        /// 
        /// Vox.SoundNode _backgroundMusic;
        /// public AudioClip _clip;
        /// 
        /// public void PlayMusic()
        /// {
        /// 	_backgroundMusic = Vox.MixedSound.Play2DSound(_clip, true, "GameplayMixer", "Music");
        /// }
        /// 
        /// public float GetMusicVolume()
        /// {
        /// 	return Vox.MixedSound.GetMixerGroupVolume("GameplayMixer", "Music");
        /// }
        /// 
        /// </code>
        /// 
        /// </remarks>  
        public static float GetMixerGroupVolume(string p_mixerName, string p_mixerGroup)
		{
			AudioMixerGroup __group = null;

			for (int i = 0; i < SoundModule.instance.listAudioMixers.Count; i++) 
			{
				if (SoundModule.instance.listAudioMixers [i].name == p_mixerName) 
				{
					__group = SoundModule.instance.listAudioMixers[i].FindMatchingGroups (p_mixerGroup)[0];
					break;
				}
			}

			if (__group == null) 
			{
				Debug.LogError("No group with this name added to Current Mixer. Maybe you forgot to add it manually");
				return 0;
			}

			float __value = 0;

			if(__group.audioMixer.GetFloat (__group.name + " Volume", out __value) == false)
			{
				Debug.LogError ("No exposed volume was found");	
			}	

			return __value;	
		}

		/// <summary>
		/// Set the volume of the desired  OutputMixerGroup.
		/// </summary>
		/// <remarks>
		/// Set the current volume which is used to control all the sounds of a OutputMixerGroup.
		/// 
		/// <code>
		/// 
		/// Vox.SoundNode _backgroundMusic;
		/// public AudioClip _clip;
		/// 
		/// public void PlayMusic()
		/// {
		/// 	_backgroundMusic = Vox.MixedSound.Play2DSound(_clip, true, "GameplayMixer", "Music");
		/// }
		/// 
		/// public float SetMusicVolume(float p_volume)
		/// {
		/// 	return Vox.MixedSound.GetMixerGroupVolume("GameplayMixer", "Music", p_volume);
		/// }
		/// 
		/// </code>
		/// 
		/// </remarks>  
		public static void SetMixerGroupVolume(string p_mixerName, string p_mixerGroup, float p_volume)
		{
			AudioMixerGroup __group = null;

			for (int i = 0; i < SoundModule.instance.listAudioMixers.Count; i++) 
			{
				if (SoundModule.instance.listAudioMixers [i].name == p_mixerName) 
				{
					__group = SoundModule.instance.listAudioMixers[i].FindMatchingGroups (p_mixerGroup)[0];
					break;
				}
			}

			if (__group == null) 
			{
				Debug.LogError("No group with this name added to Current Mixer. Maybe you forgot to add it manually");
				return;
			}

			if (__group.audioMixer.SetFloat (__group.name + " Volume", p_volume) == false) 
			{
				Debug.LogError ("No exposed volume was found");	
			}	
		}

		/// <summary>
		/// Resume all the sounds of a outputMixerGroup if they were paused.
		/// </summary>
		/// <remarks>
		/// Resume  all the sounds of a outputMixerGroup if they were paused.
		/// Use that instead of muting sounds if the game is paused or something similar, generally more useful for fx than music.
		/// 
		/// <code>
		/// 
		/// Vox.SoundNode _screamFXNode;
		/// public AudioClip _clip;
		/// 
		/// public void PlayScream()
		/// {
		/// 	_screamFXNode = Vox.MixedSound.Play2DSound(_clip, true, "GameplayMixer", "FX");
		/// }
		/// 
		/// public void PauseGame()
		/// {
		/// 	Vox.MixedSound.PauseMixerGroup("GameplayMixer", "FX"));
		/// }
		/// 
		/// public void ResumeGame()
		/// {
		/// 	Vox.MixedSound.ResumeMixerGroup("GameplayMixer", "FX"));
		/// }
		/// 
		/// </code>
		/// 
		/// </remarks>  
		public static void ResumeMixerGroup(string p_mixerName, string p_mixerGroup)
		{
			SoundModule.instance.ResumeMixerGroupSounds(p_mixerName, p_mixerGroup);
		}

		/// <summary>
		/// Pause all the sounds of a outputMixerGroup if they were playing.
		/// </summary>
		/// <remarks>
		/// Pause  all the sounds of a outputMixerGroup if they were playing.
		/// Use that instead of muting sounds if the game is paused or something similar, generally more useful for fx than music.
		/// 
		/// <code>
		/// 
		/// Vox.SoundNode _screamFXNode;
		/// public AudioClip _clip;
		/// 
		/// public void PlayScream()
		/// {
		/// 	_screamFXNode = Vox.MixedSound.Play2DSound(_clip, true, "GameplayMixer", "FX");
		/// }
		/// 
		/// public void PauseGame()
		/// {
		/// 	Vox.MixedSound.PauseMixerGroup("GameplayMixer", "FX"));
		/// }
		/// 
		/// public void ResumeGame()
		/// {
		/// 	Vox.MixedSound.ResumeMixerGroup("GameplayMixer", "FX"));
		/// }
		/// 
		/// </code>
		/// 
		/// </remarks>  
		public static void PauseMixerGroup(string p_mixerName, string p_mixerGroup)
		{
			SoundModule.instance.PauseMixerGroupSounds(p_mixerName, p_mixerGroup);
		}

		/// <summary>
		/// Stop all the sounds of an outputMixerGroup, removing them.
		/// </summary>
		/// <remarks>
		/// Stop all the sounds of an outputMixerGroup, removing them.
		/// 
		/// <code>
		/// 
		/// public AudioClip _clip;
		/// 
		/// public void PlayMonsterNoise(GameObject p_monster)
		/// {
		/// 	Vox.MixedSound.Play2DSound(_clip, p_monster, false,"GameplayMixer", "FX");
		/// }
		/// 
		/// public void RemoveAllGameplayFX()
		/// {
		/// 	Vox.MixedSound.StopMixerGroup("GameplayMixer", "FX");
		/// }
		/// 
		/// </code>
		/// 
		/// </remarks>  
		public static void StopMixerGroup(string p_mixerName, string p_mixerGroup)
		{
			SoundModule.instance.StopMixerGroupSounds (p_mixerName, p_mixerGroup);
		}

		/// <summary>
		/// Request to get exposed float in passed mixer
		/// </summary>
		/// <remarks>
		/// Request to get exposed float in passed mixer.
		/// For more information on exposed parameters check <A href="http://unity3d.com/pt/learn/tutorials/topics/audio/exposed-audiomixer-parameters?playlist=17096"><STRONG>this video</STRONG></A>.
		/// </remarks>
		public static float GetMixerExposedVariable(string p_mixerName, string p_key)
		{
			for (int i = 0; i < SoundModule.instance.listAudioMixers.Count; i++) 
			{
				if (SoundModule.instance.listAudioMixers [i].name == p_mixerName) 
				{
					float __value = -1;

					if (SoundModule.instance.listAudioMixers [i].GetFloat (p_key, out __value)) 
					{
						return __value;
					}
					else
					{
						Debug.LogError ("No exposed variable was found");	
					}	
				}
			}

			return -1;
		}

		/// <summary>
		/// Set an exposed float in passed the passed mixer
		/// </summary>
		/// <remarks>
		/// Set an exposed float in passed the passed mixer
		/// For more information on exposed parameters check <A href="http://unity3d.com/pt/learn/tutorials/topics/audio/exposed-audiomixer-parameters?playlist=17096"><STRONG>this video</STRONG></A>
		/// </remarks>
		public static void SetMixerExposedVariable(string p_mixerName, string p_key, float p_value)
		{
			for (int i = 0; i < SoundModule.instance.listAudioMixers.Count; i++) 
			{
				if (SoundModule.instance.listAudioMixers [i].name == p_mixerName) 
				{
					if(SoundModule.instance.listAudioMixers [i].SetFloat (p_key, p_value) == false)
					{
						Debug.LogError ("No exposed variable was found");	
					}	
					break;
				}
			}
		}

		/// <summary>
		/// Transition current <A href="http://unity3d.com/pt/learn/tutorials/topics/audio/audio-mixer-snapshots?playlist=17096"><STRONG>Snapshot</STRONG></A> state of mixer to the passed snapshot name.
		/// </summary>
		/// <remarks>
		/// Transition current Snapshot state of mixer to the passed snapshot name.
		/// Snapshots are types of states a mixer can have, each one having its own volumes and such.
		/// For more information on SnapShots check <A href="http://unity3d.com/pt/learn/tutorials/topics/audio/exposed-audiomixer-parameters?playlist=17096"><STRONG>this video</STRONG></A>.
		/// </remarks> 
		public static void TransitionMixerToSnapShot(string p_mixerName, string p_snapShot, float p_transitionTime)
		{
			AudioMixerSnapshot __snapShotToChange = null;

			for (int i = 0; i < SoundModule.instance.listAudioMixers.Count; i++) 
			{
				if (SoundModule.instance.listAudioMixers [i].name == p_mixerName) 
				{
					__snapShotToChange = SoundModule.instance.listAudioMixers [i].FindSnapshot (p_snapShot);
					break;
				}
			}

			if (__snapShotToChange == null) 
			{
				Debug.LogError("No Snapshot with this name added to Current Mixer. Maybe you forgot to add it manually");
				return;
			}

			__snapShotToChange.TransitionTo (p_transitionTime);
		}
		#endregion
	}
}