using UnityEngine;

/**
 * 存活时间管理
 * 超过设置时间后自动死亡（由于游戏中很多游戏物体都会自行死亡，所以自成一类）
 */
public class LifeTimeManager : MonoBehaviour
{
    public float lifeTime = 0.1f;           // 生存期，超过这个时间自动销毁

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifeTime);  // 在lifeTime秒之后销毁自身
    }


}
