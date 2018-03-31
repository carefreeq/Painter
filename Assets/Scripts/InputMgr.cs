using UnityEngine;
using System.Collections;
namespace Painter
{
    public interface IInput
    {
        void Down(RaycastHit hit);
        void Drag(RaycastHit hit);
    }
    public class InputMgr : MonoBehaviour
    {
        public static bool IsUI { get; private set; }
        public static IInput Target { get; private set; }
        private Vector3 last;
        [SerializeField]
        private Material mat;
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                IsUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
                if (!IsUI)
                {
                    last = Input.mousePosition;
                    RaycastHit h = MainRay(last);
                    if (h.transform)
                    {
                        Target = h.transform.GetComponent<IInput>();
                        if (Target != null)
                        {
                            Target.Down(h);
                        }
                    }
                }
            }
            if (!IsUI && Input.GetMouseButton(0))
            {
                if (Vector3.Distance(last, Input.mousePosition) > 1e-4)
                {
                    last = Input.mousePosition;
                    RaycastHit h = MainRay(last);
                    if (h.transform != null)
                    {
                        IInput t = h.transform.GetComponent<IInput>();
                        if (Target == t)
                        {
                            Target.Drag(h);
                        }
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                Target = null;
                IsUI = false;
            }
        }
        private void OnRenderObject()
        {
            if (!IsUI)
            {
                RaycastHit h = MainRay(Input.mousePosition);
                if (h.transform)
                {
                    if (h.transform.GetComponent<IInput>() != null)
                    {
                        mat.SetColor("_Color",Painter.Color);
                        DrawWrieCircle(h.point, Quaternion.LookRotation(h.normal), mat, Painter.Size * 1e-3f);
                        DrawWrieLine(h.point, h.point + h.normal * Painter.Size * 1e-3f, mat);
                    }
                }
            }
        }
        public static RaycastHit MainRay(Vector3 point)
        {
            return MainRay(point, -1);
        }
        public static RaycastHit MainRay(Vector3 point, LayerMask mask)
        {
            return MainRay(point, 200f, mask);
        }
        public static RaycastHit MainRay(Vector3 point, float rayLength, LayerMask mask)
        {
            RaycastHit hit;
            Physics.Raycast(Camera.main.ScreenPointToRay(point), out hit, rayLength, mask);
            return hit;
        }
        private const float DoublePI = 6.28318530717958f;
        public static void DrawWrieCircle(Vector3 position, Quaternion rotation, Material mat, float radius = 1.0f)
        {
            float angle = DoublePI / 24;
            GL.PushMatrix();
            mat.SetPass(0);
            GL.Begin(GL.LINE_STRIP);
            for (float i = 0; i <= DoublePI; i += angle)
            {
                GL.Vertex(rotation * (new Vector3(Mathf.Cos(i) * radius, Mathf.Sin(i) * radius, 0.0f)) + position);
            }
            GL.End();
            GL.PopMatrix();
        }
        public static void DrawWrieLine(Vector3 from, Vector3 to, Material mat)
        {
            GL.PushMatrix();
            mat.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Vertex(from);
            GL.Vertex(to);
            GL.End();
            GL.PopMatrix();
        }
    }
}