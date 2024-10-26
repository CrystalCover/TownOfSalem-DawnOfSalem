using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using static System.Reflection.BindingFlags;

namespace Eca.DawnOfSalem
{
    [HarmonyPatch(typeof(GameSceneChatListener))]
    internal class JudgementVotesSorter
    {
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
        }

        [HarmonyPatch("HandleOnInnocent")]
        [HarmonyPrefix]
        private static void HandleOnInnocent(GameSceneChatListener __instance) => HandleOnJudgement(__instance);

        [HarmonyPatch("HandleOnGuilty")]
        [HarmonyPrefix]
        private static void HandleOnGuilty(GameSceneChatListener __instance) => HandleOnJudgement(__instance);
    }
}
