using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerBehavior : MonoBehaviour
{

    public  UnityEvent OnDeath;                    // 死亡事件
    private const float lerpSmooth = 4;             // 平滑值
    private Color playerColor = Color.white;        // 角色颜色
    public SpriteRenderer[] spriteRenderer;         // 精灵渲染组件
    private PlayerInformation playerInformation;    // 角色信息组件
    private CapsuleCollider2D capsuleCollider;      // 胶囊碰撞器
    private BloodStripController bloodStrip;        // 血条控制
    private bool deathFlag = false;                 // 标记是否死亡
    private Color currentColor = Color.white;       // 当前颜色
    private bool ishide = false;                    // 隐藏自身标记
    private float hideSpeed = 0f;                   // 隐藏的速度

    /**
     * 得到人物朝向
     */
    public Vector3 Orientation
    {
        get => transform.localScale.x >= 0f ? Vector3.right : Vector3.left;
    }

    private void Awake()
    {
        //spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        playerInformation = GetComponent<PlayerInformation>();
    }

    // Start is called before the first frame update
    void Start()
    {
        bloodStrip = GetComponentInChildren<BloodStripController>();
    }

    // Update is called once per frame
    void Update()
    {
        // 将角色颜色平滑到白色
        float alpha = currentColor.a;
        currentColor = Color.Lerp(currentColor, playerColor, lerpSmooth * Time.deltaTime);
        currentColor.a = alpha;
        //spriteRenderer.color = color;

        if (!deathFlag)
        {   // 如果没有死
            if (!playerInformation.IsAlive)
            {// 如果死了，就标记死亡,这里执行角色在刚刚死亡那一刻发生的事情
                deathFlag = true;
                OnDeath?.Invoke();
                Death();
            }
        }
        else
        {
            // 死亡之后执行的事
            if (bloodStrip != null && bloodStrip.CurrentBloodChangeValue <= 0f)
            {
                bloodStrip.LerpHide(3);              // 让血条逐渐隐藏
                Destroy(bloodStrip.gameObject, 60f); // 一分钟之后销毁血条
            }

            if (ishide)
            {
                // 插值隐藏自己，将精灵组件的透明度降低
                currentColor.a = Mathf.Lerp(currentColor.a, 0f, hideSpeed * Time.deltaTime);
                if (currentColor.a < 0.01f)
                    currentColor.a = 0f;
            }
        }

        SetSpriteColor(currentColor);               // 设置精灵组件颜色值
    }

    /**
     * 收到伤害
     * 
     * 参数:
     * damage: 收到的伤害值
     */
    public void Injury(int damage)
    {
        playerInformation.CurrentBlood -= damage;
        //spriteRenderer.color = Color.red;
        currentColor = Color.red;
        SetSpriteColor(Color.red);
        if (bloodStrip != null)
            bloodStrip.BloodValue = (float) playerInformation.CurrentBlood / playerInformation.maxBlood;
    }

    /**
     * 恢复体力
     * 
     * 参数:
     * vitality: 恢复的体力值
     */
    public void Recover(int vitality)
    {
        playerInformation.CurrentBlood += vitality;
        if (bloodStrip != null)
            bloodStrip.BloodValue = (float)playerInformation.CurrentBlood / playerInformation.maxBlood;
    }

    private void SetSpriteColor(Color color)
    {
        foreach(var sprite in spriteRenderer)
        {
            sprite.color = color;
        }
    }

    /**
     * 角色死亡处理
     */
    private void Death()
    {
        if (capsuleCollider != null)
            capsuleCollider.enabled = false;        // 禁用触发器
        
    }

    /**
     * 逐渐隐藏自身
     * 
     * 参数
     *  speed: 以多快的速度隐藏
     */
    public void LerpHide(float speed = 3)
    {
        ishide = true;
        hideSpeed = speed;
    }


    /**
     * 改变角色朝向
     * 
     * 参数
     *  direction:方向，1朝右，-1朝左
     * 
     */
    public void Turn(float direction)
    {
        if (direction != 1 && direction != -1)
            throw new ArgumentOutOfRangeException("PlayerBehavior Method Turn direction", "值必须为1或-1");
        if (bloodStrip != null)
        {
            var bscale = bloodStrip.transform.localScale;
            bscale.x = Mathf.Abs(bscale.x) * direction;
            bloodStrip.transform.localScale = bscale;
        }
        transform.localScale = Vector3.one;
        var scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
    }
}
