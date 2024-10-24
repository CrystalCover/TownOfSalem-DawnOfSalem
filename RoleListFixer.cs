using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static CustomGameSetup.Role;
using static System.Reflection.BindingFlags;

namespace Eca.DawnOfSalem
{
    [HarmonyPatch(typeof(GameRulesService), nameof(GameRulesService.Load))]
    internal class RoleListFixer
    {
        private static ILocalizationService LocalizationService => GlobalServiceLocator.LocalizationService;

        private static IGameRulesService GameRulesService => GlobalServiceLocator.GameRulesService;

        private static GameRules GameRules => GameRulesService.GetGameRules();

        private static Dictionary<int, Role> Roles => GameRules.Roles;

        private static Dictionary<int, List<Role>> RandomRoles => GameRules.RandomRoles;

        private static void Postfix()
        {
            Dictionary<string, string> stringTable = (Dictionary<string, string>)typeof(LocalizationService).GetField("stringTable_", Instance | Public | NonPublic).GetValue(LocalizationService);
            bool isSpanish = LocalizationService.GetUILanguageId() == 3;
            string roleName = isSpanish ? "Mafia Asesino" : "Mafia Killing";
            string roleNameColored = isSpanish ? "<color=#DD0000>Mafia</color> <color=#1D7CF2>Asesino</color>" : "<color=#DD0000>Mafia</color> <color=#1D7CF2>Killing</color>";
            stringTable.Add("GUI_ROLE_203", roleName);
            stringTable.Add("GUI_ROLE_203_COLORED", roleNameColored);
            stringTable.Add("GUI_ROLE_204", roleName);
            stringTable.Add("GUI_ROLE_204_COLORED", roleNameColored);
            ColorUtility.TryParseHtmlString("#DD0000", out Color color);
            Roles.Add(203, new Role(203, stringTable["GUI_ROLE_203"], color, default));
            Roles.Add(204, new Role(204, stringTable["GUI_ROLE_204"], color, default));
            RandomRoles[(int)RANDOM_MAFIA] = (new[] { Roles[(int)MAFIA_DECEPTION], Roles[203], Roles[(int)MAFIA_SUPPORT] }).ToList();
            RandomRoles[(int)MAFIA_DECEPTION] = RandomRoles[(int)MAFIA_DECEPTION].OrderBy(role => role.Name).ToList();
            RandomRoles[(int)RANDOM_NEUTRAL] = (new[] { Roles[(int)NEUTRAL_BENIGN], Roles[(int)NEUTRAL_EVIL], Roles[(int)NEUTRAL_KILLING] }).ToList();
            RandomRoles[(int)COVEN_RANDOM_COVEN] = RandomRoles[(int)COVEN_RANDOM_COVEN].OrderBy(role => role.Name).ToList();
            RandomRoles[(int)COVEN_TOWN_INVESTIGATIVE] = RandomRoles[(int)COVEN_TOWN_INVESTIGATIVE].OrderBy(role => role.Name).ToList();
            RandomRoles[(int)COVEN_TOWN_PROTECTIVE] = RandomRoles[(int)COVEN_TOWN_PROTECTIVE].OrderBy(role => role.Name).ToList();
            RandomRoles[(int)COVEN_RANDOM_MAFIA] = (new[] { Roles[(int)MAFIA_DECEPTION], Roles[204], Roles[(int)MAFIA_SUPPORT] }).ToList();
            RandomRoles[(int)COVEN_MAFIA_DECEPTION] = RandomRoles[(int)COVEN_MAFIA_DECEPTION].OrderBy(role => role.Name).ToList();
            RandomRoles[(int)COVEN_RANDOM_NEUTRAL] = (new[] { Roles[(int)COVEN_NEUTRAL_BENIGN], Roles[(int)COVEN_NEUTRAL_CHAOS], Roles[(int)COVEN_NEUTRAL_EVIL], Roles[(int)COVEN_NEUTRAL_KILLING] }).ToList();
            RandomRoles[(int)COVEN_NEUTRAL_BENIGN] = RandomRoles[(int)COVEN_NEUTRAL_BENIGN].OrderBy(role => role.Name).ToList(); RandomRoles[(int)COVEN_ANY].Remove(Roles[(int)COVEN_NEUTRAL_CHAOS]);
            RandomRoles[(int)COVEN_NEUTRAL_KILLING] = RandomRoles[(int)COVEN_NEUTRAL_KILLING].AddItem(Roles[(int)JUGGERNAUT]).OrderBy(role => role.Name).ToList();
            RandomRoles[(int)COVEN_ANY].Remove(Roles[(int)COVEN_NEUTRAL_CHAOS]);
            RandomRoles[(int)COVEN_NEUTRAL_CHAOS] = RandomRoles[(int)COVEN_NEUTRAL_CHAOS].AddItem(Roles[100]).OrderBy(role => role.Name).ToList();
            RandomRoles[203] = (new[] { Roles[(int)AMBUSHER], Roles[(int)GODFATHER], Roles[(int)MAFIOSO] }).ToList();
            RandomRoles[204] = (new[] { Roles[(int)AMBUSHER], Roles[(int)GODFATHER], Roles[(int)MAFIOSO] }).ToList();
        }
    }
}
