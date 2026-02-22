// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections;
using CrazyMinnow.SALSA;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{

    /// <summary>
    /// Sequencer command Emote(emoteName, [on|off], [oneway|roundtrip], [duration], [subject], [nowait])
    /// 
    /// - emoteName: The emote name.
    /// - on|off: Turn the emote on or off. Default: on.
    /// - oneway|roundtrip: ExpressionHandler value. Default: oneway.
    /// - duration: Duration in seconds. Default: 2.
    /// - subject: The GameObject with the Emoter component. Default: speaker.
    /// - nowait: If specified, don't wait for emote to finish.
    /// </summary>
    public class SequencerCommandEmote : SequencerCommand
    {

        public IEnumerator Start()
        {
            var emoteName = GetParameter(0);
            var animateOn = !(string.Equals(GetParameter(1), "off", System.StringComparison.OrdinalIgnoreCase) || string.Equals(GetParameter(1), "false", System.StringComparison.OrdinalIgnoreCase));
            var handler = string.Equals(GetParameter(2), "roundtrip", System.StringComparison.OrdinalIgnoreCase) ? ExpressionComponent.ExpressionHandler.RoundTrip : ExpressionComponent.ExpressionHandler.OneWay;
            var duration = GetParameterAsFloat(3);
            var subjectTransform = GetSubject(4, speaker);
            var nowait = string.Equals(GetParameter(5), "nowait", System.StringComparison.OrdinalIgnoreCase);
            var emoter = (subjectTransform != null) ? subjectTransform.GetComponentInChildren<Emoter>() : null;
            if (emoter == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning("Dialogue System: Sequencer: Emote(" + GetParameters() + ") command: Can't find Emoter component on subject.");
            }
            else if (string.IsNullOrEmpty(emoteName))
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning("Dialogue System: Sequencer: Emote(" + GetParameters() + ") command: No emote name specified.");
            }
            else
            {
                if (DialogueDebug.LogInfo) Debug.Log("Dialogue System: Sequencer: Emote(" + GetParameters() + ")", emoter);

                foreach (EmoteExpression exp in emoter.emotes)
                {
                    for (int com = 0; com < exp.expData.components.Count; com++)
                    {
                        if (exp.expData.components[com].durationOn > duration)
                            duration = exp.expData.components[com].durationOn;
                        if (exp.expData.components[com].durationOff > duration)
                            duration = exp.expData.components[com].durationOff;
                    }
                }

                emoter.ManualEmote(emoteName, handler, duration, animateOn);
                if (!nowait)
                {
                    yield return new WaitForSeconds(duration);
                }

            }
            Stop();
        }

    }

}
