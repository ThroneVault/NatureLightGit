/*
 The purpose of this script is to create cross-section material instances
 and - in case of capped sections - to scale the capped section prefabs to fit the model GameObject .
 */
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace WorldSpaceTransitions
{

    [ExecuteInEditMode]

    public class SectionSetup : MonoBehaviour
    {
        [Tooltip("Reassign after geometry change")]
        public GameObject model;
        public BoundsMode boundsMode = BoundsMode.world;
        private static BoundsMode bMode;

        public List<ShaderSubstitute> shaderSubstitutes;

        [System.Serializable]
        public struct ShaderSubstitute
        {
#if UNITY_EDITOR
            [ReadOnly]
#endif
            public Shader original;
            public Shader substitute;


            public ShaderSubstitute(Shader orig, Shader subst)
            {
                original = orig; substitute = subst;
            }
        }

        public enum BoundsMode { local, world };


        #if UNITY_EDITOR

        void OnValidate()
        {
            bMode = boundsMode;
            if (model)
            {
                transform.rotation = model.transform.rotation;

                Bounds bounds = GetBounds(model);
                Debug.Log(bounds.ToString());

                CappedSectionCorner csc = GetComponent<CappedSectionCorner>();
                if (csc) csc.Size(bounds);
                CappedSectionBox csb = GetComponent<CappedSectionBox>();
                if (csb) csb.Size(bounds);
                Planar_xyzClippingSection pcs = GetComponent<Planar_xyzClippingSection>();
                if (pcs) pcs.Size(bounds);
            }
        }

        public void CheckShaders()
        {
            List<Shader> shaderList = new List<Shader>();
            Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers)
            {
                Material[] mats = r.sharedMaterials;
                foreach (Material m in mats)
                {
                    Shader sh = m.shader;
                    if (!shaderList.Contains(sh)) shaderList.Add(sh);
                }
            }
            string shaderFamilyName = "CrossSection/";
            if (GetComponent<CappedSectionCorner>()) shaderFamilyName = "Clipping/Corner/";
            if (GetComponent<CappedSectionBox>()) shaderFamilyName = "Clipping/Box/";

            Debug.Log(shaderFamilyName);

            shaderSubstitutes.Clear();

            foreach (Shader sh in shaderList)
            {
                Shader substitute = getSubstitute(sh.name, shaderFamilyName);
                if (substitute != null) shaderSubstitutes.Add(new ShaderSubstitute(sh, substitute));
            }
        }

        public void CreateSectionMaterials()
        {
            Dictionary<Shader, Shader> shadersToreplace = new Dictionary<Shader, Shader>();
            foreach (ShaderSubstitute ssub in shaderSubstitutes)
            {
                shadersToreplace.Add(ssub.original, ssub.substitute);
            }
            Dictionary<Material, Material> materialsToreplace = new Dictionary<Material, Material>();

            Undo.RegisterFullObjectHierarchyUndo(model, "crossSection material assign");

            Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers)
            {
                Material[] mats = r.sharedMaterials;
                Material[] newMats = new Material[mats.Length];
                for (int i = 0; i < mats.Length; i++)
                {
                    Shader sh = mats[i].shader;
                    if (shadersToreplace.ContainsKey(sh))
                    {
                        if (!materialsToreplace.ContainsKey(mats[i]))
                        {
                            string fpath = AssetDatabase.GetAssetPath(mats[i]);
                            string newName = Path.GetFileNameWithoutExtension(fpath) + "_cs";
                            string dirname = Path.GetDirectoryName(fpath);
                            if (mats[i].name == "Default-Material") dirname = "Assets";
                            string newpath = Path.Combine(dirname, newName + ".mat");

                            Material newMaterial = (Material)AssetDatabase.LoadAssetAtPath(newpath, typeof(Material));
                            if (newMaterial == null)
                            {
                                newMaterial = new Material(mats[i]);
                                newMaterial.name = newName;
                                newMaterial.shader = shadersToreplace[mats[i].shader];
                                AssetDatabase.CreateAsset(newMaterial, newpath);
                            }
                            else
                            {
                                newMaterial.shader = shadersToreplace[mats[i].shader];
                            }
                            materialsToreplace.Add(mats[i], newMaterial);
                        }
                        newMats[i] = materialsToreplace[mats[i]];
                    }
                    else
                    {
                        newMats[i] = mats[i];
                    }
                }
                r.materials = newMats;
            }
        }
        #endif
        Bounds GetBounds(GameObject go)
        {
            Renderer[] allRenderers = go.GetComponentsInChildren<Renderer>();
            Quaternion quat = go.transform.rotation;//object axis AABB

            Bounds bounds = new Bounds();

            if (boundsMode == BoundsMode.local) go.transform.rotation = Quaternion.identity;

            MeshFilter[] meshes = go.GetComponentsInChildren<MeshFilter>();
            for (int i = 0; i < meshes.Length; i++)
            {
                Mesh ms = meshes[i].sharedMesh;
                int vc = ms.vertexCount;
                for (int j = 0; j < vc; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        bounds = new Bounds(meshes[i].transform.TransformPoint(ms.vertices[j]), Vector3.zero);
                    }
                    else
                    {
                        bounds.Encapsulate(meshes[i].transform.TransformPoint(ms.vertices[j]));
                    }
                }
            }
            Vector3 localCentre = go.transform.InverseTransformPoint(bounds.center);
            go.transform.rotation = quat;
            bounds.center = go.transform.TransformPoint(localCentre);
            return bounds;
        }

        Shader getSubstitute(string shaderName, string familyName)
        {
            if (shaderName.Contains(familyName)) return null;

            shaderName = shaderName.Replace("Legacy Shaders/", "");
            string newName = familyName + shaderName;
            newName = newName.Replace("Transparent/VertexLit", "Transparent/Specular");
            if (newName.Contains("Transparent/VertexLit"))
            {
                newName = familyName + "Transparent/Specular";
            }
            else if (newName.Contains("Transparent"))
            {
                newName = familyName + "Transparent/Diffuse";
            }

            if (!Shader.Find(newName)) newName = familyName + "Standard";

            return Shader.Find(newName);
        }

    }
}
