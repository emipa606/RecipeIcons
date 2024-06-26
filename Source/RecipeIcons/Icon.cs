﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;

namespace RecipeIcons;

[StaticConstructorOnStartup]
public class Icon
{
    public static readonly Icon missing = new Icon("RecipeIcons/Missing", true);
    public static readonly Icon A = new Icon("RecipeIcons/A", true);
    public static readonly Icon B = new Icon("RecipeIcons/B", true);
    public static readonly Icon C = new Icon("RecipeIcons/C", true);
    public static readonly Icon X = new Icon("RecipeIcons/X", true);
    public static readonly Icon More = new Icon("RecipeIcons/More");
    private static readonly Texture2D human = ContentFinder<Texture2D>.Get("RecipeIcons/Human");

    private static readonly Dictionary<string, Icon> mapStuffCategoryIcons = new Dictionary<string, Icon>();
    public static readonly Dictionary<ThingDef, ThingDef> corpseMap = new Dictionary<ThingDef, ThingDef>();

    private static readonly Rect rect0011 = new Rect(0f, 0f, 1f, 1f);

    private static readonly Dictionary<Def, Icon> map = new Dictionary<Def, Icon>();

    private static readonly Dictionary<string, Icon> mapCats = new Dictionary<string, Icon>();

    private static readonly FieldInfo fieldCategories =
        typeof(ThingFilter).GetField("categories", BindingFlags.NonPublic | BindingFlags.Instance);

    private static FieldInfo fieldStuffCategoriesToAllow =
        typeof(ThingFilter).GetField("stuffCategoriesToAllow", BindingFlags.NonPublic | BindingFlags.Instance);

    public readonly bool isMissing;

    public readonly Material material;
    public readonly Texture texture;
    public readonly Texture2D texture2D;
    public readonly ThingDef thingDef;
    public Color textureColor;

    static Icon()
    {
        mapStuffCategoryIcons[StuffCategoryDefOf.Metallic.defName] =
            new Icon(ContentFinder<Texture2D>.Get("RecipeIcons/Categories/Metallic"), true);
        mapStuffCategoryIcons[StuffCategoryDefOf.Woody.defName] =
            new Icon(ContentFinder<Texture2D>.Get("RecipeIcons/Categories/Woody"), true);
        mapStuffCategoryIcons[StuffCategoryDefOf.Stony.defName] =
            new Icon(ContentFinder<Texture2D>.Get("UI/Icons/ThingCategories/StoneBlocks"), true);
        mapStuffCategoryIcons[StuffCategoryDefOf.Fabric.defName] =
            new Icon(ContentFinder<Texture2D>.Get("UI/Icons/ThingCategories/Textiles"), true);
        mapStuffCategoryIcons[StuffCategoryDefOf.Leathery.defName] =
            new Icon(ContentFinder<Texture2D>.Get("UI/Icons/ThingCategories/Leathers"), true);
        mapStuffCategoryIcons["Gemstones"] =
            new Icon(ContentFinder<Texture2D>.Get("RecipeIcons/Categories/Gemstone"), true);

        foreach (var thing in DefDatabase<ThingDef>.AllDefs.Where(x => x.race?.corpseDef != null))
        {
            corpseMap[thing.race.corpseDef] = thing;
        }
    }

    public Icon(ThingDef thing)
    {
        thingDef = thing;
        if (thing == null)
        {
            return;
        }

        texture2D = thing.uiIcon == BaseContent.BadTex ? null : thing.uiIcon;
        textureColor = thing.uiIconColor;

        if (texture2D == null && corpseMap.TryGetValue(thing, out var value))
        {
            thingDef = thing = value;
            texture2D = thing.uiIcon == BaseContent.BadTex ? null : thing.uiIcon;
        }

        if (thing == ThingDefOf.Human)
        {
            texture2D = human;
        }

        if (thing.graphic == null || thing.graphicData == null ||
            thing.graphicData.shaderType != ShaderTypeDefOf.CutoutComplex || thing.graphic.MatSingle.NullOrBad())
        {
            return;
        }

        material = thing.graphic.MatSingle;
        texture = material.mainTexture;
    }

