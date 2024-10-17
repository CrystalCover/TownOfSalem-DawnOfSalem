using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Eca.DawnOfSalem
{
    internal class NameEnhancers
    {
        private static IGameService GameService => GlobalServiceLocator.GameService;

        private static GameState ActiveGameState => GameService.ActiveGameState;

        private static string GetPlayerNameBoldWithPlayerPositionSpriteTagIfEnabled(int position) => StringUtils.GetPlayerSpriteTagIfEnabled(position) + string.Format("<b>{0}</b>", ActiveGameState.Players[position].Name);

        [HarmonyPatch(typeof(ChatController))]
        private class ChatControllerFix
        {
            internal static int Position { get; set; } = -1;

            [HarmonyPatch("AddUserMessage", new[] { typeof(string), typeof(string), typeof(string) })]
            [HarmonyPrefix]
            private static bool AddUserMessage(ChatController __instance, string user, string text, string style)
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
                foreach (Player player in winners)
                    winnersFix.Add(new Player(player.Position) { Name = GetPlayerNameMarkupWithPlayerPositionSpriteTag(player.Position) });
                winners = winnersFix.OrderBy(player => player.Position).ToList();
            }
        }

        [HarmonyPatch(typeof(PreGameSceneChatListener))]
        private class PreGameSceneChatListenerFix
        {
            private static MethodInfo AddUserMessage => typeof(ChatController).GetMethod("AddUserMessage", new[] { typeof(string), typeof(string), typeof(string) });

            [HarmonyPatch("HandleOnChatMessage")]
            [HarmonyPrefix]
            private static void HandleOnChatMessage(int position) => ChatControllerFix.Position = position;

            [HarmonyPatch("HandleOnUserChosenName")]
            [HarmonyPrefix]
            private static void HandleOnUserChosenName(int position, ref string name) => name = GetPlayerNameBoldWithPlayerPositionSpriteTagIfEnabled(position);
        }
    }
}