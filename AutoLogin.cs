using HarmonyLib;
using System;
using UnityEngine;
using static Client.LoginType;
using static Scene;

namespace Eca.DawnOfSalem
{
    [HarmonyPatch(typeof(ApplicationService))]
    internal class AutoLogin
    {
        private static bool AutoLoggedIn { get; set; } = false;

        private static ISteamService SteamService => GlobalServiceLocator.SteamService;

        private static IUserService UserService => GlobalServiceLocator.UserService;

        private static readonly byte[] keyBytes = Array.ConvertAll(new[] { 100, 49, 98, 51, 53, 48, 54, 48, 51, 55, 57, 56, 52, 100, 55, 48, 57, 98, 48, 100, 98, 57, 49, 48, 97, 56, 51, 97, 53, 48, 52, 49 }, @int => Convert.ToByte(@int));

        private static readonly byte[] ivBytes = Array.ConvertAll(new[] { 100, 50, 53, 100, 50, 48, 99, 57, 101, 50, 50, 51, 52, 101, 48, 48 }, @int => Convert.ToByte(@int));

        private static string Username => Crypto.AESDecrypt(Convert.FromBase64String(PlayerPrefs.GetString("Login.Username")), keyBytes, ivBytes);

        private static string Password => Crypto.AESDecrypt(Convert.FromBase64String(PlayerPrefs.GetString("Login.Password")), keyBytes, ivBytes);

        [HarmonyPatch("ShowScene")]
        [HarmonyPrefix]
        private static bool ShowScene(ApplicationService __instance, Scene scene)
        {
            if ((scene != Login && scene != BigLogin) || AutoLoggedIn)
                return true;
            AutoLoggedIn = true;
            if (!PlayerPrefs.HasKey("Login.Username") || !PlayerPrefs.HasKey("Login.Password"))
                return true;
            if (SteamService.IsInitialized)
                UserService.LoginViaSteamUsername(SteamService.GetUserSteamID(), Username, Password, SteamUsernamePassword, __instance.VersionInfo.build, ClientXMLFlags.GetClientXmlFlagSetting(ClientXMLFlags.ClientXmlFlag.AutoreportHostServerDisconnects));
            else
                UserService.Login(Username, Password, Web, __instance.VersionInfo.build, ClientXMLFlags.GetClientXmlFlagSetting(ClientXMLFlags.ClientXmlFlag.AutoreportHostServerDisconnects));
            return false;
        }
    }
}
