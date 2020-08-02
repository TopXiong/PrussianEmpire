using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIStartMenu : MonoBehaviour
{

    public GameObject BrightnessPanel;
    public GameObject SoundPanel;
    public GameObject CollectionPanel;
    public GameObject ChoosePanel;
    public GameObject DetailPanel;
    public GameObject AddButton;
    public Sprite KilianDetail, LennartDetail;

    private int PlayerCount =1;
    private int CurrentPlayer;          // 0 =Kilian,1=Lennart


    /// <summary>
    /// 游戏开始，场景跳转
    /// </summary>
    public void GameStart()
    {
        System.GC.Collect();
        SceneManager.LoadScene("GameScene_1");
    }

    public void AddPlayer()
    {
        PlayerCount = 2;
        if(CurrentPlayer == 0)
        {
            DetailPanel.GetComponent<Image>().sprite = LennartDetail;
            PlayerSelect.selectPlayers.Add(1);
        }
        else if (CurrentPlayer == 1)
        {
            DetailPanel.GetComponent<Image>().sprite = KilianDetail;
            PlayerSelect.selectPlayers.Add(0);
        }
        AddButton = EventSystem.current.currentSelectedGameObject;
        AddButton.SetActive(false);
    }

    public void ShowDetailPanel()
    {
        DetailPanel.SetActive(true);
        if (EventSystem.current.currentSelectedGameObject.name.Equals("Kilian"))
        {
            CurrentPlayer = 0;
            DetailPanel.GetComponent<Image>().sprite = KilianDetail;
        }else if (EventSystem.current.currentSelectedGameObject.name.Equals("Lennart"))
        {
            CurrentPlayer = 1;
            DetailPanel.GetComponent<Image>().sprite = LennartDetail;
        }
        PlayerSelect.selectPlayers.Add(CurrentPlayer);
    }

    public void ShowChoosePanel()
    {
        ChoosePanel.SetActive(true);
    }

    public void ShowCollectionPanel()
    {
        CollectionPanel.SetActive(true);
    }

    public void ShowSoundPanel()
    {
        SoundPanel.SetActive(true);
    }

    public void ShowBrightnessPanel()
    {
        BrightnessPanel.SetActive(true);
    }

    public void Close()
    {
        var button = EventSystem.current.currentSelectedGameObject;
        button.transform.parent.gameObject.SetActive(false);
        PlayerCount = 1;
        AddButton.SetActive(true);
        if (PlayerSelect.selectPlayers.Count > 0)
        {
            PlayerSelect.selectPlayers = new List<int>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
