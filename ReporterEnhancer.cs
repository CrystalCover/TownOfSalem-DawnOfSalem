using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using static System.Reflection.BindingFlags;

namespace Eca.DawnOfSalem
{
    [HarmonyPatch(typeof(ReportPlayerController), "Start")]
    internal class ReporterEnhancer
    {
        private static IEnumerator Start(ReportPlayerController __instance, IEnumerator __result)
        {
            while (__result.MoveNext())
                yield return __result.Current;
            List<string> options = new List<string>();
            for (int i = 6; i <= 8; i++)
            {
                if (i == 8 && GlobalServiceLocator.GameService.ActiveEndGameData?.WinningGroup == -1)
                    continue;
                options.Add(GlobalServiceLocator.LocalizationService.GetLocalizedString("GUI_REPORT_REASON" + i));
                ((Dictionary<int, int>)typeof(ReportPlayerController).GetField("reasonMap_", Instance | Public | NonPublic).GetValue(__instance)).Add(i, i);
            }
            if (options.Count != 0)
                __instance.ReasonDropdown.AddOptions(options);
        }

        private static void Postfix(ReportPlayerController __instance, ref IEnumerator __result) => __result = Start(__instance, __result);
    }
}
