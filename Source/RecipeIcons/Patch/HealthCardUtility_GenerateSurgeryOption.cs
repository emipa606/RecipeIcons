using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace RecipeIcons.Patch;

[HarmonyPatch(typeof(HealthCardUtility), "GenerateSurgeryOption")]
internal class HealthCardUtility_GenerateSurgeryOption
{
    private static FloatMenuOption Postfix(FloatMenuOption option, RecipeDef recipe)
    {
        var shownItemField = AccessTools.Field(typeof(FloatMenuOption), "shownItem");
        var itemIconField = AccessTools.Field(typeof(FloatMenuOption), "itemIcon");
        var drawPlaceHolderIconField = AccessTools.Field(typeof(FloatMenuOption), "drawPlaceHolderIcon");

        if (shownItemField.GetValue(option) != null || itemIconField.GetValue(option) != null)
        {
            return option;
        }

        var icon = Icon.GetIcon(recipe);
        if (icon == Icon.Missing || icon.thingDef == null)
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