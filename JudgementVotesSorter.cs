using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static System.Reflection.BindingFlags;
using static System.String;

namespace Eca.DawnOfSalem
{
    [HarmonyPatch(typeof(GameSceneChatListener))]
    internal class JudgementVotesSorter
    {
        private static IGameService GameService => GlobalServiceLocator.GameService;

        private static GameState ActiveGameState => GameService.ActiveGameState;

        private static Player PlayerOnTrial => ActiveGameState.PlayerOnTrial;

        private static Dictionary<int, int> JudgementVotes { get; } = new Dictionary<int, int>();

        [HarmonyPatch("HandleOnTellJudgementVotes")]
        [HarmonyPrefix]
        private static bool HandleOnTellJudgementVotes(int position, int vote)
        {
            if (JudgementVotes.ContainsKey(100))
                return true;
            JudgementVotes.Add(position, vote);
            return false;
        }

        private static void HandleOnJudgement(GameSceneChatListener __instance)
        {
            JudgementVotes.Add(100, 100);
            JudgementVotes.OrderBy(judgement => judgement.Key).ForEach(judgement => typeof(GameSceneChatListener).GetMethod("HandleOnTellJudgementVotes", Instance | Public | NonPublic).Invoke(__instance, new object[] { judgement.Key, judgement.Value }));
            JudgementVotes.Clear();
            PlayerOnTrial.Name = Format("<color=#{0}><b>{1}</b></color>", ColorUtility.ToHtmlStringRGB(Color.white), PlayerOnTrial.Name);
        }

        [HarmonyPatch("HandleOnInnocent")]
        [HarmonyPrefix]
        private static void HandleOnInnocentPrefix(GameSceneChatListener __instance) => HandleOnJudgement(__instance);

        [HarmonyPatch("HandleOnInnocent")]
        [HarmonyPostfix]
        private static void HandleOnInnocentPostfix() => PlayerOnTrial.Name = PlayerOnTrial.Name.Substring(PlayerOnTrial.Name.IndexOf("<b>") + 3, PlayerOnTrial.Name.IndexOf("</b>") - PlayerOnTrial.Name.IndexOf("<b>") - 3);

        [HarmonyPatch("HandleOnGuilty")]
        [HarmonyPrefix]
        private static void HandleOnGuiltyPrefix(GameSceneChatListener __instance) => HandleOnJudgement(__instance);

        [HarmonyPatch("HandleOnGuilty")]
        [HarmonyPostfix]
        private static void HandleOnGuiltyPostfix() => PlayerOnTrial.Name = PlayerOnTrial.Name.Substring(PlayerOnTrial.Name.IndexOf("<b>") + 3, PlayerOnTrial.Name.IndexOf("</b>") - PlayerOnTrial.Name.IndexOf("<b>") - 3);
    }
}
