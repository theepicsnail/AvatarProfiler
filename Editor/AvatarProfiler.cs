using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VRC.Core;

[CustomEditor(typeof(VRCSDK2.VRC_AvatarDescriptor))]
public class AvatarProfiler : AvatarDescriptorEditor
{

    private ProfilerResult result = null;
    public void OnEnable()
    {
        result = new ProfilerResult(((VRCSDK2.VRC_AvatarDescriptor)target).transform);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        result.OnInspectorGUI();
    }
    
    class ProfilerResult
    {
        private int bones = 0;
        private int renderers = 0;
        private int materials = 0;

        public ProfilerResult(Transform root)
        {
            profile(root);
        }
        
        private void profile(Transform root)
        {
            foreach (Renderer renderer in root.GetComponents<Renderer>())
            {
                renderers++;
                materials += renderer.materials.Length;
            }

            foreach (DynamicBone bone in root.GetComponents<DynamicBone>())
            {
                countBones(bone);
            }

            foreach (Transform child in root)
            {
                profile(child);
            }
        }

        private void countBones(DynamicBone bone)
        {
            Transform root = bone.m_Root;
            if (root == null)
            {
                return;
            }

            HashSet<Transform> excludes = new HashSet<Transform>();
            excludes.UnionWith(bone.m_Exclusions);

            bones += countNodes(root, excludes);
        }

        private int countNodes(Transform root, HashSet<Transform> excludes)
        {
            if (excludes.Contains(root)) return 0;

            int res = 1;
            foreach (Transform child in root)
            {
                res += countNodes(child, excludes);
            }

            return res;
        }

        public void OnInspectorGUI()
        {
            GUILayout.Label("Profiler:");
            EditorGUILayout.TextArea(string.Format(@"
Renderers: {0}
Bones: {1}
Materials: {2}
", renderers, bones, materials));
        }
    }
}
