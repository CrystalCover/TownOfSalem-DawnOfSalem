using HarmonyLib;
using System.Timers;

namespace Eca.DawnOfSalem
{
    //[HarmonyPatch(typeof(BigGameSceneUIController))]
    internal class PanelsHider
    {
        [HarmonyPatch("HandleServerOnFirstDayTransition")]
        [HarmonyPostfix]
        private static void HandleServerOnFirstDayTransition(BigGameSceneUIController __instance)
        {
            __instance.HideRoleCard();
            __instance.HideRoleList();
            Timer timer = new Timer()
            {
                AutoReset = false,
                Interval = __instance.GraveyardRoleListDualDrawer.SlideTime * 1000
            };
            timer.Elapsed += delegate
            {
                __instance.HideGraveyard();
                timer.Dispose();
            };
            timer.Start();
        }
    }
}