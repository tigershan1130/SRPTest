﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;


public class SRPChanger : EditorWindow
{
    private bool useSRP;
    [MenuItem("Tools/SRPChanger")]
    static void Create()
    {
        EditorWindow.GetWindow<SRPChanger>();
    }

    void OnEnable()
    {
        useSRP =  (GraphicsSettings.renderPipelineAsset != null);
    }

    // GUI処理
    void OnGUI()
    {
        EditorGUILayout.LabelField("ScriptableRenderPipeline(SRP)的切换");
        EditorGUILayout.LabelField("创建自定义管线");
        EditorGUILayout.LabelField("");
        if (EditorApplication.isPlaying)
        {
            EditorGUILayout.LabelField("运行期间无法更改");
            if (useSRP)
            {
                EditorGUILayout.LabelField("SRP使用中 ");
            }
            else
            {
                EditorGUILayout.LabelField("SRP未使用");
            }
            return;
        }
        bool oldFlag = useSRP;
        useSRP = EditorGUILayout.Toggle("是否使用srp？", useSRP);
        if (oldFlag != useSRP)
        {
            if (useSRP)
            {
                UseSRP();
            }
            else
            {
                NonSRP();
            }
        }
    }

    private static void UseSRP()
    {
        var instance = ScriptableObject.CreateInstance<MyScriptableRenderPipeline>();
        instance.name = "MyPipeline";
        AssetDatabase.CreateAsset(instance, "Assets/Datas/SRP/MyScriptableRenderPipeline.asset");
        GraphicsSettings.renderPipelineAsset = instance;
        ChangeShader(true);
    }

    private static void NonSRP()
    {
        GraphicsSettings.renderPipelineAsset = null;
        ChangeShader(false);
    }

    private static void ChangeShader(bool useSRP)
    {
        var guids = AssetDatabase.FindAssets("t:material");
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null || mat.shader ==null || mat.shader.name == null) { continue; }
            string newShader = mat.shader.name;
            if (useSRP)
            {
                newShader = newShader.Replace("App/NonSRP/", "App/SRP/");
            }
            else
            {
                newShader = newShader.Replace("App/SRP/", "App/NonSRP/");
            }
            mat.shader = Shader.Find(newShader);
            EditorUtility.SetDirty(mat);
        }
        AssetDatabase.SaveAssets();
    }

}
