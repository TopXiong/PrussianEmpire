using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * 角色（包括玩家与敌人）的一些基本信息
 * 可以通过游戏物体上挂载的这个类来判断是否是攻击对象
 * 可以修改这个类的信心
 * 可以根据这个类来显示UI信息
 */
public class PlayerInformation : MonoBehaviour
{
    public int maxBlood;            // 血量最大值    
    public PlayerType playerType;   // 角色类型
    public int bulletAmaount;       // 子弹数量
    private int currentBlood;       // 当前血量
    private int critProbability = 0;// 暴击几率 （最小值0，最大值100）（暂未投入实际应用）
    public bool IsAlive             // 是否生存 （只读）
    {
        get => currentBlood > 0;
    }

    /**
     * 设置/获取暴击率，最小值0，最大值100，超过范围自动使用边界值
     * 参数:
     * value: 设置的暴击率数值
     * 返回值:
     * 返回暴击率
     */
    public int CritProbability
    {
        get => critProbability;
        set 
        {
            critProbability = value;
            if (value > 100)
                critProbability = 100;
            else if (value < 0)
                critProbability = 0;
        }
    }
    
    /**
     * 设置当前血量
     */
    public int CurrentBlood
    {
        get => currentBlood;
        set
        {
            currentBlood = value;
            if (currentBlood < 0)
                currentBlood = 0;
            else if (currentBlood > maxBlood)
                currentBlood = maxBlood;
        }
    }

    public bool CheckIfAttack       // 是否可以进行攻击
    {
        get => bulletAmaount > 0; 
    }

    private void Start()
    {
        currentBlood = maxBlood;
        // Debug.Log(gameObject.name);
    }
}
