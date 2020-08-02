using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    public int attackPower = 10;            // 攻击力
    private PlayerType targetType;          // 攻击目标类型
    private float virtualY;                 // 虚拟Y轴，用来判断物体是否在同一个Z轴上
    private float lifeTime;                 // 存活时间
    /**
    * 赋值自己并实例化，初始化速度方向，以及攻击目标类型
    * @param
    * position:
    *  复制体的初始位置
    * rotation:
    * 复制体的初始方向
    * parent:
    * 父游戏物体
    * virtualY:
    * 虚拟Y轴，用来判断物体是否在同一个Z轴上
    * lifeTime:
    * 存活时间
    * targetType:
    * 复制体的攻击目标类型
    */
    public Transform CopySelf(Vector3 position, Quaternion rotation, Transform parent,float virtualY,float lifeTime, PlayerType targetType)
    {
        // 待实现
        // 粗略实现
        Transform copy = Instantiate(transform, position, rotation, parent);
        Knife knife = copy.GetComponent<Knife>();
        knife.targetType = targetType;
        knife.virtualY = virtualY;
        knife.lifeTime = lifeTime;
        copy.transform.localScale = parent.localScale;
        return copy;
    }


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Death(lifeTime));
    }

    /**
     * 协程，延时一段时间后销毁自身
     */
    IEnumerator Death(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("OnTriggerEnter");
        if (!collision.tag.Equals("Player")) return;
        GameObject gameObject = collision.gameObject;
        PlayerInformation playerInfo = gameObject.GetComponent<PlayerInformation>();
        if (playerInfo.playerType == targetType)
        {
            if (Mathf.Abs(gameObject.transform.position.y - virtualY) > GameManager.instance.sameLineInterval)
                return;
            PlayerBehavior mainPlayerBehavior = gameObject.GetComponent<PlayerBehavior>();
            mainPlayerBehavior.Injury(attackPower);
            Destroy(this.gameObject);
        }
    }

}
