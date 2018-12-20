using UnityEngine;


namespace WorldSpaceTransitions
{

    public class SinglePlaneSection : MonoBehaviour
    {


        void Start()
        {
            Shader.EnableKeyword("CLIP_PLANE");
        }


        void OnEnable()
        {
            Shader.EnableKeyword("CLIP_PLANE");
            //Shader.EnableKeyword("CLIP_PLANE");
        }

        void OnDisable()
        {
            Shader.DisableKeyword("CLIP_PLANE");
            //Shader.DisableKeyword("CLIP_PLANE");
        }

        void OnApplicationQuit()
        {
            //disable clipping so we could see the materials and objects in editor properly
            Shader.DisableKeyword("CLIP_PLANE");

        }

    }
}
