using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundBehavior : MonoBehaviour
{
    public float gameX = 12.5f;
    public float gameY = 7f;
    public bool display = true; 

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Game Height: " + gameX);
        Debug.Log("Game Width: " + gameY);


        if (display) {
            transform.localScale = new Vector3(gameX, 1, gameY);
        } else {
            transform.localScale = new Vector3(0, 0, 0);
        }

        //Debug.Log(transform.localScale);

    }

    // Update is called once per frame
    void Update()
    {


        if (display)
        {
            transform.localScale = new Vector3(gameX, 1, gameY);
        }
        else
        {
            transform.localScale = new Vector3(0, 0, 0);
        }
    }
}
