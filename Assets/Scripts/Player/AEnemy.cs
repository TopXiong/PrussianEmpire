using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AEnemy : MonoBehaviour
{



    /**
     * 得到距离自己最近的主角坐标
     * @param
     * position 返回最近的主角坐标
     * 
     * @return
     * true: 有最近的主角， false 没有主角
     */
    public bool TryGetNearestPlayerPosition(PlayerInformation[] mainPlayerInfo, out Vector3 position)
    {
        position = default;
        bool result = false;
        if (mainPlayerInfo == null) return false;
        float distance = float.MaxValue;
        for (int i = 0; i < mainPlayerInfo.Length; ++i)
        {
            if (mainPlayerInfo[i] == null) continue;
            if (mainPlayerInfo[i].IsAlive)
            {
                float x = Vector3.Distance(transform.position, mainPlayerInfo[i].transform.position);
                if (x < distance)
                {
                    result = true;
                    position = mainPlayerInfo[i].transform.position;
                    distance = x;
                }
            }
        }
        return result;
    }
}
