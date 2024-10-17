using HarmonyLib;

namespace Eca.DawnOfSalem
{
    [HarmonyPatch(typeof(GameSceneChatListener))]
    internal class BlackmailerHearingFixer
    {
        private static IGameService GameService => GlobalServiceLocator.GameService;

        private static GameState ActiveGameState => GameService.ActiveGameState;

        private static Player Me => ActiveGameState.Me;

        [HarmonyPatch("HandleOnPrivateMessageCommunicated")]
        [HarmonyPrefix]
        private static bool HandleOnPrivateMessageCommunicated(int fromPosition) => fromPosition != Me.Position;
    }
}