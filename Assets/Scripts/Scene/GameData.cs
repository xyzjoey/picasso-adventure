using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string[] colors;
    public int[] scores;

    public GameData(Color[] colors, int[] scores)
    {
        SetColors(colors);
        this.scores = scores;
    }

    public void SetColors(Color[] colors)
    {
        this.colors = new string[colors.Length];
        for (int i = 0; i < colors.Length; ++i) this.colors[i] = ColorUtility.ToHtmlStringRGBA(colors[i]);
    }

    public Color[] GetColors()
    {
        Color[] returnColors = new Color[colors.Length];

        for (int i = 0; i < colors.Length; ++i)
        {
            if (!ColorUtility.TryParseHtmlString("#" + colors[i], out returnColors[i]))
                returnColors[i] = new Color(1, 1, 1);
        }

        return returnColors;
    }

    public void SetScores(int[] scores)
    { this.scores = scores; }

    public int[] GetScores()
    { return scores; }
}
