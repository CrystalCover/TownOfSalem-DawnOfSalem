using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Eca.DawnOfSalem
{
    [HarmonyPatch(typeof(BaseLobbySceneController))]
    internal class FixedLobbyRoleLists
    {
        private static ILobbyService LobbyService => GlobalServiceLocator.LobbyService;

        private static Lobby CurrentLobby => LobbyService.CurrentLobby;

        private static GameMode GameMode => CurrentLobby.GameMode;

        private class OnRoleButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
        {
            internal BaseLobbySceneController BaseLobbySceneController { get; set; }

            private LobbyRoleDescriptionController LobbyRoleDescription => BaseLobbySceneController.LobbyRoleDescription;

            internal Role Role { get; set; }

            public void OnPointerEnter(PointerEventData eventData)
            {
                LobbyRoleDescription?.gameObject.SetActive(true);
                LobbyRoleDescription?.SetRole(Role);
            }

            public void OnPointerExit(PointerEventData eventData) => LobbyRoleDescription?.gameObject.SetActive(false);
        }

        [HarmonyPatch("AutoStartLobbyIfWeAreLastPlayer")]
        [HarmonyPrefix]
        private static void AutoStartLobbyIfWeAreLastPlayer(BaseLobbySceneController __instance)
        {
            __instance.LobbyRoleDescription?.gameObject.SetActive(false);
            if (GameMode.RoleList == null)
                return;
            __instance.RoleList.Clear();
            foreach (Role role in GameMode.RoleList)
            {
                OnRoleButtonHover onRoleButtonHover = __instance.RoleList.AddItem(role).gameObject.AddComponent<OnRoleButtonHover>();
                onRoleButtonHover.BaseLobbySceneController = __instance;
                onRoleButtonHover.Role = role;
            }
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


        [HarmonyPatch("HandleClickRoleSelected")]
        [HarmonyPrefix]
        private static void HandleClickRoleSelected(BaseLobbySceneController __instance) => __instance.LobbyRoleDescription?.gameObject.SetActive(true);

        [HarmonyPatch("HandleServerOnUserLeftLobby")]
        [HarmonyPrefix]
        private static bool HandleServerOnUserLeftLobby(BaseLobbySceneController __instance, LobbyUser user)
        {
            __instance.UserList.RemoveListItem(user);
            __instance.PlayerCountLabel.SetText(__instance.UserList.Count.ToString());
            return false;
        }

        [HarmonyPatch("HandleOnRoleAdded")]
        [HarmonyPostfix]
        private static void HandleOnRoleAdded(BaseLobbySceneController __instance, Role role)
        {
            OnRoleButtonHover onRoleButtonHover = __instance.RoleList.GetListItem(__instance.RoleList.Count - 1).gameObject.AddComponent<OnRoleButtonHover>();
            onRoleButtonHover.BaseLobbySceneController = __instance;
            onRoleButtonHover.Role = role;
        }
    }
}
