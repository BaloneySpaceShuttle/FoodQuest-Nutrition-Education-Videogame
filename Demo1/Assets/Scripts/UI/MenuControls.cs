using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuControls : MonoBehaviour {
    
    public void LaunchGameNormalMode() {
        SceneManager.LoadScene("startscene");
    } 
    
     public void RestartGameNormalMode() {
        Destroy(GameObject.Find ("GameManager(Clone)"));
        Destroy(GameObject.Find ("HUD(Clone)"));
        // Destroy(GameObject.Find ("MainCamera(Clone)"));
        Destroy(GameObject.Find ("EventSystem(Clone)"));
        Destroy(GameObject.Find ("Player(Clone)"));
        
        SceneManager.LoadScene("startscene");
    } 
}
