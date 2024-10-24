using BepInEx;
using HarmonyLib;
using static System.Reflection.BindingFlags;

namespace Eca.DawnOfSalem
{
    [BepInPlugin(GUID, Name, Version)]
    internal class Plugin : BaseUnityPlugin
    {
        private const string GUID = "Eca.DawnOfSalem";

        private const string Name = "DawnOfSalem";

        private const string Version = "1.0";

        internal static Harmony Harmony { get; } = new Harmony(GUID);

        private void Awake() => Harmony.PatchAll();

        [HarmonyPatch(typeof(LobbySceneChatListener), "HandleServerOnHowManyPlayersAndGames")]
        private class Reporter
        {
            private static void Postfix(LobbySceneChatListener __instance)
            {
                ChatController chatController = (ChatController)typeof(LobbySceneChatListener).GetField("chatController", Instance | Public | NonPublic).GetValue(__instance);
                chatController.AddMessage("<color=#631631>Dawn of Salem</color>");
            }
        }
    }
}
