using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Vox;

namespace VoxInternal
{
	[Serializable]
	public class SoundClipData
	{
		public string id = "";
		public AudioClip clip = null;
		public float volume = 1f;
		public bool isLoop = false;
		public bool isMixedAudio = true; 
		public AudioMixerGroup mixerGroup = null;
		public bool expandedInfo = true;
	}

	/// <summary>
	/// Class which stores required Sound Module information via ScriptableObject.
	/// </summary>
	[Serializable]
	public class EditorSoundData : ScriptableObject
	{
		public bool enableSoundInEditor = true;
		public List<AudioMixer>mixersList = new List<AudioMixer>();

		public List<SoundClipData>soundClipList = new List<SoundClipData>();
	}
}