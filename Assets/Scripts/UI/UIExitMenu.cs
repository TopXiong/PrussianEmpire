using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UIExitMenu : MonoBehaviour
{
    public AudioClip clickAudio;    // 点击音效
    private RectTransform exit;     // 退出游戏物体transform组件    
    public AnimationClip animClip;  // OnExit动画
    private Animator anim;          // 动画机
    private bool isExit = false;    // 是否是退出菜单界面
    private bool isNotPausle = true;// 是否不是暂停状态
    private AudioSource audioPlay;  // 音乐播放
    private void Awake()
    {
        anim = GetComponent<Animator>();
        exit = GetComponent<RectTransform>();
        audioPlay = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isExit)
            {
                OnExit();
            }
        }
        if (isExit && isNotPausle)
        {
            if (exit.anchoredPosition.y <= 0)
            {
                Time.timeScale = 0;
                isNotPausle = false;
                System.GC.Collect();        // 游戏暂停, 手动清理一次内存
            }
        }

    }


    public void OnContinue()
    {
        Time.timeScale = 1;
        audioPlay.clip = clickAudio;
        audioPlay.Play();
        anim.Play("OnRegame");
        isExit = false;
        Debug.Log("OnContinue");
    }

    public void OnQuit()
    {
        audioPlay.clip = clickAudio;
        audioPlay.Play();
        OnContinue();
        // EditorApplication.isPlaying = false;
        Debug.Log("OnQuit");
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();  
#endif
    }

    public void OnExit()
    {
        //if (animClip == null)
        //    return;
        isExit = true;
        anim.Play("OnExit");
        Debug.Log("OnExit");
    }

}
