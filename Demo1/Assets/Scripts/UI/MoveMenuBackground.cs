using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMenuBackground : MonoBehaviour {

	public float speed;
    public float tolerance;
    public float startYPos;
    public float startXPos;
    public float endYPos;
    public float endXPos;
    
    private float targetX;
    private float targetY;
    private Vector3 position;
   
    // Update is called once per frame
    void Update () {
        // Get current position
        position = transform.position;
        float curYPos = position.y;
        float curXPos = position.x;
 
        // Move between the start and end vectors
        if(isApproximate(curYPos, endYPos, tolerance) && isApproximate(curXPos, endXPos, tolerance))
        {
            targetX = startXPos;
            targetY = startYPos;
        }else if (isApproximate(curYPos, startYPos, tolerance) && isApproximate(curXPos, startXPos, tolerance))
        {
            targetX = endXPos;
            targetY = endYPos;
        }
 
        // Update position
        transform.position = Vector3.Lerp(position, new Vector3(targetX, targetY, position.z), speed * Time.deltaTime);
 
    }
 
    bool isApproximate(float a, float b, float tolerance) {
        return Mathf.Abs (a - b) < tolerance;
    }
}
