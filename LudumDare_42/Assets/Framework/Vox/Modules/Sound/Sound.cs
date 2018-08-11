#define SKIP_CODE_IN_DOCUMENTATION

using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using VoxInternal;

namespace Vox
{
    /// <summary>
    /// Default Sound Layers to be used in SoundNode when not on mixed mode.
    /// </summary>
    /// <remarks>
    /// Default Sound Layers to be used in SoundNode when not on mixed mode.
    /// Feel free to modify these layers depending on the necessity of your project.
    /// </remarks>
    public enum SoundLayerType
    {
        NOT_SET,
        MUSIC,
        GAMEPLAY_FX,
        HUD_FX,
        AMBIENCE
    }

    /// <summary>
    /// Class used to create SoundNode and control them without using Unity AudioMixer.
    /// </summary>
    /// <remarks>
    /// Class used to create SoundNodes and control them without using Unity AudioMixer.
    /// Instead it gives you the option to manage sounds via Vox.SoundLayerType, attaching nodes to these these layers.
    /// 
    /// Example Code: 
    /// <code>
    /// 
    /// Vox.SoundNode _backgroundMusic;
    /// public AudioClip _clip;
    /// 
    /// public void PlayMusic()
    /// {
    /// 	_backgroundMusic = Vox.Sound.Play2DSound(_clip, true, Vox.SoundLayerType.MUSIC);
    /// }
    /// 
    /// public float GetMusicVolume()
    /// {
    /// 	return Vox.Sound.GetLayerVolume(Vox.SoundLayerType.MUSIC);
    /// }
    /// 
    /// </code>
    ///  
    /// </remarks>
    public class Sound : BaseSound
    {
        #region Private Internal Only

#if SKIP_CODE_IN_DOCUMENTATION
        /// @cond
#endif

