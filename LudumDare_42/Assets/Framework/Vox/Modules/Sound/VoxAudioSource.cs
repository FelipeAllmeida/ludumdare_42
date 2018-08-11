using UnityEngine;

namespace VoxInternal
{
    public enum AudioSourceState
    {
        FREE,

        USED
    }

    public class VoxAudioSource : MonoBehaviour
    {
        public AudioSourceState state;

        public Transform targetTransform;

        private AudioSource _audioSource;

        public AudioSource audioSource
        {
            get
            {
                return _audioSource;
            }

            set
            {
                _audioSource = value;
            }
        }

        public void Update()
        {
            if (targetTransform != null)
            {
                transform.position = targetTransform.position;
            }
        }

        public void Clear()
        {
            state = AudioSourceState.FREE;
            audioSource.Stop();
            transform.localPosition = new Vector3(0, 0, 0);
            targetTransform = null;
        }

        public void CopyAudioProperties(AudioSource p_source)
        {
            audioSource.rolloffMode = p_source.rolloffMode;
            audioSource.minDistance = p_source.minDistance;
            audioSource.maxDistance = p_source.maxDistance;
            audioSource.spread = p_source.spread;
            audioSource.spatialBlend = p_source.spatialBlend;
            audioSource.dopplerLevel = p_source.dopplerLevel;
            audioSource.outputAudioMixerGroup = p_source.outputAudioMixerGroup;
            audioSource.priority = p_source.priority;
            audioSource.spatialize = p_source.spatialize;
            audioSource.volume = p_source.volume;
            audioSource.pitch = p_source.pitch;
            audioSource.playOnAwake = p_source.playOnAwake;
            audioSource.spatializePostEffects = p_source.spatializePostEffects;
            audioSource.spatialize = p_source.spatialize;
            audioSource.reverbZoneMix = p_source.reverbZoneMix;
        }
    }
}