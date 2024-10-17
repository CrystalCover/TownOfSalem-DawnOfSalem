using HarmonyLib;

namespace Eca.DawnOfSalem
{
    internal class StoryAndPatchNotesOnHomeScreen
    {
        //[HarmonyPatch(typeof(BaseHomeSceneController))]
        private class BaseHomeSceneControllerFix
        {
        }

        //[HarmonyPatch(typeof(BaseLoginSceneController))]
        private class BaseLoginSceneControllerFix
        {
        }
    }
}