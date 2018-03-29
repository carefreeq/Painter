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
    }
}