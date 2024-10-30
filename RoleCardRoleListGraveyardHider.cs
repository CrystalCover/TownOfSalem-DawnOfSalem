using HarmonyLib;
using UnityEngine;
using static System.Reflection.BindingFlags;

namespace Eca.DawnOfSalem
{
    [HarmonyPatch(typeof(BigGameSceneUIController), "HandleServerOnFirstDayTransition")]
    internal class RoleCardRoleListGraveyardHider
    {
        private static void Postfix(BigGameSceneUIController __instance)
        {
            bool autoExpandEnabled = GlobalServiceLocator.UserService.Settings.AutoexpandEnabled;
            GamePanelDrawer roleCardDrawer = __instance.RoleCardDrawer;
            typeof(GamePanelDrawer).GetField("shown", Instance | Public | NonPublic).SetValue(roleCardDrawer, false);
            ((RectTransform)typeof(GamePanelDrawer).GetField("rectTransform", Instance | Public | NonPublic).GetValue(roleCardDrawer)).anchoredPosition = (Vector2)typeof(GamePanelDrawer).GetField("closedPosition", Instance | Public | NonPublic).GetValue(roleCardDrawer);
            __instance.RoleCardMaximizeTab.SetActive(true);
            if (autoExpandEnabled)
            {
                VerticalPanelSizer targetMenuVerticalSizer = __instance.TargetMenuVerticalSizer;
                typeof(VerticalPanelSizer).GetField("isExpanded_", Instance | Public | NonPublic).SetValue(targetMenuVerticalSizer, true);
                ((RectTransform)typeof(VerticalPanelSizer).GetField("rectTransform", Instance | Public | NonPublic).GetValue(targetMenuVerticalSizer)).anchorMax = (Vector2)typeof(VerticalPanelSizer).GetField("expandedAnchorMax", Instance | Public | NonPublic).GetValue(targetMenuVerticalSizer);
            }
            GYRLDualPanelDrawer graveyardRoleListDualDrawer = __instance.GraveyardRoleListDualDrawer;
            if (GlobalServiceLocator.GameService.ActiveGameState.GameMode.LobbyType != LobbyType.Custom)
            {
                typeof(GYRLDualPanelDrawer).GetField("roleListShown_", Instance | Public | NonPublic).SetValue(graveyardRoleListDualDrawer, false);
                typeof(GYRLDualPanelDrawer).GetField("roleListMidShown_", Instance | Public | NonPublic).SetValue(graveyardRoleListDualDrawer, false);
                graveyardRoleListDualDrawer.roleListRectTransform.anchoredPosition = (Vector2)typeof(GYRLDualPanelDrawer).GetField("roleListClosedPosition", Instance | Public | NonPublic).GetValue(graveyardRoleListDualDrawer);
            }
            typeof(GYRLDualPanelDrawer).GetField("graveyardShown_", Instance | Public | NonPublic).SetValue(graveyardRoleListDualDrawer, false);
            graveyardRoleListDualDrawer.graveyardRectTransform.anchoredPosition = (Vector2)typeof(GYRLDualPanelDrawer).GetField("graveyardClosedPosition", Instance | Public | NonPublic).GetValue(graveyardRoleListDualDrawer);
            typeof(GYRLDualPanelDrawer).GetMethod("SetTabStates", Instance | Public | NonPublic).Invoke(graveyardRoleListDualDrawer, null);
            if (autoExpandEnabled)
            {
                VerticalPanelSizer chatVerticalSizer = __instance.ChatVerticalSizer;
                typeof(VerticalPanelSizer).GetField("isExpanded_", Instance | Public | NonPublic).SetValue(chatVerticalSizer, true);
                ((RectTransform)typeof(VerticalPanelSizer).GetField("rectTransform", Instance | Public | NonPublic).GetValue(chatVerticalSizer)).anchorMax = (Vector2)typeof(VerticalPanelSizer).GetField("expandedAnchorMax", Instance | Public | NonPublic).GetValue(chatVerticalSizer);
            }
        }
    }
}
