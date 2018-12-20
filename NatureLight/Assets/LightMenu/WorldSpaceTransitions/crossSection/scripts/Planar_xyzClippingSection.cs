using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace WorldSpaceTransitions
{
    public class Planar_xyzClippingSection : MonoBehaviour
    {
        CrossSectionObjectSetup cs_setup;
        private GameObject xyzSectionPanel;
        private Text topSliderLabel, middleSliderLabel, bottomSliderLabel;
        private Slider slider;
        private Toggle xtoggle, ytoggle, ztoggle, gizmotoggle;

        private Vector3 sectionplane = Vector3.up;

        public GameObject model;

        public Transform ZeroAlignment;

        public Bounds bounds;

        public enum ConstrainedAxis { X, Y, Z };
        public ConstrainedAxis selectedAxis = ConstrainedAxis.Y;

        private GameObject rectGizmo;

        private Vector3 zeroAlignmentVector = Vector3.zero;

        public bool gizmoOn = true;

        private Vector3 sliderRange = Vector3.zero;

        private float sectionX = 0;
        private float sectionY = 0;
        private float sectionZ = 0;

        void Awake()
        {
            xyzSectionPanel = GameObject.Find("xyzSectionPanel");
            if (xyzSectionPanel)
            {
                slider = xyzSectionPanel.GetComponentInChildren<Slider>();
                topSliderLabel = xyzSectionPanel.transform.Find("sliderPanel/MaxText").GetComponent<Text>();
                middleSliderLabel = xyzSectionPanel.transform.Find("sliderPanel/Slider").GetComponentInChildren<Text>();
                bottomSliderLabel = xyzSectionPanel.transform.Find("sliderPanel/MinText").GetComponent<Text>();
                if (xyzSectionPanel.transform.Find("axisOptions"))
                {
                    xtoggle = xyzSectionPanel.transform.Find("axisOptions/Panel/X_Toggle").GetComponent<Toggle>();
                    ytoggle = xyzSectionPanel.transform.Find("axisOptions/Panel/Y_Toggle").GetComponent<Toggle>();
                    ztoggle = xyzSectionPanel.transform.Find("axisOptions/Panel/Z_Toggle").GetComponent<Toggle>();
                    xtoggle.isOn = selectedAxis == ConstrainedAxis.X;
                    ytoggle.isOn = selectedAxis == ConstrainedAxis.Y;
                    ztoggle.isOn = selectedAxis == ConstrainedAxis.Z;
                }
                if (xyzSectionPanel.transform.Find("gizmoToggle"))
                {
                    gizmotoggle = xyzSectionPanel.transform.Find("gizmoToggle").GetComponent<Toggle>();
                    gizmotoggle.isOn = gizmoOn;
                }
            }
            if (ZeroAlignment) zeroAlignmentVector = ZeroAlignment.position;
        }

        void Start()
        {
            if (slider) slider.onValueChanged.AddListener(SliderListener);
            if (xyzSectionPanel) xyzSectionPanel.SetActive(enabled);
            Shader.DisableKeyword("CLIP_TWO_PLANES");
            Shader.EnableKeyword("CLIP_PLANE");
            Shader.SetGlobalVector("_SectionPlane", Vector3.up);
            if (xtoggle) xtoggle.onValueChanged.AddListener(delegate { SetAxis(xtoggle.isOn, ConstrainedAxis.X); });
            if (ytoggle) ytoggle.onValueChanged.AddListener(delegate { SetAxis(ytoggle.isOn, ConstrainedAxis.Y); });
            if (ztoggle) ztoggle.onValueChanged.AddListener(delegate { SetAxis(ztoggle.isOn, ConstrainedAxis.Z); });
            if (gizmotoggle) gizmotoggle.onValueChanged.AddListener(GizmoOn);

            sliderRange = new Vector3((float)SignificantDigits.CeilingToSignificantFigures((decimal)(1.08f * 2 * bounds.extents.x), 2),
            (float)SignificantDigits.CeilingToSignificantFigures((decimal)(1.08f * 2 * bounds.extents.y), 2),
            (float)SignificantDigits.CeilingToSignificantFigures((decimal)(1.08f * 2 * bounds.extents.z), 2));
            sectionX = bounds.min.x + sliderRange.x;
            sectionY = bounds.min.y + sliderRange.y;
            sectionZ = bounds.min.z + sliderRange.z;
            setupGizmo();
            setSection();
        }

        public void SliderListener(float value)
        {
            if (middleSliderLabel) middleSliderLabel.text = value.ToString("0.0");

            switch (selectedAxis)
            {
                case ConstrainedAxis.X:
                    sectionX = value + zeroAlignmentVector.x;
                    if (rectGizmo) rectGizmo.transform.position = new Vector3(sectionX, bounds.center.y, bounds.center.z);
                    break;
                case ConstrainedAxis.Y:
                    sectionY = value + zeroAlignmentVector.y;
                    if (rectGizmo) rectGizmo.transform.position = new Vector3(bounds.center.x, sectionY, bounds.center.z);
                    break;
                case ConstrainedAxis.Z:
                    sectionZ = value + zeroAlignmentVector.z;
                    if (rectGizmo) rectGizmo.transform.position = new Vector3(bounds.center.x, bounds.center.y, sectionZ);
                    break;
            }
            Shader.SetGlobalVector("_SectionPoint", new Vector3(sectionX, sectionY, sectionZ));
        }

        public void SetAxis(bool b, ConstrainedAxis a)
        {
            if (b)
            {
                selectedAxis = a;
                Debug.Log(a);
                RectGizmo rg = rectGizmo.GetComponent<RectGizmo>();
                rg.transform.position = Vector3.zero;
                rg.SetSizedGizmo(bounds.size, selectedAxis);
                setSection();
            }
        }

        void setSection()
        {
            float sliderMaxVal = 0f;
            float sliderVal = 0f;
            float sliderMinVal = 0f;
            Vector3 sectionpoint = new Vector3(sectionX, sectionY, sectionZ);
            Debug.Log(bounds.ToString());
            Debug.Log(selectedAxis.ToString());
            switch (selectedAxis)
            {
                case ConstrainedAxis.X:
                    sectionplane = Vector3.right;
                    rectGizmo.transform.position = new Vector3(sectionX, bounds.center.y, bounds.center.z);
                    sliderMaxVal = bounds.min.x + sliderRange.x - zeroAlignmentVector.x;
                    sliderVal = sectionX - zeroAlignmentVector.x;
                    sliderMinVal = bounds.min.x - zeroAlignmentVector.x;
                    break;
                case ConstrainedAxis.Y:
                    sectionplane = Vector3.up;
                    rectGizmo.transform.position = new Vector3(bounds.center.x, sectionY, bounds.center.z);
                    sliderMaxVal = bounds.min.y + sliderRange.y - zeroAlignmentVector.y;
                    sliderVal = sectionY - zeroAlignmentVector.y;
                    sliderMinVal = bounds.min.y - zeroAlignmentVector.y;
                    break;
                case ConstrainedAxis.Z:
                    sectionplane = Vector3.forward;
                    rectGizmo.transform.position = new Vector3(bounds.center.x, bounds.center.y, sectionZ);
                    sliderMaxVal = bounds.min.z + sliderRange.z - zeroAlignmentVector.z;
                    sliderVal = sectionZ - zeroAlignmentVector.z;
                    sliderMinVal = bounds.min.z - zeroAlignmentVector.z;
                    break;
                default:
                    Debug.Log("case default");
                    break;
            }

            Shader.SetGlobalVector("_SectionPoint", sectionpoint);
            Shader.SetGlobalVector("_SectionPlane", sectionplane);


            if (topSliderLabel) topSliderLabel.text = sliderMaxVal.ToString("0.0");
            if (bottomSliderLabel) bottomSliderLabel.text = sliderMinVal.ToString("0.0");

            if (slider)
            {
                slider.maxValue = sliderMaxVal;

                slider.value = sliderVal;
                middleSliderLabel.text = sliderVal.ToString("0.0");

                slider.minValue = sliderMinVal;
            }
        }

        void setupGizmo()
        {
            rectGizmo = Resources.Load("rectGizmo") as GameObject;
            if (rectGizmo) Debug.Log("rectGizmo");
            if (cs_setup) Debug.Log("cs_setup");
            rectGizmo = Instantiate(rectGizmo, bounds.center + (-bounds.extents.y + (slider ? slider.value : 0) + zeroAlignmentVector.y) * transform.up, Quaternion.identity) as GameObject;

            RectGizmo rg = rectGizmo.GetComponent<RectGizmo>();

            rg.SetSizedGizmo(bounds.size, selectedAxis);
            /* Set rectangular gizmo size here: inner width, inner height, border width.
             */
            rectGizmo.SetActive(false);

        }

        void OnEnable()
        {
            Shader.EnableKeyword("CLIP_PLANE");
            if (xyzSectionPanel) xyzSectionPanel.SetActive(true);
            if (slider)
            {
                Shader.SetGlobalVector("_SectionPoint", new Vector3(sectionX, sectionY, sectionZ));
            }
        }

        void OnDisable()
        {
            if (xyzSectionPanel) xyzSectionPanel.SetActive(false);
            Shader.DisableKeyword("CLIP_PLANE");
        }

        void OnApplicationQuit()
        {
            Shader.DisableKeyword("CLIP_PLANE");
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                Collider coll = model.GetComponent<Collider>();
                if (!coll) return;
                if (coll.Raycast(ray, out hit, 10000f))
                {
                    if (gizmoOn) rectGizmo.SetActive(true);
                    StartCoroutine(dragGizmo());
                }
                else
                {
                    rectGizmo.SetActive(false);
                }
            }
        }

        IEnumerator dragGizmo()
        {
            float cameraDistance = Vector3.Distance(bounds.center, Camera.main.transform.position);
            Vector3 startPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraDistance));
            Vector3 startPos = rectGizmo.transform.position;
            Vector3 translation = Vector3.zero;
            Camera.main.GetComponent<maxCamera>().enabled = false;
            if (slider) slider.onValueChanged.RemoveListener(SliderListener);
            while (Input.GetMouseButton(0))
            {
                translation = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraDistance)) - startPoint;
                Vector3 projectedTranslation = Vector3.Project(translation, sectionplane);
                Vector3 newPoint = startPos + projectedTranslation;
                switch (selectedAxis)
                {
                    case ConstrainedAxis.X:
                        if (newPoint.x > bounds.max.x) sectionX = bounds.max.x;
                        else if (newPoint.x < bounds.min.x) sectionX = bounds.min.x;
                        else sectionX = newPoint.x;
                        break;
                    case ConstrainedAxis.Y:
                        if (newPoint.y > bounds.max.y) sectionY = bounds.max.y;
                        else if (newPoint.y < bounds.min.y) sectionY = bounds.min.y;
                        else sectionY = newPoint.y;
                        break;
                    case ConstrainedAxis.Z:
                        if (newPoint.z > bounds.max.z) sectionZ = bounds.max.z;
                        else if (newPoint.z < bounds.min.z) sectionZ = bounds.min.z;
                        else sectionZ = newPoint.z;
                        break;
                }
                setSection();
                yield return null;
            }
            Camera.main.GetComponent<maxCamera>().enabled = true;
            if (slider) slider.onValueChanged.AddListener(SliderListener);
        }

        public void GizmoOn(bool val)
        {
            gizmoOn = val;
            if (rectGizmo) rectGizmo.SetActive(val);
        }

        public void Size(Bounds b)
        {
            bounds = b;
        }

    }
}
