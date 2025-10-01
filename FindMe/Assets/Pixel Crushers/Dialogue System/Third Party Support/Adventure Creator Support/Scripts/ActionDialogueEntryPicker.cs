#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem
{

	public class ActionDialogueEntryPicker
	{

		private  DialogueDatabase database = null;
		private string currentConversation = string.Empty;
        private bool isValid { get { return entryTexts != null; } }
        private string[] entryTexts = null;
        private Dictionary<int, int> idToIndex = new Dictionary<int, int>();
        private Dictionary<int, int> indexToId = new Dictionary<int, int>();

        public ActionDialogueEntryPicker(DialogueDatabase database, string conversationTitle, int entryID)
        {
            this.database = database ?? ActionConversationPicker.FindInitialDatabase();
            CheckConversation(conversationTitle);
        }

        private int GetID(int index)
        {
            return indexToId.ContainsKey(index) ? indexToId[index] : -1;
        }

        private int GetIndex(int id)
        {
            return idToIndex.ContainsKey(id) ? idToIndex[id] : -1;
        }

        public int DoLayout(DialogueDatabase database, string label, string conversationTitle, int id)
        {
            this.database = database ?? ActionConversationPicker.FindInitialDatabase();
            CheckConversation(conversationTitle);
            if (isValid)
            {
                return GetID(EditorGUILayout.Popup(label, GetIndex(id), entryTexts));
            }
            else
            {
                return EditorGUILayout.IntField(label, id);
            }
        }

        private void CheckConversation(string conversationTitle)
        {
            if (currentConversation != conversationTitle)
            {
                indexToId.Clear();
                idToIndex.Clear();
                currentConversation = conversationTitle;
                entryTexts = null;
                var conversation = database.GetConversation(currentConversation);
                if (conversation == null) return;
                entryTexts = new string[conversation.dialogueEntries.Count];
                for (int i = 0; i < conversation.dialogueEntries.Count; i++)
                {
                    var entry = conversation.dialogueEntries[i];
                    if (entry == null) continue;
                    var text = (entry.id == 0) ? "<START>" : entry.subtitleText;
                    if (!string.IsNullOrEmpty(text)) text = text.Replace("/", "\u2215");
                    entryTexts[i] = "[" + entry.id + "] " + text;
                    const int MaxTextLength = 60;
                    if (entryTexts[i].Length > MaxTextLength) entryTexts[i] = entryTexts[i].Substring(0, MaxTextLength);
                    if (idToIndex.ContainsKey(entry.id))
                    {
                        Debug.LogWarning("Dialogue System entry picker: Conversation '" + conversation.Title + "' contains a duplicate entry ID " + entry.id + ": " + entry.DialogueText);
                    }
                    else
                    {
                        idToIndex.Add(entry.id, i);
                        if (!indexToId.ContainsKey(i))
                        { 
                            indexToId.Add(i, entry.id);
                        }
                    }
                }
            }
        }

    }
}
#endif