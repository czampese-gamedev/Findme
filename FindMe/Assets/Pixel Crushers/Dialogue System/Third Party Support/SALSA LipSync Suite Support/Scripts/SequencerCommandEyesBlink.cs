// Copyright(c) Pixel Crushers.All rights reserved.

using UnityEngine;
using System.Collections;
using CrazyMinnow.SALSA;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{

    /// <summary>
    /// Sequencer command EyesBlink([durationOn], [durationHold], [durationOff], [subject], [nowait])
    /// 
    /// - durationOn: Duration to keep eyes open before blinking. Omit for default blink.
    /// - durationHold: Duration to keep eyes closed while blinking. Omit for default blink.
    /// - durationOff: Duration to keep eyes open after blinking. Omit for default blink.
    /// - subject: The GameObject with the Eyes component. Default: speaker.
    /// - nowait: If specified, don't wait for blink sequence to finish.
    /// </summary>
    public class SequencerCommandEyesBlink : SequencerCommand
    {

        public IEnumerator Start()
        {
            var useCustomDuration = !(string.IsNullOrEmpty(GetParameter(0)) && string.IsNullOrEmpty(GetParameter(0)) && string.IsNullOrEmpty(GetParameter(2)));
            var durationOn = GetParameterAsFloat(0);
            var durationHold = GetParameterAsFloat(1);
            var durationOff = GetParameterAsFloat(2);
            var subject = GetSubject(3, speaker);
            var nowait = string.Equals(GetParameter(5), "nowait", System.StringComparison.OrdinalIgnoreCase);
            var eyes = (subject != null) ? subject.GetComponentInChildren<Eyes>() : null;
            if (eyes == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning("Dialogue System: Sequencer: EyesBlink(" + GetParameters() + ") command: No Eyes component found on " + subject, subject);
            }
            else
            {
                if (DialogueDebug.LogInfo)
                {
                    if (useCustomDuration) Debug.Log("Dialogue System: Sequencer: EyesBlink(on=" + durationOn + ", hold=" + durationHold + ", off=" + durationOff + ", " + subject + ", nowait=" + nowait, subject);
                    else Debug.Log("Dialogue System: Sequencer: EyesBlink(on=default, hold=default, off=default, " + subject + ", nowait=" + nowait, subject);
                }

                if (!useCustomDuration)
                {
                    foreach (EyesExpression exp in eyes.eyes)
                    {
                        for (int com = 0; com < exp.expData.components.Count; com++)
                        {
                            if (exp.expData.components[com].durationOn > durationOn)
                                durationOn = exp.expData.components[com].durationOn;
                            if (exp.expData.components[com].durationHold > durationHold)
                                durationHold = exp.expData.components[com].durationHold;
                            if (exp.expData.components[com].durationOff > durationOff)
                                durationOff = exp.expData.components[com].durationOff;
                        }
                    }
                }


                if (useCustomDuration)
                    eyes.NewBlink(durationOn, durationHold, durationOff);
                else
                    eyes.NewBlink();

                if (!nowait) yield return new WaitForSeconds(durationOn + durationHold + durationOff);
            }
            Stop();
        }

    }

}
