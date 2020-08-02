using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameManager instance = null;             // 单例模式 （该组件只能挂在GameManger游戏物体上，并且不应该通过其它任何方式实例化）
    private PlayerInformation[] mainPlayersInfo = null;    // 主角信息
    [HideInInspector]
    public GameObject[] mainPlayers = null;                // 玩家     
    public UnityEvent OnGameOver = null;                   // 游戏失败事件
    public UnityEvent OnGameClear = null;                  // 游戏通关事件
    public float sameLineInterval = 0.5f;                  // Y轴在这个间隔范围内那么两个物体就算在同一Z轴上       
    public float BGPlayerApperProbability = 0.01f;          // 当随机值小于这个值的时候出现背景小兵
    private bool gameClearFlag = false;                    // 游戏通关标志
    private bool isGameClear = false;                      // 是否通关
    private bool gameOverFlag = false;                     // 是否失败  
    public int level;                                      // 当前关卡
    public Transform[] initPoints;                         // 玩家出生点
    public GameObject[] playerPrefabs;                     // 玩家预制体
    public List<GameObject> enemysWare1 = new List<GameObject>();
    public List<GameObject> enemysWare2 = new List<GameObject>();
    public int EnemyCount;
    public GameObject BGPlayer;
    [SerializeField]
    private int enemyDeathCount = 0;


    private IEnumerator BGPlayerManager()
    {
        float interval = 1f;
        float temp = interval;
        while (true)
        {
            temp -= Time.deltaTime;
            if (temp > 0)
            {
                continue;
            }
            float appear = UnityEngine.Random.value;
            if (appear < BGPlayerApperProbability)
            {
                Instantiate(BGPlayer, new Vector3(-10.64f, 0.21f, 0f), Quaternion.identity).transform.localScale = new Vector3(0.3f,0.3f,0.3f);
            }
            yield return null;
        }
    }

    public void EnemyDeath()
    {
        enemyDeathCount++;


        if (level == 1)
        {
            if (enemyDeathCount >= EnemyCount)
            {
                IsGameClear = true;
            }
            if (enemyDeathCount == 5)
            {
                foreach (var enemy in enemysWare2)
                    enemy.SetActive(true);
                Camera.main.GetComponent<CameraController>().rightBoundary = 20f;
            }
        }
        if (level == 2)
        {
            if (enemyDeathCount >= EnemyCount)
            {
                IsGameClear = true;
            }
            if (enemyDeathCount == 3)
            {
                foreach (var enemy in enemysWare2)
                    enemy.SetActive(true);
                Camera.main.GetComponent<CameraController>().rightBoundary = 20f;
            }
        }
    }

    public bool GameOver                                   // 是否游戏失败
    {
        get
        {   // 玩家都不存活，判断为游戏结束标志
            if (mainPlayers == null)
                return true;
            for (int i = 0; i < mainPlayers.Length; ++i)
                if (mainPlayersInfo[i].IsAlive)
                    return false;
            return true;
        }
    }

    /**
     * 游戏玩家信息（只读）
     */
    public PlayerInformation[] MainPlayersInfo
    {
        get => mainPlayersInfo;
    }

    public bool IsGameClear                                // 是否通关
    {
        get => isGameClear;
        set => isGameClear = value;
    }

    public void GameClear()
    {
        isGameClear = true;
    }

    private void Awake()
    {
        level = int.Parse(SceneManager.GetActiveScene().name.Split('_')[1]);
        instance = this;  // 初始化自身实例    
        //Debug.Log("GameManager Awake");
        if (PlayerSelect.selectPlayers != null)
        {
            //Debug.Log("PlayerSelect not null");
            mainPlayers = new GameObject[PlayerSelect.selectPlayers.Count];
            for (int i = 0; i < mainPlayers.Length; ++i)    // i代表玩家，0表示玩家1,1就是玩家2，一次类推（当然没有更多了玩家了）
            {
                mainPlayers[i] = Instantiate(playerPrefabs[PlayerSelect.selectPlayers[i]], initPoints[i].position, Quaternion.identity);
                var player = mainPlayers[i].GetComponent<MainPlayerController>();
                player.PlayerNumber = i + 1;                // 设置玩家编号（由于玩家的不同控制也不同）
                //Debug.Log(mainPlayers[i]);
                //Debug.Log(mainPlayers[i].tag);
            }
        }
        //Debug.Log("玩家数量" + mainPlayers.Length);
        

        if (!(mainPlayers == null || mainPlayers.Length == 0))
        {
            //Debug.Log("GameManager Init main players information");
            mainPlayersInfo = new PlayerInformation[mainPlayers.Length];
            for (int i = 0; i < mainPlayers.Length; i++)
                mainPlayersInfo[i] = mainPlayers[i].GetComponent<PlayerInformation>();
        }
    }

    void Start()
    {
        EnemyCount = enemysWare1.Count + enemysWare2.Count;       
        foreach(var enemy in enemysWare1)
        {
            enemy.GetComponent<PlayerBehavior>().OnDeath.AddListener(EnemyDeath);
        }
        foreach (var enemy in enemysWare2)
        {
            enemy.GetComponent<PlayerBehavior>().OnDeath.AddListener(EnemyDeath);
        }
        StartCoroutine(BGPlayerManager());
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameClear)
        {
            // 如果游戏通关了会怎样
            if (!gameClearFlag)
            {
                gameClearFlag = true;       // 设置为游戏通关
                OnGameClear?.Invoke();
            }
            Destroy(gameObject);
        }

        if (GameOver)
        {
            // 如果游戏失败了会怎样
            if (!gameOverFlag)
            {
                gameOverFlag = true;
                OnGameOver?.Invoke();
            }
            Destroy(gameObject);
        }
    }
}
