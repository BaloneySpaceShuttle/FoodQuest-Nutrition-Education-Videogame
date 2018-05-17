using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainCamera : MonoBehaviour {
    
    // public Transform cameraTarget;
    public GameObject cameraTarget;
    // public GameObject screenFadeGO;
    public float cameraSpeed;
    public float minX;
    public float minY;
    public float maxX;
    public float maxY;
    public static MainCamera instance = null;
    
    private Camera cam;
    
    void Awake() {
        
        if (instance == null) {
            instance = this;
        }
        
        else if (instance != this) {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        cam = GetComponent<Camera>();
    }
    
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
        transform.position = new Vector3(
            GameObject.FindGameObjectWithTag("Player").transform.position.x, 
            GameObject.FindGameObjectWithTag("Player").transform.position.y, 
            -10f);
    }
    
    void FixedUpdate() {

		if (cameraTarget != null) {

			// Lerp smoothes movement from the starting position
			// to the targets position 
			var newPos = Vector2.Lerp (
                transform.position,
				GameObject.FindGameObjectWithTag("Player").transform.position,
				Time.deltaTime * cameraSpeed);

			// cameras new postion
			var vect3 = new Vector3 (newPos.x, newPos.y - 0.1f, -10f);      //-0.1y to offset due to UI controls

			// Clamp gets the cameras x position and clamps
			// it between the min and max value
			var clampX = Mathf.Clamp (vect3.x, minX, maxX);
			var clampY = Mathf.Clamp (vect3.y, minY, maxY);

			// Move the camera
			transform.position = new Vector3(clampX, clampY, -10f);
		}
	}
    
    void Update() {
        // R/255 G/255 B/255
        if(SceneManager.GetActiveScene().name == "house1interior" || 
            SceneManager.GetActiveScene().name == "house2interior"||
            SceneManager.GetActiveScene().name == "house3interior"||
            SceneManager.GetActiveScene().name == "house4interior") {
            GetComponent<Camera>().backgroundColor = new Color(0.59608f, 0.40784f, 0.25098f, 0);
        }
        else if(SceneManager.GetActiveScene().name == "lakescene") {
            GetComponent<Camera>().backgroundColor = new Color(0.22745f, 0.38f, 0.6549f, 0);
        }
        else {
            GetComponent<Camera>().backgroundColor = new Color(0.33725f, 0.63137f, 0.33333f, 0);
        }
    }
    
}
