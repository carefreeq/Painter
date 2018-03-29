using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Painter
{
    public enum PaintType
    {
        paint = 0,
        eraser = 1,
        fuzzy = 2,
    }
    public static class Painter
    {
        public static string[] Extensions { get; private set; }
        private static Color color = new Color(1, 1, 1, 1);
        public static Color Color { get { return color; } set { if (ColorEvent != null) ColorEvent(value); color = value; } }
        public static event Action<Color> ColorEvent;
        public static Texture2D Shape { get; set; }
        public static Texture2D Ground { get; set; }
        public static int Size { get; set; }
        public static float Span { get; set; }
        public static float Opacity { get; set; }
        public static PaintType Type { get; set; }
        static Painter()
        {

            Extensions = new string[] { ".png", ".jpg" };
            Size = 32;
            Span = 0.01f;
            Opacity = 1.0f;
            Type = PaintType.paint;
        }
    }
}