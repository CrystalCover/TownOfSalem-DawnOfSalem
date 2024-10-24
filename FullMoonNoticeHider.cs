using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using static CustomGameSetup.Role;

namespace Eca.DawnOfSalem
{
    [HarmonyPatch(typeof(GameSceneChatListener), "HandleOnFullMoonNight")]
    internal class FullMoonNoticeHider
    {
        private static IGameService GameService => GlobalServiceLocator.GameService;

        private static GameState ActiveGameState => GameService.ActiveGameState;

        private static List<Role> RoleList => ActiveGameState.RoleList;

        private static GameRules GameRules => GameService.ActiveGameRules;

        private static Dictionary<int, Role> Roles => GameRules.Roles;

        private static bool Prefix() => new List<CustomGameSetup.Role>(new[] { WEREWOLF, RANDOM_NEUTRAL, NEUTRAL_KILLING, ANY, JUGGERNAUT, COVEN_RANDOM_NEUTRAL, COVEN_NEUTRAL_KILLING, COVEN_ANY }).FindAll(role => RoleList.Contains(Roles[(int)role])).Any();
    }
}
