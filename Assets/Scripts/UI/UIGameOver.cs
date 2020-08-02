using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIGameOver : MonoBehaviour
{
    public Sprite gameClear;        // 游戏胜利图片
    public Sprite gameOver;         // 游戏失败图片

    private Image imageShow;        // 显示游戏结束的图像UI
    private Image childImage;       // 按钮物体上的图像UI
    private bool isGameOver = false;        // 游戏是否结束标志
    private Color color;                    // 图像UI最初的颜色
    private const float colorSmooth = 4;    // 颜色变化插值平滑值
    private Button btn;                     // UI按钮
    private void Awake()
    {
        imageShow = GetComponent<Image>();
        btn = GetComponentInChildren<Button>();
        childImage = GetComponentInChildren<Image>();
        childImage.raycastTarget = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.color = imageShow.color;
        Color color = imageShow.color;
        color.a = 0f;
        imageShow.color = color;
        imageShow.raycastTarget = false;
        btn.enabled = false;
        //GameManager.instance.OnGameOver.AddListener(SetIsGameOver);
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver)
        {
            imageShow.color = Color.Lerp(imageShow.color, color, colorSmooth * Time.deltaTime);
            if (imageShow.color.a < color.a && Mathf.Abs(imageShow.color.a - color.a) < 0.05)
            {
                imageShow.color = color;
                imageShow.raycastTarget = true;
                btn.enabled = true;
                childImage.raycastTarget = true;
            }
        }
    }

 
    /**
     * 游戏失败，延时1秒后显示失败UI
     */
    public void GameOver()
    {
        imageShow.sprite = gameOver;
        Invoke("SetIsGameOver", 1f);    // 延时1秒调用
    }

    /**
      * 游戏胜利，延时1秒后显示胜利UI
      */
    public void GameClear()
    {
        imageShow.sprite = gameClear;
        Invoke("SetIsGameOver", 1f);
    }

    private void SetIsGameOver()
    {
        isGameOver = true;
    }


    /**
     * 返回到开始游戏界面（选人界面）
     */
    public void ReturnToMainInterface()
    {
        //Debug.Log("Return to StartScene");
        //PlayerSelect.currentSelect = 0;
        //PlayerSelect.selectPlayers.Clear();
        if (GameManager.instance.GameOver)
        {
            SceneManager.LoadScene("StartScene2.0");
            return;
        }
        int level = int.Parse(SceneManager.GetActiveScene().name.Split('_')[1]);
        if(level < 2)
        {
            SceneManager.LoadScene("GameScene_" + (level+1));
        }else
        {
            SceneManager.LoadScene("StartScene2.0");
        }
    }
}
