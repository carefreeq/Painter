using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Painter
{
    public class ColourMgr : MonoBehaviour
    {
        [RequireComponent(typeof(RawImage))]
        public class ColourZone : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
        {
            public static ColourZone Instance { get; private set; }
            public RectTransform Cursor { get; set; }
            private RectTransform rect;
            private CanvasScaler canvas;
            private Vector2 uv;
            private void Awake()
            {
                Instance = this;
                rect = GetComponent<RectTransform>();
                canvas = transform.GetComponentInParent<CanvasScaler>();
                if (canvas == null)
                    while (true)
                    {
                        canvas = canvas.GetComponentInParent<CanvasScaler>();
                        if (canvas != null)
                            break;
                    }
                Painter.ColorEvent += SetColor;
            }
            public void SetColor(Color color)
            {
                Rect r = MyRectToScreen();
                Vector3 c = Vector3.zero;
                Color.RGBToHSV(color, out c.z, out c.x, out c.y);
                Vector2 v = new Vector2(Mathf.Clamp(Mathf.Lerp(r.xMin, r.xMax, c.x), r.xMin + ColourMgr.ColourEdga.x, r.xMax - ColourMgr.ColourEdga.x), Mathf.Clamp(Mathf.Lerp(r.yMin, r.yMax, c.y), r.yMin + ColourMgr.ColourEdga.y, r.yMax - ColourMgr.ColourEdga.y));
                Cursor.position = v;
            }
            public void OnPointerDown(PointerEventData eventData)
            {
                Vector2 p = eventData.position;
                uv = GetUV(ref p);
                Cursor.position = p;
                ColourMgr.View.color = Dye();
            }
            public void OnDrag(PointerEventData eventData)
            {
                Vector2 p = eventData.position;
                uv = GetUV(ref p);
                Cursor.position = p;
                ColourMgr.View.color = Dye();
            }
            public void OnPointerUp(PointerEventData eventData)
            {
                Painter.Color = Dye();
            }
            public static Color Dye()
            {
                return ColourMgr.Colour.GetPixelBilinear(Instance.uv.x, Instance.uv.y);
            }
            Vector2 GetUV(ref Vector2 pos)
            {
                Rect r = MyRectToScreen();
                pos = new Vector2(Mathf.Clamp(pos.x, r.xMin + ColourMgr.ColourEdga.x, r.xMax - ColourMgr.ColourEdga.x), Mathf.Clamp(pos.y, r.yMin + ColourMgr.ColourEdga.y, r.yMax - ColourMgr.ColourEdga.y));
                Vector2 v = new Vector2(Mathf.InverseLerp(r.xMin, r.xMax, pos.x), Mathf.InverseLerp(r.yMin, r.yMax, pos.y));
                return v;
            }
            Rect MyRectToScreen()
            {
                Vector2 size = new Vector2(rect.rect.width / canvas.referenceResolution.x * Screen.width, rect.rect.height / canvas.referenceResolution.y * Screen.height);
                Vector2 pos = new Vector2(rect.position.x - size.x / 2, rect.position.y - size.y / 2);
                Rect r = new Rect(pos, size);
                return r;
            }
        }
        [RequireComponent(typeof(Slider))]
        public class ColourHue : MonoBehaviour, IDragHandler, IPointerUpHandler
        {
            public void OnDrag(PointerEventData eventData)
            {
                ColourMgr.View.color = ColourZone.Dye();
            }
            public void OnPointerUp(PointerEventData eventData)
            {
                ColourMgr.View.color = ColourZone.Dye();
                Painter.Color = ColourZone.Dye();
            }
        }
        public static ColourMgr Instance { get; private set; }
        public static Color Color { get; private set; }
        public static Image View { get { return Instance.view; } }
        public static Texture2D Colour { get; private set; }
        public static Vector2 ColourEdga { get; set; }
        [SerializeField]
        private RawImage colorHue;
        [SerializeField]
        private RawImage colorZone;
        [SerializeField]
        private Slider HueSlider;
        [SerializeField]
        private RectTransform cursor;
        [SerializeField]
        private Image view;
        [SerializeField]
        private Button[] history;
        void Awake()
        {
            Instance = this;
            ColourEdga = new Vector2(5f, 10f);
            HueSlider.maxValue = .999f;
            HueSlider.minValue = 0f;
            colorZone.gameObject.AddComponent<ColourZone>();
            ColourZone.Instance.Cursor = cursor;
            HueSlider.gameObject.AddComponent<ColourHue>();
            HueSlider.onValueChanged.AddListener(UpdateColorAtlas);

            Colour = new Texture2D(64, 64, TextureFormat.ARGB32, false);
            colorZone.texture = Colour;
            UpdateColorAtlas(.0f);
            colorHue.texture = CreateHue(4, 32);

            for (int i = 0; i < history.Length; i++)
            {
                Button b = history[i];
                b.onClick.AddListener(() => { view.color = Painter.Color = b.image.color; });
            }
            Painter.ColorEvent += (c) =>
            {
                for (int i = history.Length - 1; i > 0; i--)
                {
                    history[i].image.color = history[i - 1].image.color;
                }
                history[0].image.color = c;
            };
            Painter.ColorEvent += (c) =>
            {
                float h, s, v;
                Color.RGBToHSV(c, out h, out s, out v);
                HueSlider.value = h;
            };
        }
        public void UpdateColorAtlas(float i)
        {
            i = Mathf.Abs(i % 1f);
            Color[] c = new Color[Colour.GetPixels().Length];
            int size = (int)Mathf.Sqrt(c.Length);
            for (int y = 0; y < Colour.height; y++)
                for (int x = 0; x < Colour.width; x++)
                    c[x + y * size] = Color.HSVToRGB(i, (float)x / Colour.width, (float)y / Colour.height);
            Colour.SetPixels(c);
            Colour.Apply();
        }
        Texture2D CreateHue(int width, int height)
        {
            Texture2D h = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Color[] hc = new Color[width * height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    hc[x + y * width] = Color.HSVToRGB((float)y / height, 1f, 1f);
            h.SetPixels(hc);
            h.Apply();
            return h;
        }
    }
}