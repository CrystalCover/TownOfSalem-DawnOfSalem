using HarmonyLib;
using static Client.LoginType;
using static ClientXMLFlags;
using static ClientXMLFlags.ClientXmlFlag;
using static Crypto;
using static Scene;
using static StringUtils;
using static System.Convert;
using static System.Guid;
using static System.Reflection.BindingFlags;
using static System.String;
using static System.Text.Encoding;
using static UnityEngine.PlayerPrefs;
using System;
using System.Diagnostics;

namespace Eca.DawnOfSalem
{
    [HarmonyPatch(typeof(ApplicationService), nameof(ApplicationService.ShowScene))]
    internal class AutoLogin
    {
        private static bool AutoLoggedIn { get; set; } = false;

        private static string UsernameKey => (string)typeof(BaseLoginSceneController).GetField("LOGIN_KEY", Static | Public | NonPublic).GetValue(null);

        private static string PasswordKey => (string)typeof(BaseLoginSceneController).GetField("PASSWORD_KEY", Static | Public | NonPublic).GetValue(null);

        private static ISteamService SteamService => GlobalServiceLocator.SteamService;

        private static IUserService UserService => GlobalServiceLocator.UserService;

        private static byte[] KeyBytes => ASCII.GetBytes(HasKey(KeyBytesKey) ? SimpleEncryptDecrypt(GetString(KeyBytesKey), 653252328) : NewGuid().ToString("N"));

        private static string KeyBytesKey => (string)typeof(BaseLoginSceneController).GetField("CRYP_KEY", Static | Public | NonPublic).GetValue(null);

        private static byte[] IVBytes => ASCII.GetBytes(HasKey(IVBytesKey) ? SimpleEncryptDecrypt(GetString(IVBytesKey), 653252328) : NewGuid().ToString("N").Substring(0, 16));

        private static string IVBytesKey => (string)typeof(BaseLoginSceneController).GetField("IV_KEY", Static | Public | NonPublic).GetValue(null);

        private static string Username => AESDecrypt(FromBase64String(GetString(UsernameKey)), KeyBytes, IVBytes);

        private static string Password => AESDecrypt(FromBase64String(GetString(PasswordKey)), KeyBytes, IVBytes);

        private static bool Prefix(ApplicationService __instance, Scene scene)
        {
            if ((scene != Login && scene != BigLogin) || AutoLoggedIn)
                return true;
            AutoLoggedIn = true;
            if (!HasKey(UsernameKey) || IsNullOrEmpty(Username) || !HasKey(PasswordKey) || IsNullOrEmpty(Password) || Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
                return true;
            if (SteamService.IsInitialized)
                UserService.LoginViaSteamUsername(SteamService.GetUserSteamID(), Username, Password, SteamUsernamePassword, __instance.VersionInfo.build, GetClientXmlFlagSetting(AutoreportHostServerDisconnects));
            else
                UserService.Login(Username, Password, Web, __instance.VersionInfo.build, GetClientXmlFlagSetting(AutoreportHostServerDisconnects));
            return false;
        }
    }
}
