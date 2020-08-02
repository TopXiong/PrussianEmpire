using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(Random.value > 0.3f)
        {
            GetComponent<Animator>().Play("行动");
        }
        else
        {
            GetComponent<Animator>().Play("举旗行动");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * 3 * Time.fixedDeltaTime);
    }
}
