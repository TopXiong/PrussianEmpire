using JetBrains.Annotations;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStartMenu : MonoBehaviour
{
    //private RectTransform rectTransform;
    //private bool isNext = false;
    //private Vector3 targetPosition = new Vector3(-605, 0, 0);
    //private float speed = 2000f;
    private Animator anim;

    public AudioClip selectClip;            // 选择音效

    //public Image[] select1 = new Image[2];                  // 第一个界面的两个图标
    //public Image[] select2 = new Image[2];                  
    //public Sprite[] spriteSelectPlayer1 = new Sprite[2];    //0黑1亮
    //public Sprite[] spriteSelectPlayer2 = new Sprite[2];


    public Image imagePlayer1;                              // 用来显示玩家1选择的人物图像UI
    public Image imagePlayer2;

    /* 可以选择的人物图片介绍 （玩家1和玩家2的图片并不一样，所以用了两个数组） */
    public Sprite[] spritePlayer1 = new Sprite[2];          // 可以选择的人物图片 （玩家1）
    public Sprite[] spritePlayer2 = new Sprite[2];          

    //private bool isSelect = true;            // 是否在选人的界面
    private int currentSelect = 0;           // 当前选择角色
    private AudioSource audioPlay;           // 声音播放
    private void Awake()
    {
        anim = GetComponent<Animator>();
        audioPlay = GetComponent<AudioSource>();
       // rectTransform = GetComponent<RectTransform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (!isSelect) return;
        //if (PlayerSelect.currentSelect == 0)
        //{
        //    if (Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.Space))
        //    {
        //        NextStep();
        //    }
        //}
        //else
        //{
        //    if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Space))
        //    {
        //        NextStep();
        //    }
        //}
    }

    /**
     * 玩家1选人
     */
    public void Select1()
    {
        currentSelect = 0;
        //if (PlayerSelect.currentSelect == 0)
        //{
        //    select1[0].sprite = spriteSelectPlayer1[0];
        //    select1[1].sprite = spriteSelectPlayer2[1];
        //}
        //else
        //{
        //    select2[0].sprite = spriteSelectPlayer1[0];
        //    select2[1].sprite = spriteSelectPlayer2[1];
        //}
        NextStep();

    }
    

    /**
     * 玩家2选人
     */
    public void Select2()
    {
        currentSelect = 1;
        //if (PlayerSelect.currentSelect == 0)
        //{
        //    select1[0].sprite = spriteSelectPlayer1[1];
        //    select1[1].sprite = spriteSelectPlayer2[0];
        //}
        //else
        //{
        //    select2[0].sprite = spriteSelectPlayer1[1];
        //    select2[1].sprite = spriteSelectPlayer2[0];
        //}
        NextStep();

    }
    /**
     * 开始游戏，加载场景
     */
    public void StartGame()
    {
        audioPlay.clip = selectClip;
        audioPlay.Play();
        SceneManager.LoadScene("GameScene_1");
    }
    /**
     * 退出游戏
    */
    public void QuitGame()
    {
        audioPlay.clip = selectClip;
        audioPlay.Play();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /**
     * 下一步
     */
    public void NextStep()
    {
        PlayerSelect.selectPlayers.Add(currentSelect);
        PlayerSelect.currentSelect++;
        currentSelect = 0;
        audioPlay.clip = selectClip;
        audioPlay.Play();
        switch (PlayerSelect.currentSelect)
        {
            case 1:                 // 玩家1选择了
                imagePlayer1.sprite = spritePlayer1[PlayerSelect.selectPlayers[0]];
                //isSelect = false;
                anim.Play("Next");
                break;
            case 2:                 // 玩家2选择了
                imagePlayer2.sprite = spritePlayer2[PlayerSelect.selectPlayers[1]];
                //isSelect = false;
                anim.Play("Next2");
                break;
            default:
                throw new ArgumentException("角色选择参数异常", "值应该为1，或2");
        }

        //PlayerSelect.selectPlayers = new int[1];
        //PlayerSelect.selectPlayers[0] = Random.Range(0,2);      // 玩家1随机选择了角色
    }

    /**
     * 添加2p
     */
    public void AddPlayer2()
    {
        anim.Play("Next1");
        //isSelect = true;
        audioPlay.clip = selectClip;
        audioPlay.Play();
    }
}
