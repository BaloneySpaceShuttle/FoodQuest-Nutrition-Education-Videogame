using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {
    
	private float speed = 5;
    private float horzMove;
    private float vertMove;
    public static Player instance = null;
    
    private Text infoText;
    private Text fibreText;
    private Text proteinText;
    private Text carbsText;
    private Text fatsText;
    private Text scoreText;
    private Text hitsText;
    private Text damageText;
    private Text nutritionText;
    private Image screenFade;
    private GameObject gameOverGO;
    private GameObject touchControlsGO;
    private GameObject playerStatusGO;
    private Button fishButton;
    
    private int food;
    private int fibre;
    private int protein;
    private int carbs;
    private int fats;
    
    private int intputCounter = 0;

	private Rigidbody2D rb;
    // private MainCamera cm;
    
    private Coroutine isFishing = null;
    private bool FlyRod_Running = false;
    private bool FishButtonPressed = false;
    
    private bool controllerInputs = true;               // Used to turn off player controls
    private bool isAttackInCooldown = false;            // Used to delay attack to prevent player spanning
    private bool isKnockedback = false;                 // Used to check if player is recieving a rigidbody force after a hit
    private bool playerEnergyDepleted = false;         // Used to check if player nutrition level is zero
    
    private bool boss1Defeat = false;            // Used to check if boss1 is defeated
    private bool boss2Defeat = false;            // Used to check if boss2 is defeated
    
	private Animator animator;
    
    private GameObject equipmentGO;                     //eqipment gameobject
    private GameObject swordGO;                         //sword gameobject
    private GameObject toolsGO;                         //tools gameobject
    private GameObject rodGO;                           //fishingrod gameobject
    
    /*Score Calculating Variables....Still In Beta Testing*/
    private struct Score {
        public int DamageDealtScore;
        public int HitsTakenScore;
        public int NutrientScore;
       
       public int CalcOverallScore() {
           return (DamageDealtScore * NutrientScore) - (HitsTakenScore * 5);
       }
       
    }
    
    private enum AttkDirection {
		LEFT, RIGHT, UP, DOWN
	}
    
    private AttkDirection atkDir;
    private Score playerScore;
    
	void Awake() {
        if (instance == null) {
            instance = this;
        }
        
        else if (instance != this) {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(gameObject);
        
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
        
        equipmentGO = GameObject.Find("Equipment");
		swordGO = GameObject.Find("Sword");
		toolsGO = GameObject.Find("Tools");
		rodGO = GameObject.Find("FishingRod");
        
        // Turns off the Sword & rod
		swordGO.GetComponent<Renderer>().enabled = false;
		rodGO.GetComponent<Renderer>().enabled = false;
        
        if(SceneManager.GetActiveScene().name == "startscene") {
            this.SetPosition(14, 9);
        }
        else {
            this.SetPosition(0, 0);
        }
        
        
        
	}
    
    void Start () {
        
            // foodText = GameObject.Find ("HUD(Clone)/FoodText").GetComponent<UnityEngine.UI.Text>();
            
            // --DEVELOPING--
            infoText = GameObject.Find ("HUD(Clone)/InfoText").GetComponent<UnityEngine.UI.Text>();
            fibreText = GameObject.Find ("HUD(Clone)/FibreText").GetComponent<UnityEngine.UI.Text>();
            proteinText = GameObject.Find ("HUD(Clone)/ProteinText").GetComponent<UnityEngine.UI.Text>();
            carbsText = GameObject.Find ("HUD(Clone)/CarbsText").GetComponent<UnityEngine.UI.Text>();
            fatsText = GameObject.Find ("HUD(Clone)/FatsText").GetComponent<UnityEngine.UI.Text>();
            
            scoreText = GameObject.Find ("HUD(Clone)/GameOver/ScoreText").GetComponent<UnityEngine.UI.Text>();
            hitsText = GameObject.Find ("HUD(Clone)/GameOver/HitsText").GetComponent<UnityEngine.UI.Text>();
            damageText = GameObject.Find ("HUD(Clone)/GameOver/DamageText").GetComponent<UnityEngine.UI.Text>();
            nutritionText = GameObject.Find ("HUD(Clone)/GameOver/NutritionText").GetComponent<UnityEngine.UI.Text>();
            
            
            fishButton = GameObject.Find("HUD(Clone)/TouchControls/FishButton").GetComponent<UnityEngine.UI.Button>();
            screenFade = GameObject.Find("HUD(Clone)/ScreenFade").GetComponent<UnityEngine.UI.Image>();
            gameOverGO = GameObject.Find("HUD(Clone)/GameOver");
            touchControlsGO = GameObject.Find("HUD(Clone)/TouchControls");
            playerStatusGO = GameObject.Find("HUD(Clone)/PlayerStatus");
            
            fishButton.gameObject.SetActive(false);         //switch off fish button
            gameOverGO.gameObject.SetActive(false);         //switch off gameOver UI
        
            //Get the current food point total stored in GameManager.instance between levels.
            food = GameManager.instance.playerFoodPoints;
            protein = GameManager.instance.playerProteinPoints;
            carbs = GameManager.instance.playerCarbsPoints;
            fats = GameManager.instance.playerFatsPoints;
            fibre = 0;
            playerScore.DamageDealtScore = 0;
            playerScore.HitsTakenScore = 0;
            playerScore.NutrientScore = 0;
            
            InvokeRepeating("RefreshUI", 0.0f, 10f);            //start RefreshUI as repeating function
            InvokeRepeating("CheckPlayerStats", 0.0f, 0.5f);      //start CheckPlayerStats as repeating function
            InvokeRepeating("CheckIfWalking", 0.0f, 4f);        //start CheckIfWalking as repeating function
		}
    
    void OnTriggerEnter2D(Collider2D col) {

		if (col.tag == "Item") {
            
            float dist = DistanceBetweenTwoPoints(gameObject.transform.position.x, 
                col.gameObject.transform.position.x,
                gameObject.transform.position.y,
                col.gameObject.transform.position.y);
                
            infoText.text = col.gameObject.name; // DistanceBetweenTwoPoints(x1 x2 y1 y2)
            
            
            AddItem(col);
            
        }
        
        if(col.tag == "Exit") {
            StartCoroutine(EnterLevel(col));
        }
    }
    
    void OnTriggerStay2D(Collider2D col) {
        
        if(col.tag == "FishingTrigger") {
            fishButton.gameObject.SetActive(true);
            
            if(Input.GetKeyDown (KeyCode.Equals)) {
                PlayerFish(col);
            }
            
            if(FishButtonPressed) {
                PlayerFish(col);
            }
        }
        // Player HIT
        if(col.tag == "Enemy" && swordGO.GetComponent<Renderer>().enabled == false) {
        }
    }
    
    void OnTriggerExit2D(Collider2D col) {
        
        if(col.tag == "FishingTrigger") {
            fishButton.gameObject.SetActive(false);
        }
        
        
    }
    
    void SetPosition(float xPos, float yPos) {
        this.transform.position = new Vector3(xPos, yPos, 0);
        // GameObject.Find("Main Camera").transform.position = new Vector3(xPos, yPos, -10f);
    }
    
    void SetPosition(Vector3 newPos) {
        this.transform.position = newPos;
        // GameObject.Find("Main Camera").transform.position = new Vector3(newPos.x, newPos.y, -10f);
    }
    
    void Update() {
        // food -= 1;
        
        
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
            //do stuff when idle
        }
        
    }

	void FixedUpdate() {

		//  keyboard input
		// float horzMove = Input.GetAxisRaw ("Horizontal");
		// float vertMove = Input.GetAxisRaw ("Vertical");
        // horzMove = Input.GetAxisRaw ("Horizontal");
		// vertMove = Input.GetAxisRaw ("Vertical");


        
        if (rb.IsSleeping()) {
            rb.WakeUp();
        }
// #if UNITY_STANDALONE || UNITY_WEBPLAYER
		if (Input.GetKey ("w") || Input.GetKey ("s")) {
            horzMove = Input.GetAxisRaw ("Horizontal");
            vertMove = Input.GetAxisRaw ("Vertical");
            
            if(FlyRod_Running) {
                CancelCoroutinte(isFishing, StopFlyRod);
            }
            
			if (Input.GetKey ("s")) {
				animator.Play ("WalkDown");
				atkDir = AttkDirection.DOWN; // Set attack direction to down and call for animation
			}

			if (Input.GetKey ("w")) {
				animator.Play ("WalkUp");
                atkDir = AttkDirection.UP;
			}
		} else {
            horzMove = Input.GetAxisRaw ("Horizontal");
            vertMove = Input.GetAxisRaw ("Vertical");

			if (Input.GetKey ("d")) {
                
                if(FlyRod_Running) {
                    CancelCoroutinte(isFishing, StopFlyRod);
                }
                
				animator.Play ("WalkRight");
                atkDir = AttkDirection.RIGHT;
			}

			if (Input.GetKey ("a")) {
                
                if(FlyRod_Running) {
                    CancelCoroutinte(isFishing, StopFlyRod);
                }
                
				animator.Play ("WalkLeft");
                atkDir = AttkDirection.LEFT;
			}
		}

		if (Input.GetKeyUp ("w") || Input.GetKeyUp ("a") || Input.GetKeyUp ("s") || Input.GetKeyUp ("d")) {

			//animator.Play ("Idle");

			float centerX = (float)Math.Round(Convert.ToDouble(transform.position.x));
			float centerY = (float)Math.Round(Convert.ToDouble(transform.position.y));

			transform.position = new Vector2(centerX, centerY);

            // atkDir = AttkDirection.DOWN;
            
            
            
            if (Input.GetKeyUp ("w")) {
                animator.Play ("IdleUpward");
                atkDir = AttkDirection.UP;
            }
            
            if(Input.GetKeyUp ("a")) {
                animator.Play ("IdleLeft");
                atkDir = AttkDirection.LEFT;
            }
            
            if(Input.GetKeyUp ("s")) {
                animator.Play ("Idle");
                atkDir = AttkDirection.DOWN;
            }
            
            if(Input.GetKeyUp ("d")) {
                animator.Play ("IdleRight");
                atkDir = AttkDirection.RIGHT;
            }
		}
        
        if (Input.GetKeyDown(KeyCode.Return) && !isAttackInCooldown) {
            PlayerAttack();
            // food -= 15;
            // foodText.text = "Food: " + food;
        }
        
        //remove after testing
        if(Input.GetKeyDown (KeyCode.Equals)) {
            PlayerFish();
        }
// #endif     
        if(controllerInputs == false) {
            rb.velocity = new Vector2(0, 0);
        } else {
            rb.velocity = new Vector2(horzMove * speed, vertMove * speed);
        }
        
        if(carbs == 0 && fats == 0) {
            GameIsOver();
        }
        
        playerScore.NutrientScore = (carbs + protein) - fats;
        
	}
    
    IEnumerator EnterLevel(Collider2D exitTile) {
        
        ExitLevel el = exitTile.gameObject.GetComponent<ExitLevel>();
        string level = el.enterScene;
        
        screenFade.GetComponent<ScreenFade>().FadeOut();
        DisableControllerInputs(); // disable player movement from inputs
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(level);
        EnableControllerInputs(); // enable player movement from inputs
        
        if(el.exitDirection == "North") {
            SetPosition((float)el.nextX, (float)el.nextY + 0.8f);
        }
        else if(el.exitDirection == "South") {
            SetPosition((float)el.nextX, (float)el.nextY - 0.8f);
        }
        else if(el.exitDirection == "East") {
            SetPosition((float)el.nextX + 1f, (float)el.nextY);
        }
        else if(el.exitDirection == "West") {
            SetPosition((float)el.nextX - 1f, (float)el.nextY);
        }
        else {
            SetPosition((float)el.nextX, (float)el.nextY);
        }
        
    }
    
    void AddItem(Collider2D item) {
        
        if(item.gameObject.GetComponent<CollectableItem>().protein > 0) {
            FillProtein(item.gameObject.GetComponent<CollectableItem>().protein);
        }
        
        if(item.gameObject.GetComponent<CollectableItem>().carbs > 0) {
            FillCarbs(item.gameObject.GetComponent<CollectableItem>().carbs);
        }
        
        if(item.gameObject.GetComponent<CollectableItem>().fats > 0) {
            FillFats(item.gameObject.GetComponent<CollectableItem>().fats);
        }
        
        if(item.gameObject.GetComponent<CollectableItem>().fibre > 0) {
            FillFibre(item.gameObject.GetComponent<CollectableItem>().fibre);
        }
        item.gameObject.SetActive(false);
    }
    
    void CancelCoroutinte(Coroutine co, Action stoppers) {
        StopCoroutine(co);
        stoppers();
    }
    
    void PlayerAttack() {
        
        switch (atkDir) {
            
            case AttkDirection.DOWN: {
                // animator.Play ("Idle");
                StartCoroutine (AnimateSword(0f, 0f, 0.1f));
                break;
            }

            case AttkDirection.LEFT: {
                animator.Play ("PointLeft");
                StartCoroutine (AnimateSword(-90f, -0.2f, -0.55f));
                break;
            }

            case AttkDirection.RIGHT: {
                animator.Play ("PointRight");
                StartCoroutine (AnimateSword(90f, 0.3f, 0.1f));
                break;
            }

            case AttkDirection.UP: {
                animator.Play ("PointUpward");
                StartCoroutine (AnimateSword(180f, 0f, 0f));
                break;
            }

		}
    }
    
    void PlayerFish() {     
        //used when no tile present, might delete this
        animator.Play ("ReachingRight");
        isFishing = StartCoroutine(FlyRod(0f,  -8f ,0.85f, 0f));
    }
    
    void PlayerFish(Collider2D tile) {
        
        string fishDir = tile.gameObject.GetComponent<FishingTrigger>().fishingDirection;
        
        switch (fishDir) {
            
            case "South": {
                isFishing = StartCoroutine(FlyRod(0f, 0f, 0f, 0f));
                break;
            }

            case "West": {
                animator.Play ("ReachingLeft");
                isFishing = StartCoroutine(FlyRod(-180f, 8f, -0.85f, 0f));
                break;
            }

            case "East": {
                animator.Play ("ReachingRight");
                isFishing = StartCoroutine(FlyRod(0f, -8f, 0.85f, 0f));
                break;
            }

            case "North": {
                // animator.Play ("PointUpward");
                isFishing = StartCoroutine(FlyRod(0f, 0f, 0f, 0f));
                break;
            }

		}
    }
    
    void StopFlyRod() {
        rodGO.GetComponent<Renderer>().enabled = false;
        animator.Play ("Idle");
        FlyRod_Running = false;
    }
    
    IEnumerator AnimateSword(float rotation, float hzMove, float vertMove) {
        
		swordGO.GetComponent<Renderer>().enabled = true;                     // Make sword appear
		equipmentGO.transform.rotation = Quaternion.Euler(0, 0, rotation);   // Rotate sword into position
		
        equipmentGO.transform.position = new Vector3(
            transform.position.x + hzMove, 
			transform.position.y + vertMove, 
            transform.position.z);                                           // Move sword vertically
        
        
        SetAttackModeToCooldown();                                           // Set Attack Cooldown to prevent spamming attacks
		DrainCarbs(5);                                                      // Use carbs to attack
        yield return new WaitForSeconds (.2f);                               // Wait a second before hiding sword again
		
        swordGO.GetComponent<Renderer>().enabled = false;                   // Make sword disappear
        animator.Play ("Idle");                                             // Play idle animation
    }
    
    IEnumerator FlyRod(float rotationY, float rotationZ, float hzMove, float vertMove) {
        
        FlyRod_Running = true;
        
        rodGO.GetComponent<Renderer>().enabled = true; // Make rod appear
        toolsGO.transform.rotation = Quaternion.Euler(0, rotationY, rotationZ); // Rotate rod into position
		
        toolsGO.transform.position = new Vector3(
            transform.position.x + hzMove, 
            transform.position.y + vertMove, 
            transform.position.z); // move rod position
        
        yield return new WaitForSeconds (2f);
        FillProtein(15);
        StopFlyRod();
    }
    
    IEnumerator GetHit(GameObject enemy) {     //apply "knockback()", subtract health
        
        yield return new WaitForSeconds (0f);    //small delay to let enemy attack animation to begin

        //...get enemy transform, reverse player velocity from that...
        Knockback(enemy);
    }
    
    IEnumerator AttackModeModeToCooldown() {
        isAttackInCooldown = true;
        yield return new WaitForSeconds (0.5f);
        
        isAttackInCooldown = false;
    }
    
    IEnumerator ResetUI() {
        yield return new WaitForSeconds(2f);
        
        infoText.text = "";
        fibreText.text = "FibreText: " + protein;
        proteinText.text = "Protein: " + protein;
        carbsText.text = "Carbs: " + carbs;
        fatsText.text = "Fats: " + fats;
    }
    
    void RefreshUI() {
        infoText.text = "";
        fibreText.text = "Fibre: " + fibre;
        proteinText.text = "Protein: " + protein;
        carbsText.text = "Carbs: " + carbs;
        fatsText.text = "Fats: " + fats;

    }
    
    void CheckPlayerStats() {
        
        if(protein < 0) {
            protein = 0;
        }
        
        if(carbs < 0) {
            carbs = 0;
        }
        
        if(fats < 0) {
            fats = 0;
        }
        
        
        
    }
    
    float DistanceBetweenTwoPoints(float x1, float x2, float y1, float y2) {
        return (float)Math.Sqrt((Math.Pow(x1-x2,2) + Math.Pow(y1-y2,2)));
    }
    
    void SetAttackModeToCooldown() {
        StartCoroutine(AttackModeModeToCooldown());
    }
    
    void Knockback(GameObject enemy) {          // applies reflective force onto playerGO via rigidbody
        
        int magnitude = 7;    
        float x1 = enemy.transform.position.x;
        float y1 = enemy.transform.position.y;
        float x2 = gameObject.transform.position.x;
        float y2 = gameObject.transform.position.y;
        Vector2 force = new Vector2((x2-x1), (y2-y1)); //vector maths equation
        force.Normalize();
        playerScore.HitsTakenScore += 1;            // add hit to score
        EnableKnockedback();
        rb.AddForce(force * magnitude);        //push player in opposite direction "knockback"
        DisableKnockedback();
    }
    
    void EnableKnockedback() {
        isKnockedback = true;
    }
    
    void DisableKnockedback() {
        isKnockedback = false;
    }
    
    void EnableControllerInputs() {
        controllerInputs = true;
    }
    
    void DisableControllerInputs() {
        controllerInputs = false;
    }
    
    void FillCarbs(int val) {
        carbs += val;
        carbsText.text = "Carbs: " + carbs + "(+" + val + ")";
    }
    
    void DrainCarbs(int val) {
        if(carbs > 0) {
            carbs -= val;
            carbsText.text = "Carbs: " + carbs + "(-" + (int)val + ")";
        }
        else {
            DrainFats(val);
        }
    }
    
    void FillFibre(int val) {
        fibre += val;
        fibreText.text = "Fibre: " + fibre + "(+" + val + ")";
    }
    
    void DrainFibre(int val) {
        fibre -= val;
        fibreText.text = "Fibre: " + fibre + "(-" + (int)val + ")";
    }
    
    void FillProtein(int val) {
        protein += val;
        proteinText.text = "Protein: " + protein + "(+" + val + ")";
    }
    
    void DrainProtein(int val) {
        protein -= val;
        proteinText.text = "Protein: " + protein + "(-" + val + ")";
    }
    
    void FillFats(int val) {
        fats += val;
        fatsText.text = "Fats: " + fats + "(+" + val + ")";
    }
    
    void DrainFats(int val) {
        fats -= val;
        fatsText.text = "Fats: " + fats + "(-" + val + ")";
    }

    void CheckIfWalking() {
        
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        
        if(currentState.IsName("WalkUp") || currentState.IsName("WalkDown") || currentState.IsName("WalkRight") || currentState.IsName("WalkLeft")) {
            // return true;
            // DrainCarbs(1);
            carbs -= 1;
            carbsText.text = "Carbs: " + carbs;
            // carbs -= 1;
        }
        else {
            // return false;
        }
        
    }
    
    void GameIsOver() {
        DisableControllerInputs();
        CancelInvoke("RefreshUI");                          //cancel repeating functions
        CancelInvoke("CheckPlayerStats");                   //cancel repeating functions
        CancelInvoke("CheckIfWalking");                     //cancel repeating functions
        
        infoText.gameObject.SetActive(false);               //switch off
        // playerStatusGO.gameObject.SetActive(false);         //switch off
        touchControlsGO.gameObject.SetActive(false);        //switch off
        proteinText.gameObject.SetActive(false);            //switch off
        carbsText.gameObject.SetActive(false);              //switch off
        fatsText.gameObject.SetActive(false);               //switch off
        fibreText.gameObject.SetActive(false);               //switch off
        
        
        gameOverGO.gameObject.SetActive(true);              //switch on
        scoreText.text = "Score: " + playerScore.CalcOverallScore();
        hitsText.text = "Hits Taken: " + playerScore.HitsTakenScore;
        damageText.text = "Damage Dealt: " + playerScore.DamageDealtScore;
        nutritionText.text = "Nutrition Level: " + playerScore.NutrientScore;
        
    }
    
    public void TakeDamage(GameObject enemy) {
        StartCoroutine(GetHit(enemy));
    }
    
    public bool isInKnockback() {
        return isKnockedback;
    }
    
    public void MoveLeft() {;
        horzMove = -1f;
        animator.Play ("WalkLeft");
        atkDir = AttkDirection.LEFT;
    }
    
    public void MoveRight() {
        horzMove = 1f;
        animator.Play ("WalkRight");
        atkDir = AttkDirection.RIGHT;
    }
    
    public void MoveUp() {
        vertMove = 1f;
        animator.Play ("WalkUp");
        atkDir = AttkDirection.UP;
    }
    
    public void MoveDown() {
        vertMove = -1f;
        animator.Play ("WalkDown");
        atkDir = AttkDirection.DOWN; // Set attack direction to down and call for animation
    }
    
    public void IdleUpward() {
        vertMove = 0f;
        animator.Play ("IdleUpward");
        atkDir = AttkDirection.UP;
    }
    
    public void IdleDownward() {
        vertMove = 0f;
        animator.Play ("Idle");
        atkDir = AttkDirection.DOWN;
    }
    
    public void IdleLeft() {
        horzMove = 0f;
        animator.Play ("IdleLeft");
        atkDir = AttkDirection.LEFT;
    }
    
    public void IdleRight() {
        horzMove = 0f;
        animator.Play ("IdleRight");
        atkDir = AttkDirection.RIGHT;
    }    
    
    public void LaunchAttack() {
        if(!isAttackInCooldown) {
            PlayerAttack();
        }
    }
    
    public void Fish() {
        // PlayerFish(col);
        FishButtonPressed = true;
    }
    
    public void FishOff() {
        // PlayerFish(col);
        FishButtonPressed = false;
    }
    
    public void Boss1Defeated() {
        boss1Defeat = true;
        playerScore.DamageDealtScore += 10;
        if(boss1Defeat && boss2Defeat) {
            GameIsOver();
        }
    }
    
    public void Boss2Defeated() {
        boss2Defeat = true;
        playerScore.DamageDealtScore += 10;
        if(boss1Defeat && boss2Defeat) {
            GameIsOver();
        }
    }
    
    public void EnemyDefeat() {
        playerScore.DamageDealtScore += 2;
    }
    
}
