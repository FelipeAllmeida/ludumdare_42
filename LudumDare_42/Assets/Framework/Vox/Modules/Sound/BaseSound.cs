using UnityEngine;

using VoxInternal;

namespace Vox
{
    /// <summary>
    /// Base class that has functions available both for MixedSound and Sound classes.
    /// </summary>
    /// <remarks>
    /// Base class that has functions available both for MixedSound and Sound classes.
    /// Do not use it directly, give preference to the type of sound used in the project.
    /// </remarks> 
    public abstract class BaseSound
    {
        /// <summary>
        /// Resume all paused sound nodes.
        /// </summary>
        /// <remarks>
        /// Resume all paused sound nodes. 
        /// This is indepent of whether the node was created with Sound or MixedSound.
        /// </remarks>
        public static void ResumeAllSounds()
        {
            SoundModule.instance.ResumeAllSounds();
        }

        /// <summary>
        /// Pause all playing sound nodes.
        /// </summary>
        /// <remarks>
        /// Pause all playing sound nodes.
        /// This is indepent of whether the node was created with Sound or MixedSound.
        /// </remarks>
        public static void PauseAllSounds()
        {
            SoundModule.instance.PauseAllSounds();
        }

        /// <summary>
        /// Stond and remove all sound nodes.
        /// </summary>
        /// <remarks>
        /// Stond and remove all sound nodes.
        /// This is indepent of whether the node was created with Sound or MixedSound.
        /// </remarks>
        public static void StopAllSounds()
        {
            SoundModule.instance.StopAllSounds();
        }

        /// <summary>
        /// Mute all sounds.
        /// </summary>
        /// <remarks>
        /// Mute all sounds. This is accomplished by pausing the current AudioListener.
        /// This is indepent of whether the node was created with Sound or MixedSound.
        /// </remarks>
        public static void MuteAllSounds()
        {
            AudioListener.pause = true;
        }

        /// <summary>
        /// UnMute all sounds.
        /// </summary>
        /// <remarks>
        /// UnMute all sounds. This is accomplished by resuming the current AudioListener.
        /// This is indepent of whether the node was created with Sound or MixedSound.
        /// </remarks>
        public static void UnMuteAllSounds()
        {
            AudioListener.pause = false;
        }

        /// <summary>
        /// Clear all sound nodes
        /// </summary>
        /// <remarks>
        /// ClearAllNodes should be called to clean all sound nodes.
        /// This is indepent of whether the node was created with Sound or MixedSound.
        /// Useful to make sure nothing is left behind when not intended.
        /// 
        /// <code>
        /// 
        /// Vox.SoundNode _monsterSnore;
        /// public GameObject _monster;
        /// public AudioClip _clip;
        /// 
        /// public void MonsterWentToSleep()
        /// {
        /// 	_monsterSnore = Vox.Sound.Play3DSound(_clip, _monster, true, Vox.SoundLayerType.GAMEPLAY_FX);
        /// }
        /// 		
        /// void override OnUnloadedScene()
        /// {
        /// 	Vox.Sound.ClearAllNodes();
        /// 	//Or Vox.MixedSound.ClearAllNodes();
        /// }
        /// 
        /// 
        /// </code>
        ///  
        /// </remarks>  
        public static void ClearAllNodes()
        {
            SoundModule.instance.ClearAllNodes();
        }

        /// <summary>
        /// Clear all sound nodes with passed tag.
        /// </summary>
        /// <remarks>
        /// ClearAllNodes should be called to clean all sound which had the passed tag attached to them. For more information check Vox.SoundNode.tag.
        /// This is indepent of whether the node was created with Sound or MixedSound.
        /// Useful to make sure nothing is left behind when not intended.
        /// 
        /// <code>
        /// 
        /// Vox.SoundNode _monsterSnore;
        /// public GameObject _monster;
        /// public AudioClip _clip;
        /// 
        /// public void MonsterWentToSleep()
        /// {
        /// 	_monsterSnore = Vox.Sound.Play3DSound(_clip, _monster, true, Vox.SoundLayerType.GAMEPLAY_FX);
        /// 	_monsterSnore.tag = "GameScene";
        /// }
        /// 		
        /// void override OnUnloadedScene()
        /// {
        /// 	Vox.Sound.ClearNodesWithTag("GameScene");
        /// 	//Or Vox.MixedSound.ClearNodesWithTag("GameScene");
        /// }
        /// 
        /// 
        /// </code>
        ///  
        /// </remarks> 
        public static void ClearNodesWithTag(string p_tag)
        {
            SoundModule.instance.ClearNodesWithTag(p_tag);
        }

        /// <summary>
        /// Clear all sound nodes except for the ones with the passed tag.
        /// </summary>
        /// <remarks>
        /// ClearAllNodes should be called to clean all sounds except for the ones which had the passed tag attached to them. For more information check Vox.SoundNode.tag.
        /// This is indepent of whether the node was created with Sound or MixedSound.
        /// Useful to make sure nothing is left behind when not intended.
        /// 
        /// <code>
        /// 
        /// void override OnUnloadedScene()
        /// {
        /// 	Vox.Sound.ClearNodesExceptForTag("GlobalNodes");
        /// 	//Vox.MixedSound.ClearNodesExceptForTag("GlobalNodes");
        /// }
        /// 
        /// 
        /// </code>
        ///  
        /// </remarks> 
        public static void ClearNodesExceptForTag(string p_tag)
        {
            SoundModule.instance.ClearNodesExceptForTag(p_tag);
        }
    }
}