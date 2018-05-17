using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour {
    
    public GameObject gameManager;
    public GameObject player;
    public GameObject hud;
    public GameObject mainCamera;
    public GameObject eventsystem;

	void Awake () {
        
        if (GameManager.instance == null) {
            Instantiate(gameManager);
        }
        
        if (HUD.instance == null) {
            Instantiate(hud);
        }
        
        if (MainCamera.instance == null) {
            Instantiate(mainCamera);
        }
        
        if (EventSystem.instance == null) {
            Instantiate(eventsystem);
        }
        
        if (Player.instance == null) {
            Instantiate(player);
        } 
        
        
    }
}
