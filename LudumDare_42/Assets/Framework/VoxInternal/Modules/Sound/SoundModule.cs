using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using Vox;

namespace VoxInternal
{
    using System.Linq;

    /// <summary>
    /// Module which updates Sound functions from a list of SoundNodes.
    /// </summary>
    public class SoundModule : Module
    {
        private const int AudioSourceInitialBuffer = 30;

        #region Public Static Data
        /// <summary>
        /// Create automatically the unique instance of this class.
        /// </summary>
        private static SoundModule _instance;

        public static SoundModule instance
        {
            get
            {
                return _instance ?? (_instance = InstanceInitialize());
            }
        }
        #endregion

        #region Public Data
        public List<AudioMixer> listAudioMixers;
        private List<VoxAudioSource> listVoxSources = new List<VoxAudioSource>();
        public Dictionary<AudioModelType, AudioSource> _dictAudioSourceModels = new Dictionary<AudioModelType, AudioSource>( );

        private Transform audioSourceParent;
        private bool doesIncreaseAudioSourceOnDemand = false;
        #endregion

        #region Private Get-Only Data
        private List<SoundNode> _currentNodeList = new List<SoundNode>();

        public List<SoundNode> currentNodeList
        {
            get { return _currentNodeList; }
        }
        #endregion

        #region Private Data
        private float[] _arraySoundLayerVolumes;
        #endregion

        #region Singleton Instance Initialization
        /// <summary>
        /// Handles the logic required in the inicialization of the instance.
        /// </summary>
        static private SoundModule InstanceInitialize()
        {
            SoundModule __instance = new SoundModule();
 
            __instance.InitializeLayers();
            __instance.InitializeAudioSources();
            __instance.InitializeAudioModels();

            ModuleCore.instance.AddModule(__instance);

            EditorSoundData _soundData = (EditorSoundData) Resources.Load("VoxSoundData/VoxSoundSavedData");

            __instance.listAudioMixers = _soundData.mixersList;
            return __instance;
        }
        #endregion

        #region Node Control
        /// <summary>
        /// Add SoundNode p_nodule to currentNodeList.
        /// </summary>
        public void AddNode(SoundNode p_nodule)
        {
            _currentNodeList.Add(p_nodule);
        }

        /// <summary>
        /// Remove SoundNode p_nodule to currentNodeList.
        /// </summary>
        public void RemoveNode(int p_id)
        {
            SoundNode __node = this.currentNodeList.FirstOrDefault<SoundNode>( t => t.id == p_id );

            if (__node != null)
            {
                __node.Clear();
                _currentNodeList.Remove( __node );
            }
        }

        /// <summary>
        /// Clear all nodes in the current list
        /// </summary>
        public override void ClearAllNodes()
        {
            for (int i = _currentNodeList.Count - 1; i >= 0; i--)
            {
                RemoveNode( _currentNodeList[i].id);
            }
        }

        /// <summary>
        /// Clear all nodes in the current list with the passed tag
        /// </summary>
        public override void ClearNodesWithTag(string p_tag)
        {
            for (int i = _currentNodeList.Count - 1; i >= 0; i--)
            {
                if (_currentNodeList[i] != null)
                {
                    if (_currentNodeList[i].tag == p_tag)
                    {
                        RemoveNode( _currentNodeList[i].id );
                    }
                }
            }
        }

        /// <summary>
        /// Clear all nodes in the current list except for the nodes with the passed tag
        /// </summary>
        public override void ClearNodesExceptForTag(string p_tag)
        {
            for (int i = _currentNodeList.Count - 1; i >= 0; i--)
            {
                if (_currentNodeList[i] != null)
                {
                    if (_currentNodeList[i].tag != p_tag)
                    {
                        RemoveNode( _currentNodeList[i].id );
                    }
                }
            }
        }

        /// <summary>
        /// Update all active TweenNode in currentNodeList.
        /// </summary>
        public override void ModuleUpdate()
        {
            for (int i = _currentNodeList.Count - 1; i >= 0; i--)
            {
                if (_currentNodeList[i] != null)
                {
                    if (_currentNodeList[i].IsFinished( ))
                    {
                        if (_currentNodeList[i].onFinishedPlaying != null)
                            _currentNodeList[i].onFinishedPlaying( );

                        RemoveNode( _currentNodeList[i].id );
                    }
                    else
                    {
                        _currentNodeList[i].Update( );
                    }
                }
                else
                {
                    _currentNodeList.RemoveAt(i);
                }
            }
        }
        #endregion

