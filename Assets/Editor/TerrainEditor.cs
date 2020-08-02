using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Content;
using UnityEditor.Rendering;

[CustomEditor(typeof(TerrainNavigation))]
public class TerrainEditor : Editor
{
    protected TerrainNavigation terrain;

    private void OnEnable()
    {
        terrain = (TerrainNavigation)target;
    }

    private void OnSceneGUI()
    {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        var e = Event.current;
        //如果是鼠标左键
        if (e.button == 0 && e.type == EventType.MouseDown)
        {
            //获取由鼠标位置产生的射线
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            //计算碰撞
            if (Physics.Raycast(ray, out var hitinfo, 2000))
            {
                //tileObject.setDataFromPosition(hitinfo.point.x, hitinfo.point.z, tileObject.dataID);
                terrain.SetTerrainFromPostion(hitinfo.point);
                
                Debug.Log(hitinfo.collider.name);
            }

            //Debug.Log(pos);
        }
    //    Debug.Log(e.mousePosition);
    }
}
