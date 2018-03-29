using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Events;
namespace Painter
{
    public class PaintMgr : MonoBehaviour
    {
        [SerializeField]
        private Toggle grid;
        [SerializeField]
        private ToggleGroup shapeList;
        [SerializeField]
        private ToggleGroup groundList;
        public void SetShape(Texture2D tex)
        {
            Painter.Shape = tex;
        }
        public void SetGround(Texture2D tex)
        {
            Painter.Ground = tex;
        }
        public void SetSize(float size)
        {
            Painter.Size = (int)size;
        }
        public void SetOpacity(float opacity)
        {
            Painter.Opacity = opacity;
        }
        public void SetSpan(float span)
        {
            Painter.Span = span;
        }
        public void SetPaintType(int type)
        {
            Painter.Type = (PaintType)type;
        }
        public void Clear()
        {
            if (DrawModel.Target)
                DrawModel.Target.Clear();
        }
        public void UpdoDraw()
        {
            if (DrawModel.Target)
                DrawModel.Target.Undo();
        }

        private void Awake()
        {
            List<Texture2D> shapes = GetTextures(Application.streamingAssetsPath + "/Shape");
            for (int i = 0; i < shapes.Count; i++)
            {
                Texture2D t = shapes[i];
                CreateGrid(shapeList, t, i < 1, (b) => { if (b) Painter.Shape = t; });
                if (i < 1)
                    Painter.Shape = t;
            }
            List<Texture2D> grounds = GetTextures(Application.streamingAssetsPath + "/Ground");
            for (int i = 0; i < grounds.Count; i++)
            {
                Texture2D t = grounds[i];
                CreateGrid(groundList, t, i < 1, (b) => { if (b) Painter.Ground = t; });
                if (i < 1)
                    Painter.Ground = t;
            }
        }
        private List<Texture2D> GetTextures(string path)
        {
            List<Texture2D> ts = new List<Texture2D>();
            FileInfo[] fs = new DirectoryInfo(path).GetFiles();
            foreach (FileInfo f in fs)
            {
                if (Painter.Extensions.Contains(f.Extension.ToLower()))
                {
                    using (FileStream _f = f.OpenRead())
                    {
                        byte[] d = new byte[_f.Length];
                        _f.Read(d, 0, d.Length);
                        Texture2D t = new Texture2D(0, 0);
                        if (t.LoadImage(d))
                            ts.Add(t);
                    }
                }
            }
            return ts;
        }
        void CreateGrid(ToggleGroup group, Texture2D tex, bool isOn, UnityAction<bool> click)
        {
            Toggle t = Instantiate(grid);
            t.transform.SetParent(group.transform);
            t.targetGraphic.GetComponent<RawImage>().texture = tex;
            t.group = group;
            t.isOn = isOn;
            t.onValueChanged.AddListener(click);
        }
    }
}
