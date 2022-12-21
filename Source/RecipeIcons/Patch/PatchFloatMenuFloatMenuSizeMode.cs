using HarmonyLib;
using Verse;

namespace RecipeIcons.Patch;

[HarmonyPatch(typeof(FloatMenu), "SizeMode", MethodType.Getter)]
internal class PatchFloatMenuFloatMenuSizeMode
{
    private static FloatMenuSizeMode Postfix(FloatMenuSizeMode value)
    {
        return RecipeIcons.settings.disableSmallMenus ? FloatMenuSizeMode.Normal : value;
    }
}