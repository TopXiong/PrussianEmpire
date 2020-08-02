using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ambulance : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject rescue;
    public float rescueSpeed = 6;          // 恢复速度 （默认每秒6点血）
    private float rescueCount = 0;
    private List<PlayerBehavior> players = new List<PlayerBehavior>();
    private Dictionary<PlayerBehavior, GameObject> map = new Dictionary<PlayerBehavior, GameObject>();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rescueCount += rescueSpeed * Time.deltaTime;
        foreach(var player in players)
        {
            player.Recover((int)(rescueCount));
        }
        if (rescueCount > 1)
            rescueCount = 0;        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            var info = collision.gameObject.GetComponent<PlayerInformation>();
            if (info.playerType == PlayerType.Player)
            {
                var pb = info.GetComponent<PlayerBehavior>();
                players.Add(pb);
                var t = info.transform.Find("Rescue");
                map.Add(pb, Instantiate(rescue, t.position, Quaternion.identity, t));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            var info = collision.gameObject.GetComponent<PlayerInformation>();
            if (info.playerType == PlayerType.Player)
            {
                var pb = info.GetComponent<PlayerBehavior>();
                players.Remove(pb);
                Destroy(map[pb]);
                map.Remove(pb);
            }
        }
    }
}
