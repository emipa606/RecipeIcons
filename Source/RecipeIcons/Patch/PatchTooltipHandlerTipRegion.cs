using HarmonyLib;
using RimWorld;
using Verse;

namespace RecipeIcons.Patch;

[HarmonyPatch(typeof(BillUtility), "LayoutTooltip")]
internal static class PatchTooltipHandlerTipRegion
{
    private static bool Prefix(RecipeDef recipe)
    {
        if (!RecipeIcons.settings.enableTooltip)
        {
            return true;
        }

        return !RecipeTooltip.IsRecipe(recipe);
    }
}