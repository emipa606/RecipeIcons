﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RecipeIcons
{
    class RecipeTooltip
    {

        static FieldInfo fieldShownItem = typeof(FloatMenuOption).GetField("shownItem", BindingFlags.NonPublic | BindingFlags.Instance);
        static Dictionary<string, RecipeDef> recipeDatabase = new Dictionary<string, RecipeDef>();
        static string RecipeKey(RecipeDef def) { return def.LabelCap + "|" + def.ProducedThingDef?.defName; }
        static string RecipeKey(FloatMenuOption option) { return option.Label + "|" + (fieldShownItem.GetValue(option) as ThingDef)?.defName; }

        private static readonly Color ColorBGActive = new ColorInt(21, 25, 29).ToColor;
        private static readonly Color ColorBorder = new ColorInt(255, 255, 0).ToColor;
        private static readonly Color ColorTextActive = Color.white;
        private static readonly Color ColorTextIngCount = new Color(0.5f, 0.5f, 0.5f);
        private static readonly int iconSize = 28;

        TextLayout layout = new TextLayout();
        struct IconExplanation {
            public ThingFilter filter;
            public Icon icon;
        }
        List<IconExplanation> needExplain = new List<IconExplanation>();

        static RecipeDef FindRecipe(FloatMenuOption option)
        {
            if (recipeDatabase.Count == 0)
            {
                foreach (RecipeDef def in DefDatabase<RecipeDef>.AllDefs)
                {
                    recipeDatabase[RecipeKey(def)] = def;
                }
            }

            return recipeDatabase.TryGetValue(RecipeKey(option));
        }


        public void ShowAt(FloatMenuOption option, float x, float y)
        {
            RecipeDef recipe = FindRecipe(option);
            if (recipe == null) return;

            layout.lineHeight = 24;
            layout.padding = 8;
            layout.StartMeasuring();
            Layout(recipe);

            Rect rectMenu = new Rect(x, y, layout.Width, layout.Height);
            if (rectMenu.y + rectMenu.height > UI.screenHeight)
            {
                rectMenu.y = UI.screenHeight - rectMenu.height;
            }

            Find.WindowStack.ImmediateWindow(1265324534, rectMenu, WindowLayer.Super, delegate
            {
                Draw(rectMenu.AtZero(), recipe);
            }, false, false, 1);
        }

        void Layout(RecipeDef recipe)
        {
            Color color = GUI.color;
            needExplain.Clear();

            Text.Anchor = TextAnchor.MiddleLeft;
            Text.Font = GameFont.Small;

            layout.Text("Consumes: ");

            bool first = true;
            foreach (IngredientCount ing in recipe.ingredients)
            {
                if (!first) layout.Text(" ");
                else first = false;

                Icon icon = Icon.getIcon(recipe, ing);
                float count = ing.GetBaseCount();

                if (ing.filter?.AllowedThingDefs != null)
                {
                    ThingDef def = ing.filter.AllowedThingDefs.Where(x => recipe.fixedIngredientFilter.Allows(x) && !x.smallVolume).FirstOrDefault();
                    if (def == null) def = ing.filter.AllowedThingDefs.Where(x => recipe.fixedIngredientFilter.Allows(x)).FirstOrDefault();

                    if (def != null)
                    {
                        float multiplier = recipe.IngredientValueGetter.ValuePerUnitOf(def);
                        if (multiplier > 0) count /= multiplier;
                    }
                }

                if (count != 1)
                {
                    GUI.color = ColorTextIngCount * color;
                    layout.Text(count + "x");
                }

                if (icon.isMissing)
                {
                    icon = icon == Icon.missing ? Icon.getVariableIcon(needExplain.Count) : icon;
                    needExplain.Add(new IconExplanation { filter = ing.filter, icon = icon } );
                }

                GUI.color = ColorTextActive * color;
                layout.Icon(icon, iconSize);
            }
            layout.Newline();

            GUI.color = color;

            ThingDef example = null;
            List<ThingDefCountClass> products = recipe.products;
            if (products.Count == 0 && recipe.specialProducts != null && recipe.specialProducts.Contains(SpecialProductType.Butchery) && recipe.ingredients.Count > 0)
            {
                example = recipe.ingredients[0].filter.AllowedThingDefs.FirstOrDefault(x => recipe.fixedIngredientFilter.Allows(x) && x.butcherProducts != null);
                if (example == null) example = recipe.ingredients[0].filter.AllowedThingDefs.Where(x => recipe.fixedIngredientFilter.Allows(x)).Select(x => Icon.corpseMap.TryGetValue(x)).FirstOrDefault(x => x?.butcherProducts != null);

                products = example?.butcherProducts;
            }

            if (products != null && (products.Count > 1 || (products.Count == 1 && products[0].count > 1)))
            {
                layout.Text("Produces: ");

                first = true;
                foreach (ThingDefCountClass res in products)
                {
                    if (!first) layout.Text(" ");
                    else first = false;

                    if (res.count != 1)
                    {
                        GUI.color = ColorTextIngCount * color;
                        layout.Text(res.count + "x");
                    }

                    GUI.color = ColorTextActive * color;
                    layout.Icon(Icon.getIcon(res.thingDef), iconSize);
                }

                if (example != null)
                {
                    GUI.color = color;
                    layout.Text(" (example for ");
                    layout.Icon(Icon.getIcon(example), iconSize);
                    GUI.color = color;
                    layout.Text(")");
                }

                layout.Newline();
            }

            GUI.color = color;
            if (recipe.skillRequirements != null && recipe.skillRequirements.Count > 0)
            {
                layout.Text("Requires: ");
                int count = 0;
                foreach (SkillRequirement req in recipe.skillRequirements)
                {
                    if (count++ != 0) layout.Text(", ");
                    layout.Text(req.Summary);
                }
                layout.Newline();
            }

            GUI.color = color;
            if (needExplain.Count > 0)
            {
                layout.Newline();
                layout.Text("Where: ");
                layout.Newline();

                for (int i = 0; i < needExplain.Count; i++)
                {
                    layout.Icon(needExplain[i].icon, iconSize);
                    layout.Text(" is any of: ");

                    int displayedCount = 0;
                    foreach (ThingDef def in needExplain[i].filter.AllowedThingDefs.Where(x => recipe.fixedIngredientFilter.Allows(x)))
                    {
                        if (displayedCount > 9)
                        {
                            layout.Icon(Icon.More, iconSize);
                            break;
                        }

                        layout.Icon(Icon.getIcon(def), iconSize);
                        displayedCount++;
                    }

                    layout.Newline();
                }
            }
            Text.Anchor = TextAnchor.UpperLeft;
        }


        void Draw(Rect rect, RecipeDef recipe)
        {

            Color color = GUI.color;

            GUI.color = ColorBGActive * color;
            GUI.DrawTexture(rect, BaseContent.WhiteTex);

            GUI.color = ColorBorder * color;
            Widgets.DrawAtlas(rect, TexUI.FloatMenuOptionBG);

            GUI.color = ColorTextActive * color;

            GUI.color = color;

            layout.StartDrawing(rect.x, rect.y);
            Layout(recipe);
        }
    }
}
