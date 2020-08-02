using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGunEnemyBehavior : MonoBehaviour
{
    public float attackInterval = 2f;       // 射击时间间隔
    public int bulletCountPerTime = 5;      // 每次射击多少发子弹
    private float attackTimeCount = 0f;     // 射击计时
    public Transform attackPoint;           // 枪口
    public GameObject bulletPrefab;         // 子弹预制体
    private Bullet bullet;                  // 子弹类
    private Animator anim;                  // 动画组件
    private PlayerInformation info;         // 自身信息
    private PlayerBehavior behavior;        // 自身行为
    private bool deathFlag = false;         // 死亡标记
    // Start is called before the first frame update
    void Start()
    {
        bullet = bulletPrefab.GetComponent<Bullet>();
        anim = GetComponentInChildren<Animator>();
        info = GetComponent<PlayerInformation>();
        behavior = GetComponent<PlayerBehavior>();
        behavior.Turn(-1);  // 朝左
    }

    // Update is called once per frame
    void Update()
    {
        if (deathFlag) return;
        if (!info.IsAlive)
        {
            deathFlag = true;
            behavior.Turn(1);   // 朝右
            anim.Play("Death");
            Destroy(gameObject, 30f);       // 30s后销毁自己
            return;
        }
        attackTimeCount += Time.deltaTime;
        if (attackTimeCount >= attackInterval)
        {
            attackTimeCount -= attackInterval;
            anim.Play("Attack");
            StartCoroutine(Attack());
        }
    }

    private void FixedUpdate()
    {
        var state = anim.GetCurrentAnimatorStateInfo(0);
        if (state.IsName("Death"))
        {
            transform.Translate(Vector3.right * 3f * Time.fixedDeltaTime);
        }
    }

    IEnumerator Attack()
    {
        for (int i = 0; i < bulletCountPerTime; ++i)
        {
            if (info.IsAlive)
            {
                var position = transform.position;
                position.x = attackPoint.position.x;
                position.z = 0f;
                var velocity = Vector3.right;
                if (transform.localScale.x < 0)
                    velocity = Vector3.left;
                bullet.CopySelf(position, velocity, attackPoint.localPosition.y, PlayerType.Player)
                    .GetComponent<Bullet>().injuryValue = 10;
                yield return new WaitForSeconds(0.1f);  // 每个0.1s射一发子弹
            }
        }
    }
    
}
