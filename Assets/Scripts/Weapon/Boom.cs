using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boom : MonoBehaviour
{
    public float Delay =3f;
    public GameObject bombSpecialEffect;
    private SpriteRenderer sprite;
    public int injuryValue = 30;
    private List<PlayerBehavior> playerList
    = new List<PlayerBehavior>();       // 在爆炸范围内的玩家

    private List<GameObject> wireList
    = new List<GameObject>();       // 在爆炸范围内的玩家

    void Update()
    {
        Delay -= Time.deltaTime;
        if (Delay <= 0)
        {
            Bomb();
            gameObject.SetActive(false);
        }
    }

    void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    void Bomb()
    {
        Hide();
        Destroy(gameObject, 30f); // 30秒后销毁自身
        Instantiate(bombSpecialEffect, transform.position, Quaternion.identity);
        foreach (var player in playerList)
        {
            player.Injury(injuryValue);
        }
        foreach(var wire in wireList)
        {
            wire.GetComponent<SpriteRenderer>().enabled = false;
            wire.GetComponent<BoxCollider2D>().enabled = false;
            wire.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
            //wire.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.tag + collision.name);
        if (collision.CompareTag("Wire"))
        {
            wireList.Add(collision.gameObject);
        }
        if (collision.CompareTag("Player"))
        {
            var info = collision.GetComponent<PlayerInformation>();
            if (info.playerType == PlayerType.Enemy)
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
            if (info.playerType == PlayerType.Enemy)
            {   // 检测到玩家离开爆炸范围内，将玩家移除集合
                playerList.Remove(info.GetComponent<PlayerBehavior>());
            }
        }
    }

    void Hide()
    {
        var color = sprite.color;
        color.a = 0f;
        sprite.color = color;
    }

}
