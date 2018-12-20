using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WorldSpaceTransitions
{
    [ExecuteInEditMode]
    public class SectionPlaneHatched : MonoBehaviour
    {
        public GameObject toBeSectioned;
        [SerializeField]
        private Bounds bounds;
        private Transform gizmo;

        void Start()
        {
            GizmoFollow g = (GizmoFollow)FindObjectOfType(typeof(GizmoFollow));
            if (g)
            {
                gizmo = g.transform;
            }
            this.enabled = g;
        }

        void Update()
        {
            transform.rotation = gizmo.rotation * Quaternion.Euler(180, 0, 0);
            Plane plane = new Plane(gizmo.forward, gizmo.position);
            Ray ray = new Ray(bounds.center, gizmo.forward);
            float rayDistance;
            if (plane.Raycast(ray, out rayDistance))
            {
                transform.position = ray.GetPoint(rayDistance);
            }
            else
            {
                ray = new Ray(bounds.center, -gizmo.forward);
                if (plane.Raycast(ray, out rayDistance)) transform.position = ray.GetPoint(rayDistance);
            }
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (toBeSectioned) bounds = CrossSectionObjectSetup.CalculateMeshBounds(toBeSectioned);
            transform.localScale = Vector3.one * bounds.size.magnitude;
            transform.position = bounds.center;
        }
#endif
    }
}
