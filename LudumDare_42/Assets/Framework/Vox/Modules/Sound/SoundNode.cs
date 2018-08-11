#define SKIP_CODE_IN_DOCUMENTATION
using System;

using UnityEngine;

using VoxInternal;

namespace Vox
{
    using UnityEngine.Audio;

    /// <summary>
    /// Instance of a generated sound.
    /// </summary>
    /// <remarks>
    /// A SoundNode is a structure that stores a sound and its associated parameters, such as the AudioSource related to that sound.
    /// It is also very useful for allowing organization with sound tasks, grouping them with layers or mixers.
    /// 
    /// <code>
    /// 
    /// Audioclip _endLevelFX;
    /// 
    /// Vox.SoundNode __endMusicLevel = Vox.MixedSound.Play2DSound(_endLevelFX, false, "GameplayMixer", "FX");
    /// 
    /// __endMusicLevel.onFinishedPlaying += delegate()
    /// {
    /// 	ShowEndLevelResultsScreen();
    /// };
    /// 
    /// </code>
    /// 
    /// </remarks> 
    public class SoundNode
    {
        #region Private Internal Only
#if SKIP_CODE_IN_DOCUMENTATION
        /// @cond
#endif
        private VoxAudioSource _voxSource;
		public SoundLayerType soundLayer;

        private bool _isPaused = false;

        private int _id;
        private float _nodeVolume;

        #region Constructor

        /// <summary>
        /// Constructor of Vox.SoundNode. Used internally by the MixedSound class to create the sound, do not use it.
        /// </summary>
        /// <remarks>
        /// Constructor of Vox.SoundNode. Used internally by the MixedSound class to create the sound, do not use it. To create SoundNode use the MixedSound class.		
        /// </remarks>
        public SoundNode(VoxAudioSource p_voxSource, AudioClip p_clip, bool p_loop, float p_volume, string p_parentMixerName, string p_outgroupName, int p_id)
        {
            this._id = p_id;
            this._nodeVolume = p_volume;
            this._voxSource = p_voxSource;
            this._voxSource.audioSource.clip = p_clip;
            this._voxSource.audioSource.loop = p_loop;
            this._voxSource.audioSource.volume = p_volume;

            foreach (AudioMixer __mixer in SoundModule.instance.listAudioMixers)
            {
                if (__mixer.name == p_parentMixerName)
                {
                    this._voxSource.audioSource.outputAudioMixerGroup = __mixer.FindMatchingGroups(p_outgroupName)[0];
                    break;
                }
            }
        }

		/// <summary>
		/// Constructor of Vox.SoundNode. Used internally by the MixedSound class to create the sound, do not use it.
		/// </summary>
		/// <remarks>
		/// Constructor of Vox.SoundNode. Used internally by the MixedSound class to create the sound, do not use it. To create SoundNode use the MixedSound class.		
		/// </remarks>
		public SoundNode(VoxAudioSource p_voxSource, AudioClip p_clip, bool p_loop, float p_volume, SoundLayerType p_layer,  int p_id)
		{
			this._id = p_id;
            this._nodeVolume = p_volume;
            this._voxSource = p_voxSource;
			this._voxSource.audioSource.clip = p_clip;
			this._voxSource.audioSource.loop = p_loop;
			this._voxSource.audioSource.volume = p_volume;
			this.soundLayer = p_layer;
		}
        #endregion

#if SKIP_CODE_IN_DOCUMENTATION

        /// @endcond
#endif
        #endregion

        #region Public Action Data

        /// <summary>
        /// Action that is called when sound has finished playing.
        /// </summary>
        /// <remarks>
        /// Action that is called when sound has finished playing. Very useful for chaining events with sounds.
        /// 
        /// Example below:
        /// <code>
        /// 
        /// Audioclip _endLevelFX;
        /// 
        /// Vox.SoundNode __endMusicLevel = Vox.MixedSound.Play2DSound(_endLevelFX, false, "GameplayMixer", "FX");
        /// 
        /// __endMusicLevel.onFinishedPlaying += delegate()
        /// {
        /// 	ShowEndLevelResultsScreen();
        /// };
        /// 
        /// </code>
        /// 
        /// </remarks>
        public Action onFinishedPlaying;
        #endregion

        #region Public Data

