using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShowAllEnemyDeath : MonoBehaviour
{
    private Text text;                  // 文本组件
    public void ShowAllEnemyDeathText()
    {
        //gameObject.SetActive(true);
        Color color = text.color;
        color.a = 200;
        text.color = color;
    }

    public void ShowYouAreDeathText()
    {
        text.text = "You Are Death";
        ShowAllEnemyDeathText();
    }

    private void Awake()
    {
        text = GetComponent<Text>();
    }
}
