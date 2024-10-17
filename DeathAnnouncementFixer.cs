using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eca.DawnOfSalem
{
    [HarmonyPatch(typeof(GameSceneChatListener))]
    internal class DeathAnnouncementFixer
    {
        private static IEnumerator RevealHowDied(GameSceneChatListener __instance, List<ChatMessage> chatLogKilledHowMessages)
        {
            yield return new WaitForSeconds(4);
            foreach (ChatMessage chatLogKilledHowMessage in chatLogKilledHowMessages)
            {
                __instance.chatBox.AddMessage(chatLogKilledHowMessage);
                __instance.chatLog.AddMessage(chatLogKilledHowMessage);
            }
        }

        [HarmonyPatch("RevealHowDied")]
        [HarmonyPostfix]
        private static void RevealHowDiedPostfix(GameSceneChatListener __instance, ref IEnumerator __result, List<ChatMessage> chatLogKilledHowMessages) => __result = RevealHowDied(__instance, chatLogKilledHowMessages);

        private static IEnumerator RevealRole(GameSceneChatListener __instance, List<ChatMessage> chatLogRoleMessage)
        {
            foreach (ChatMessage item in chatLogRoleMessage)
            {
                __instance.chatBox.AddMessage(item);
                __instance.chatLog.AddMessage(item);
            }
            yield return new WaitForSeconds(GlobalServiceLocator.GameService.ActiveGameState.GameMode.TimingProfile["notice_role_reveal"]);
        }

        [HarmonyPatch("RevealRole")]
        [HarmonyPostfix]
        private static void RevealRolePostfix(GameSceneChatListener __instance, ref IEnumerator __result, List<ChatMessage> chatLogRoleMessage) => __result = RevealRole(__instance, chatLogRoleMessage);
    }
}