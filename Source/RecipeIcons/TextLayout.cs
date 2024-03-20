using UnityEngine;
using Verse;

namespace RecipeIcons;

public class TextLayout
{
    private static Rect rect;
    private float calculatedHeight;


    private float calculatedWidth;

    public float lineHeight;

    private bool measuring;
    public float padding;
    public float startX;
    public float x;
    public float y;
    public float Width => calculatedWidth + padding;
    public float Height => calculatedHeight + lineHeight + padding;

    public void StartMeasuring()
    {
        measuring = true;
        calculatedWidth = 0;
        calculatedHeight = 0;
        x = padding;
        y = padding;
    }

    public void StartDrawing(float x, float y)
    {
        measuring = false;
        startX = this.x = x + padding;
        this.y = y + padding;
    }

    public void Text(string text)
    {
        var size = Verse.Text.CalcSize(text);

        if (!measuring)
        {
            rect.x = x;
            rect.y = y;
            rect.width = size.x;
            rect.height = size.y;
            Widgets.Label(rect, text);
        }

        x += size.x;
        if (x > calculatedWidth)
        {
            calculatedWidth = x;
        }

        if (y > calculatedHeight)
        {
            calculatedHeight = y;
        }
    }


    public void Icon(Icon icon, int width)
    {
        if (!measuring)
        {
            rect.x = x;
            rect.y = y + ((lineHeight - width) / 2) - 1;
            rect.width = width;
            rect.height = width;
            icon.Draw(rect);
        }

        x += width;
        if (x > calculatedWidth)
        {
            calculatedWidth = x;
        }

        if (y > calculatedHeight)
        {
            calculatedHeight = y;
        }
    }

    public void Newline()
    {
        x = startX;
        y += lineHeight;
    }
}