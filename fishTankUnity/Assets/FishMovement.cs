using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//this script is managing all the fish mouvements and behaviors
public class FishMovement : MonoBehaviour
{
    //game dimension
    float gameW = -1;
    float gameH = -1 ;

    float fishLength = 12;

    //direction of the fish
    float directionX;
    float directionZ;

    //new direction that the fish is turning to
    float newDirX;
    float newDirZ;

    float initialSpeed= 0.5f;
    float speed;
    float initialRotationSpeed = 0.05f;
    float rotationSpeed;

    //distance to the boundarie, which if crossed make the fish change direction
    float offSetHorizontal = 12;
    float offSetUpDown = 8;

    //contail all the different animations of the fish
    Animation anim;

    string along = ""; //along wich border the fish is, "no" if the fish is along none 

    bool isFaster = false; //is the fish accelerating


    // Start is called before the first frame update
    void Start()
    {
        //get the field dimensions
        gameW = 4 * GameObject.Find("Ground").GetComponent<GroundManager>().gameW;
        gameH = 4 * GameObject.Find("Ground").GetComponent<GroundManager>().gameH;

        //give a random initial direction to the fish
        float randDir = Random.Range(0, Mathf.PI * 2);
        directionX = Mathf.Cos(randDir);
        directionZ = Mathf.Sin(randDir);

        newDirX = directionX;
        newDirZ = directionZ;

        anim = GetComponent<Animation>();

        speed = initialSpeed;
        rotationSpeed = initialRotationSpeed;
    }

    // Update is called once per frame
    void Update(){

        //update the fish object position and orientation in the game
        transform.position += new Vector3(directionX, 0, directionZ) * speed;
        transform.eulerAngles = new Vector3(0, getOrientation(), 0);

        //the fish can start rushing randomly (only if it met canRush criterias)
        if (Random.Range(0, 300) == 1 && canRush()){
            goFaster();
        }

        //if the fish is no longer rush but still faster than the initial, decrease slightly the speed
        if(!isFaster && speed > initialSpeed){
            speed = Mathf.Max(speed - 0.01f, initialSpeed);
        }

        //test is the fish is swimming toward a wall and if it's not already turning, if yes, we will change it direction by updating newDir
        if (newDirX == directionX && newDirZ == directionZ && runToTheWall())
        {
            Debug.Log("Turn");
            //verify that the fish is only near one wall
            if (isNearCorner() == "NO")
            {
                //if the fish go toward the right wall
                if (collide("right"))
                {
                    //if the fish is going toward the wall not to perpendicullary, it will just start to swim along the wall
                    if (directionX < 0.75)
                    {
                        newDirX = 0;
                        newDirZ = (directionZ < 0) ? -1 : 1;
                        along = "right";

                    }
                    else //else just give a new rndom direction to the fish
                    {
                        updateDir(getOrientation2(-directionX, directionZ), Mathf.PI / 2);
                    }

                }
                else if (collide("left"))
                {


                    if (directionX > -0.75)
                    {
                        newDirX = 0;
                        newDirZ = (directionZ < 0) ? -1 : 1;
                        along = "left";
                    }
                    else
                    {
                        updateDir(getOrientation2(-directionX, directionZ), Mathf.PI / 2);

                    }
                }
                else if (collide("up"))
                {
                    if (directionZ < 0.75)
                    {
                        
                        newDirX = (directionX < 0) ? -1 : 1;
                        newDirZ = 0;
                        along = "up";
                        
                    }
                    else
                    {
                        updateDir(getOrientation2(directionX, -directionZ), Mathf.PI / 2);
                    }


                }
                else if (collide("down")){


                    if (directionZ > -0.75)
                    {

                        newDirX = (directionX < 0) ? -1 : 1;
                        newDirZ = 0;
                        along = "down";
                        
                    }
                    else
                    {

                        updateDir(getOrientation2(directionX, -directionZ), Mathf.PI /2);
                    }
                }
            
            // if the fish is near a corner, give it a new random direction opposite to the corner
            }else{
                
                if (isNearCorner() == "LD") updateDir(Mathf.PI/4, Mathf.PI/2);
                if (isNearCorner() == "LU") updateDir(-Mathf.PI / 4, Mathf.PI / 2);
                if (isNearCorner() == "RD") updateDir(-3*Mathf.PI / 4 , Mathf.PI / 2);
                if (isNearCorner() == "RU") updateDir(3*Mathf.PI / 4, Mathf.PI / 2);
            }

        //if the fish should not start turning
        }else{

            //if the fish is not turning 
            if (directionX == newDirX && directionZ == newDirZ){

                //give back the initial animation in cas it has been modified
                anim.Play("mouvement");

                //if it met certain condition (canTurnEverywhere), the fish can randomly start to turn
                if (canTurnEverywhere() && (Random.Range(0, 100) == 1) && speed == initialSpeed) {
                    updateDir(getOrientation2(directionX, directionZ), Mathf.PI);
                    //the fish has 1 chance on 2 to start rushing when starting to turn
                    if (Random.Range(0, 1) == 0) goFaster();
                }

                if (Random.Range(0, 100) == 1 && ((directionX == 0 && Mathf.Abs(directionZ) == 1) || (directionZ == 0 && Mathf.Abs(directionX) == 1))){

                    float angle = 0;
                    if (along == "right") angle = (directionZ == 1) ? angle = -Mathf.PI * 3 / 4 : Mathf.PI * 3 / 4;
                    if (along == "left") angle = (directionZ == 1) ? angle = -Mathf.PI / 4 : Mathf.PI / 4;
                    if (along == "up") angle = (directionX == 1) ? angle = -Mathf.PI / 4 : -Mathf.PI * 3 / 4;
                    if (along == "down") angle = (directionX == 1) ? angle = Mathf.PI / 4 : Mathf.PI * 3 / 4;
                    updateDir(angle, Mathf.PI / 2);
                }
            }
            else
            {
                //update slightly the diection of the fish, to draw near the new direction the fish will have after the turn
                directionX = (directionX > newDirX) ? Mathf.Max(newDirX, directionX - rotationSpeed) : Mathf.Min(newDirX, directionX + rotationSpeed);
                directionZ = (directionZ > newDirZ) ? Mathf.Max(newDirZ, directionZ - rotationSpeed) : Mathf.Min(newDirZ, directionZ + rotationSpeed);
            }
        }

        
    }
   
