// Copyright(c) Pixel Crushers.All rights reserved.

using UnityEngine;
using CrazyMinnow.SALSA;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{

    /// <summary>
    /// Sequencer command EyesLook([target], [subject])
    /// 
    /// - target: The GameObject to look at. Can also be <c>none</c>. Default: listener.
    /// - subject: The GameObject with the Eyes component. Default: speaker.
    /// </summary>
    public class SequencerCommandEyesLook : SequencerCommand
    {

        public void Start()
        {
            var target = GetSubject(0, listener);
            var subject = GetSubject(1, speaker);
            var eyes = (subject != null) ? subject.GetComponentInChildren<Eyes>() : null;
            if (eyes == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning("Dialogue System: Sequencer: EyesLook(" + GetParameters() + ") command: No Eyes component found on subject " + subject, subject);
            }
            else
            {
                if (DialogueDebug.LogInfo) Debug.Log("Dialogue System: Sequencer: EyesLook(target=" + target + ", subject=" + subject + ")", subject);
                eyes.lookTarget = target;
            }
            Stop();
        }

    }

}
