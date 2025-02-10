using UnityEngine;

namespace FSF.Collection.Utilities{
public static class ColorUtility
{
    public static Color WithAlpha(this Color color, int alpha){
        return new Color(color.r, color.g, color.b, alpha / 255f);
    }

    public static Color WithAlpha(this Color color, float alpha){
        return new Color(color.r, color.g, color.b, alpha);
    }

    public static Texture2D ColorToTexture2D(Color color, int width = 1, int height = 1){
        Texture2D texture = new Texture2D(width, height);
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
        return texture;
    }
}}
