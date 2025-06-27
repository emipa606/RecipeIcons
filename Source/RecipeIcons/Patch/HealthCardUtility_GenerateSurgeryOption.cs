using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace RecipeIcons.Patch;

[HarmonyPatch(typeof(HealthCardUtility), "GenerateSurgeryOption")]
internal class HealthCardUtility_GenerateSurgeryOption
{
    private static void Postfix(ref FloatMenuOption __result, RecipeDef recipe)
    {
        var shownItemField = AccessTools.Field(typeof(FloatMenuOption), "shownItem");
        var itemIconField = AccessTools.Field(typeof(FloatMenuOption), "iconThing");
        var drawPlaceHolderIconField = AccessTools.Field(typeof(FloatMenuOption), "drawPlaceHolderIcon");

        if (shownItemField.GetValue(__result) != null || itemIconField.GetValue(__result) != null)
        {
            return;
        }

        var icon = Icon.GetIcon(recipe);
        if (icon == Icon.Missing || icon.thingDef == null)
        {
            return;
        }

        shownItemField.SetValue(__result, icon.thingDef);
        drawPlaceHolderIconField.SetValue(__result, false);
        __result.iconColor = Color.white;
        __result.forceThingColor = Color.white;
    }
}