        /// <summary>
        /// Tag which can be used later to identificate and relate this sounds to others. Allowing them to be cleaned separedly if required.
        /// </summary>
        /// <remarks>
        /// Tags are useful to be set when you are creating sounds that should stay even if their a scene is unloaded, or if you are using several sounds related to a state of a state machine and want to clear them easily when changing a state.
        /// You can use the tags manually in your own or use the functions Vox.Sound.ClearNodesExceptForTag() or Vox.MixedSound.ClearNodesWithTag().
        /// 
        /// Example of a tag being set is show below:
        /// 
        /// <code>
        /// // Script inside a state of a statemachine
        /// 
        /// Vox.SoundNode _monsterSnore;
        /// public GameObject _monster;
        /// public AudioClip _clip;
        /// 
        /// public void MonsterWentToSleep()
        /// {
        /// 	_monsterSnore = Vox.MixedSound.Play3DSound(_clip, _monster, true, "GameplayMixer", "EnemiesFX");
        /// 	_monsterSnore.tag = "GameplayState";
        /// }
        /// 
        ///
        /// void override StateOnUnloadedScene()
        /// {
        /// 	Vox.Tween.ClearNodesWithTag("GameplayState");
        /// }
        /// 
        /// </code>
        /// 
        /// </remarks>
        public string tag = string.Empty;
        #endregion

        #region Private Get-Only Data

        /// <summary>
        /// Unique id among others SoundNodes. Used for debug and get only.
        /// </summary>
        /// <remarks>
        /// Unique id among others SoundNodes. Used for debug and get only.
        /// </remarks>
        public int id
        {
            get
            {
                return _id;
            }
        }

        public bool isPlaying
        {
            get
            {
                return this._voxSource.audioSource.isPlaying;
            }
        }

        public float audioSourceVolume
        {
            get
            {
                return this._voxSource.audioSource.volume;
            }

            set
            {
                this._voxSource.audioSource.volume = value;
            }
        }

        public float volume
        {
            get
            {
                return this._nodeVolume;
            }
            set
            {
                this._nodeVolume = value;
                this._voxSource.audioSource.volume = this._nodeVolume * SoundModule.instance.GetLayerVolume(this.soundLayer);
            }
        }

        public float pitch
        {
            get
            {
                return this._voxSource.audioSource.pitch;
            }

            set
            {
                this._voxSource.audioSource.pitch = value;
            }
        }

        /// <summary>
        /// Returns the AudioSource associated with the SoundNode.
        /// </summary>
        /// <remarks>
        /// Returns the AudioSource associated with the SoundNode.
        /// For more information check: <https://docs.unity3d.com/ScriptReference/AudioSource.html>
        /// </remarks>
        public VoxAudioSource VoxVoxSource
        {
            get
            {
                return this._voxSource;
            }
        }
        #endregion

        #region Node Control
        public bool IsFinished()
        {
            if (this._voxSource.audioSource.clip == null)
            {
                return true;
            }

            if (this._voxSource.audioSource.clip.loadState != AudioDataLoadState.Loading)
            {
                return this._voxSource.audioSource.isPlaying == false && this._isPaused == false;
            }

            return false;
        }

        public void Update()
        {
            this._voxSource.Update();
        }

        /// <summary>
        /// Resume soundNode if previously paused.
        /// </summary>
        /// <remarks>
        /// Resume soundNode if previously paused.
        /// </remarks>
        public void Resume()
        {
            this._isPaused = false;

            if (this._voxSource.audioSource != null)
            {
                this._voxSource.audioSource.Play();
            }
        }

        /// <summary>
        /// Pause soundNode if playing.
        /// </summary>
        /// <remarks>
        /// Pause soundNode if playing.
        /// </remarks>
        public void Pause()
        {
            this._isPaused = true;

            if (this._voxSource.audioSource != null)
            {
                this._voxSource.audioSource.Pause();
            }
        }

        public bool IsPaused()
        {
            return this._isPaused;
        }

        /// <summary>
        /// Force the sound to stop but calling its onFinish callback if exists.
        /// </summary>
        /// <remarks>
        /// Force the sound to stop but calling its onFinish callback if exists.
        /// </remarks>
        public void Anticipate()
        {
            if (this.onFinishedPlaying != null)
                this.onFinishedPlaying();

            this.Cancel();
        }

        /// <summary>
        /// Force the sound to stop without calling any callback.
        /// </summary>
        /// <remarks>
        /// Force the sound to stop without calling any callback.  Same as SoundNode.Cancel();
        /// </remarks>
        public void Stop()
        {
            this._isPaused = false;

            SoundModule.instance.RemoveNode(this.id);
        }

        /// <summary>
        /// Force the sound to stop without calling any callback.
        /// </summary>
        /// <remarks>
        /// Force the sound to stop without calling any callback. Same as SoundNode.Cancel();
        /// </remarks>
        public void Cancel()
        {
            this.Stop();
        }

        public void Clear()
        {
            this._voxSource.Clear();
            this.onFinishedPlaying = null;
        }

        #endregion
    }
}