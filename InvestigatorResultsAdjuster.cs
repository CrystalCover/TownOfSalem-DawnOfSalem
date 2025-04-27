using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static CustomGameSetup.Role;
using static Factions;
using static System.String;

namespace Eca.DawnOfSalem
{
    [HarmonyPatch(typeof(GameSceneChatListener), "HandleOnStringTableMessage")]
    internal class InvestigatorResultsAdjuster
    {
        private static IGameService GameService => GlobalServiceLocator.GameService;

        private static GameRules GameRules => GameService.ActiveGameRules;

        private static Dictionary<int, Role> Roles => GameRules.Roles;

        private static GameState ActiveGameState => GameService.ActiveGameState;

        private static List<Role> RoleList => ActiveGameState.RoleList;

        private static List<CustomGameSetup.Role> GetRandomRoles(CustomGameSetup.Role roleEnum)
        {
            List<CustomGameSetup.Role> randomRoles = new List<CustomGameSetup.Role>() { roleEnum };
            Role role = Roles[(int)roleEnum];
            string roleFaction;
            switch ((Factions)role.Faction)
            {
                case Town:
                    roleFaction = "TOWN";
                    break;
                case Mafia:
                    roleFaction = "MAFIA";
                    break;
                case Coven:
                    roleFaction = "COVEN";
                    break;
                default:
                    roleFaction = "NEUTRAL";
                    break;
            }
            string roleAlignment = Empty;
            bool covenOnly = role.CovenOnly;
            switch (roleEnum)
            {
                case BODYGUARD:
                case DOCTOR:
                case CRUSADER:
                case TRAPPER:
                    roleAlignment = "PROTECTIVE";
                    break;
                case ESCORT:
                case MAYOR:
                case MEDIUM:
                case RETRIBUTIONIST:
                case TRANSPORTER:
                case BLACKMAILER:
                case CONSIGLIERE:
                case CONSORT:
                    roleAlignment = "SUPPORT";
                    break;
                case INVESTIGATOR:
                case LOOKOUT:
                case SHERIFF:
                case SPY:
                case TRACKER:
                case PSYCHIC:
                    roleAlignment = "INVESTIGATIVE";
                    break;
                case VAMPIRE_HUNTER:
                case VETERAN:
                case VIGILANTE:
                case GODFATHER:
                case MAFIOSO:
                case ARSONIST:
                case SERIALKILLER:
                case WEREWOLF:
                case AMBUSHER:
                case JUGGERNAUT:
                    roleAlignment = "KILLING";
                    break;
                case DISGUISER:
                case FORGER:
                case FRAMER:
                case JANITOR:
                case HYPNOTIST:
                    roleAlignment = "DECEPTION";
                    break;
                case AMNESIAC:
                case SURVIVOR:
                case GUARDIAN_ANGEL:
                    roleAlignment = "BENIGN";
                    break;
                case EXECUTIONER:
                case JESTER:
                case WITCH:
                case COVEN_LEADER:
                case POTION_MASTER:
                case HEX_MASTER:
                case NECROMANCER:
                case POISONER:
                case MEDUSA:
                    roleAlignment = "EVIL";
                    break;
                case VAMPIRE:
                case PLAGUEBEARER:
                case PIRATE:
                case (CustomGameSetup.Role)100:
                    roleAlignment = "CHAOS";
                    break;
                default:
                    covenOnly = roleEnum.ToString().StartsWith("COVEN_");
                    if (!roleEnum.ToString().StartsWith((covenOnly ? "COVEN_" : Empty) + "RANDOM_") && roleEnum.ToString() != (covenOnly ? "COVEN_" : Empty) + "ANY")
                        roleAlignment = roleEnum.ToString().TrimStart(((covenOnly ? "COVEN_" : Empty) + roleFaction + "_").ToCharArray());
                    randomRoles.Remove(roleEnum);
                    break;
            }
            CustomGameSetup.Role roleName;
            if (!covenOnly)
            {
                if (!IsNullOrEmpty(roleFaction) && Enum.TryParse("RANDOM_" + roleFaction, out roleName))
                    randomRoles.Add(roleName);
                if (!IsNullOrEmpty(roleAlignment) && Enum.TryParse(roleFaction + "_" + roleAlignment, out roleName))
                    randomRoles.Add(roleName);
                randomRoles.Add(ANY);
                if (roleEnum == WITCH)
                    return randomRoles;
            }
            if (!IsNullOrEmpty(roleFaction) && Enum.TryParse("COVEN_RANDOM_" + roleFaction, out roleName))
                randomRoles.Add(roleName);
            if (!IsNullOrEmpty(roleAlignment) && Enum.TryParse("COVEN_" + roleFaction + "_" + roleAlignment, out roleName))
                randomRoles.Add(roleName);
            randomRoles.Add(COVEN_ANY);
            return randomRoles;
        }

