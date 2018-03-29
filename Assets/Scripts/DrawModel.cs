using UnityEngine;
using System.Collections.Generic;
using System;

namespace Painter
{
    [RequireComponent(typeof(MeshCollider))]
    public class DrawModel : MonoBehaviour, IInput
    {
        public static DrawModel Target { get; private set; }
        private Texture2D canvas;
        private Texture2D source;
        private Texture2D fuzzy;
        private Vector2 last;

        private int undo = 0;
        private List<Color[]> step = new List<Color[]>();
        private void Start()
        {
            Texture2D t = (Texture2D)transform.GetComponent<MeshRenderer>().material.mainTexture;
            if (t)
            {
                canvas = new Texture2D(t.width, t.height, TextureFormat.ARGB32, false);
                canvas.SetPixels(t.GetPixels());
                canvas.Apply(false);
            }
            else
                canvas = new Texture2D(512, 512, TextureFormat.ARGB32, false);
            transform.GetComponent<MeshRenderer>().material.mainTexture = canvas;
            source = new Texture2D(canvas.width, canvas.height);
            source.SetPixels(canvas.GetPixels());
        }
        private void Draw(Vector2 point)
        {
            Vector2 s = new Vector2(Painter.Size, Painter.Size) * 0.5f;
            for (int y = 0; y < Painter.Size; y++)
                for (int x = 0; x < Painter.Size; x++)
                {
                    int _x = (int)(canvas.width * point.x - s.x) + x;
                    int _y = (int)(canvas.height * point.y - s.y) + y;
                    _x = _x < 0 ? _x + canvas.width : (_x > canvas.width ? _x % canvas.width : _x);
                    _y = _y < 0 ? _x + canvas.height : (_y > canvas.height ? _y % canvas.height : _y);
                    Color c = canvas.GetPixel(_x, _y);
                    float a = Painter.Shape.GetPixelBilinear((float)x / Painter.Size, (float)y / Painter.Size).a - 1 + Painter.Opacity;
                    switch (Painter.Type)
                    {
                        case PaintType.eraser:
                            canvas.SetPixel(_x, _y, Color.Lerp(c, source.GetPixel(_x, _y), a));
                            break;
                        case PaintType.fuzzy:
                            canvas.SetPixel(_x, _y, Color.Lerp(c, fuzzy.GetPixelBilinear((float)_x / canvas.width, (float)_y / canvas.height), a));
                            break;
                        case PaintType.paint:
                        default:
                            Color _c = Painter.Ground.GetPixel(_x, _y) * Painter.Color;
                            canvas.SetPixel(_x, _y, Color.Lerp(c, _c, a));
                            break;
                    }
                }
            canvas.Apply(false);
        }
        public void Clear()
        {
            canvas.SetPixels(source.GetPixels());
            canvas.Apply(false);
        }
        public void Undo()
        {
            if (step.Count > 0 && undo < step.Count)
            {
                canvas.SetPixels(step[step.Count - (++undo)]);
                canvas.Apply(false);
            }
        }
        public void Down(RaycastHit hit)
        {
            Target = this;
            if (step.Count > 10)
                step.RemoveAt(0);
            undo = 0;
            step.Add(canvas.GetPixels());
            //计算模糊贴图
            if (Painter.Type == PaintType.fuzzy)
                for (int i = 1; i < 20; i++)
                    fuzzy = ScaleTexture(canvas, canvas.width - i * 20);
            last = hit.point;
            Draw(hit.textureCoord);
        }
        public void Drag(RaycastHit hit)
        {
            if (Vector2.Distance(last, hit.point) > Painter.Span)
            {
                last = hit.point;
                Draw(hit.textureCoord);
            }
        }
        private Texture2D ScaleTexture(Texture2D t, int size)
        {
            Texture2D _t = new Texture2D(size, size, TextureFormat.ARGB32, false);
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                    _t.SetPixel(x, y, t.GetPixelBilinear((float)x / size, (float)y / size));
            _t.Apply(false);
            return _t;
        }
    }
}