using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class EnvironmentCheck
{
    private static SerializedObject tagManager;
    private static SerializedProperty layers;
    private static SerializedProperty sortingLayers;
    private static List<string> sortingLayerNames;
    private static List<int> sortingLayerIds;
    private static int sortingLayerDefaultIndex;

    private enum ModifySortingLayerType
    {
        None,
        ChangeId,
        CreateNew
    }
    private enum AddSortingLayerOrder
    {
        FromBegin,
        FromEnd
    }

    static EnvironmentCheck()
    {
        tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

        sortingLayerIds = new List<int>();
        sortingLayerNames = new List<string>();

        Debug.Log("#### Check environment ####");
        Debug.Log("--- check layers ---");

        layers = tagManager.FindProperty("layers");

        AddLayer("SpecialMapping", 31);
        AddLayer("WorldAnchor", 30);
        AddLayer("FloatKeyboard", 29);

        Debug.Log("--- check sorting layers ---");
         

        ReadSortingLayers();

        SetSortingLayers("Background", 100, false, AddSortingLayerOrder.FromBegin);
        SetSortingLayers("Anchor", 101, false, AddSortingLayerOrder.FromEnd);
        SetSortingLayers("NormalEffect", 102, false, AddSortingLayerOrder.FromEnd);
        SetSortingLayers("UI", 103, false, AddSortingLayerOrder.FromEnd);
        SetSortingLayers("UIEffect", 104, false, AddSortingLayerOrder.FromEnd);
        SetSortingLayers("Float", 105, false, AddSortingLayerOrder.FromEnd);
        SetSortingLayers("System", 106, false, AddSortingLayerOrder.FromEnd);

        Debug.Log("#### Check environment Finish! ####");
    }
     
    static bool isHasLayer(string layer)
    {
        for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.layers.Length; i++)
        {
            if (UnityEditorInternal.InternalEditorUtility.layers[i].Contains(layer))
                return true;
        }
        return false;
    }

    private static void AddLayer(string layerName, int layerIndex)
    {
        if (!isHasLayer(layerName))
        {
            Debug.Log("Layer [" + layerName + "] should be added!");
            if (layerIndex <= layers.arraySize)
            {
                SerializedProperty layer = layers.GetArrayElementAtIndex(layerIndex);

                string oldLayerName = layer.stringValue;
                if (string.IsNullOrEmpty(oldLayerName))
                {
                    layer.stringValue = layerName;
                    tagManager.ApplyModifiedProperties();
                    tagManager.Update();
                    Debug.Log(" --> Add layer successful!");
                }
                else
                {
                    Debug.LogWarning(" --> This layer index has been taken by layer [" + oldLayerName + "]!");
                }
            }
        }
        else
        {
            Debug.Log("Layer [" + layerName + "] Ready.");
        }
    }

    private static void ReadSortingLayers()
    {
        sortingLayerNames.Clear();
        sortingLayerIds.Clear();

        sortingLayers = tagManager.FindProperty("m_SortingLayers");
        for (int i = 0; i < sortingLayers.arraySize; i++)
        {
            SerializedProperty p = sortingLayers.GetArrayElementAtIndex(i);

            string propertyPath = p.propertyPath;
            string layerName = null;
            while (p.NextVisible(true))
            {
                if (p.propertyPath.StartsWith(propertyPath))
                {
                    if (p.name == "name")
                    {
                        layerName = p.stringValue;
                    }
                    else if (p.name == "uniqueID")
                    {
                        //Debug.Log("Sorting Layer [" + layerName + "] id=" + p.intValue);
                        sortingLayerNames.Add(layerName);
                        sortingLayerIds.Add(p.intValue);

                        if (p.intValue == 0)
                        {
                            sortingLayerDefaultIndex = i;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
            //SerializedObject layerObj = (SerializedObject)layerProp.objectReferenceValue;

        }
    }
     

    private static void SetSortingLayers(string name, int id, bool beforeDefault, AddSortingLayerOrder order)
    {
        sortingLayers = tagManager.FindProperty("m_SortingLayers");
        ModifySortingLayerType type = ModifySortingLayerType.None;
        int index = 0;
        if (order == AddSortingLayerOrder.FromBegin)
            index = 0;
        else if (order == AddSortingLayerOrder.FromEnd)
        {
            index = sortingLayers.arraySize;
        }

        int findIndex = sortingLayerNames.IndexOf(name);
        if (findIndex >= 0)
        {
            int findId = sortingLayerIds[findIndex];
            if (findId == id)
            {
                // 已经存在且ID正确 
                type = ModifySortingLayerType.None;

                Debug.Log("Sorting layer [" + name + "] ready.");
            }
            else
            {
                // 存在但ID不正确，需要修改ID
                type = ModifySortingLayerType.ChangeId;
                index = findIndex;
                Debug.Log("Sorting layer [" + name + "] exist but need correct id");
            }
        }
        else
        {
            // 不存在，需要创建 
            type = ModifySortingLayerType.CreateNew;
            Debug.Log("Sorting layer [" + name + "] not exist!");
        }

        if (type != ModifySortingLayerType.None)
        {


            if (type == ModifySortingLayerType.CreateNew)
                sortingLayers.InsertArrayElementAtIndex(index);

            SerializedProperty p = sortingLayers.GetArrayElementAtIndex(index);

            p.NextVisible(true);    // name
            p.stringValue = name;

            p.NextVisible(true);    // uniqueID
            p.intValue = id;

            //p.NextVisible(true);    // locked
            //if (type == ModifySortingLayerType.CreateNew)
            //p.boolValue = false;

            Debug.Log("--> Sorting layer [" + name + "] modify OK!");

            tagManager.ApplyModifiedProperties();
            tagManager.Update();

            ReadSortingLayers();
        }


    }

}
