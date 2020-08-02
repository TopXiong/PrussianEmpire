using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * 
 * 子弹类，应挂载在子弹物体上，决定子弹的行为
 *
 */
public class Bullet : MonoBehaviour
{
    //public float offsetPivot = 1;       // 锚点偏移
    public GameObject hit;              // 击伤特效
    public int injuryValue = 10;        // 伤害值 （可调）
    public float speed = 10;            // 子弹速度 m/s （可调）
    private PlayerType targetType = PlayerType.Enemy;      // 攻击目标类型 （由指定攻击对象指出）
    private float survivalPeriod = 5;   // 生存期（从出生到销毁的最长时间）如果以后想调试更改，可以改成public

    private Rigidbody2D rigid;          // 刚体组件
    private SpriteRenderer[] sprites;   // 身上的精灵组件
    private Transform son;              // 子游戏物体
    private bool isCrit = false;        // 是否暴击 （先写着，暂时还没有实际用途）
    /**
     * 赋值自己并实例化，初始化速度方向，以及攻击目标类型
     * @param
     * position:
     *  复制体在地面的具体位置
     * rotation:
     * 复制体的初始方向
     * velocity:
     * 复制体的速度（矢量）
     * height;
     * 子弹距离地面的高度
     * targetType:
     * 复制体的攻击目标类型
     * isCrit:
     * 是否暴击（暂时没用到）
     */
    public Transform CopySelf(Vector3 position, Vector3 velocity, float height, PlayerType targetType, bool isCrit = false)
    {
        //Debug.Log("Bullet Height:" + height);
        Transform copy = Instantiate(transform, position, Quaternion.identity);
        Bullet bullet = copy.GetComponent<Bullet>();
        bullet.targetType = targetType;
        bullet.rigid.velocity = velocity.normalized * speed;
        bullet.isCrit = isCrit;
        var sonPosition = new Vector3(0, height, -height);
        bullet.son.localPosition = sonPosition;
        if (velocity.x < 0)
        {   // 让子弹朝向速度方向
            Vector3 scale = copy.transform.localScale;
            scale.x = -scale.x;
            copy.transform.localScale = scale;
        }
        return copy;
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        son = transform.Find("RealBullet");
    }

    // Start is called before the first frame update
    void Start()
    {
        sprites = GetComponentsInChildren<SpriteRenderer>();
        Destroy(gameObject, survivalPeriod);        // 一定时间后销毁自身
    }


    void Update()
    {
        // 控制自游戏物体的z坐标等于y坐标的相反数
        var position = son.localPosition;
        if (position.y < 0)
            position.y = 0;
        position.z = -position.y;
        son.localPosition = position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Bullet OnTriggerEnter2D" + collision.tag);
        GameObject target = collision.gameObject;
        var pos = son.position;
        if (target.CompareTag("Obstacle"))
        {
            if (transform.position.y >= target.transform.position.y - GameManager.instance.sameLineInterval)
            {
                //Destroy(gameObject);
                if (hit != null)
                {
                    Vector3 scale = Vector3.one;
                    if (transform.localScale.x * collision.transform.localScale.x < 0)
                        scale.x = -scale.x;
                    
                    var h = Instantiate(hit, pos, Quaternion.identity, collision.transform);
                    h.transform.localScale = scale;
                    //Debug.Log(h.transform.localScale);
                }
                rigid.velocity = Vector3.zero;      // 打到物体了，速度停止
                Hide();         // 先隐藏，它的生命周期结束自然死亡
                enabled = false;   // 禁用组件，不然虽然看不见他了，但还可能对敌人造成伤害
                GetComponentInChildren<PolygonCollider2D>().enabled = false;
            }
        }
        if (target.CompareTag("Player"))
        {
            if (Mathf.Abs(transform.position.y - target.transform.position.y) > GameManager.instance.sameLineInterval)
                return;
            PlayerInformation targetInfo = target.GetComponent<PlayerInformation>();
            PlayerBehavior playerBehavior = target.GetComponent<PlayerBehavior>();     // 待初始化
            if (targetInfo.playerType == targetType)
            {
                if (isCrit)
                {
                    // 如果该次攻击造成暴击
                    // 因为实际没有用到，所以没有实现
                }
                else
                {
                    playerBehavior.Injury(injuryValue);    // 如果检测到敌人在伤害触发范围类就调用给予一定伤害
                    if (hit != null)
                    {
                        Vector3 scale = Vector3.one;
                        if (transform.localScale.x * collision.transform.localScale.x < 0)
                            scale.x = -scale.x;
                        var h = Instantiate(hit, pos, Quaternion.identity, collision.transform);
                        h.transform.localScale = scale;
                        //Debug.Log(h.transform.localScale);
                    }
                }

                //Destroy(gameObject);                   // 碰到敌人，直接销毁自己
                rigid.velocity = Vector3.zero;
                Hide();
                enabled = false;   // 禁用组件，不然虽然看不见他了，但还可能对敌人造成伤害
                GetComponentInChildren<PolygonCollider2D>().enabled = false;
            }
        }
    }


    private void Hide()
    {
        if (sprites == null) return;
        foreach (var sprite in sprites)
        {
            var color = sprite.color;
            color.a = 0f;
            sprite.color = color;
        }
    }
}
