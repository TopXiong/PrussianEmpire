using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : MonoBehaviour
{
    public int injuryValue = 20;            // 伤害值
    private PlayerType targetType;          // 攻击目标类型
    private Transform child;
    private List<PlayerBehavior> players
        = new List<PlayerBehavior>();       // 范围内玩家


    public GameObject CopySelf(Vector3 position, float height, PlayerType target)
    {
        var poison = Instantiate(gameObject, position, Quaternion.identity)
            .GetComponent<Poison>();
        poison.targetType = target;
        poison.child.localScale = new Vector3(0, height, -height);
        return poison.gameObject;
    }

    private void Awake()
    {
        child = transform.Find("ChildPoison");
    }

    // Start is called before the first frame update
    void Start()
    {
        Invoke("Attack", 0.1f);
    }

    void Attack()
    {
        foreach(var player in players)
        {
            player.Injury(injuryValue);
        }
        Destroy(gameObject);
    }

    private void Update()
    {
        var pos = child.localPosition;
        pos.z = -pos.y;
        child.localPosition = pos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var info = collision.GetComponent<PlayerInformation>();
            if (info.playerType == targetType)
            {
                if (Mathf.Abs(transform.position.y - collision.transform.position.y) <= GameManager.instance.sameLineInterval)
                {
                    var playerBehavior = info.GetComponent<PlayerBehavior>();
                    if (!players.Contains(playerBehavior))
                        players.Add(playerBehavior);
                }
            }
        }
    }

}
