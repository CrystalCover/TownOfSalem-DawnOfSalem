using HarmonyLib;
using static Client.Platform;

namespace Eca.DawnOfSalem
{
    [HarmonyPatch(typeof(BaseLoginSceneController), nameof(BaseLoginSceneController.HandleClickLogin))]
    internal class NonSteamLoginFixer
    {
        private static ApplicationController ApplicationController => ApplicationController.ApplicationContext;

        private static PlatformConfig PlatformConfig => ApplicationController.PlatformConfig;

        private static void Prefix() => PlatformConfig.Platform = WEB;

        private static void Postfix() => PlatformConfig.Platform = STEAM;
    }
}
