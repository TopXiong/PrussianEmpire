using System;
using UnityEngine;

public class MainPlayerController : MonoBehaviour
{
    public int playerNumber;            // 玩家编号(只有一号和2号)
    public float moveSpeed = 10;        // 移动速度
    public Transform bulletPoint;       // 子弹出生点
    public float attackInterval = 5;    // 攻击时间间隔
    public GameObject bullet;           // 子弹
    public GameObject boom;             // 炸弹
    public AudioClip attackAudio;       // 攻击音效
    public AudioClip moveAudio;         // 移动音效
    private Bullet bulletBehavior;      // 子弹行为
    private float attackTimeCount = 0;  // 攻击时间间隔计时
    // 角色键盘控制键
    private KeyCode rightMove, leftMove, attack, search;
    private KeyCode upMove, downMove;
    private Animator anim;                  // 动画播放器
    bool isInit = false;                    // 是否已经被初始化
    //private Rigidbody2D rigid;              // 刚体组件
    private Transform bloodStrip;           // 血条控件
    private PlayerBehavior playerBehavior;  // 自身角色行为
    private PlayerInformation info;         // 角色基本信息
    private AudioSource audioPlay;          // 播放音乐
    private Vector3 left;                   // 人物朝左缩放
    private Vector3 right;                  // 人物朝右缩放
    private bool isDeath = false;           // 是否死亡标记
    /**
     * 
     * 设置玩家编号(玩家1或者玩家2)
     * 玩家的键盘控制会根据玩家编号改变
     * 玩家1使用WASD移动J攻击，玩家二使用上下左右移动，数字键盘1攻击
     * 
     * 参数:
     * value: 玩家编号，1或2
     */
    public int PlayerNumber
    {
        set
        {
            if (value != 1 && value != 2)
                throw new ArgumentOutOfRangeException("PlayerNumber", "The value must be 1 or 2");
            playerNumber = value;
            switch (playerNumber)
            {
                case 1:
                    {
                        rightMove = KeyCode.D;
                        leftMove = KeyCode.A;
                        attack = KeyCode.J;
                        search = KeyCode.K;
                        upMove = KeyCode.W;
                        downMove = KeyCode.S;
                    }
                    break;
                case 2:
                    {
                        rightMove = KeyCode.RightArrow;
                        leftMove = KeyCode.LeftArrow;
                        attack = KeyCode.Keypad1;
                        search = KeyCode.Keypad2;
                        upMove = KeyCode.UpArrow;
                        downMove = KeyCode.DownArrow;
                    }
                    break;
            }
            isInit = true;
        }
    }

    private void Awake()
    {
        if (!TryGetComponent<Animator>(out anim))
            anim = GetComponentInChildren<Animator>();
        //rigid = GetComponent<Rigidbody2D>();
        playerBehavior = GetComponent<PlayerBehavior>();
        playerBehavior.OnDeath.AddListener(() =>
       {
           anim.Play("Death"); // 播放死亡动画
            isDeath = true;
            // 如果角色死亡，就让当前组件失效
            //this.enabled = false;
        });
        audioPlay = GetComponent<AudioSource>();
        info = GetComponent<PlayerInformation>();
        left = right = transform.localScale;
        left.x = -left.x;
    }

    // Start is called before the first frame update
    void Start()
    {
        bulletBehavior = bullet.GetComponent<Bullet>();
        attackTimeCount = attackInterval;
        PlayerNumber = playerNumber;
        bloodStrip = transform.Find("BloodStrip");

    }

    // Update is called once per frame
    void Update()
    {

        var index = anim.GetLayerIndex("Base Layer");
        var state = anim.GetCurrentAnimatorStateInfo(index);
        if (state.IsName("Death"))
        {
            if (state.normalizedTime >= 1)
            {
                // 死亡动画播放到了最后一帧
                Destroy(gameObject, 30f);       // 半分钟后销毁自身
                playerBehavior.LerpHide(3);     // 将角色隐藏起来
                this.enabled = false;           // 禁用当前组件
            }
        }
        if (!isInit || isDeath) return;

        attackTimeCount += Time.deltaTime;
        if (Input.GetKeyDown(attack))
        {
            Attack();
        }
        if (Input.GetKeyDown(search))
        {
            Search();
        }
        if (attackTimeCount > attackInterval)
            attackTimeCount = attackInterval;
    }

