// Copyright(c) Pixel Crushers.All rights reserved.

using UnityEngine;
using CrazyMinnow.SALSA;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{

    /// <summary>
    /// Sequencer command EyesRandom([head], [eye], [blink], [subject])
    /// 
    /// - head: <c>on|off</c>. Omit to turn leave head setting unchanged.
    /// - eye: <c>on|off</c>. Omit to turn leave eye setting unchanged.
    /// - blink: <c>on|off</c>. Omit to leave blink setting unchanged.
    /// - subject: The GameObject with the Eyes component. Default: speaker.
    /// </summary>
    public class SequencerCommandEyesRandom : SequencerCommand
    {

        public void Start()
        {
            var setHead = !string.IsNullOrEmpty(GetParameter(0));
            var headValue = string.Equals(GetParameter(0), "on", System.StringComparison.OrdinalIgnoreCase) || string.Equals(GetParameter(0), "true", System.StringComparison.OrdinalIgnoreCase);
            var setEyes = !string.IsNullOrEmpty(GetParameter(1));
            var eyesValue = string.Equals(GetParameter(1), "on", System.StringComparison.OrdinalIgnoreCase) || string.Equals(GetParameter(1), "true", System.StringComparison.OrdinalIgnoreCase);
            var setBlink = !string.IsNullOrEmpty(GetParameter(2));
            var blinkValue = string.Equals(GetParameter(2), "on", System.StringComparison.OrdinalIgnoreCase) || string.Equals(GetParameter(2), "true", System.StringComparison.OrdinalIgnoreCase);
            var subject = GetSubject(3, speaker);
            var eyes = (subject != null) ? subject.GetComponentInChildren<Eyes>() : null;
            if (eyes == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning("Dialogue System: Sequencer: EyesRandom(" + GetParameters() + ") command: No Eyes component found on subject " + subject, subject);
            }
            else
            {
                if (DialogueDebug.LogInfo) Debug.Log("Dialogue System: Sequencer: EyesRandom(" +
                    GetParamText("head", setHead, headValue) + ", " +
                    GetParamText("eyes", setEyes, eyesValue) +
                    GetParamText("blink", setBlink, blinkValue) + ", " + subject, subject);
                if (setHead) eyes.headRandom = headValue;
                if (setEyes) eyes.eyeRandom = eyesValue;
                if (setBlink) eyes.blinkRandom = blinkValue;
            }
            Stop();
        }

        private string GetParamText(string paramName, bool set, bool value)
        {
            return set ? (paramName + "=" + value) : (paramName + "=(don't set)");
        }
    }
}
