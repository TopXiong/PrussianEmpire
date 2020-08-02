using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 敌人的行为
 */
public class EnemyBehavior : MonoBehaviour
{
    public int enemyType = 0;                    // 敌人类型，0为正常型，1为自爆型
    public int bombValue = 30;                   // 爆炸伤害
    public GameObject bombSpecialEffect;         // 爆炸特效
    public float speed = 10;                     // 自身移动速度
    public float distance = 10;                  // 攻击范围
    public float attackInteval = 10;             // 攻击间隔
    [HideInInspector]
    public GameObject[] mainPlayers;             // 主角
    public GameObject knifeObject;               // 刺刀对象
    public Transform knifeTransform;             // 刺刀实例化位置变换
    public AnimationClip attackAnimation;        // 攻击动画
    public AudioClip moveAudio;                  // 移动音效 

    private PlayerInformation[] mainPlayerInfo;  // 主角信息
    //private IPlayerBehavior behavior;          // 自身行为
    private Animator anim;                       // 自身动画机
    private float attackIntevalCount = 0;        // 攻击间隔计时
    private Rigidbody2D rigid;                   // 刚体组件
    private Knife knife;                         // 刺刀
    private PlayerInformation myInfo;            // 自身信息
    private bool firstDeath = true;              // 用来判定是否是执行死亡的第一帧
    private Transform bloodStrip;                // 血条控件
    private BoxCollider2D boxCollider;           // 碰撞体
    private bool isAttack = false;               // 是否正在攻击
    private AudioSource audioPlay;               // 音乐播放
    private TerrainNavigation navigation;        // 地图导航
    private Vector3 navPosition;                 // 下一步位置
    private Vector3 lastPosition;                // 上一步位置
    private List<PlayerBehavior> playerList 
        = new List<PlayerBehavior>();            // 爆炸范围内的玩家    
    private PlayerBehavior myBehavior;           // 自身行为
    private Stack<TerrainNavigation.Point> stack = null;
    private bool hasBomb = false;                // 是否已经爆炸了
    void Awake()
    {
        // 初始化behavior
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        myInfo = GetComponent<PlayerInformation>();
        audioPlay = GetComponent<AudioSource>();
        myBehavior = GetComponent<PlayerBehavior>();
    }

    // Start is called before the first frame update
    void Start()
    {
        navigation = TerrainNavigation.instance;
        bloodStrip = transform.Find("BloodStrip");
        boxCollider = GetComponentInChildren<BoxCollider2D>();
        knife = knifeObject.GetComponent<Knife>();
        mainPlayers = GameManager.instance.mainPlayers;
        if (mainPlayers == null)
        {
            Debug.Log("Main Player is Null");
            return;
        }
        mainPlayerInfo = new PlayerInformation[mainPlayers.Length];
        for (int i = 0; i < mainPlayers.Length; ++i)
        {
            mainPlayerInfo[i] = mainPlayers[i].GetComponent<PlayerInformation>();
        }
        lastPosition = navPosition = transform.position;
        
        if (enemyType == 1)
        {
            // 如果是自爆型敌人，自实例化之后30s自爆
            Invoke("Bomb", 30f);
            speed *= 1.2f;      // 让自己移动速度提升1.2倍
        }
        //StartCoroutine(NavgationPosition());
    }


    //IEnumerator NavgationPosition()
    //{
    //    while (true)
    //    {
    //        if (TryGetNearestPlayerPosition(out Vector3 position))
    //        {
    //            navPosition = navigation.GetNextStep(transform.position, position, lastPosition);
    //            lastPosition = transform.position;
    //        }
    //        yield return new WaitForSeconds(0.5f);
    //        if (!myInfo.IsAlive)
    //            break;
    //    }
    //    // yield break;
    //}

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

