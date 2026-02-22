// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using CrazyMinnow.SALSA;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{

    /// <summary>
    /// Sequencer command SALSA(clip, [subject], [audioclips...]).
    /// 
    /// - clip: The path to an audio clip in a Resources folder.
    /// - subject: The GameObject with the Salsa component. Default: speaker.
    /// - Additional parameters are additional audio clips to play in sequence.
    /// </summary>
    /// <remarks>Since the Salsa component now works simply by watching
    /// audio clips on the subject, this command is just a wrapper for AudioWait().</remarks>
    public class SequencerCommandSALSA : SequencerCommandAudioWait
    {

        protected override AudioSource GetAudioSource(Transform subject)
        {
            var salsa = GetComponentInChildren<Salsa>();
            return (salsa != null && salsa.audioSrc != null) ? salsa.audioSrc : base.GetAudioSource(subject);
        }
    }
}
