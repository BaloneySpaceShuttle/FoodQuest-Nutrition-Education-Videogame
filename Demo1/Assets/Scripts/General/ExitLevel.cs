using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitLevel : MonoBehaviour {
    
    public string enterScene;
    public float nextX;
    public float nextY;
    public string exitDirection;
    
    public override string ToString() {
      return enterScene;
   }
   
   public Vector3 NextTile() {
       return new Vector3(nextX, nextY, 0);
   }

}
