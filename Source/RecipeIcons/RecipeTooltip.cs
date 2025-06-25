using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace RecipeIcons;

internal class RecipeTooltip
{
    private const int IconSize = 28;

    private static readonly FieldInfo fieldShownItem =
        typeof(FloatMenuOption).GetField("shownItem", BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly Dictionary<string, RecipeDef> recipeDatabase = new();

    private static readonly Color colorBgActive = new ColorInt(21, 25, 29).ToColor;
    private static readonly Color colorBorder = Color.white;
    private static readonly Color colorTextActive = Color.white;
    private static readonly Color colorTextIngCount = new(0.5f, 0.5f, 0.5f);
    private static readonly Color colorModName = new(0.5f, 0.5f, 0.5f);

    private readonly TextLayout layout = new();
    private readonly List<IconExplanation> needExplain = [];

    private static string recipeKey(RecipeDef def)
    {
        return (def.LabelCap + "|" + def.ProducedThingDef?.defName).Trim();
    }

    private static string recipeKey(FloatMenuOption option)
    {
        return (option.Label + "|" + (fieldShownItem.GetValue(option) as ThingDef)?.defName).Trim();
    }

    private static string recipeKeyFallback(FloatMenuOption option)
    {
        return $"{option.Label}|{null}";
    }

    private static RecipeDef findRecipe(FloatMenuOption option)
    {
        if (recipeDatabase.Count != 0)
        {
            return recipeDatabase.TryGetValue(recipeKey(option)) ??
                   recipeDatabase.TryGetValue(recipeKeyFallback(option));
        }

        foreach (var def in DefDatabase<RecipeDef>.AllDefs)
        {
            recipeDatabase[recipeKey(def)] = def;
        }

        return recipeDatabase.TryGetValue(recipeKey(option)) ?? recipeDatabase.TryGetValue(recipeKeyFallback(option));
    }

    public static bool IsRecipe(RecipeDef recipe)
    {
        return recipeDatabase.Values.Contains(recipe);
    }

    public static bool IsRecipe(FloatMenuOption option)
    {
        return findRecipe(option) != null;
    }

    public void ShowAt(FloatMenuOption option, float x, float y)
    {
        var recipe = findRecipe(option);
        if (recipe == null)
        {
            return;
        }

        layout.lineHeight = 24;
        layout.padding = 8;
        layout.StartMeasuring();
        Layout(recipe);

        var rectMenu = new Rect(x, y, layout.Width, layout.Height);
        if (rectMenu.y + rectMenu.height > UI.screenHeight)
        {
            rectMenu.y = UI.screenHeight - rectMenu.height;
        }

        Find.WindowStack.ImmediateWindow(1265324534, rectMenu, WindowLayer.Super,
            delegate { draw(rectMenu.AtZero(), recipe); }, false);
    }

    private void Layout(RecipeDef recipe)
    {
        var color = GUI.color;
        needExplain.Clear();

        Text.Anchor = TextAnchor.MiddleLeft;
        Text.Font = GameFont.Small;

        layout.Text($"{"RecipeIconsRecipeTooltipConsumes".Translate()} ");

        var first = true;
        foreach (var ing in recipe.ingredients)
        {
            if (!first)
            {
                layout.Text(" ");
            }
            else
            {
                first = false;
            }

            var icon = Icon.GetIcon(recipe, ing);
            var count = ing.GetBaseCount();

            ThingDef def = null;
            if (ing.IsFixedIngredient)
            {
                def = ing.FixedIngredient;
            }
            else if (ing.filter?.AllowedThingDefs != null)
            {
                def = ing.filter.AllowedThingDefs
                    .FirstOrDefault(x => recipe.fixedIngredientFilter.Allows(x) && !x.smallVolume) ?? ing.filter
                    .AllowedThingDefs
                    .FirstOrDefault(x => recipe.fixedIngredientFilter.Allows(x));
            }

            if (def != null)
            {
                var multiplier = recipe.IngredientValueGetter.ValuePerUnitOf(def);
                if (multiplier > 0)
                {
                    count /= multiplier;
                }
            }

            if (count != 1)
            {
                GUI.color = colorTextIngCount * color;
                layout.Text($"{count}x");
            }

            if (icon.isMissing)
            {
                icon = icon == Icon.Missing ? Icon.GetVariableIcon(needExplain.Count) : icon;
                needExplain.Add(new IconExplanation { filter = ing.filter, icon = icon });
            }

            GUI.color = colorTextActive * color;
            layout.Icon(icon, IconSize);
        }

        layout.Newline();

        GUI.color = color;

        ThingDef example = null;
        var products = recipe.products;
        var productsEmpty = products == null || products.Count == 0;
        var hasIngredients = recipe.ingredients is { Count: > 0 };

        if (productsEmpty && hasIngredients && recipe.specialProducts != null &&
            recipe.specialProducts.Contains(SpecialProductType.Butchery))
        {
            var defs = recipe.ingredients[0].filter.AllowedThingDefs;

            example = defs.FirstOrDefault(x => recipe.fixedIngredientFilter.Allows(x) && x.butcherProducts != null) ??
                      defs
                          .Where(x => recipe.fixedIngredientFilter.Allows(x))
                          .Select(x => Icon.CorpseMap.TryGetValue(x))
                          .FirstOrDefault(x => x?.butcherProducts != null);

            products = example?.butcherProducts;
        }

        if (products != null && (products.Count > 1 || products.Count == 1 && products[0].count > 1))
        {
            layout.Text($"{"RecipeIconsRecipeTooltipProduces".Translate()} ");

            first = true;
            foreach (var res in products)
            {
                if (!first)
                {
                    layout.Text(" ");
                }
                else
                {
                    first = false;
                }

                if (res.count != 1)
                {
                    GUI.color = colorTextIngCount * color;
                    layout.Text($"{res.count}x");
                }

                GUI.color = colorTextActive * color;
                layout.Icon(Icon.GetIcon(res.thingDef), IconSize);
            }

            if (example != null)
            {
                GUI.color = color;
                layout.Text($" ({"RecipeIconsRecipeTooltipExample".Translate()} ");
                layout.Icon(Icon.GetIcon(example), IconSize);
                GUI.color = color;
                layout.Text(")");
            }

            layout.Newline();
        }

        GUI.color = color;
        if (recipe.skillRequirements is { Count: > 0 })
        {
            layout.Text($"{"RecipeIconsRecipeTooltipRequires".Translate()} ");
            var count = 0;
            foreach (var req in recipe.skillRequirements)
            {
                if (count++ != 0)
                {
                    layout.Text(", ");
                }

                layout.Text(req.Summary);
            }

            layout.Newline();
        }

        GUI.color = color;
        if (needExplain.Count > 0)
        {
            layout.Newline();
            layout.Text("RecipeIconsRecipeTooltipWhere".Translate());
            layout.Newline();

            for (var i = 0; i < needExplain.Count; i++)
            {
                layout.Icon(needExplain[i].icon, IconSize);
                layout.Text($" {"RecipeIconsRecipeTooltipIsAnyOf".Translate()} ");

                var displayedCount = 0;
                foreach (var def in needExplain[i].filter.AllowedThingDefs
                             .Where(x => recipe.fixedIngredientFilter.Allows(x)))
                {
                    if (displayedCount > 9)
                    {
                        layout.Icon(Icon.More, IconSize);
                        break;
                    }

                    layout.Icon(Icon.GetIcon(def), IconSize);
                    displayedCount++;
                }

                layout.Newline();
            }
        }

        GUI.color = Color.grey;
        if (RecipeIcons.Settings.showMod && recipe.modContentPack is { IsCoreMod: false })
        {
            layout.Text(recipe.modContentPack.Name);

            layout.Newline();
        }

        GUI.color = color;
        Text.Anchor = TextAnchor.UpperLeft;
    }


    private void draw(Rect rect, RecipeDef recipe)
    {
        var color = GUI.color;

        GUI.color = colorBgActive * color;
        GUI.DrawTexture(rect, BaseContent.WhiteTex);

        GUI.color = colorBorder * color;
        Widgets.DrawAtlas(rect, TexUI.FloatMenuOptionBG);

        GUI.color = colorTextActive * color;

        GUI.color = color;

        layout.StartDrawing(rect.x, rect.y);
        Layout(recipe);
    }

    private struct IconExplanation
    {
        public ThingFilter filter;
        public Icon icon;
    }
}