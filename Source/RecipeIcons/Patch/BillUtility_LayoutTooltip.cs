using HarmonyLib;
using RimWorld;
using Verse;

namespace RecipeIcons.Patch;

[HarmonyPatch(typeof(BillUtility), "LayoutTooltip")]
internal static class BillUtility_LayoutTooltip
{
    private static bool Prefix(RecipeDef recipe)
    {
        if (!RecipeIcons.Settings.enableTooltip)
        {
            return true;
        }

        return !RecipeTooltip.IsRecipe(recipe);
    }
}