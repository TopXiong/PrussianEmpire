using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarPlane : MonoBehaviour
{
    public float moveSpeed = 5;     // 移动速度
    public bool isRight = false;    // 是否向右行驶
    public GameObject grenade;      // 手雷
    public Transform grenadePoint;  // 扔雷点
    private Grenade grenadeBehavior;// 手雷
    private bool hasThrow = false;  // 是否已经扔雷了

    void Awake()
    {
        grenadeBehavior = grenade.GetComponent<Grenade>();

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasThrow)
        {
            if (transform.position.x <= Camera.main.transform.position.x)
            {
                if (TryGetPlayerCenterPoint(out var position))
                {
                    hasThrow = true;
                    var tGrenade = grenadeBehavior.CopySelf(grenadePoint.position, Vector3.zero, position.y, PlayerType.Player)
                        .GetComponent<Grenade>();
                    tGrenade.rotateSpeed = 0;
                    tGrenade.transform.rotation = grenadePoint.rotation;
                }
                 
                //grenadeBehavior.CopySelf();
            }
        }
    }

    private void FixedUpdate()
    {
        if (isRight)
            transform.Translate(Vector3.right * moveSpeed * Time.fixedDeltaTime);
        else
            transform.Translate(Vector3.left * moveSpeed * Time.fixedDeltaTime);
    }


    private bool TryGetPlayerCenterPoint(out Vector3 position)
    {
        position = default;
        var players = GameManager.instance.MainPlayersInfo;
        int count = 0;
        float y = 0;
        foreach (var info in players)
        {
            if (info != null && info.IsAlive)
            {
                ++count;
                y += info.transform.position.y;
            }
        }
        if (count > 0)
        {
            position.y = y / count;
            return true;
        }
        return false;
    }
}
