using HarmonyLib;
using static System.Reflection.BindingFlags;

namespace Eca.DawnOfSalem
{
    [HarmonyPatch(typeof(BaseGameSceneUIController))]
    internal class CloseableLastWillsAndDeathNotes
    {
        //[HarmonyPatch(nameof(BaseGameSceneUIController.HandleOnTargetEvent))]
        //[HarmonyPrefix]
        private static bool HandleOnTargetEvent(BaseGameSceneUIController __instance, TargetEvent evt)
        {
            if (evt.EventType != TargetEventType.LastWill)
                return true;
            if (!string.IsNullOrEmpty(evt.Target.LastWill) || evt.Target.LastWillCoveredInBlood)
                __instance.ShowLastWill(__instance.OtherLastWill, evt.Target, true, true, false, evt.Target.LastWillCoveredInBlood);
            return false;
        }

        private static void CloseLastWillsDeathNotesNotepad(BaseGameSceneUIController __instance, bool closeMyLastWillDeathNote)
        {
            if (!__instance.WhoDiedLastWill.IsClosed)
                __instance.WhoDiedLastWill.Close(false);
            if (!__instance.MyLastWill.IsClosed && closeMyLastWillDeathNote)
                __instance.MyLastWill.Close(true);
            if (!__instance.OtherLastWill.IsClosed)
                __instance.OtherLastWill.Close(false);
            if (__instance.Notepad.activeSelf || __instance.Notepad.activeInHierarchy)
                __instance.Notepad.SetActive(false);
            BaseDeathNoteController currentDeathNote = (BaseDeathNoteController)typeof(BaseGameSceneUIController).GetField("m_currentDeathNote", Instance | Public | NonPublic).GetValue(__instance);
            if ((bool)currentDeathNote)
                currentDeathNote.Close(false);
            BaseDeathNoteController myDeathNote = (BaseDeathNoteController)typeof(BaseGameSceneUIController).GetField("m_myDeathNote", Instance | Public | NonPublic).GetValue(__instance);
            if ((bool)myDeathNote && closeMyLastWillDeathNote)
                myDeathNote.Close(true);
        }

        //[HarmonyPatch(nameof(BaseGameSceneUIController.HandleClickNotepad))]
        //[HarmonyPrefix]
        private static void HandleClickNotepad(BaseGameSceneUIController __instance) => CloseLastWillsDeathNotesNotepad(__instance, true);

        //[HarmonyPatch(nameof(BaseGameSceneUIController.ShowLastWill))]
        //[HarmonyPrefix]
        private static void ShowLastWill(BaseGameSceneUIController __instance, LastWillController willController, ref bool closeable)
        {
            //CloseLastWillsDeathNotesNotepad(__instance, willController != __instance.WhoDiedLastWill);
            closeable = true;
        }

        //[HarmonyPatch(nameof(BaseGameSceneUIController.ShowDeathNote))]
        //[HarmonyPrefix]
        private static void ShowDeathNote(BaseGameSceneUIController __instance, ref bool closeable)
        {
            //CloseLastWillsDeathNotesNotepad(__instance, closeable);
            closeable = true;
        }
    }
}