    public Icon(Texture2D tex, bool isMissing = false)
    {
        texture2D = tex;
        textureColor = Color.white;
        this.isMissing = isMissing;
    }

    public Icon(string texPath, bool isMissing = false) : this(ContentFinder<Texture2D>.Get(texPath), isMissing)
    {
    }

    public bool Draw(Rect rect)
    {
        if (material == null)
        {
            return Draw2D(rect);
        }

        if (texture.width != texture.height && rect.width == rect.height)
        {
            var ratio = texture.width / (float)texture.height;
            if (ratio < 1)
            {
                rect.x += (rect.width - (rect.width * ratio)) / 2;
                rect.width *= ratio;
            }
            else
            {
                rect.y += (rect.height - (rect.height / ratio)) / 2;
                rect.height /= ratio;
            }
        }


        Graphics.DrawTexture(rect, texture, rect0011, 0, 0, 0, 0, new Color(1, 1, 1, GUI.color.a), material, -1);
        return true;
    }

    public bool Draw2D(Rect rect)
    {
        if (texture2D == null)
        {
            return false;
        }

        if (texture2D.width != texture2D.height && rect.width == rect.height)
        {
            var ratio = texture2D.width / (float)texture2D.height;
            if (ratio < 1)
            {
                rect.x += (rect.width - (rect.width * ratio)) / 2;
                rect.width *= ratio;
            }
            else
            {
                rect.y += (rect.height - (rect.height / ratio)) / 2;
                rect.height /= ratio;
            }
        }

        GUI.color = new Color(textureColor.r, textureColor.g, textureColor.b, GUI.color.a);
        GUI.DrawTexture(rect, texture2D);
        return true;
    }

    public static Icon getIcon(Def def)
    {
        if (def == null)
        {
            return missing;
        }

        if (map.TryGetValue(def, out var res))
        {
            return res;
        }

        res = getIconWithoutCache(def);
        map.Add(def, res);
        return res;
    }

    private static Icon getIconWithoutCache(Def def)
    {
        if (def is RecipeDef recipe)
        {
            if (recipe.products != null)
            {
                foreach (var item in recipe.products)
                {
                    if (item.thingDef == null)
                    {
                        continue;
                    }

                    return new Icon(item.thingDef);
                }
            }

            foreach (var ing in recipe.ingredients)
            {
                if (ing == null)
                {
                    continue;
                }

                if (!ing.IsFixedIngredient)
                {
                    continue;
                }

                return new Icon(ing.FixedIngredient);
            }
        }

        if (def is ThingDef thing)
        {
            return new Icon(thing);
        }

        return missing;
    }

    public static Icon getIcon(RecipeDef recipe, IngredientCount ing)
    {
        Icon res;
        if (ing == null)
        {
            return missing;
        }

        if (ing.IsFixedIngredient)
        {
            return getIcon(ing.FixedIngredient);
        }

        if (ing.filter == null)
        {
            return missing;
        }

        if (fieldCategories.GetValue(ing.filter) is string name)
        {
            if (mapCats.TryGetValue(name, out res))
            {
                return res;
            }

            var cat = DefDatabase<ThingCategoryDef>.GetNamed(name, false);
            res = cat == null ? missing : new Icon(cat.icon);

            mapCats.Add(name, res);
            return res;
        }

        var def = recipe.ProducedThingDef;
        if (def?.stuffCategories == null)
        {
            return missing;
        }

        foreach (var cat in def.stuffCategories)
        {
            res = mapStuffCategoryIcons.TryGetValue(cat.defName);
            if (res != missing && res != null)
            {
                return res;
            }
        }

        return missing;
    }

    public static Icon getVariableIcon(int num)
    {
        switch (num)
        {
            case 0:
                return A;
            case 1:
                return B;
            case 2:
                return C;
            default:
                return X;
        }
    }
}