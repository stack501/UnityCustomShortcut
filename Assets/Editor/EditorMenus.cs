using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

static class EditorMenus
{
    [MenuItem("Tools/Toggle Inspector Lock %l")] // Ctrl + l
    static void ToggleInspectorLock()
    {
        ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
        ActiveEditorTracker.sharedTracker.ForceRebuild();
    }

    [MenuItem("Tools/Inspector DebugMode %t")] // Ctrl + t
    static void DebugModeInspector()
    {
        var window = Resources.FindObjectsOfTypeAll<EditorWindow>();
        var inspectorWindow = ArrayUtility.Find( window, c => c.GetType().Name == "InspectorWindow" );

        if ( inspectorWindow == null ) return;

        var inspectorType = inspectorWindow.GetType();
        var tracker = ActiveEditorTracker.sharedTracker;
        var isNormal = tracker.inspectorMode == InspectorMode.Normal;
        var methodName = isNormal ? "SetDebug" : "SetNormal";

        var methodInfo = inspectorType.GetMethod( methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance );
        methodInfo.Invoke( inspectorWindow, null );
        tracker.ForceRebuild();
    }

    [MenuItem("Tools/Prefabs Select %q")] // Ctrl + q
    static void SelectPrefab()
    {
        GameObject obj = Selection.activeGameObject;
        if(obj != null)
        {
            var targetPrefab = PrefabUtility.GetPrefabParent(obj);
            EditorGUIUtility.PingObject(targetPrefab);
        }
        else
        {
            Debug.LogFormat("{0} is not Prefabs", obj.name);
        }
    }

    [MenuItem("Tools/Prefabs Revert %w")] // Ctrl + w
    static void RevertPrefabs()
    {
        GameObject[] objs = Selection.gameObjects;
        if(objs != null)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                var instanceRoot = PrefabUtility.FindPrefabRoot(objs[i]);
                if(instanceRoot != null)
                {
                    PrefabUtility.RevertPrefabInstance(instanceRoot);
                }
                else
                {
                    Debug.LogFormat("{0} is not Prefabs", objs[i].name);
                }
            }
        }
        else
        {
            Debug.LogError("Select Object is Null!");
        }
    }

    [MenuItem("Tools/Prefabs Apply %e")] // Ctrl + e
    static void ApplyPrefabs()
    {
        GameObject[] objs = Selection.gameObjects;
        if(objs != null)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                var instanceRoot = PrefabUtility.FindPrefabRoot(objs[i]);
                var targetPrefab = PrefabUtility.GetPrefabParent(instanceRoot);

                if(objs[i] != null || targetPrefab != null)
                {
                    PrefabUtility.ReplacePrefab(instanceRoot, targetPrefab, ReplacePrefabOptions.ConnectToPrefab);
                }
                else
                {
                    Debug.LogFormat("{0} is not Prefabs or TargetPrefab is Null{1}", objs[i].name, targetPrefab.name);
                }

            }
        }
        else
        {
            Debug.LogError("Select Object is Null!");
        }
    }

    [MenuItem("Tools/Reset Position &q")] // Alt + q
    static void ResetPosition()
    {   
        GameObject[] objs = Selection.gameObjects;

        if(objs != null)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                objs[i].transform.localPosition = Vector3.zero;
            }
        }
        else
        {
            Debug.LogError("Select Object is Null!");
        }
        
    }

    [MenuItem("Tools/Reset Rotation &w")] // Alt + w
    static void ResetRotation()
    {
        GameObject[] objs = Selection.gameObjects;

        if(objs != null)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                objs[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
        else
        {
            Debug.LogError("Select Object is Null!");
        }
    }

    [MenuItem("Tools/Reset Scale &e")] // Alt + e
    static void ResetScale()
    {
        GameObject[] objs = Selection.gameObjects;

        if(objs != null)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                objs[i].transform.localScale = Vector3.one;
            }
        }
        else
        {
            Debug.LogError("Select Object is Null!");
        }
    }

    [MenuItem("Tools/Reset All Transform &r")] // Alt + r
    static void ResetAllTransform()
    {
        GameObject[] objs = Selection.gameObjects;

        if(objs != null)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                var objTrans = objs[i].transform;
                objTrans.localPosition = Vector3.zero;
                objTrans.localRotation = Quaternion.Euler(0,0,0);
                objTrans.localScale = Vector3.one;
            }
        }
        else
        {
            Debug.LogError("Select Object is Null!");
        }
    }

    static List<Vector3> m_AllLocalPositionItem = new List<Vector3>();
    static List<Vector3> m_AllLocalRotationItem = new List<Vector3>();
    static List<Vector3> m_AllLocalScaleItem = new List<Vector3>();
    [MenuItem("Tools/Save All Transform &s")] // Alt + s
    static void SaveAllTransform()
    {
        GameObject[] objs = Selection.gameObjects;
        m_AllLocalPositionItem.Clear();

        if(objs != null && objs.Length > 0)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                var objTrans = objs[i].transform;
                m_AllLocalPositionItem.Add(objTrans.localPosition);
                m_AllLocalRotationItem.Add(objTrans.localRotation.eulerAngles);
                m_AllLocalScaleItem.Add(objTrans.localScale);
            }

            for (int i = 0; i < objs.Length; i++)
            {
                int temp = (objs.Length - 1) - i;
                Debug.LogFormat("Position Item {0} : {1} \n Rotation Item {0} : {2} \n Scale Item {0} : {3}", i, m_AllLocalPositionItem[temp], m_AllLocalRotationItem[temp], m_AllLocalScaleItem[temp]);
            }
        }
        else
        {
            Debug.LogError("Select Object is Null!");
        }

        Debug.Log("Save All Transform Property");
    }

    [MenuItem("Tools/Load All Transform &d")] // Alt + d
    static void LoadAllTransform()
    {
        GameObject[] objs = Selection.gameObjects;

        if(m_AllLocalPositionItem != null && m_AllLocalRotationItem != null && m_AllLocalScaleItem != null)
        {
            if(objs != null && objs.Length > 0)
            {
                if (m_AllLocalPositionItem.Count > 0)
                {
                    for (int i = 0; i < objs.Length; i++)
                    {
                        var objTrans = objs[i].transform;
                        objTrans.localPosition = m_AllLocalPositionItem[0];
                        objTrans.localRotation = Quaternion.Euler(m_AllLocalRotationItem[0].x, m_AllLocalRotationItem[0].y, m_AllLocalRotationItem[0].z);
                        objTrans.localScale = m_AllLocalScaleItem[0];
                    }
                }
                else
                {
                    for (int i = 0; i < objs.Length; i++)
                    {
                        var objTrans = objs[i].transform;
                        objTrans.localPosition = m_AllLocalPositionItem[i];
                        objTrans.localRotation = Quaternion.Euler(m_AllLocalRotationItem[i].x, m_AllLocalRotationItem[i].y, m_AllLocalRotationItem[i].z);
                        objTrans.localScale = m_AllLocalScaleItem[i];
                    }
                }
            }
            else
            {
                Debug.LogError("Select Object is Null!");
            }
        }
        else
        {
            Debug.LogError("Items is Null!!!");
        }
        
        Debug.Log("Load All Transform Property");
    }

    [MenuItem("CONTEXT/Component/Find Component",false, 0)]
    static void FindComponent(MenuCommand menuCommand)
    {
        var component = menuCommand.context;
        if(component != null)
        {
            SceneModeUtility.SearchForType(component.GetType());
        }
    }
}
