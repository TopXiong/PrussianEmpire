using System;
using UnityEngine;

public class BloodStripController : MonoBehaviour
{
    private float currentValue = 1;        // 血条当前值，应该在0表示空血，1表示满血
    private Transform blood;               // 显示当前具体值
    private Transform bloodChange;         // 血值改变显示效果
    private float speed = 4;               // 血条变化效果速度
    private SpriteRenderer[] sprites;      // 血条所有子物体身上的精灵渲染器
    private float smooth = 0;              // 血条显示/隐藏平滑值

    private bool isLerpHide = false;       // 是否插值隐藏
    private bool isLerpDisplay = false;    // 是否插值显示
    private float currentAlpha = 1;        // 当前透明度 1:完全不透明，0:完全透明

    // Start is called before the first frame update
    void Start()
    {
        blood = transform.Find("Blood");
        bloodChange = transform.Find("BloodChange");
        sprites = GetComponentsInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bloodChange != null)
        {
            float x = bloodChange.transform.localScale.x;
            if (x != currentValue)
            {
                x = Mathf.Lerp(x, currentValue, speed * Time.deltaTime);
                if (Mathf.Abs(x - currentValue) < 0.02f)
                    x = currentValue;
                bloodChange.transform.localScale = new Vector3(x, 1, 1);
            }
        }

        if (isLerpHide)
        {
            currentAlpha = Mathf.Lerp(currentAlpha, 0, smooth * Time.deltaTime);
            if (currentAlpha < 0.02f)
            {
                currentAlpha = 0;
                isLerpHide = false;
            }
            SetAlpha(currentAlpha);
        }
        else if (isLerpDisplay)
        {
            currentAlpha = Mathf.Lerp(currentAlpha, 1, smooth * Time.deltaTime);
            if (currentAlpha > 0.98f)
            {
                currentAlpha = 1;
                isLerpDisplay = false;
            }
            SetAlpha(currentAlpha);
        }
    }

    /**
     * 设置血条透明度
     * 
     * 参数:
     *  alpha:血条透明度，1:完全不透明，0:完全透明
     */
    private void SetAlpha(float alpha)
    {
        foreach(var sprite in sprites)
        {
            Color color = sprite.color;
            color.a = alpha;
            sprite.color = color;
            // Debug.Log(sprite.name + sprite.color.a);
        }
    }

    /**
     * 设置血条底色变化速度，值越大变化越快
     * 参数：
     *      value:血条底色变化的速度
     */
    public float Speed
    {
        set => speed = value;
    }
    /**
     * 设置血条当前值
     * 参数:
     *      value:血条当前值的百分比，值必须在0-1之间
     */
    public float BloodValue
    {
        set
        {
            if (value < 0 || value > 1)
                throw new ArgumentOutOfRangeException("BloodValue", "值必须在0-1之间");
            if (blood != null)
                blood.transform.localScale = new Vector3(value, 1, 1);
            currentValue = value;
        }
    }

    /**
     * 得到血条底色变化的当前值
     */
    public float CurrentBloodChangeValue
    {
        get => bloodChange.localScale.x;
    }

    /**
     * 隐藏血条让血条不可见，但仍然存在
     */
    public void Hide()
    {
        if (sprites != null)
        {
            foreach(var sprite in sprites)
            {
                Color color = sprite.color;
                color.a = 0;
                sprite.color = color;
            }
        }
    }

    /**
     * 显示血条，让血条可见
     */
    public void Display()
    {
        if (sprites != null)
        {
            foreach (var sprite in sprites)
            {
                Color color = sprite.color;
                color.a = 1;
                sprite.color = color;
            }
        }
    }


    /**
     * 用插值将血条显示
     * 
     * 参数:
     * smooth: 插值平滑值，值越大，血条显示的越快
     */
    public void LerpDisplay(float smooth)
    {
        isLerpDisplay = true;
        this.smooth = smooth;
    }

    /**
     * 用插值将血条透明化
     * 
     * 参数:
     * smooth: 插值平滑值，值越大，血条隐藏的越快
     */
    public void LerpHide(float smooth) 
    {
        isLerpHide = true;
        this.smooth = smooth;
    }
}
