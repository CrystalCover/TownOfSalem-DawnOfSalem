using HarmonyLib;
using static System.Reflection.BindingFlags;
using static System.String;

namespace Eca.DawnOfSalem
{
    [HarmonyPatch(typeof(ChatLogController))]
    internal class InGameMessageFilterEnhancer
    {
        private static string DayNightTag => "<daynight>";

        private static string WhisperTag => "<whisper>";

        private static IGameService GameService => GlobalServiceLocator.GameService;

        private static GameState ActiveGameState => GameService.ActiveGameState;

        [HarmonyPatch(typeof(GameSceneChatListener))]
        private class InGameMessageFilterEnhancerExtra
        {
            [HarmonyPatch("HandleOnPrivateMessageSent")]
            [HarmonyPrefix]
            private static void HandleOnPrivateMessageSent(ref string message) => message += WhisperTag;

            [HarmonyPatch("HandleOnPrivateMessageReceived")]
            [HarmonyPrefix]
            private static void HandleOnPrivateMessageReceived(ref string message) => message += WhisperTag;
        }

        private class ChatMessageExtra : ChatMessage
        {
            internal string TextWithFilterTag { get; }

            private static string TextWithSpriteTag(ChatMessage chatMessage) => (string)typeof(ChatMessage).GetField("m_textWithPlayerSpriteTag", Instance | Public | NonPublic).GetValue(chatMessage);

            internal ChatMessageExtra(ChatMessage chatMessage, string tag) : base(chatMessage.Position, chatMessage.Sender, TextWithSpriteTag(chatMessage).Replace(tag, Empty), chatMessage.Style) => TextWithFilterTag = TextWithSpriteTag(chatMessage);
        }

        [HarmonyPatch(nameof(ChatLogController.AddMessage), new[] { typeof(ChatMessage), typeof(int) })]
        [HarmonyPrefix]
        private static void AddMessagePrefix(ChatLogController __instance, ref ChatMessage chatMessage)
        {
            if (chatMessage.Text == Format("{0} {1}", __instance.l10n("GUI_DAY_NUMBER"), ActiveGameState.DayNumber) || chatMessage.Text == Format("{0} {1}", __instance.l10n("GUI_NIGHT_NUMBER"), ActiveGameState.NightNumber))
                chatMessage = new ChatMessage(chatMessage.Text + DayNightTag, chatMessage.Style);
            foreach (string tag in new[] { DayNightTag, WhisperTag })
                if (chatMessage.Text.Contains(tag))
                    chatMessage = new ChatMessageExtra(chatMessage, tag);
        }

        [HarmonyPatch(nameof(ChatLogController.AddMessage), new[] { typeof(ChatMessage), typeof(int) })]
        [HarmonyPostfix]
        private static void AddMessagePostfix(ChatLogController __instance)
        {
            if (__instance.PositionFilter != 0)
                ApplyFilter(__instance);
        }

        [HarmonyPatch("ApplyFilter")]
        [HarmonyPrefix]
        private static bool ApplyFilter(ChatLogController __instance)
        {
            foreach (BaseGameChatListItem chatMessage in __instance.chatList.ListItems)
            {
                bool noPosition = chatMessage.PositionNumber == -1;
                bool positionMatch = chatMessage.PositionNumber == __instance.PositionFilter;
                bool spriteTagMatch = ((string)typeof(ChatMessage).GetField("m_textWithPlayerSpriteTag", Instance | Public | NonPublic).GetValue(chatMessage.Data)).Contains(StringUtils.GetPlayerSpriteTag(__instance.PositionFilter - 1));
                bool meMatch = __instance.PositionFilter == GlobalServiceLocator.GameService.ActiveGameState.Me.Position + 1;
                bool whisperMatch = chatMessage.Data is ChatMessageExtra whisperMessage && whisperMessage.TextWithFilterTag.Contains(WhisperTag) && meMatch;
                if (__instance.FilterToggle.isOn)
                {
                    chatMessage.Highlight.SetActive(false);
                    bool dayNightMatch = chatMessage.Data is ChatMessageExtra dayNightMessage && dayNightMessage.TextWithFilterTag.Contains(DayNightTag);
                    chatMessage.gameObject.SetActive(noPosition ? (spriteTagMatch || whisperMatch || dayNightMatch) : positionMatch);
                }
                else
                {
                    chatMessage.gameObject.SetActive(true);
                    chatMessage.Highlight.SetActive(noPosition ? (spriteTagMatch || whisperMatch) : positionMatch);
                }
            }
            return false;
        }

        [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddMessage), new[] { typeof(ChatMessage), typeof(int) })]
        private class ChatControllerFix
        {
            private static void Prefix(ref ChatMessage chatMessage)
            {
                foreach (string tag in new[] { WhisperTag })
                    if (chatMessage.Text.Contains(tag))
                        chatMessage = new ChatMessageExtra(chatMessage, tag);
            }
        }
    }
}
