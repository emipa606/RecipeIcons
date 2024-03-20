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
        var shownItemField = AccessTools.Field(typeof(FloatMenuOption), "shownItem");
        var itemIconField = AccessTools.Field(typeof(FloatMenuOption), "itemIcon");
        var drawPlaceHolderIconField = AccessTools.Field(typeof(FloatMenuOption), "drawPlaceHolderIcon");

        if (shownItemField.GetValue(option) != null)
        {
            return option;
        }

        if (itemIconField.GetValue(option) != null)
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

        shownItemField.SetValue(option, icon.thingDef);
        drawPlaceHolderIconField.SetValue(option, false);
        option.iconColor = Color.white;
        option.forceThingColor = Color.white;

        return option;
    }
}