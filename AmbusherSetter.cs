using HarmonyLib;
using System.Collections.Generic;
using static CustomGameSetup.Role;

namespace Eca.DawnOfSalem
{
    [HarmonyPatch(typeof(IGameService), "RaiseOnAmbusherNightAbility")]
    internal class AmbusherSetter
    {
        private static IGameService GameService => GlobalServiceLocator.GameService;

        private static GameState ActiveGameState => GameService.ActiveGameState;

        private static List<Player> Players => ActiveGameState.Players;

        private static GameRules ActiveGameRules => GameService.ActiveGameRules;

        private static Dictionary<int, Role> Roles => ActiveGameRules.Roles;

        private static void Postfix(int position)
        {
            if (Players[position].IsAlive)
                Players[position].CurrentRole = Roles[(int)AMBUSHER];
        }
    }
}
