using System.Collections;
using UnityEngine;
using AC;

namespace PixelCrushers.DialogueSystem.AdventureCreatorSupport
{

    /// <summary>
    /// This script ties into AC's save system. If the Dialogue Manager has a Save System
    /// component, it will use it. Otherwise it will only use the Dialogue System's 
    /// PersistentDataManager, which saves the dialogue database runtime values and
    /// any persistent data components but not Savers.
    /// </summary>
    [AddComponentMenu("Pixel Crushers/Dialogue System/Third Party/Adventure Creator/Remember Dialogue System")]
    public class RememberDialogueSystem : Remember
    {

        public bool stopConversationsWhenLoading = false;

        protected override void Start()
        {
            base.Start();
            EventManager.OnRestartGame += OnRestartGame;
        }

        protected virtual void OnDestroy()
        {
            EventManager.OnRestartGame -= OnRestartGame;
        }

        protected virtual void OnRestartGame()
        {
            PixelCrushers.SaveSystem.ResetGameState();
        }

        /// <summary>
        /// Tells the Dialogue System to save its state into an AC global variable
        /// prior to changing levels (or saving a game).
        /// </summary>
        public override string SaveData()
        {
            if (DialogueDebug.LogInfo) Debug.Log("Saving Dialogue System state to Adventure Creator.");

            DSData data = new DSData();
            data.objectID = constantID;
            data.savePrevented = savePrevented;

            if (PixelCrushers.SaveSystem.hasInstance)
            {
                data.saveData = PixelCrushers.SaveSystem.Serialize(PixelCrushers.SaveSystem.RecordSavedGameData());
            }
            else
            {
                data.saveData = PersistentDataManager.GetSaveData();
            }

            return Serializer.SaveScriptData<DSData>(data);
        }

        public override void LoadData(string stringData)
        {
            DSData data = Serializer.LoadScriptData<DSData>(stringData);
            if (data == null) return;
            SavePrevented = data.savePrevented; if (savePrevented) return;

            if (stopConversationsWhenLoading)
            {
                DialogueManager.StopAllConversations();
            }
            if (PixelCrushers.SaveSystem.hasInstance)
            {
                if (DialogueDebug.LogInfo) Debug.Log("Restoring Dialogue System state from Adventure Creator.");
                PixelCrushers.SaveSystem.ApplySavedGameData(PixelCrushers.SaveSystem.Deserialize<PixelCrushers.SavedGameData>(data.saveData));
            }
            else
            {
                PersistentDataManager.ApplySaveData(data.saveData);
            }
            AdventureCreatorBridge.UpdateSettingsFromAC();
        }

    }

    [System.Serializable]
    public class DSData : RememberData
    {

        public string saveData;

        public DSData() { }

    }
}
