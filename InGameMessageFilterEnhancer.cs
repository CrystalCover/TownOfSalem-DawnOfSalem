using HarmonyLib;
using System.Linq;
using static System.Reflection.BindingFlags;

namespace Eca.DawnOfSalem
{
    [HarmonyPatch(typeof(ChatLogController))]
    internal class InGameMessageFilterEnhancer
    {
        [HarmonyPatch("AddMessage", new[] {typeof(ChatMessage), typeof(int)})]
        [HarmonyPostfix]
        private static void AddMessage(ChatLogController __instance)
        {
            if (__instance.PositionFilter != 0)
                ApplyFilter(__instance);
        }

        [HarmonyPatch("ApplyFilter")]
        [HarmonyPrefix]
        private static bool ApplyFilter(ChatLogController __instance)
        {
            foreach (BaseGameChatListItem chatMessage in __instance.chatList.ListItems)
                if (__instance.FilterToggle.isOn)
                {
                    chatMessage.Highlight.SetActive(false);
                    chatMessage.gameObject.SetActive((chatMessage.PositionNumber == __instance.PositionFilter) || ((chatMessage.PositionNumber == -1) && (chatMessage.Data.Text.Contains(string.Format(StringUtils.GetPlayerSpriteTag(__instance.PositionFilter - 1))) || new[] { "GUI_DAY_NUMBER", "GUI_NIGHT_NUMBER" }.Where(message => chatMessage.Data.Text.Contains(__instance.l10n(message))).Any())));
                }
                else
                {
                    chatMessage.gameObject.SetActive(true);
                    chatMessage.Highlight.SetActive((chatMessage.PositionNumber == __instance.PositionFilter) || (chatMessage.PositionNumber == -1 && chatMessage.Data.Text.Contains(string.Format(StringUtils.GetPlayerSpriteTag(__instance.PositionFilter - 1)))));
                }
            typeof(ChatLogController).GetMethod("ScrollToBottom", Instance | Public | NonPublic).Invoke(__instance, null);
            return false;
        }
    }
}