        #region General Audio Functions
        /// <summary>
        /// Resume all SoundNode in currentNodeList.
        /// </summary>
        public void ResumeAllSounds()
        {
            for (int i = 0; i < _currentNodeList.Count; i++)
            {
                _currentNodeList[i].Resume();
            }
        }

        /// <summary>
        /// Pause all SoundNode in currentNodeList.
        /// </summary>
        public void PauseAllSounds()
        {
            for (int i = 0; i < _currentNodeList.Count; i++)
            {
                _currentNodeList[i].Pause();
            }
        }

        /// <summary>
        /// Stop all SoundNode in currentNodeList.
        /// </summary>
        public void StopAllSounds()
        {
            for (int i = 0; i < _currentNodeList.Count; i++)
            {
                RemoveNode( _currentNodeList[i].id );
            }
        }
        #endregion

        #region MixerMode
        /// <summary>
        /// Resume all SoundNode in currentNodeList which belong to p_mixerGroup.
        /// </summary>
        public void ResumeMixerGroupSounds(string p_mixerName, string p_mixerGroup)
        {
            AudioMixerGroup[] __group = null;

            for (int i = 0; i < SoundModule.instance.listAudioMixers.Count; i++)
            {
                if (SoundModule.instance.listAudioMixers[i].name == p_mixerName)
                {
                    __group = SoundModule.instance.listAudioMixers[i].FindMatchingGroups(p_mixerGroup);
                    break;
                }
            }

            if (__group == null)
            {
                Debug.LogError("No Group with this name added to the mixer. Maybe you forgot to add it manually");
                return;
            }

            for (int i = 0; i < _currentNodeList.Count; i++)
            {
                for (int j = 0; j < __group.Length; j++)
                {
                    if (_currentNodeList[i].VoxVoxSource.audioSource.outputAudioMixerGroup == __group[j])
                    {
                        _currentNodeList[i].Resume();
                    }
                }
            }
        }

        /// <summary>
        /// Pause all SoundNode in currentNodeList which belong to p_mixerGroup.
        /// </summary>
        public void PauseMixerGroupSounds(string p_mixerName, string p_mixerGroup)
        {
            AudioMixerGroup __group = null;

            for (int i = 0; i < SoundModule.instance.listAudioMixers.Count; i++)
            {
                if (SoundModule.instance.listAudioMixers[i].name == p_mixerName)
                {
                    __group = SoundModule.instance.listAudioMixers[i].FindMatchingGroups(p_mixerGroup)[0];
                    break;
                }
            }

            if (__group == null)
            {
                Debug.LogError("No Group with this name added to Current Mixer. Maybe you forgot to add it manually");
                return;
            }

            for (int i = 0; i < _currentNodeList.Count; i++)
            {
                if (_currentNodeList[i].VoxVoxSource.audioSource.outputAudioMixerGroup == __group)
                {
                    _currentNodeList[i].Pause();
                }
            }
        }

        /// <summary>
        /// Stop all SoundNode in currentNodeList which belong to p_mixerGroup.
        /// </summary>
        public void StopMixerGroupSounds(string p_mixerName, string p_mixerGroup)
        {
            AudioMixerGroup __group = null;

            for (int i = 0; i < SoundModule.instance.listAudioMixers.Count; i++)
            {
                if (SoundModule.instance.listAudioMixers[i].name == p_mixerName)
                {
                    __group = SoundModule.instance.listAudioMixers[i].FindMatchingGroups(p_mixerGroup)[0];
                    break;
                }
            }

            if (__group == null)
            {
                Debug.LogError("No Group with this name added to Current Mixer. Maybe you forgot to add it manually");
                return;
            }

            for (int i = 0; i < _currentNodeList.Count; i++)
            {
                if (_currentNodeList[i].VoxVoxSource.audioSource.outputAudioMixerGroup == __group)
                {
                    RemoveNode( _currentNodeList[i].id );
                }
            }
        }
        #endregion

