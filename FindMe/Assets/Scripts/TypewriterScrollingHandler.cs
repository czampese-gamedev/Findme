using System;

using Febucci.TextAnimatorCore.Text;
using Febucci.TextAnimatorForUnity;

using TMPro;

using UnityEngine;
using UnityEngine.UI;   

using UnityUtility;

public class TypewriterScrollingHandler : MonoBehaviour
{
    [SerializeField] private ScrollRect m_scrollRect;
    [SerializeField] private RectTransform m_textContainer;
    [SerializeField] private TypewriterComponent m_typewriterComponent;
    [SerializeField] private TMP_Text m_textComponent;

    [NonSerialized] private bool m_lockToBottom = false;

    private void Start()
    {
        m_typewriterComponent.onCharacterVisible.AddListener(UpdateScrollPosition);
        m_typewriterComponent.onTypewriterStart.AddListener(OnTypewriterStart);
        m_typewriterComponent.onTextShowed.AddListener(UpdateScrollPosition);

        m_scrollRect.onValueChanged.AddListener(OnManualScroll);
    }

    private void OnManualScroll(Vector2 scrollPosition)
    {
        m_lockToBottom = scrollPosition.y.Approximately(0.0f);
    }

    private void OnDestroy()
    {
        m_typewriterComponent.onCharacterVisible.RemoveListener(UpdateScrollPosition);
        m_typewriterComponent.onTypewriterStart.RemoveListener(OnTypewriterStart);
        m_typewriterComponent.onTextShowed.RemoveListener(UpdateScrollPosition);
        m_scrollRect.onValueChanged.RemoveListener(OnManualScroll);
    }

    private void OnTypewriterStart()
    {
    
        m_lockToBottom = true;
    }
    private void UpdateScrollPosition()
    {

        UpdateScrollPosition(m_typewriterComponent.TextAnimator.latestCharacterShown);
    }

    private void UpdateScrollPosition(CharacterData latestShownChar)
    {

        int characterLine = GetCharacterLine(latestShownChar);
        float shownTextHeight = GetShownTextHeight(characterLine);

        m_textContainer.sizeDelta = m_textContainer.sizeDelta.WhereY(shownTextHeight);


        if (m_lockToBottom)
        {
            m_scrollRect.verticalNormalizedPosition = 0.0f;
        }
    }

    private int GetCharacterLine(CharacterData characterData){

        TMP_TextInfo textInfo = m_textComponent.textInfo;

        if (characterData.index == 0 || characterData.index > textInfo.characterCount)
        {
            return 0;
        }

        int lineCount = textInfo.lineCount;

        for (int iLine = 0; iLine < lineCount; iLine++)
        {
            int first = textInfo.lineInfo[iLine].firstCharacterIndex;
            int last = textInfo.lineInfo[iLine].lastCharacterIndex;

            if (characterData.index.Between(first, last))
            {
                return iLine;
            }
        }
        Debug.LogError($"Failed to get line of char at index {characterData.index}");
        return -1;
    }

    private float GetShownTextHeight(int lastShownLine)
    {

        float lineSpacing = m_textComponent.lineSpacing * m_textComponent.fontSize * 0.01f;

        float heightSum = m_textComponent.margin.y; // Top
        for (int iLine = 0; iLine < lastShownLine + 1; iLine++)
        {
            heightSum += m_textComponent.textInfo.lineInfo[iLine].lineHeight + lineSpacing;
        }
        return heightSum;
    }
}