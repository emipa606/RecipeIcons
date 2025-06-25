using HarmonyLib;
using Verse;

namespace RecipeIcons.Patch;

[HarmonyPatch(typeof(FloatMenu), nameof(FloatMenu.SizeMode), MethodType.Getter)]
internal class FloatMenu_SizeMode
{
    private static FloatMenuSizeMode Postfix(FloatMenuSizeMode value)
    {
        return RecipeIcons.Settings.disableSmallMenus ? FloatMenuSizeMode.Normal : value;
    }
}