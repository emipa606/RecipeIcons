﻿using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RecipeIcons
{
    [HarmonyPatch(typeof(HealthCardUtility), "GenerateSurgeryOption")]
    class PatchHealthCardUtility
    {
        static FieldInfo fieldShownItem = typeof(FloatMenuOption).GetField("shownItem", BindingFlags.NonPublic | BindingFlags.Instance);
        static FieldInfo fieldItemIcon = typeof(FloatMenuOption).GetField("itemIcon", BindingFlags.NonPublic | BindingFlags.Instance);
        

        static FloatMenuOption Postfix(FloatMenuOption option, Pawn pawn, Thing thingForMedBills, RecipeDef recipe, IEnumerable<ThingDef> missingIngredients, BodyPartRecord part)
        {
            if (fieldShownItem.GetValue(option) as ThingDef != null) return option;

            Texture2D itemIcon = fieldItemIcon.GetValue(option) as Texture2D;
            if (itemIcon != null) return option;

            Icon icon = Icon.getIcon(recipe);
            if (icon == Icon.missing) return option;

            fieldShownItem.SetValue(option, icon.thingDef);

            return option;
        }

    }
}
