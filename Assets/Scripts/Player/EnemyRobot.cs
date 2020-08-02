using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRobot : AEnemy
{
    public float moveSpeed = 1f;                    // 移动速度

    public float attackInterval = 1f;               // 射击攻击时间间隔
    public float poisonInterval = 2f;               // 毒气攻击时间间隔
    public float persistantPoisonInterval = 15f;    // 追踪毒气时间间隔
    public float poisonLifeTime = 10f;              // 追踪毒气持续时间
    
    public float attackDistance = 10f;              // 射击攻击距离
    public float poisonDistance = 10f;              // 毒气攻击距离
    public float tracePoisonDistance = 10f;         // 追踪毒气攻击距离

    public Transform attackPoint;                   // 子弹生成点
    public Transform poisonPoint;                   // 毒气生成点
    public GameObject bulletPrefab;                 // 子弹预制体
    public GameObject poisonPrefab;                 // 毒气预制体
    public GameObject poisonTracePrefab;            // 追踪毒气预制体

    private float attackTimeCount = 0f;             // 攻击间隔计时
    private float poisonTimeCount = 0f;             // 毒气间隔计时
    private float persistantPoisonTimeCount = 0f;   // 跟踪毒气间隔计时

    // 组件部分
    private Animator anim;                          // 动画组件
    private PlayerInformation info;                 // 角色信息
    private PlayerBehavior behavior;                // 角色行为
    private PlayerInformation[] mainPlayersInfo;    // 玩家信息   

    private bool isAttack = false;                  // 攻击标记
    private bool isDeath = false;                   // 死亡标记

    // 武器部分
    private Bullet bullet;                          // 子弹
    private Poison poison;                          // 毒气

    // 导航部分
    private Vector3 navPosition;                    // 下一步位置
    private Vector3 lastPosition;                   // 上一步位置
    private TerrainNavigation navigation;           // 地图导航
    private Stack<TerrainNavigation.Point> stack = null;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        info = GetComponent<PlayerInformation>();
        behavior = GetComponent<PlayerBehavior>();
        mainPlayersInfo = GameManager.instance.MainPlayersInfo;
        navigation = TerrainNavigation.instance;
        lastPosition = navPosition = transform.position;

        bullet = bulletPrefab.GetComponent<Bullet>();
        poison = poisonPrefab.GetComponent<Poison>();
    }

    // Update is called once per frame
    void Update()
    {
        var state = anim.GetCurrentAnimatorStateInfo(0);
        if (state.IsName("Death") && state.normalizedTime >= 1f)
        {   // 如果正在播放死亡动画，并且播放到了末尾
            behavior.LerpHide();    // 隐藏自己
            enabled = false;        // 禁用组件
            return;
        }
        if (isDeath) return;
        if (!info.IsAlive)
        {
            isDeath = true;
            Destroy(gameObject, 30f);       // 30s后销毁自身
            anim.Play("Death");             // 播放死亡动画
            return;
        }

        // 计时累加
        attackTimeCount += Time.deltaTime;
        poisonTimeCount += Time.deltaTime;
        persistantPoisonTimeCount += Time.deltaTime;

        // 如果在播放攻击动画或者毒气动画，设置攻击标志为true 否则为false
        isAttack = state.IsName("Attack") || state.IsName("PoisonGas");

        // 防止计时远超最大值
        if (attackTimeCount > attackInterval) attackTimeCount = attackInterval;
        if (poisonTimeCount > poisonInterval) poisonTimeCount = poisonInterval;
        if (persistantPoisonTimeCount > persistantPoisonInterval)
            persistantPoisonTimeCount = persistantPoisonInterval;
    }

    private void FixedUpdate()
    {
        if (!info.IsAlive) return;
        if (isAttack) return;
        if (TryGetNearestPlayerPosition(mainPlayersInfo, out var position))
        {
            var pos = transform.position;
            behavior.Turn(position.x - pos.x > 0 ? 1 : -1); // 更改朝向
            
            // 攻击部分
            if (Vector3.Distance(pos, position) <= tracePoisonDistance)
                PoisonTrace();
            if (Mathf.Abs(position.y - pos.y) <= GameManager.instance.sameLineInterval)
            {
                float XDistance = Mathf.Abs(position.x - pos.x);
                if (XDistance <= poisonDistance)
                    Poison();
                if (XDistance <= attackDistance)
                    Attack();
                if (XDistance <= poisonDistance) return;
            }

            // 移动部分
            if (isAttack) return;
            if (Vector2.Distance(transform.position, navPosition) < 0.12f)
            {
                navPosition = navigation.GetNextStep(transform.position, position, lastPosition, ref stack);
                lastPosition = position;
            }
            bool isMove = false;
            if (Mathf.Abs(navPosition.x - transform.position.x) > moveSpeed * Time.deltaTime)
            {
                if (navPosition.x < transform.position.x)
                {
                    MoveLeft();
                    isMove = true;
                }
                else if (navPosition.x > transform.position.x)
                {
                    MoveRight();
                    isMove = true;
                }
            }
            if (Mathf.Abs(navPosition.y - transform.position.y) > moveSpeed * Time.deltaTime)
            {
                if (navPosition.y < transform.position.y)
                {
                    MoveDown();
                    isMove = true;
                }
                else if (navPosition.y > transform.position.y)
                {
                    MoveUp();
                    isMove = true;
                }
            }
            anim.Play(isMove ? "Move" : "Idle");
        }
    }

    void Attack()
    {
        if (attackTimeCount < attackInterval) return;
        attackTimeCount -= attackInterval;
        anim.Play("Attack");
        var position = transform.position;
        position.x = attackPoint.position.x;
        bullet.CopySelf(position, behavior.Orientation, attackPoint.localPosition.y, PlayerType.Player)
            .GetComponent<Bullet>().injuryValue = 20;
    }

    void Poison()
    {
        if (poisonTimeCount < poisonInterval) return;
        poisonTimeCount -= poisonInterval;
        anim.Play("PoisonGas");
        Invoke("InitPoison", 1f);
    }

    void InitPoison()
    {
        var position = transform.position;
        position.x = poisonPoint.position.x;
        poison.CopySelf(position, attackPoint.localPosition.y, PlayerType.Player);
    }

    void PoisonTrace()
    {
        if (persistantPoisonTimeCount < persistantPoisonInterval) return;
        persistantPoisonTimeCount -= persistantPoisonInterval;
        anim.Play("PoisonGas");
    }

    void MoveUp()
    {
        transform.Translate(Vector3.up * moveSpeed * Time.fixedDeltaTime);
    }

    void MoveDown()
    {
        transform.Translate(Vector3.down * moveSpeed * Time.fixedDeltaTime);
    }

    void MoveLeft()
    {
        behavior.Turn(-1);
        transform.Translate(Vector3.left * moveSpeed * Time.fixedDeltaTime);
    }

    void MoveRight()
    {
        behavior.Turn(1);
        transform.Translate(Vector3.right * moveSpeed * Time.fixedDeltaTime);
    }

}