    // Update is called once per frame
    void Update()
    {
        //if (enemyType == 1) return;
        var state = anim.GetCurrentAnimatorStateInfo(0);
        if (state.IsName("Death") && state.normalizedTime >= 1f)
        {// 如果当前正在播放死亡动画，且已经播放到了动画末尾，隐藏角色
            GetComponent<PlayerBehavior>().LerpHide(3);
            enabled = false;        // 禁用当前组件
        }
        if (attackIntevalCount < 0)
            attackIntevalCount += Time.deltaTime;

        if (!myInfo.IsAlive)
        {
            if (firstDeath)
            {
                if (boxCollider != null)
                    boxCollider.enabled = false;
                firstDeath = false;
                Destroy(gameObject, 30f);   // 30s后销毁自身
                anim.Play("Death");         // 播放死亡动画

            }
            return;
        }

        
    }

    private void FixedUpdate()
    {
        // 敌人的移动，攻击行为
        if (isAttack || !myInfo.IsAlive) return;
        if (enemyType == 0)
        {
            if (TryGetNearestPlayerPosition(out Vector3 position))
            {
                float x = position.x - transform.position.x;
                float y = position.y - transform.position.y;
                float distance = Mathf.Sqrt(x * x + y * y);
                bool isMove = false;
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
                    //var navPosition = navigation.GetNextStep(transform.position, position);
                    //Debug.Log(navPosition);
                    if (Vector2.Distance(transform.position, navPosition) < 0.12f)
                    {

                        navPosition = navigation.GetNextStep(transform.position, position, lastPosition, ref stack);
                        lastPosition = position;
                    }
                    if (Mathf.Abs(navPosition.x - transform.position.x) > speed * Time.deltaTime)
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
                    if (Mathf.Abs(navPosition.y - transform.position.y) > speed * Time.deltaTime)
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
                        audioPlay.clip = moveAudio;
                        audioPlay.Play();
                        anim.Play("Move");
                    }
                    else
                    {
                        StopMove();
                    }
                }
                else
                {
                    // 主角在攻击范围内
                    if (position.x < transform.position.x)
                    {
                        // 如果玩家在自己左边
                        //transform.localScale = new Vector3(-1, 1, 1);
                        //if (bloodStrip != null)
                        //{
                        //    Vector3 scale = bloodStrip.localScale;
                        //    scale.x = -Mathf.Abs(scale.x);
                        //    bloodStrip.localScale = scale;
                        //}
                        Turn(-1);
                    }
                    else
                    {
                        // 玩家在自己右边
                        //transform.localScale = Vector3.one;
                        //if (bloodStrip != null)
                        //{
                        //    Vector3 scale = bloodStrip.localScale;
                        //    scale.x = Mathf.Abs(scale.x);
                        //    bloodStrip.localScale = scale;
                        //}
                        Turn(1);
                    }

                    StopMove();
                    Attack();
                }
            }
            else
            {
                // 没有玩家存活
                anim.Play("Idle");
            }
        }
        else if (enemyType == 1)
        {
            if (TryGetNearestPlayerPosition(out Vector3 position))
            {
                var myPosition = transform.position;
                if (Mathf.Abs(position.x - myPosition.x) < 0.5f
                    && Mathf.Abs(position.y - myPosition.y) < GameManager.instance.sameLineInterval)
                {
                    //StopMove();
                    Bomb(); // 如果靠的玩家非常近，就自爆
                }
                else
                {
                    if (Vector2.Distance(transform.position, navPosition) < 0.12f)
                    {

                        navPosition = navigation.GetNextStep(transform.position, position, lastPosition, ref stack);
                        lastPosition = position;
                    }
                    if (Mathf.Abs(navPosition.x - transform.position.x) > speed * Time.deltaTime)
                    {
                        if (navPosition.x < transform.position.x)
                        {
                            MoveLeft();
                        }
                        else if (navPosition.x > transform.position.x)
                        {
                            MoveRight();
                        }
                    }
                    if (Mathf.Abs(navPosition.y - transform.position.y) > Time.deltaTime)
                    {
                        if (navPosition.y < transform.position.y)
                        {
                            MoveDown();
                        }
                        else if (navPosition.y > transform.position.y)
                        {
                            MoveUp();
                        }
                    }
                    anim.Play("Bomb");
                    audioPlay.clip = moveAudio;
                    audioPlay.Play();
                }
            }       
        }
    }


    void Bomb()
    {
        if (hasBomb || !myInfo.IsAlive) return;    // 如果已经爆炸过了或者不是存活状态，就不用再爆了
        hasBomb = true;
        Debug.Log(playerList.Count);
        myBehavior.LerpHide();  // 先隐藏自己
        myBehavior.Injury(myInfo.maxBlood); // 给自己造成成吨伤害
        Destroy(gameObject, 30f); // 半分钟后自毁
        anim.Play("Death");        // 播放死亡动画
        enabled = false;            // 禁用组件
        var position = transform.position;
        position.y -= 0.05f;    // 让爆炸效果靠下一点
        Instantiate(bombSpecialEffect, position, Quaternion.identity);    // 实例化爆炸特效
        foreach(var player in playerList)
        {
            player.Injury(bombValue);
        }
    }


    void MoveLeft()
    {
        if (bloodStrip != null)
        {
            Vector3 scale = bloodStrip.localScale;
            scale.x = -Mathf.Abs(scale.x);
            bloodStrip.localScale = scale;
        }
        transform.localScale = new Vector3(-1, 1, 1);
        //anim.Play("Move");
        transform.Translate(Vector3.left * speed * Time.fixedDeltaTime);
        //Debug.Log("Left");
    }

    void MoveRight()
    {
        if (bloodStrip != null)
        {
            Vector3 scale = bloodStrip.localScale;
            scale.x = Mathf.Abs(scale.x);
            bloodStrip.localScale = scale;
        }
        transform.localScale = Vector3.one;
        //anim.Play("Move");
        transform.Translate(Vector3.right * speed * Time.fixedDeltaTime);
        //Debug.Log("Right");
    }

    void MoveUp()
    {
        //anim.Play("Move");
        transform.Translate(Vector3.up * speed * Time.fixedDeltaTime);
        //Debug.Log("Up");
    }

    void MoveDown()
    {
        //anim.Play("Move");
        transform.Translate(Vector3.down * speed * Time.fixedDeltaTime);
        //Debug.Log("Down");
    }

    void StopMove()
    {
        anim.Play("Idle");    // Idle anim
        if (audioPlay.clip == moveAudio)
        {
            audioPlay.Pause();
            audioPlay.clip = null;
        }
    }

    /**
     * 攻击
     */
    void Attack()
    {
        if (attackAnimation == null) return;
        if (attackIntevalCount < 0) return;
        attackIntevalCount -= attackInteval;
        // anim.SetInteger("AnimState", 2);    // Attack anim
        isAttack = true;
        if (attackAnimation == null)
            Invoke("StopAttack", attackInteval);
        else
            Invoke("StopAttack", attackAnimation.length + 0.1f);
        anim.Play("Attack");
        knife.CopySelf(knifeTransform.position, knifeTransform.rotation, transform, 
            transform.position.y,attackAnimation.length, PlayerType.Player);
    }


    /**
     * 设置已经没有在攻击动画状态了
     */
    void StopAttack()
    {
        isAttack = false;
    }

    /**
     * 改变角色朝向
     * 
     * 参数
     *  direction:方向，1朝右，-1朝左
     * 
     */
    private void Turn(float direction)
    {
        if (direction != 1 && direction != -1)
            throw new ArgumentOutOfRangeException("Turn direction", "值必须为1或-1");
        if (bloodStrip != null)
        {
            var bscale = bloodStrip.localScale;
            bscale.x = Mathf.Abs(bscale.x) * direction;
            bloodStrip.localScale = bscale;
        }
        transform.localScale = Vector3.one;
        var scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Foot"))
        {   // 如果检测到主角的脚
            var info = collision.GetComponentInParent<PlayerInformation>();
            if (info.playerType == PlayerType.Player)
            {
                var playerBehavior = collision.GetComponentInParent<PlayerBehavior>();
                playerList.Add(playerBehavior);     // 将玩家行为加入列表
                //Debug.Log("检测到主角：count：" + playerList.Count);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Foot"))
        {
            var info = collision.GetComponentInParent<PlayerInformation>();
            if (info.playerType == PlayerType.Player)
            {
                var playerBehavior = collision.GetComponentInParent<PlayerBehavior>();
                playerList.Remove(playerBehavior);
            }
        }
    }
}