        private static List<string> UpdateInvestigatorResults(CustomGameSetup.Role[] roleEnums)
        {
            List<string> roleNames = new List<string>();
            foreach (CustomGameSetup.Role roleEnum in roleEnums)
                if (GetRandomRoles(roleEnum).FindAll(role => RoleList.Contains(Roles[(int)role])).Any())
                    roleNames.Add(Format("<color=#{0}><b>{1}</b></color>", ColorUtility.ToHtmlStringRGB(Roles[(int)roleEnum].Color), Roles[(int)roleEnum].Name));
            return roleNames;
        }

        private static bool Prefix(GameSceneChatListener __instance, int id)
        {
            List<string> roles;
            switch (id)
            {
                case 26:
                    roles = UpdateInvestigatorResults(new[] { ESCORT, TRANSPORTER, CONSORT, HYPNOTIST });
                    break;
                case 27:
                    roles = UpdateInvestigatorResults(new[] { DOCTOR, DISGUISER, SERIALKILLER, POTION_MASTER });
                    break;
                case 28:
                    roles = UpdateInvestigatorResults(new[] { INVESTIGATOR, CONSIGLIERE, MAYOR, TRACKER, PLAGUEBEARER, (CustomGameSetup.Role)100 });
                    break;
                case 29:
                case 38:
                    roles = UpdateInvestigatorResults(new[] { LOOKOUT, FORGER, WITCH, JUGGERNAUT, COVEN_LEADER });
                    break;
                case 30:
                    roles = UpdateInvestigatorResults(new[] { BODYGUARD, GODFATHER, ARSONIST, CRUSADER });
                    break;
                case 31:
                    roles = UpdateInvestigatorResults(new[] { VIGILANTE, VETERAN, MAFIOSO, PIRATE, AMBUSHER });
                    break;
                case 32:
                    roles = UpdateInvestigatorResults(new[] { MEDIUM, JANITOR, RETRIBUTIONIST, NECROMANCER, TRAPPER });
                    break;
                case 33:
                    roles = UpdateInvestigatorResults(new[] { SURVIVOR, VAMPIRE_HUNTER, AMNESIAC, MEDUSA, PSYCHIC });
                    break;
                case 34:
                    roles = UpdateInvestigatorResults(new[] { SPY, BLACKMAILER, JAILOR, GUARDIAN_ANGEL });
                    break;
                case 35:
                    roles = UpdateInvestigatorResults(new[] { SHERIFF, EXECUTIONER, WEREWOLF, POISONER });
                    break;
                case 37:
                    roles = UpdateInvestigatorResults(new[] { FRAMER, VAMPIRE, JESTER, HEX_MASTER });
                    break;
                default:
                    return true;
            }
            if (roles.Count == 0)
                return true;
            bool isSpanish = GlobalServiceLocator.LocalizationService.GetUILanguageId() == 3;
            string or = isSpanish ? " o " : " or ";
            string investigatorFeedback = isSpanish ? "Tu blanco puede ser {0}." : "Your target could be {0}.";
            string investigatorResults = roles.Join(null, ", ");
            if (roles.Count == 2)
                investigatorResults = investigatorResults.Replace(", ", or);
            else if (roles.Count > 2)
                investigatorResults = investigatorResults.Insert(investigatorResults.LastIndexOf(", ") + 2, or.TrimStart());
            investigatorFeedback = Format(investigatorFeedback, investigatorResults);
            __instance.chatBox.AddMessage(investigatorFeedback);
            __instance.chatLog.AddMessage(investigatorFeedback);
            return false;
        }
    }
}
