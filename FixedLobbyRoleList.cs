using HarmonyLib;

namespace Eca.DawnOfSalem
{
    [HarmonyPatch(typeof(BaseLobbySceneController))]
    internal class FixedLobbyRoleList
    {
        private static ILobbyService LobbyService => GlobalServiceLocator.LobbyService;

        private static Lobby CurrentLobby => LobbyService.CurrentLobby;

        private static GameMode GameMode => CurrentLobby.GameMode;

        [HarmonyPatch("AutoStartLobbyIfWeAreLastPlayer")]
        [HarmonyPrefix]
        private static void AutoStartLobbyIfWeAreLastPlayer(BaseLobbySceneController __instance)
        {
            if (GameMode.LobbyType == LobbyType.Custom || GameMode.RoleList == null)
                return;
            __instance.RoleList.Clear();
            foreach (Role role in GameMode.RoleList)
                ((RoleListItem)__instance.RoleList.AddItem(role)).OnRoleButtonClicked += delegate { __instance.LobbyRoleDescription?.SetRole(role); };
            __instance.RoleCountLabel.SetText(__instance.RoleList.Count.ToString());
        }

        [HarmonyPatch("AddUser")]
        [HarmonyPrefix]
        private static bool AddUser(BaseLobbySceneController __instance, LobbyUser user)
        {
            __instance.UserList.AddItem(user);
            __instance.PlayerCountLabel.SetText(__instance.UserList.Count.ToString());
            return false;
        }

        [HarmonyPatch("HandleServerOnUserLeftLobby")]
        [HarmonyPrefix]
        private static bool HandleServerOnUserLeftLobby(BaseLobbySceneController __instance, LobbyUser user)
        {
            __instance.UserList.RemoveListItem(user);
            __instance.PlayerCountLabel.SetText(__instance.UserList.Count.ToString());
            return false;
        }
    }
}
