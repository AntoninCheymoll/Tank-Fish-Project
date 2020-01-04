using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FishMovement : MonoBehaviour
{
    float gameX = -1;
    float gameY = -1 ;

    float fishLength = 2.3f;
    float fishWidth = 0.8f;

    public float directionX = 1;
    public float directionY = 0;

    public float speed = 0.3f;

    public float borderOffset = 5f;



    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(gameX == -1)
        {
            gameX = GameObject.Find("Ground").GetComponent<GroundBehavior>().gameX;
            gameY = GameObject.Find("Ground").GetComponent<GroundBehavior>().gameY;
        }
       

        //float rad = getRadian();
        transform.position += (new Vector3(directionX, 0, directionY))*speed;

        if (transform.position.x > gameX * 4)
        {
            updateDir(Mathf.PI);
            //directionX = -1;

        }
        else if (transform.position.x < -(gameX * 4))
        {
            updateDir(0);
            //directionX = 1;
        }

       
        if (transform.position.z > gameY * 4)
        {
            updateDir(-Mathf.PI/2);

        }
        else if (transform.position.z < -(gameY * 4))
        {
            updateDir(Mathf.PI/2);
        }

        transform.eulerAngles = new Vector3(0, getOrientation()/2/Mathf.PI*360, 0);

        /*
        if( newDirX != directionX || newDirY != directionY)
        {
            directionX = (directionX > newDirX) ? Mathf.Max(newDirX, directionX - 0.1f) : Mathf.Min(newDirX, directionX + 0.1f);
            directionY = (directionY > newDirY) ? Mathf.Max(newDirY, directionY - 0.1f) : Mathf.Min(newDirY, directionY + 0.1f);
        }*/
    }
   
    float getOrientation()
    {
        return Mathf.Atan2(directionX, directionY);
    }

    void updateDir(float angle)
    {
        float randAngle = Random.Range(angle - Mathf.PI/4, angle + Mathf.PI/4);
        directionX = Mathf.Cos(randAngle);
        directionY = Mathf.Sin(randAngle);
}
}
