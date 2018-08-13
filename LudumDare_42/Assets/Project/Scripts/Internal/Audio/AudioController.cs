using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vox;

namespace Internal.Audio
{
    public enum Tags
    {
        Ambiente_WaterFlooding,
        Ambience_WaterFillingAdjacent,

        Background_Menu,
        Background_Game,

        SFX_Mouse_Click,
        SFX_Interact_Door,
        SFX_Interact_ControlPanel,
        SFX_Interact_MainFrame,
        SFX_Interact_Player,
        SFX_Interact_WaterPump,
        SFX_Interact_Generator,
        SFX_Mouse_Click_Button
    }

    public class AudioController : MonoBehaviour
    {
        [Header("References")]
        public List<AudioTrigger> _listReferenceAudioTriggers;

        [Header("Configuration")]
        [SerializeField] private float _volumeBackground;
        [SerializeField] private float _volumeAmbience;
        [SerializeField] private float _volumeGameSFX;
        [SerializeField] private float _volumeUserInterfaceSFX;

        private Dictionary<Tags, AudioTrigger> _dictAudioTriggers = new Dictionary<Tags, AudioTrigger>();

        public static AudioController Instance { get; private set; }

	    // Use this for initialization
	    void Start ()
        {
            Instance = this;

            _listReferenceAudioTriggers.ForEach(x => _dictAudioTriggers.Add(x.tag, x));
        }
	
        public void Play(Tags p_tag)
        {
            if (!_dictAudioTriggers.ContainsKey(p_tag))
            {
                Debug.LogWarning($"Trigger for tag '{p_tag}' does not exist.");
                return;
            }

            AudioTrigger __trigger = _dictAudioTriggers[p_tag];

            __trigger.Node?.Cancel();
            SoundNode __node = Sound.Play2DSound(__trigger.GetRandomClip(), __trigger.isLoop, GetVolumeForLayer(__trigger.layer), __trigger.layer);
            __trigger.SetNode(__node);
        }

        public void Stop(Tags p_tag)
        {
            if (!_dictAudioTriggers.ContainsKey(p_tag))
            {
                Debug.LogWarning($"Trigger for tag '{p_tag}' does not exist.");
                return;
            }

            AudioTrigger __trigger = _dictAudioTriggers[p_tag];

            __trigger.Node?.Cancel();
        }

        private float GetVolumeForLayer(SoundLayerType p_layer)
        {
            switch(p_layer)
            {
                case SoundLayerType.AMBIENCE:
                    return _volumeAmbience;
                case SoundLayerType.GAMEPLAY_FX:
                    return _volumeGameSFX;
                case SoundLayerType.HUD_FX:
                    return _volumeUserInterfaceSFX;
                case SoundLayerType.MUSIC:
                    return _volumeBackground;
                default:
                    return 1f;
            }
        }
    }
}

