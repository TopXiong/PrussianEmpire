using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmine : MonoBehaviour
{
    public int injuryValue = 30;           // 伤害值
    public GameObject bomb;                // 爆炸特效
    public AudioClip bombAudio;            // 爆炸声音
    private AudioSource audioPlay;         // 声音播放
    private EdgeCollider2D edgeCollider;   // 边缘碰撞器
    private SpriteRenderer sprite;         // 精灵渲染器
    private void Awake()
    {
        audioPlay = GetComponent<AudioSource>();
        edgeCollider = GetComponent<EdgeCollider2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Foot"))
        {
            var info = collision.transform.parent.GetComponent<PlayerInformation>();
            if (info.playerType == PlayerType.Player)
            {
                var behavior = info.gameObject.GetComponent<PlayerBehavior>();
                behavior.Injury(injuryValue);

                Instantiate(bomb, transform.position, Quaternion.identity);
                audioPlay.clip = bombAudio;
                audioPlay.Play();
                sprite.color = new Color(1, 1, 1, 0);
                edgeCollider.enabled = false;
                this.enabled = false;
                Invoke("DestroySelf", 5f);
                // Destroy(gameObject);
            }
        }
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}