        #region NoMixerMode
        /// <summary>
        /// Initializing all audio layers
        /// </summary>
        private void InitializeLayers()
        {
            _arraySoundLayerVolumes = new float[10];

            for (int i = 0; i < _arraySoundLayerVolumes.Length; i++)
            {
                _arraySoundLayerVolumes[i] = 1;
            }
        }

		/// <summary>
		/// Get volume from p_layer.
		/// </summary>
		public float GetLayerVolume(SoundLayerType p_layer)
		{
			return _arraySoundLayerVolumes[(int) p_layer];
		}

		/// <summary>
		/// Set volume of p_layer.
		/// </summary>
		public void SetLayerVolume(SoundLayerType p_layer, float p_volume)
		{
			_arraySoundLayerVolumes[(int) p_layer] = p_volume;

			for (int i = 0; i < _currentNodeList.Count; i++)
			{
				if (_currentNodeList[i].soundLayer == p_layer)
				{
					_currentNodeList[i].audioSourceVolume = _currentNodeList[i].volume * p_volume;
				}
			}
		}

        /// <summary>
        /// Initializing all audio layers
        /// </summary>
        private void InitializeAudioSources()
        {
            this.listVoxSources = new List<VoxAudioSource>();

            GameObject __parent  = new GameObject("VoxAudioSources");
            audioSourceParent = __parent.transform;
            audioSourceParent.SetParent( GameCore.instance.transform );

            for (int i = 0; i < AudioSourceInitialBuffer; i++)
            {
                AddAudioSourceToList();
            }
        }

        /// <summary>
        /// Initializing all audio layers
        /// </summary>
        private void InitializeAudioModels()
        {
            this._dictAudioSourceModels = new Dictionary<AudioModelType, AudioSource>();

            this._dictAudioSourceModels.Add(AudioModelType.DEFAULT,Resources.Load<AudioSource>("VoxSoundData/DefaultSourceModel"));
        }

        public VoxAudioSource GetFreeVoxAudioSource(AudioModelType p_model = AudioModelType.DEFAULT)
        {
            VoxAudioSource __source = this.listVoxSources.FirstOrDefault(__t => __t.state == AudioSourceState.FREE) ?? this.AddAudioSourceToList();
            __source.state = AudioSourceState.USED;
            __source.CopyAudioProperties(_dictAudioSourceModels[p_model]);

            return __source;
        }

        private VoxAudioSource AddAudioSourceToList()
        {
            GameObject __instance = new GameObject( "VoxAudioSource " + this.listVoxSources.Count );
            __instance.transform.SetParent(audioSourceParent);

            VoxAudioSource __voxAudioSource = __instance.AddComponent<VoxAudioSource>( );
            __voxAudioSource.audioSource = __instance.AddComponent<AudioSource>( );
            this.listVoxSources.Add(__voxAudioSource);

            return __voxAudioSource;
        }

		/// <summary>
		/// Pause SoundNode in currentNodeList  f p_layer.
		/// </summary>
		public void PauseLayerSounds(SoundLayerType p_layer)
		{
			for (int i = 0; i < _currentNodeList.Count; i++)
			{
				if (_currentNodeList[i].soundLayer == p_layer)
				{
					_currentNodeList[i].Pause();
				}
			}
		}

		/// <summary>
		/// Resume SoundNode in currentNodeList  f p_layer.
		/// </summary>
		public void ResumeLayerSounds(SoundLayerType p_layer)
		{
			for (int i = 0; i < _currentNodeList.Count; i++)
			{
				if (_currentNodeList[i].soundLayer == p_layer)
				{
					_currentNodeList[i].Resume();
				}
			}
		}

		/// <summary>
		/// Stop SoundNode in currentNodeList  f p_layer.
		/// </summary>
		public void StopLayerSounds(SoundLayerType p_layer)
		{
			for (int i = 0; i < _currentNodeList.Count; i++)
			{
				if (_currentNodeList[i].soundLayer == p_layer)
				{
					RemoveNode( _currentNodeList[i].id );
				}
			}
		}
        #endregion
    }
}