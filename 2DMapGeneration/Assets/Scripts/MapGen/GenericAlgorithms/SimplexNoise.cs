using UnityEngine;
using System.Collections.Generic;
using System;
using System.Xml.Linq;

public class SimplexNoise
{
    private List<Vector2> gradients = new List<Vector2>()
    {
        new Vector2(1,0),
        new Vector2(0.9239f,0.3827f),
        new Vector2(0.7071f,0.7071f),
        new Vector2(0.3827f,0.9239f),
        new Vector2(0,1),
        new Vector2(-0.3827f,0.9239f),
        new Vector2(-0.7071f,0.7071f),
        new Vector2(-0.9239f,0.3827f),
        new Vector2(-1,0),
        new Vector2(-0.9239f,-0.3827f),
        new Vector2(-0.7071f,-0.7071f),
        new Vector2(-0.3827f,-0.9239f),
        new Vector2(0,-1),
        new Vector2(0.3827f,-0.9239f),
        new Vector2(0.7071f,-0.7071f),
        new Vector2(0.9239f,-0.3827f)
    };

    private float multiplier;

    public SimplexNoise(float multiplier)
    {
        this.multiplier = multiplier;
    }

    private int Hash(int x, int y)
    {
        int h = x * 374761393 + y * 668265263; // duże liczby pierwsze
        h = (h ^ (h >> 13)) * 1274126177;
        return h ^ (h >> 16);
    }

    private Vector2 GetHashedGradient(float x, float y)
    {
        int xi = (int)MathF.Floor(x);
        int yi = (int)MathF.Floor(y);

        int hash = Hash(xi, yi);
        int index = Math.Abs(hash) % gradients.Count;

        return gradients[index].normalized;
    }

    public float Sample(float x, float y)
    {
        float F = (MathF.Sqrt(3f) - 1f) / 2f;
        float G = (3f - MathF.Sqrt(3f)) / 6f;

        float xs = MathF.Floor(x + (x+y)*F);
        float ys = MathF.Floor(y + (x+y)*F);

        float X0 = xs - (xs+ys)*G;
        float Y0 = ys - (xs+ys)*G;

        float x0 = x-X0;
        float y0 = y-Y0;

        int xBigger = (x0 >= y0) ? 1 : 0;
        int yBigger = (x0 < y0) ? 1 : 0;

        float xs1 = xs + xBigger;
        float ys1 = ys + yBigger;
        float xs2 = xs + 1;
        float ys2 = ys + 1;

        float X1 = xs1 - (xs1+ys1)*G;
        float Y1 = ys1 - (xs1+ys1)*G;
        float X2 = xs2 - (xs2+ys2)*G;
        float Y2 = ys2 - (xs2+ys2)*G;

        float x1 = x-X1;
        float y1 = y-Y1;
        float x2 = x-X2;
        float y2 = y-Y2;

        float dot0 = Vector2.Dot(GetHashedGradient(xs,ys), new Vector2(x0, y0));
        float dot1 = Vector2.Dot(GetHashedGradient(xs1,ys1), new Vector2(x1, y1));
        float dot2 = Vector2.Dot(GetHashedGradient(xs2,ys2), new Vector2(x2, y2));

        float d0 = x0*x0 + y0*y0;
        float d1 = x1*x1 + y1*y1;
        float d2 = x2*x2 + y2*y2;

        float contr0 = MathF.Max(0, MathF.Pow((0.5f-d0), 4)) * dot0;
        float contr1 = MathF.Max(0, MathF.Pow((0.5f-d1), 4)) * dot1;
        float contr2 = MathF.Max(0, MathF.Pow((0.5f-d2), 4)) * dot2;

        return multiplier*(contr0 + contr1 + contr2); //for multiplier ~70f, result is about [-1,1]
    }
}