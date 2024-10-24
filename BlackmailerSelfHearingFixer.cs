using HarmonyLib;

namespace Eca.DawnOfSalem
{
    [HarmonyPatch(typeof(GameSceneChatListener), "HandleOnPrivateMessageCommunicated")]
    internal class BlackmailerSelfHearingFixer
    {
        private static IGameService GameService => GlobalServiceLocator.GameService;

        private static GameState ActiveGameState => GameService.ActiveGameState;

        private static Player Me => ActiveGameState.Me;

        private static bool Prefix(int fromPosition) => fromPosition != Me.Position;
    }
}
