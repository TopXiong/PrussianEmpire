using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEnemyBehavior : AEnemy
{
    public float fireInteval = 2f;                  // 放炮时间间隔
    private float fireTimeCount = 0f;               // 放炮时间计数
    public Transform firePoint;                     // 炮弹生成点
    public GameObject shellPrefab;                  // 炮弹预制体
    private Shell shell;                            // 炮弹组件

    private PlayerInformation info;                 // 角色信息
    private PlayerBehavior behavior;                // 角色行为

    private PlayerInformation[] mainPlayerInfo;     // 玩家信息

    private Animator anim;                          // 动画组件

    private bool deathFlag = false;                 // 死亡标记

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        info = GetComponent<PlayerInformation>();
        behavior = GetComponent<PlayerBehavior>();
        anim = GetComponentInChildren<Animator>();
        mainPlayerInfo = GameManager.instance.MainPlayersInfo;
        shell = shellPrefab.GetComponent<Shell>();
    }

    // Update is called once per frame
    void Update()
    {
        var state = anim.GetCurrentAnimatorStateInfo(0);
        if (state.IsName("Death") && state.normalizedTime >= 1f)
        {
            if (state.loop == false)
            {
                // 如果正在播放死亡动画并且播放到了最后一帧
                enabled = false; // 禁用组件
                behavior.LerpHide();
            }
        }

        if (deathFlag) return;      // 死亡后什么都不做
        if (!info.IsAlive)
        {
            Destroy(gameObject, 30f);
            anim.Play("Death");
            deathFlag = true;       // 标记死亡
            return;
        }
        fireTimeCount += Time.deltaTime;
        if (fireTimeCount > fireInteval)
            fireTimeCount = fireInteval;

        Attack();

        if (TryGetNearestPlayerPosition(mainPlayerInfo, out var position))
        {
            if (position.x < transform.position.x)
            {   // 玩家在自己左边
                behavior.Turn(-1);
            }
            else
            {
                behavior.Turn(1);
            }
        }
    }


    private void FixedUpdate()
    {
        var state = anim.GetCurrentAnimatorStateInfo(0);
        if (state.IsName("Death") && state.loop)
        {
            behavior.Turn(1);   // 向右转
            transform.Translate(Vector3.right * 3f * Time.fixedDeltaTime);  // 向右逃跑，3m/s;
        }
    }
    void Attack()
    {
        if (TryGetNearestPlayerPosition(mainPlayerInfo, out var pos))
        {
            if (fireTimeCount < fireInteval) return;
            fireTimeCount -= fireInteval;
            var position = firePoint.position;
            position.z = transform.position.y;
            position.y = firePoint.localPosition.y; // 高度
            //Debug.Log(firePoint.up);
            //var speed = firePoint.up;
            //speed.z = -0.1f;
            anim.Play("Fire");
            shell.CopySelf(position, GetSpeedFromPosition(pos) , PlayerType.Player);
        }
    }


    /**
     * 
     * 根据落点计算速度
     * 
     * 参数
     * position落点，炮弹将落于此点
     * 
     * 返回值
     * 伪3D中的速度，矢量，x为水平方向，y为空中方向（高度），z为远方
     */
    Vector3 GetSpeedFromPosition(Vector3 position)
    {
        var up = firePoint.up;
        var alpha = up.y / up.x;
        var a = position.x - firePoint.position.x;  // 与position的水平距离
        var h = firePoint.localPosition.y;          // 与地面的高度
        var v = position.y - transform.position.y;  // 与position的远方距离
        var m = 2 * (alpha * a + h);
        if (m < 0f) m = 0f;
        var t = Mathf.Sqrt(m / shell.gravity);      // 计算炮弹从出生到落地时间
        if (t < 0.0001f)
            t = 0.000001f;
        var x = a / t;                              // 水平方向速度
        var y = alpha * x;                          // 高度方向速度
        var z = v / t;                              // 远方向速度
        return new Vector3(x, y, z);
    }
}
