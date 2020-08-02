using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public float gravity = 10f;             // 重力加速度
    public int injuryValue = 30;         // 爆炸伤害
    public GameObject bombSpecialEffect;    // 爆炸特效

    private Vector3 speed = default;        // 速度
    private PlayerType targetType 
        = PlayerType.Player;                // 攻击目标类型  
    private Transform child;                // 子物体

    private SpriteRenderer sprite;          // 精灵组件

    private List<PlayerBehavior> playerList
        = new List<PlayerBehavior>();       // 在爆炸范围内的玩家

    

    private void Awake()
    {
        child = transform.Find("RealShell");
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    /**
     * 
     * 实例化自身
     * 
     * 参数
     * position: 伪3D空间内坐标 x为水平方向，y为空中方向，z为远处方向
     * 
     * speed: 伪3D空间速度
     * 
     * target: 攻击目标类型
     */
    public GameObject CopySelf(Vector3 position, Vector3 speed, PlayerType target)
    {
        var pos = position;
        pos.y = position.z;
        pos.z = 0f;
        var self = Instantiate(gameObject, pos, Quaternion.identity).GetComponent<Shell>();
        self.speed = speed;
        self.targetType = target;
        pos.y = position.y;
        pos.x = 0f;
        pos.z = -pos.y;
        self.child.localPosition = pos;
        var up = speed;
        up.z = 0f;
        up.y += speed.z;
        up.Normalize();
        self.child.up = up;
        return self.gameObject;
    }




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 控制高度方向
        var tspeed = speed;
        tspeed.y += speed.z;
        tspeed.z = 0f;
        tspeed.Normalize();
        child.up = tspeed;
        var position = child.localPosition;
        if (position.y < 0f)
        {
            position.y = 0f;
            // 落到地面了，然后爆炸
            Bomb();
            enabled = false;    // 爆炸后禁用组件
        }
        position.z = -position.y;
        child.localPosition = position;
        
    }


    void Bomb()
    {
        Hide();
        Destroy(gameObject, 30f); // 30秒后销毁自身
        Instantiate(bombSpecialEffect, transform.position, Quaternion.identity);
        foreach(var player in playerList)
        {
            player.Injury(injuryValue);
        }
    }

    
    void Hide()
    {
        var color = sprite.color;
        color.a = 0f;
        sprite.color = color;
    }

    private void FixedUpdate()
    {
        this.speed.y -= gravity * Time.fixedDeltaTime; // 重力加速度影响
        // 控制水平面方向
        Vector3 speed = this.speed;
        speed.y = speed.z;
        speed.z = 0f;
        transform.Translate(speed * Time.fixedDeltaTime);
        child.localPosition += new Vector3(0f, this.speed.y, 0f) * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var info = collision.GetComponent<PlayerInformation>();
            if (info.playerType == targetType)
            {   // 检测到玩家进入爆炸范围内，将玩家加入集合
                playerList.Add(info.GetComponent<PlayerBehavior>());
                //Debug.Log("检测到玩家，加入集合中");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var info = collision.GetComponent<PlayerInformation>();
            if (info.playerType == targetType)
            {   // 检测到玩家离开爆炸范围内，将玩家移除集合
                playerList.Remove(info.GetComponent<PlayerBehavior>());
            }
        }
    }
}
