using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkyLight : MonoBehaviour
{

    private Image image;
    private float smooth = 2;
    private Color color;
    private void Awake()
    {
        image = GetComponent<Image>();
        color = image.color;
        color.a = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        image.color = Color.Lerp(image.color, color, Time.deltaTime * smooth);
    }
}
