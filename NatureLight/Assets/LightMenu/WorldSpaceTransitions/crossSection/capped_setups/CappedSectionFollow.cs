//The purpose of this script is to setup and update the global shader properties for the capped sections 

using UnityEngine;
using CreateAndOperate;

public class CappedSectionFollow : MonoBehaviour {

    private enum Mode { box, corner };
    private Mode sectionMode;

    private Vector3 tempPos;
    private Vector3 tempScale;
    private Quaternion tempRot;


    //用来保存在allCreatedPrefabs中的信息，用来实时更新对应的参数
    [HideInInspector]
    public AllCreatedPrefabs FatherScript;
    [HideInInspector]
    public int index = 0;



    public bool followPosition = true;
    //public bool followRotation = true;

    void Awake()
    {
        if (gameObject.GetComponent<CappedSectionBox>()) sectionMode = Mode.box;
        if (gameObject.GetComponent<CappedSectionCorner>()) sectionMode = Mode.corner;
    }
    void Start()
    {
        Shader.SetGlobalColor("_SectionColor", Color.black);
        SetSection();
    }

    void Update () {
	
		if(tempPos!=transform.position || tempRot != transform.rotation || tempScale != transform.localScale)
        {

			tempPos = transform.position;
			tempRot = transform.rotation;
            tempScale = transform.localScale;
			SetSection();
		}
        //Shader.SetGlobalVector("_SectionDirX", transform.right);
        //  Shader.SetGlobalVector("_SectionDirY", transform.up);
        //Shader.SetGlobalVector("_SectionDirZ", transform.forward);


        if (FatherScript != null)
        {
            FatherScript.SectionDirX[index] = transform.right;
            FatherScript.SectionDirY[index] = transform.up;
            FatherScript.SectionDirZ[index] = transform.forward;
        }


    }


    void OnDisable() {

        Shader.DisableKeyword("CLIP_BOX");
        Shader.DisableKeyword("CLIP_CORNER");
	}

    void OnEnable()
    {
        if (sectionMode == Mode.box)  Shader.EnableKeyword("CLIP_BOX");
        if (sectionMode == Mode.corner) Shader.EnableKeyword("CLIP_CORNER");
        SetSection();
    }


    void OnApplicationQuit()
    {
        Shader.DisableKeyword("CLIP_BOX");
        Shader.DisableKeyword("CLIP_CORNER");
    }

    void SetSection()
    {
        if (followPosition)
        {
            // Shader.SetGlobalVector("_SectionCentre", transform.position);
            //Shader.SetGlobalVector("_SectionScale", transform.localScale);

            if (FatherScript != null)
            {
                FatherScript.SectionCentre[index] = transform.position;
                FatherScript.SectionScale[index] = transform.localScale;
            }

        }
	}

}