        /// <summary>
        /// Private function that actually creates SoundNode.
        /// </summary>
		/// 
		private static SoundNode PlaySound(AudioClip p_clip, bool p_isLoop, float p_volume, SoundLayerType p_layer, Transform p_target = null)
        {
			SoundNode __nodule = new SoundNode( SoundModule.instance.GetFreeVoxAudioSource( AudioModelType.DEFAULT ), p_clip, p_isLoop, p_volume, p_layer  , SoundModule.instance.GetNextNodeID());

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

        public static SoundNode Play2DSound(AudioClip p_clip, bool p_isLoop, float p_volume, SoundLayerType p_layer)
        {
			return PlaySound(p_clip, p_isLoop, p_volume, p_layer, null );
        }

		public static SoundNode Play3DSound(AudioClip p_clip, bool p_isLoop, float p_volume, GameObject p_gameObject ,  SoundLayerType p_layer)
        {
			return PlaySound(p_clip, p_isLoop, p_volume, p_layer, p_gameObject.transform );
        }

#if SKIP_CODE_IN_DOCUMENTATION
        /// @endcond
#endif

        #endregion

        #region Layers Management
        /// <summary>
        /// Get volume of the desired  Vox.SoundLayerType.
        /// </summary>
        /// <remarks>
        /// Get the current volume which is used to control all the sounds of a Vox.SoundLayerType.
        /// 
        /// <code>
        /// 
        /// Vox.SoundNode _backgroundMusic;
        /// public AudioClip _clip;
        /// 
        /// public void PlayMusic()
        /// {
        /// 	_backgroundMusic = Vox.Sound.Play2DSound(_clip, true, Vox.SoundLayerType.MUSIC);
        /// }
        /// 
        /// public float GetMusicVolume()
        /// {
        /// 	return Vox.Sound.GetLayerVolume(Vox.SoundLayerType.MUSIC);
        /// }
        /// 
        /// </code>
        /// 
        /// </remarks>  
        public static float GetLayerVolume(SoundLayerType p_layer)
        {
            return SoundModule.instance.GetLayerVolume( p_layer );
        }

        /// <summary>
        /// Set the volume of the desired  Vox.SoundLayerType.
        /// </summary>
        /// <remarks>
        /// Set the current volume which is used to control all the sounds of a Vox.SoundLayerType.
        /// 
        /// <code>
        /// 
        /// Vox.SoundNode _backgroundMusic;
        /// public AudioClip _clip;
        /// 
        /// public void PlayMusic()
        /// {
        /// 	_backgroundMusic = Vox.Sound.Play2DSound(_clip, true, Vox.SoundLayerType.MUSIC);
        /// }
        /// 
        /// public float MuteMusic()
        /// {
        /// 	return Vox.Sound.SetLayerVolume(Vox.SoundLayerType.MUSIC, 0);
        /// }
        /// 
        /// </code>
        /// 
        /// </remarks>  
        public static void SetLayerVolume(SoundLayerType p_layer , float _volume)
        {
            SoundModule.instance.SetLayerVolume( p_layer , _volume );
        }

        /// <summary>
        /// Resume all the sounds of a Vox.SoundLayerType if they were paused.
        /// </summary>
        /// <remarks>
        /// Resume  all the sounds of a Vox.SoundLayerType if they were paused.
        /// Use that instead of muting sounds if the game is paused or something similar, generally more useful for fx than music.
        /// 
        /// <code>
        /// 
        /// Vox.SoundNode _screamFXNode;
        /// public AudioClip _clip;
        /// 
        /// public void PlayScream()
        /// {
        /// 	_screamFXNode = Vox.Sound.Play2DSound(_clip, true, Vox.SoundLayerType.GAMEPLAY_FX);
        /// }
        /// 
        /// public void PauseGame()
        /// {
        /// 	Vox.Sound.PauseLayerSounds(Vox.SoundLayerType.GAMEPLAY_FX);
        /// }
        /// 
        /// public void ResumeGame()
        /// {
        /// 	Vox.Sound.ResumeLayerSounds(Vox.SoundLayerType.GAMEPLAY_FX);
        /// }
        /// 
        /// </code>
        /// 
        /// </remarks>  
        public static void ResumeLayerSounds(SoundLayerType p_layer)
        {
            SoundModule.instance.ResumeLayerSounds( p_layer );
        }

        /// <summary>
        /// Pauses all the sounds of a Vox.SoundLayerType if they were playing.
        /// </summary>
        /// <remarks>
        /// Pauses all the sounds of a Vox.SoundLayerType if they were playing.
        /// Use that instead of muting sounds if the game is paused or something similar, generally more useful for fx than music.
        /// 
        /// <code>
        /// 
        /// Vox.SoundNode _screamFXNode;
        /// public AudioClip _clip;
        /// 
        /// public void PlayScream()
        /// {
        /// 	_screamFXNode = Vox.Sound.Play2DSound(_clip, true, Vox.SoundLayerType.GAMEPLAY_FX);
        /// }
        /// 
        /// public void PauseGame()
        /// {
        /// 	Vox.Sound.PauseLayerSounds(Vox.SoundLayerType.GAMEPLAY_FX);
        /// }
        /// 
        /// public void ResumeGame()
        /// {
        /// 	Vox.Sound.ResumeLayerSounds(Vox.SoundLayerType.GAMEPLAY_FX);
        /// }
        /// 
        /// </code>
        /// 
        /// </remarks>  
        public static void PauseLayerSounds(SoundLayerType p_layer)
        {
            SoundModule.instance.PauseLayerSounds( p_layer );
        }

        /// <summary>
        /// Stop all the sounds of a Vox.SoundLayerType, removing them.
        /// </summary>
        /// <remarks>
        /// Stop all the sounds of a Vox.SoundLayerType, removing them.
        /// 
        /// <code>
        /// 
        /// public AudioClip _clip;
        /// 
        /// public void PlayMonsterNoise(GameObject p_monster)
        /// {
        /// 	Vox.Sound.Play2DSound(_clip, p_monster, false, Vox.SoundLayerType.GAMEPLAY_FX);
        /// }
        /// 
        /// public void RemoveAllGameplayFX()
        /// {
        /// 	Vox.Sound.StopLayerSounds(Vox.SoundLayerType.GAMEPLAY_FX);
        /// }
        /// 
        /// </code>
        /// 
        /// </remarks>  
        public static void StopLayerSounds(SoundLayerType p_layer)
        {
            SoundModule.instance.StopLayerSounds( p_layer );
        }
        #endregion
    }
}