    //give the current orientation of the fish in degree
    float getOrientation(){
        return Mathf.Atan2(directionX, directionZ) / 2 / Mathf.PI * 360;
    }

    //give the orientation of the given parameters in radian
    float getOrientation2(float x, float y){
        return Mathf.Atan2(y, x);
    }

    //give a new direction the fish should turning toward by changing newDir, the new direction is random, deriving from "dir" in a maximum angle given by "angle" (both in radian) 
    void updateDir(float dir, float angle){

        float randAngle = Random.Range(dir - angle/2, dir + angle/2);

        //run the turning animation in function of the new direction
        if (randAngle < dir)
        {
            anim.Play("left");
        }else {
            anim.Play("right");
        }

        //update the direction
        newDirX = Mathf.Cos(randAngle);
        newDirZ = Mathf.Sin(randAngle);
    }

    //return true if the fish is near a wall and swiming in its direction
    bool runToTheWall(){
        return (collide("right") && newDirX > 0) || (collide("left") && newDirX < 0) ||
                (collide("up") && newDirZ > 0) || (collide("down") && newDirZ < 0 );
    }

    //return true if the fish is near a wall defined by "dir"
    bool collide(string dir){
        if (dir == "left") return transform.position.x - fishLength * Mathf.Abs(directionX) < -gameW - offSetHorizontal;
        if (dir == "right") return transform.position.x + fishLength * Mathf.Abs(directionX) > gameW + offSetHorizontal;
        if (dir == "down") return transform.position.z - fishLength * Mathf.Abs(directionZ) < -gameH - offSetUpDown;
        if (dir == "up") return transform.position.z + fishLength * Mathf.Abs(directionZ) > gameH + offSetUpDown;

        return false;
    }

    //same as above, but use given position instead of the fish's one
    bool collideGivenPos(float posX, float posZ)
    {
        return posX - fishLength * Mathf.Abs(directionX) < -gameW - offSetHorizontal ||
        posX + fishLength * Mathf.Abs(directionX) > gameW + offSetHorizontal ||
        posZ - fishLength * Mathf.Abs(directionZ) < -gameH - offSetUpDown ||
        posZ + fishLength * Mathf.Abs(directionZ) > gameH + offSetUpDown;

    }

    //return a string deffining the corner the fish is near of, return "NO" if it's near no corner (L = left, R = Right, D = down, U = Up)
    string isNearCorner(){

        Vector3 pos = transform.position;
        float cornerSize = 20;

        if (pos.x < -gameW + cornerSize && pos.z < -gameH + cornerSize ) return "LD";
        if (pos.x < -gameW + cornerSize && pos.z > gameH - cornerSize) return "LU";
        if (pos.x > gameW - cornerSize && pos.z < -gameH + cornerSize) return "RU";
        if (pos.x > gameW - cornerSize && pos.z > gameH - cornerSize) return "RD";
        return "NO";
    }

    //we consider that the fish can turn in any direction if it's not too close to any wall
    bool canTurnEverywhere(){

        Vector3 pos = transform.position;
        float offSet = 20;

        return (pos.x < gameW - offSet) && (pos.x > -gameW + offSet) && (pos.z < gameH - offSet) && (pos.z > -gameH +offSet);
    }

    //the fish can rush if it wan't face a wall too soon in it's curent direction
    bool canRush(){
        float xDir = transform.position.x + directionX * 500;
        float zDir = transform.position.z + directionZ * 500;

        return !collideGivenPos(xDir, zDir);   
    }

    //the fish start to rush
    void goFaster(){

        //acceleration the animation speed
        isFaster = true;
        anim["mouvement"].speed = 3;
        anim["right"].speed = 3;
        anim["left"].speed = 3;

        //accelerate the speed
        speed = 1f;
        rotationSpeed = 0.1f; 

        //start the timer that will give back its initial speed
        StartCoroutine(backToNormalSpeed());
    }

    //the fish stop rushing
    void normalSpeed()
    {
        isFaster = false;

        //reduce the animation speed
        anim["mouvement"].speed = 1;
        anim["right"].speed = 1;
        anim["left"].speed = 1;

        rotationSpeed = initialRotationSpeed;
        //the global speed is not reduced here, as it will be done gradually in Update()
    }

    //routine that will run the function stoping the fish run after 0.5 sec
    IEnumerator backToNormalSpeed(){

        yield return new WaitForSeconds(0.5f);
        normalSpeed();
    }

}
