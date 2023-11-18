#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ChangeAnimSpeed : EditorWindow
{
    AnimationClip anim;
    string dir;
    float mul;
    [MenuItem("Tools/HakuShiro/Anim/ChangeAnimSpeed")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow<ChangeAnimSpeed>();
    }

    void OnGUI()
    {
    anim = (AnimationClip)EditorGUILayout.ObjectField("animation", anim, typeof(AnimationClip), true);
    dir=(string)EditorGUILayout.TextField("出力ディレクトリ(Assets\\hoge)", dir);
    mul=(float)EditorGUILayout.FloatField("倍率", mul);


    if(anim!= null && dir !=""){
        if(GUILayout.Button("出力")){
            ChangeAndExportAnimations();
        }
    }

    }

    private void ChangeAndExportAnimations()
    {
        if (!AssetDatabase.IsValidFolder(dir))
        {
            Debug.LogError("Output directory doesn't exist!");
            return;
        }

        AnimationClip clip = anim;
        if (clip != null)
        {
            AnimationClip newClip = new AnimationClip();
            EditorUtility.CopySerialized(clip, newClip);

            EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(clip);
            foreach (EditorCurveBinding curveBinding in curveBindings)
            {
                AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, curveBinding);
                Keyframe[] keys = curve.keys;
                for (int i = 0; i < keys.Length; i++)
                {
                    keys[i].time /= mul; // キーフレームの時間を変更
                    keys[i].inTangent*=mul;
                    keys[i].outTangent*=mul;
                    
                }
                curve.keys = keys;
                AnimationUtility.SetEditorCurve(newClip, curveBinding, curve);
            }

            string outputPath = Path.Combine(dir, clip.name + "_x"+mul+".anim");
            AssetDatabase.CreateAsset(newClip, outputPath);
            Debug.Log("Animation speed changed and exported: " + outputPath);
        }
    }
}

#endif
