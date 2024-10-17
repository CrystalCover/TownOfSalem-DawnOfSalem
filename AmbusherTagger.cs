using HarmonyLib;
using System.Collections.Generic;
using static CustomGameSetup.Role;

namespace Eca.DawnOfSalem
{
    [HarmonyPatch(typeof(IGameService))]
    internal class AmbusherTagger
    {
        private static IGameService GameService => GlobalServiceLocator.GameService;

        private static GameState ActiveGameState => GameService.ActiveGameState;

        private static List<Player> Players => ActiveGameState.Players;

        private static GameRules ActiveGameRules => GameService.ActiveGameRules;

        private static Dictionary<int, Role> Roles => ActiveGameRules.Roles;

        [HarmonyPatch("RaiseOnAmbusherNightAbility")]
        [HarmonyPostfix]
        private static void RaiseOnAmbusherNightAbility(int position)
        {
            if (Players[position].IsAlive)
                Players[position].CurrentRole = Roles[(int)AMBUSHER];
        }
    }
}