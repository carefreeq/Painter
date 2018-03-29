using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
namespace Painter
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private Transform target;
        [SerializeField]
        private new Camera camera;
        [SerializeField]
        private float distance = 5.0f;
        [SerializeField]
        private float maxdis = 10.0f;
        [SerializeField]
        private float mindis = 1.0f;
        [SerializeField]
        private Vector2 angle = new Vector2(45f,45f);

        void Start()
        {
            angle.x = camera.transform.localRotation.eulerAngles.x;
            angle.y = camera.transform.localRotation.eulerAngles.y;
        }

        void Update()
        {
            if (!InputMgr.IsUI)
            {
                if (Input.GetMouseButton(0))
                {
                    if (InputMgr.Target == null)
                    {
                        angle.y += Input.GetAxis("Mouse X") * 4f;
                        angle.x -= Input.GetAxis("Mouse Y") * 3f;
                        angle.x = Mathf.Clamp(angle.x % 360, -90f, 90f);
                        camera.transform.rotation = Quaternion.Euler(angle.x, angle.y, 0);
                    }
                }
                distance -= Input.GetAxis("Mouse ScrollWheel") * 2f;
                distance = Mathf.Clamp(distance, mindis, maxdis);
                camera.transform.position = camera.transform.rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;
            }
        }
    }
}