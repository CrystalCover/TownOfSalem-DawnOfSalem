using HarmonyLib;
using UnityEngine;
using static System.Reflection.BindingFlags;

namespace Eca.DawnOfSalem
{
    [HarmonyPatch(typeof(BigGameSceneUIController))]
    internal class PanelsHider
    {
        [HarmonyPatch("HandleServerOnFirstDayTransition")]
        [HarmonyPostfix]
        private static void HandleServerOnFirstDayTransition(BigGameSceneUIController __instance)
        {
            GamePanelDrawer roleCardDrawer = __instance.RoleCardDrawer;
            typeof(GamePanelDrawer).GetField("shown", Instance | Public | NonPublic).SetValue(roleCardDrawer, false);
            ((RectTransform)typeof(GamePanelDrawer).GetField("rectTransform", Instance | Public | NonPublic).GetValue(roleCardDrawer)).anchoredPosition = (Vector2)typeof(GamePanelDrawer).GetField("closedPosition", Instance | Public | NonPublic).GetValue(roleCardDrawer);
            __instance.RoleCardMaximizeTab.SetActive(true);
            if (GlobalServiceLocator.UserService.Settings.AutoexpandEnabled)
            {
                VerticalPanelSizer targetMenuVerticalSizer = __instance.TargetMenuVerticalSizer;
                typeof(VerticalPanelSizer).GetField("isExpanded_", Instance | Public | NonPublic).SetValue(targetMenuVerticalSizer, true);
                ((RectTransform)typeof(VerticalPanelSizer).GetField("rectTransform", Instance | Public | NonPublic).GetValue(targetMenuVerticalSizer)).anchorMax = (Vector2)typeof(VerticalPanelSizer).GetField("expandedAnchorMax", Instance | Public | NonPublic).GetValue(targetMenuVerticalSizer);
            }
            GYRLDualPanelDrawer graveyardRoleListDualDrawer = __instance.GraveyardRoleListDualDrawer;
            typeof(GYRLDualPanelDrawer).GetField("roleListShown_", Instance | Public | NonPublic).SetValue(graveyardRoleListDualDrawer, false);
            typeof(GYRLDualPanelDrawer).GetField("roleListMidShown_", Instance | Public | NonPublic).SetValue(graveyardRoleListDualDrawer, false);
            graveyardRoleListDualDrawer.roleListRectTransform.anchoredPosition = (Vector2)typeof(GYRLDualPanelDrawer).GetField("roleListClosedPosition", Instance | Public | NonPublic).GetValue(graveyardRoleListDualDrawer);
            typeof(GYRLDualPanelDrawer).GetField("graveyardShown_", Instance | Public | NonPublic).SetValue(graveyardRoleListDualDrawer, false);
            graveyardRoleListDualDrawer.graveyardRectTransform.anchoredPosition = (Vector2)typeof(GYRLDualPanelDrawer).GetField("graveyardClosedPosition", Instance | Public | NonPublic).GetValue(graveyardRoleListDualDrawer);
            typeof(GYRLDualPanelDrawer).GetMethod("SetTabStates", Instance | Public | NonPublic).Invoke(graveyardRoleListDualDrawer, null);
            if (GlobalServiceLocator.UserService.Settings.AutoexpandEnabled)
            {
                VerticalPanelSizer chatVerticalSizer = __instance.ChatVerticalSizer;
                typeof(VerticalPanelSizer).GetField("isExpanded_", Instance | Public | NonPublic).SetValue(chatVerticalSizer, true);
                ((RectTransform)typeof(VerticalPanelSizer).GetField("rectTransform", Instance | Public | NonPublic).GetValue(chatVerticalSizer)).anchorMax = (Vector2)typeof(VerticalPanelSizer).GetField("expandedAnchorMax", Instance | Public | NonPublic).GetValue(chatVerticalSizer);
            }
        }
    }
}
