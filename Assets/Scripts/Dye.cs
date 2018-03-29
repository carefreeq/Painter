using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Painter
{
    public class Dye : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        [SerializeField]
        private Image view;
        [SerializeField]
        private RectTransform cursor;
        private Texture2D tex;
        private void Start()
        {
            tex = new Texture2D(Screen.width, Screen.height);
        }
        public void OnDrag(PointerEventData eventData)
        {
            view.color = tex.GetPixelBilinear(eventData.position.x / Screen.width, eventData.position.y / Screen.height);
            cursor.position = eventData.position;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Painter.Color = view.color;
            cursor.gameObject.SetActive(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            RenderTexture o = RenderTexture.active;
            RenderTexture oc = Camera.main.targetTexture;
            RenderTexture rc = new RenderTexture(tex.width, tex.height, 8);
            Camera.main.targetTexture = rc;
            Camera.main.Render();
            RenderTexture.active = rc;
            tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
            tex.Apply();
            Camera.main.targetTexture = oc;
            RenderTexture.active = o;
            Destroy(rc);
            view.color = tex.GetPixelBilinear(eventData.position.x / Screen.width, eventData.position.y / Screen.height);
            cursor.gameObject.SetActive(true);
            cursor.localScale = Vector3.one;
            cursor.position = eventData.position;
        }
    }
}