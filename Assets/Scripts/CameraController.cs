using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    /* 地图的边界点 相机在这个范围内移动 */
    public float leftBoundary;                  // 左边边界点
    public float rightBoundary;                 // 右边边界点

    public float cameraMoveSmooth = 3;          // 相机移动平滑值
    public float halfLong = 8;                  // 相机视口长度的一半
    private PlayerInformation[] mainPlayerInfo; // 主角的信息
    private GameManager gameManager;
    private bool canMove = true;                // 相机是否可以移动           
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
        mainPlayerInfo = gameManager.MainPlayersInfo;
        gameManager.OnGameClear.AddListener(() =>
        {
            canMove = false;            // 游戏通关后相机就不可移动了
        });
    }

    // Update is called once per frame
    private void Update()
    {
        if (mainPlayerInfo != null)
        {   // 限定主角不会走出相机视口
            foreach (var info in mainPlayerInfo)
            {
                if (info != null)
                {
                    Vector3 position = info.transform.position;
                    if (info.transform.position.x > transform.position.x + halfLong)
                    {
                        position.x = transform.position.x + halfLong;
                        info.transform.position = position;
                    }
                    else if (info.transform.position.x < transform.position.x - halfLong)
                    {
                        position.x = transform.position.x - halfLong;
                        info.transform.position = position;
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        // 玩家的移动在FixedUpdate，所以相机的跟随也在FixedUpdate
        if (canMove)
        {
            if (TryGetPlayerCenterPoint(out Vector3 point))
            {
                // Debug.Log("camera move");
                point.x = Mathf.Clamp(point.x, leftBoundary, rightBoundary);
                transform.position = Vector3.Lerp(transform.position, point, Time.fixedDeltaTime * cameraMoveSmooth);
            }
        }
    }

    /**
     * 试图得到所有玩家的中间位置坐标
     * 
     * 参数:
     *  position: 返回给调用者的中间坐标
     * 返回值:
     *  true: 得到了中间坐标
     *  false: 没有得到中间坐标（没有存活的玩家）
     */

    public bool TryGetPlayerCenterPoint(out Vector3 position)
    {
        position = transform.position;
        int count = 0;
        float x = 0;
        if (mainPlayerInfo == null || mainPlayerInfo.Length == 0)
            return false;
        for (int i = 0; i < mainPlayerInfo.Length; i++)
        {
            if (mainPlayerInfo[i] != null && mainPlayerInfo[i].IsAlive)
            {
                ++count;
                x += mainPlayerInfo[i].transform.position.x;
            }
        }
        if (count > 0)
        {
            position.x = x / count;
            return true;
        }
        return false;
    }
}
