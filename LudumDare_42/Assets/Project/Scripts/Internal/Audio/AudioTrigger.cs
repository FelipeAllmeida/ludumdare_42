using System;
using System.Collections.Generic;
using UnityEngine;
using Vox;

namespace Internal.Audio
{
    [Serializable]
    public class AudioTrigger
    {
        public Tags tag;
        public bool isLoop;
        public SoundLayerType layer;
        public List<AudioClip> listClips;

        public SoundNode Node { get; private set; }

        public AudioClip GetRandomClip() => listClips[UnityEngine.Random.Range(0, listClips.Count - 1)];

        public void SetNode(SoundNode p_node) => Node = p_node;
    }
}

