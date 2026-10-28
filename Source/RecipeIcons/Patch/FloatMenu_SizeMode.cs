using HarmonyLib;
using Verse;

namespace RecipeIcons.Patch;

[HarmonyPatch(typeof(FloatMenu), nameof(FloatMenu.SizeMode), MethodType.Getter)]
internal static class FloatMenu_SizeMode
{
    public static FloatMenuSizeMode Postfix(FloatMenuSizeMode value)
    {
        return RecipeIcons.Settings.disableSmallMenus ? FloatMenuSizeMode.Normal : value;
    }
}