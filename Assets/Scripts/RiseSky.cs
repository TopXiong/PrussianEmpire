using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * 死亡升天类，控制游戏角色死亡升天
 */
public class RiseSky : MonoBehaviour
{
    private float riseSpeed = 0.8f;
    private float riseSkyTime = 1;      // 升天时长1s
    private float riseSkyCount = -0.5f;
    private PlayerInformation playerInfo;
    private SpriteRenderer[] spriteRenderer;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private Transform bloodStrip;
    private BloodStripController bloodStripController = null;

    private Color currentColor = Color.white;
    //private Rigidbody2D rigid;

    private void Awake()
    {
        playerInfo = GetComponent<PlayerInformation>();
        spriteRenderer =  GetComponentsInChildren<SpriteRenderer>();
        
        if (!TryGetComponent(out anim))
            anim = GetComponentInChildren<Animator>();

        //rigid = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponentInChildren<BoxCollider2D>();
        bloodStrip = transform.Find("BloodStrip");
        if (bloodStrip != null)
        {
            bloodStripController = bloodStrip.GetComponent<BloodStripController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerInfo.IsAlive)
        {
            if (boxCollider != null)
                boxCollider.enabled = false;
            if (bloodStrip != null)
            {
                //if (bloodStrip.gameObject.activeSelf)
                //    bloodStrip.gameObject.SetActive(false);
                // 血条不应该随着角色改变
                bloodStrip.parent = null;
                if (bloodStripController.CurrentBloodChangeValue < 0.02)
                {
                    bloodStripController.LerpHide(4);       // 让血条隐藏
                }
            }
            anim.enabled = false;
            riseSkyCount += Time.deltaTime;
            if (riseSkyCount < 0)
            {
                // 判断倒地方向
                float downDirection = 1f;
                if (transform.localScale.x < 1)
                    downDirection = -1f;
                // 旋转倒地
                transform.rotation = Quaternion.Euler(Vector3.forward * (riseSkyCount + 0.5f) * 180 * downDirection);
                return;
            }
            //Color color = spriteRenderer.color;
            if (riseSkyCount > riseSkyTime)
            {
                // 死透了
                this.enabled = false;
                currentColor.a = 0;
                //spriteRenderer.color = color;
                SetSpriteColor(currentColor);
                //gameObject.SetActive(false);            // 游戏物体失活
                Destroy(bloodStrip.gameObject);           // 销毁血条
                Destroy(gameObject);                      // 销毁自身
                return;
            }
            // 升天逐渐透明
            currentColor.a = (riseSkyTime - riseSkyCount) / riseSkyTime;
            //spriteRenderer.color = color;
            SetSpriteColor(currentColor);
            transform.position += Vector3.up * riseSpeed * Time.deltaTime;
        }
    }


    private void SetSpriteColor(Color color)
    {
        if (spriteRenderer == null) return;
        foreach(var sprite in spriteRenderer)
        {
            sprite.color = color;
        }
    }
}
