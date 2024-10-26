using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static XMLGameRules;

namespace Eca.DawnOfSalem
{
    internal class MoreExtendedPlayerNumbers
    {
        private static IGameService GameService => GlobalServiceLocator.GameService;

        private static GameState ActiveGameState => GameService.ActiveGameState;

        private static string GetPlayerNameBoldWithPlayerPositionSpriteTagIfEnabled(int position) => StringUtils.GetPlayerSpriteTagIfEnabled(position) + string.Format("<b>{0}</b>", ActiveGameState.Players[position].Name);

        [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddUserMessage), new[] { typeof(string), typeof(string), typeof(string) })]
        private class ChatControllerFix
        {
            internal static int Position { get; set; } = -1;

            private static bool Prefix(ChatController __instance, string user, string text, string style)
            {
                if (Position == -1)
                    return true;
                __instance.AddUserMessage(Position, user, text, style);
                Position = -1;
                return false;
            }
        }

        [HarmonyPatch(typeof(GameSceneChatListener))]
        private class GameSceneChatListenerFix
        {
            private static string GetPlayerNameMarkupWithPlayerPositionSpriteTag(int position) => StringUtils.GetPlayerSpriteTag(position) + string.Format("<color=#{0}><b>{1}</b></color>", ColorUtility.ToHtmlStringRGB(Color.white), ActiveGameState.Players[position].Name);

            [HarmonyPatch("HandleOnUserChosenName")]
            [HarmonyPrefix]
            private static void HandleOnUserChosenName(int position, ref string name) => name = GetPlayerNameBoldWithPlayerPositionSpriteTagIfEnabled(position);

            [HarmonyPatch("HandleOnGameOver")]
            [HarmonyPrefix]
            private static void HandleOnGameOver(ref List<Player> winners)
            {
                List<Player> winnersFix = new List<Player>();
                winners.ForEach(player => winnersFix.Add(new Player(player.Position) { Name = GetPlayerNameMarkupWithPlayerPositionSpriteTag(player.Position) }));
                winners = winnersFix.OrderBy(player => player.Position).ToList();
            }
        }

        [HarmonyPatch(typeof(PreGameSceneChatListener))]
        private class PreGameSceneChatListenerFix
        {
            [HarmonyPatch("HandleOnChatMessage")]
            [HarmonyPrefix]
            private static void HandleOnChatMessage(int position) => ChatControllerFix.Position = position;

            [HarmonyPatch("HandleOnUserChosenName")]
            [HarmonyPrefix]
            private static void HandleOnUserChosenName(int position, ref string name) => name = GetPlayerNameBoldWithPlayerPositionSpriteTagIfEnabled(position);
        }
    }
}
