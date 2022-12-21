using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace RecipeIcons.Patch;

[HarmonyPatch(typeof(HealthCardUtility), "GenerateSurgeryOption")]
internal class PatchHealthCardUtility
{
    private static FloatMenuOption Postfix(FloatMenuOption option, RecipeDef recipe)
    {
        if (option.shownItem != null)
        {
            return option;
        }

        if (option.itemIcon != null)
        {
            return option;
        }

        var icon = Icon.getIcon(recipe);
        if (icon == Icon.missing)
        {
            return option;
        }

        if (icon.thingDef == null)
        {
            return option;
        }

        option.shownItem = icon.thingDef;
        option.drawPlaceHolderIcon = false;
        option.iconColor = Color.white;
        option.forceThingColor = Color.white;

        return option;
    }
}