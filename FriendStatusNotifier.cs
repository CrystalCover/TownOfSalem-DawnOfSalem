using HarmonyLib;
using static ActivityStatus;
using static System.Reflection.BindingFlags;
using static ToasterData;

namespace Eca.DawnOfSalem
{
    [HarmonyPatch(typeof(NotificationsToasterMasterController))]
    internal class FriendStatusNotifier
    {
        private static IFriendService FriendService => GlobalServiceLocator.FriendService;

        private static NotificationsToasterMasterController NotificationsToasterMasterController { get; set; }

        private static ILocalizationService LocalizationService => GlobalServiceLocator.LocalizationService;

        private static void OnFriendUpdated(Friend friend)
        {
            if (friend.Status == Offline)
                typeof(NotificationsToasterMasterController).GetMethod("AddItemToList", Instance | Public | NonPublic).Invoke(NotificationsToasterMasterController, new[] { new ToasterData((ToasterDataType)631, friend.AccountId, friend.UserName, friend.UserName, LocalizationService.GetLocalizedString("GUI_FRIEND_STATUS_OFFLINE")) });
            else if (friend.Status == Online)
                typeof(NotificationsToasterMasterController).GetMethod("AddItemToList", Instance | Public | NonPublic).Invoke(NotificationsToasterMasterController, new[] { new ToasterData((ToasterDataType)631, friend.AccountId, friend.UserName, friend.UserName, LocalizationService.GetLocalizedString("GUI_FRIEND_STATUS_ONLINE")) });
        }

        [HarmonyPatch("AddListeners")]
        [HarmonyPostfix]
        private static void AddListeners(NotificationsToasterMasterController __instance)
        {
            if (FriendService == null)
                return;
            NotificationsToasterMasterController = __instance;
            FriendService.OnFriendUpdated += OnFriendUpdated;
        }

        [HarmonyPatch("RemoveListeners")]
        [HarmonyPostfix]
        private static void RemoveListeners()
        {
            if (FriendService == null)
                return;
            FriendService.OnFriendUpdated -= OnFriendUpdated;
            NotificationsToasterMasterController = null;
        }
    }
}
