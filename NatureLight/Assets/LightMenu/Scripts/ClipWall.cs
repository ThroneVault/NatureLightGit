using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ClipWall : MonoBehaviour
{

    void Start()
    {
        Shader.DisableKeyword("CLIP_PLANE");
        Shader.DisableKeyword("CLIP_CUBE");
        Shader.EnableKeyword("CLIP_CUBE");
        Shader.SetGlobalFloat("_Radius", 1f);
    }

    void Update()
    {

        Shader.SetGlobalVector("_SectionPoint", transform.position);
        Shader.SetGlobalVector("_SectionPlane", transform.forward);
        Shader.SetGlobalVector("_SectionPlane2", transform.up);
        //Shader.SetGlobalFloat("_Radius", 0.05f);


        Shader.SetGlobalVector("_SectionScale", transform.localScale);
    }

}
