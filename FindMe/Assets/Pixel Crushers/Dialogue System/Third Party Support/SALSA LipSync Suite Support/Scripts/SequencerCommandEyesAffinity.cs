// Copyright(c) Pixel Crushers.All rights reserved.

using UnityEngine;
using CrazyMinnow.SALSA;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{

    /// <summary>
    /// Sequencer command EyesAffinity([on|off], [percentage], [timerMin], [timerMax], [subject])
    /// 
    /// - on|off: Set affinity on or off. Default: on.
    /// - percentage: Affinity percentage out of 1.0. Default: 0.75.
    /// - timerMin: Timer min in seconds. Default: 2.
    /// - timerMax: Timer max in seconds. Default: 5.
    /// - subject: The GameObject with the Eyes component. Default: speaker.
    /// </summary>
    public class SequencerCommandEyesAffinity: SequencerCommand
    {

        public void Start()
        {
            var setAffinity = string.Equals(GetParameter(0), "on", System.StringComparison.OrdinalIgnoreCase) || string.Equals(GetParameter(0), "true", System.StringComparison.OrdinalIgnoreCase);
            var percentage = GetParameterAsFloat(1, 0.5f);
            var timerMin = GetParameterAsFloat(2, 2f);
            var timerMax = GetParameterAsFloat(3, 5f);
            var subject = GetSubject(4, speaker);
            var eyes = (subject != null) ? subject.GetComponentInChildren<Eyes>() : null;
            if (eyes == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning("Dialogue System: Sequencer: EyesAffinity(" + GetParameters() + ") command: No Eyes component found on subject " + subject, subject);
            }
            else
            {
                if (DialogueDebug.LogInfo)
                {
                    if (setAffinity) Debug.Log("Dialogue System: Sequencer: EyesAffinity(" +
                    (setAffinity ? "on" : "off") + ", percentage=" + percentage + ", timerMin=" + timerMin +
                    ", timerMax=" + timerMax + ", " + subject, subject);
                    else Debug.Log("Dialoge System: Sequencer: EyesAffinity(off,-,-,-," + subject, subject);
                }
                eyes.useAffinity = setAffinity;
                eyes.affinityPercentage = percentage;
                eyes.affinityTimerRange = new Vector2(timerMin, timerMax);
            }
            Stop();
        }
    }
}
