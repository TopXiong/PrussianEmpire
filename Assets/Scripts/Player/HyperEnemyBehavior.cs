using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyperEnemyBehavior : MonoBehaviour
{
    public float speed = 10;                     // 自身移动速度
    public float distance = 10;                  // 攻击范围
    public float attackInteval = 10;             // 攻击间隔
    public float throwInteval = 10;              // 扔雷时间间隔
    public int bulletInjury = 20;                // 子弹伤害
    public float grenadeInjury = 30;             // 手雷的伤害
    [HideInInspector]
    public GameObject[] mainPlayers;              // 主角
    public GameObject bullet;                     // 子弹
    public Transform bulletTransform;             // 子弹出生点
    public GameObject grenade;                    // 手雷
    public Transform grenadeTransform;            // 手雷出生点
    public float grenadeSpeed = 2;                // 扔出手雷的水平速度
    public float grenadeTime = 0.5f;              // 手雷出生时间点 
    public AnimationClip shootClip;               // 射击动画
    public AnimationClip throwClip;               // 扔雷动画
    public AudioClip shootAudio;                  // 射击音效
    public AudioClip moveAudio;                   // 移动音效


    private float throwDistance = 4;             // 扔雷距离
    private PlayerInformation[] mainPlayerInfo;  // 主角信息
    //private IPlayerBehavior behavior;          // 自身行为
    private Animator anim;                       // 自身动画机
    private float attackIntevalCount = 0;        // 攻击间隔计时
    private float throwIntevalCount = 90;        // 扔雷时间间隔计时
    private Rigidbody2D rigid;                   // 刚体组件
    private PlayerInformation myInfo;            // 自身信息
    private Transform bloodStrip;                // 血条控件
    private bool isAttack = false;               // 是否正在攻击
    private Bullet bulletBehavior;               // 子弹行为
    private Grenade grenadeBehavior;             // 手雷行为
    private PlayerBehavior playerBehavoir;       // 自身的角色行为
    private bool isRight;                        // 是否朝右(控制扔出手雷的方向)
    private AudioSource audioSource;             // 音乐播放
    private TerrainNavigation navigation;        // 地形导航
    private Vector3 navPosition;                 // 下一步
    private Vector3 lastPosition;                // 上一次的目标点

    private Stack<TerrainNavigation.Point> stack;
    void Awake()
    {
        // 初始化behavior
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        myInfo = GetComponent<PlayerInformation>();
        playerBehavoir = GetComponent<PlayerBehavior>();
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {

        bulletBehavior = bullet.GetComponent<Bullet>();
        grenadeBehavior = grenade.GetComponent<Grenade>();
        bloodStrip = transform.Find("BloodStrip");
        throwIntevalCount = throwInteval;
        mainPlayers = GameManager.instance.mainPlayers;
        if (mainPlayers == null) return;
        mainPlayerInfo = new PlayerInformation[mainPlayers.Length];
        for (int i = 0; i < mainPlayers.Length; ++i)
        {
            mainPlayerInfo[i] = mainPlayers[i].GetComponent<PlayerInformation>();
        }
        navigation = TerrainNavigation.instance;
        lastPosition = navPosition = transform.position;
    }
    /**
     * 得到距离自己最近的主角坐标
     * @param
     * position 返回最近的主角坐标
     * 
     * @return
     * true: 有最近的主角， false 没有主角
     */
    bool TryGetNearestPlayerPosition(out Vector3 position)
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

    private void Update()
    {
        if (!myInfo.IsAlive)
        {   // 如果死亡
            anim.Play("Death");         // 播放死亡动画
            Destroy(gameObject, 30f);   // 30s后销毁自身
            var state = anim.GetCurrentAnimatorStateInfo(0);
            if (state.IsName("Death") && state.normalizedTime >= 1.0f)
            {
                // 如果当前正在播放死亡动画，并且已经播放完了
                playerBehavoir.LerpHide(3);     // 让角色消失
                enabled = false;                // 禁用当前组件
            }
        }
    }

    private void FixedUpdate()
    {
        attackIntevalCount += Time.fixedDeltaTime;
        throwIntevalCount += Time.fixedDeltaTime;
        if (attackIntevalCount > attackInteval)
            attackIntevalCount = attackInteval;
        if (throwIntevalCount > throwInteval)
            throwIntevalCount = throwInteval;
        if (isAttack || !myInfo.IsAlive) return;
        if (TryGetNearestPlayerPosition(out Vector3 position))
        {
            float x = position.x - transform.position.x;
            float y = position.y - transform.position.y;
            float distance = Mathf.Sqrt(x * x + y * y);
            if (distance > this.distance || Mathf.Abs(y) > GameManager.instance.sameLineInterval)
            {   // 如果主角在攻击范围外
                //if (position.x < transform.position.x)
                //{   // 如果主角在自己左边
                //    MoveLeft();
                //}
                //else if (position.x > transform.position.x + 0.01f)
                //{
                //    // 主角在自己右边
                //    MoveRight();
                //}

                //if (position.y < transform.position.y)
                //{
                //    // 如果主角在自己下面
                //    MoveDown();
                //}
                //else if (position.y > transform.position.y + GameManager.instance.sameLineInterval)
                //{
                //    // 玩家在自己上面
                //    MoveUp();
                //}
                bool isMove = false;
                if (Vector2.Distance(transform.position, navPosition) < 0.2f)
                {

                    navPosition = navigation.GetNextStep(transform.position, position, lastPosition, ref stack);
                    lastPosition = position;
                }
                if (Mathf.Abs(navPosition.x - transform.position.x) > 0.1f)
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
                if (Mathf.Abs(navPosition.y - transform.position.y) > 0.02f)
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
                if (isMove)
                {
                    audioSource.clip = moveAudio;
                    audioSource.Play();
                }
                else
                {
                    anim.Play("Idle");
                }
            }
            else
            {
                anim.Play("Idle");
                if (position.x < transform.position.x)
                {
                    // 如果玩家在自己左边
                    transform.localScale = new Vector3(-1, 1, 1);
                    if (bloodStrip != null)
                    {
                        Vector3 scale = bloodStrip.localScale;
                        scale.x = -Mathf.Abs(scale.x);
                        bloodStrip.localScale = scale;
                    }
                }
                else
                {
                    // 玩家在自己右边
                    transform.localScale = Vector3.one;
                    if (bloodStrip != null)
                    {
                        Vector3 scale = bloodStrip.localScale;
                        scale.x = Mathf.Abs(scale.x);
                        bloodStrip.localScale = scale;
                    }
                }
                // 主角在攻击范围内
                if (distance < throwDistance)
                {   // 在扔雷的攻击范围内
                    ThrowGrenade();     // 有雷扔雷
                }
                Shooting();         // 无雷射击
            }
        }
        else
        {
            // 没有玩家存活
            anim.Play("Idle");
        }
    }


    void MoveLeft()
    {
        anim.Play("Move");
        if (bloodStrip != null)
        {
            Vector3 scale = bloodStrip.localScale;
            scale.x = -Mathf.Abs(scale.x);
            bloodStrip.localScale = scale;
        }
        transform.localScale = new Vector3(-1, 1, 1);
        transform.Translate(Vector3.left * speed * Time.fixedDeltaTime);
    }

    void MoveRight()
    {
        anim.Play("Move");
        if (bloodStrip != null)
        {
            Vector3 scale = bloodStrip.localScale;
            scale.x = Mathf.Abs(scale.x);
            bloodStrip.localScale = scale;
        }
        transform.localScale = Vector3.one;
        transform.Translate(Vector3.right * speed * Time.fixedDeltaTime);
    }

    void MoveUp()
    {
        anim.Play("Move");
        transform.Translate(Vector3.up * speed * Time.fixedDeltaTime);
    }

    void MoveDown()
    {
        anim.Play("Move");
        transform.Translate(Vector3.down * speed * Time.fixedDeltaTime);
    }

    /**
     * 射击
     */
    void Shooting()
    {
        if (attackIntevalCount < attackInteval)
            return;
        anim.Play("Attack");
        attackIntevalCount -= attackInteval;
        Vector3 velocity = Vector3.right;
        if (transform.localScale.x < 0)
            velocity.x = -1;
        var position = transform.position;
        var bulletPos = bulletTransform.position;
        position.x = bulletPos.x;

        bulletBehavior.CopySelf(position, velocity,
            bulletPos.y - position.y, PlayerType.Player)
            .GetComponent<Bullet>().injuryValue = bulletInjury; // 实例化子弹，并重置伤害值
        isAttack = true;
        Invoke("CancelAttack", shootClip.length);
        audioSource.clip = shootAudio;
        audioSource.Play();
    }
    /**
     * 扔雷
     */
    void ThrowGrenade()
    {
        if (attackIntevalCount < attackInteval || throwIntevalCount < throwInteval)
            return;
        anim.Play("Throw");
        attackIntevalCount -= attackInteval;
        throwIntevalCount -= throwInteval;
        isRight = transform.localScale.x > 0;
        Invoke("InitGrenade", grenadeTime);
        isAttack = true;
        Invoke("CancelAttack", throwClip.length);
    }

    /**
     * 初始化手雷
     */
    void InitGrenade()
    {
        Vector3 velocity = new Vector3(grenadeSpeed, 0, 0);
        if (!isRight)
            velocity.x = -velocity.x;
        grenadeBehavior.CopySelf(grenadeTransform.position, velocity, transform.position.y, PlayerType.Player);
    }

    /*
     * 设置不在攻击动画状态了
     */
    void CancelAttack()
    {
        isAttack = false;
    }
}