    private void FixedUpdate()
    {
        // 将角色的移动控制写在了FixedUpdate
        if (CanMove())
        {
            bool isMove = false;
            if (Input.GetKey(rightMove))
            {
                isMove = true;
                MoveRight();
            }
            else if (Input.GetKey(leftMove))
            {
                isMove = true;
                MoveLeft();
            }
            if (Input.GetKey(upMove))
            {
                isMove = true;
                MoveUp();
            }
            else if (Input.GetKey(downMove))
            {
                isMove = true;
                MoveDown();
            }
            if (!isMove)
            {
                anim.Play("Idle");
                //audioPlay.Stop();
                //audioPlay.clip = null;
            }
            else
            {
                audioPlay.clip = moveAudio;
                audioPlay.Play();
            }
        }

    }

    void MoveRight()
    {
        transform.localScale = right;
        if (bloodStrip != null) // 由于血条作为玩家的子物体，但是不应该随着玩家方向的改变而改变
        {
            Vector3 scale = bloodStrip.localScale;
            scale.x = Mathf.Abs(scale.x);
            bloodStrip.localScale = scale;
        }
        //Vector3 position = transform.position + Vector3.right * moveSpeed * Time.fixedDeltaTime;
        //rigid.MovePosition(new Vector2(position.x, position.y));
        anim.Play("Move");
        transform.Translate(Vector3.right * moveSpeed * Time.fixedDeltaTime);
    }

    void MoveLeft()
    {
        transform.localScale = left;
        if (bloodStrip != null)
        {
            Vector3 scale = bloodStrip.localScale;
            scale.x = -Mathf.Abs(scale.x);
            bloodStrip.localScale = scale;
        }
        //Vector3 position = transform.position + Vector3.left * moveSpeed * Time.fixedDeltaTime;
        //rigid.MovePosition(new Vector2(position.x, position.y));
        anim.Play("Move");
        transform.Translate(Vector3.left * moveSpeed * Time.fixedDeltaTime);
    }

    void MoveUp()
    {
        //Vector3 position = transform.position + Vector3.up * moveSpeed * Time.fixedDeltaTime;
        //rigid.MovePosition(new Vector2(position.x, position.y));
        anim.Play("Move");
        transform.Translate(Vector3.up * moveSpeed * Time.fixedDeltaTime);
    }

    void MoveDown()
    {
        //Vector3 position = transform.position + Vector3.down * moveSpeed * Time.fixedDeltaTime;
        //rigid.MovePosition(new Vector2(position.x, position.y));
        anim.Play("Move");
        transform.Translate(Vector3.down * moveSpeed * Time.fixedDeltaTime);
    }

    /// <summary>
    /// TODO
    /// </summary>
    void Search()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Search")) return;
        if (attackTimeCount >= attackInterval)
        {
            attackTimeCount -= attackInterval;
            anim.Play("Search");
            if (transform.localScale.x < 0)
            {
                Instantiate(boom, transform.position + new Vector3(-0.8f, 0.7f, 0f), Quaternion.identity);
            }
            else
            {
                Instantiate(boom, transform.position + new Vector3(0.8f, 0.7f, 0f), Quaternion.identity);
            }
            

            audioPlay.clip = attackAudio;
            audioPlay.Play();
        }
    }

    void Attack()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack")) return; // 如果正在播放攻击动画，就不要攻击了
        if (attackTimeCount >= attackInterval)
        {
            attackTimeCount -= attackInterval;
            anim.Play("Attack");
            Vector3 velocity = Vector3.right;
            if (transform.localScale.x < 0)
                velocity.x = -velocity.x;
            var position = transform.position;
            var bulletPos = bulletPoint.position;
            position.x = bulletPos.x;
            bulletBehavior.CopySelf(position, velocity,
                bulletPos.y - position.y, PlayerType.Enemy);
            audioPlay.clip = attackAudio;
            audioPlay.Play();
        }
    }

    /**
     * 角色是否可以移动
     * 
     * 返回值
     * true:可以移动
     * false:不能移动
     */
    bool CanMove()
    {
        return !isDeath && attackTimeCount >= attackInterval;
    }


}
