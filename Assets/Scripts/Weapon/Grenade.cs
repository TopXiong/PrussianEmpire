using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public int injuryValue = 30;             // 伤害值
    private PlayerType targetType = PlayerType.Player;   // 攻击目标类型
    private Rigidbody2D rigid;
    private float realY = -2.22f;               // 落地的Y轴坐标
    private List<PlayerBehavior> players = new List<PlayerBehavior>();
    public float speedY = 0;                    // Y轴初速度
    public float gravity = 20;                  // 重力加速度
    public GameObject bomb;                     // 爆炸特效
    public float rotateSpeed;                   // 旋转速度
    public AudioClip bombAudio;                 // 爆炸音效
    private AudioSource audioPlay;              // 声音播放
    private SpriteRenderer sprite;              // 精灵渲染器

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector3.left * 2;
        audioPlay = GetComponent<AudioSource>();
        sprite = GetComponent<SpriteRenderer>();
    }

    //  摘要:
    //      复制自身并进行实例化
    //
    //  参数:
    //      position:
    //      要实例化的位置世界坐标
    //      velocity:
    //      手雷的速度
    //      targetType:
    //      手雷的攻击目标类型
    //      realY:
    //      手雷落地的Y轴坐标
    //
    //  返回值:
    //      实例化自身的游戏对象
    public GameObject CopySelf(Vector3 position, Vector3 velocity, float realY, PlayerType targetType)
    {
        var temp = Instantiate(gameObject, position, Quaternion.identity);
        var Grenade = temp.GetComponent<Grenade>();
        Grenade.rigid.velocity = velocity;
        Grenade.targetType = targetType;
        Grenade.realY = realY;
        if (velocity.x < 0)
        {
            Vector3 scale = temp.transform.localScale;
            scale.x = -scale.x;
            temp.transform.localScale = scale;
        }


        return temp;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y > realY)
        {
            speedY += gravity * Time.deltaTime;
            transform.position += Vector3.down * Time.deltaTime * speedY;
            if (rigid.velocity.x > 0)
            {
                transform.Rotate(Vector3.forward * (-Time.deltaTime * rotateSpeed));
            }
            else
            {
                transform.Rotate(Vector3.forward * (Time.deltaTime * rotateSpeed));
            }
        }
        else
        {
            Vector3 position = transform.position;
            position.y = realY;
            transform.position = position;
            Bomb();
        }
    }

    /**
     * 爆炸，给予范围内敌人一定伤害
     */
    void Bomb()
    {
        Instantiate(bomb, transform.position, Quaternion.identity);
        foreach(var player in players)
        {
            if (Mathf.Abs(transform.position.y - player.transform.position.y) <= GameManager.instance.sameLineInterval)
                player.Injury(injuryValue);
        }
        audioPlay.clip = bombAudio;
        audioPlay.Play();
        sprite.color = new Color(1, 1, 1, 0);
        this.enabled = false;
        Invoke("DestroySelf", 5f);
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            var player = collision.gameObject.GetComponent<PlayerInformation>();
            if (player.playerType == targetType)
            {
                players.Add(collision.gameObject.GetComponent<PlayerBehavior>());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            var player = collision.gameObject.GetComponent<PlayerInformation>();
            if (player.playerType == targetType)
            {
                players.Remove(collision.gameObject.GetComponent<PlayerBehavior>());
            }
        }
    }
}
