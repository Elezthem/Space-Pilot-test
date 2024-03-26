using UnityEngine;
using System.Collections;

public class PlayerManager_pilot : MonoBehaviour {
		
	///***********************************************************************
	/// Main Player Ship Class.
	/// This class manages playerShip's health, score and collision events.
	///***********************************************************************

	//Static Variables
	public static int playerHealth;
	public GameObject[] healthIcons;

	public static int playerScore = 0;

	//AudioClips
	public AudioClip eatSfx;
	public AudioClip hitSfx;

	//Flag, when we have collided with something and have a few second to get back to game
	public static bool  reborn = false;

	//GameObjects References
	public GameObject sparksFX;
	public GameObject scoreTextDynamic;

	void Awake (){
		playerHealth = 3;
		playerScore = 0;
		reborn = false;
										
		//Disable screen dimming on handheld devices
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		
		//Fill all healthIcons
		foreach(GameObject item in healthIcons) {
			item.GetComponent<Renderer>().enabled = true;
		}
	}

		
	void Update (){
		
		if(!GameController_pilot.gameOver)
			calculateScore();
	}

	///***********************************************************************
	/// calculate players score
	///***********************************************************************
	void calculateScore (){
		if(!PauseManager.isPaused) {
			playerScore += ( GameController_pilot.current_level * (int)Mathf.Log(GameController_pilot.current_level + 1, 2) );
			scoreTextDynamic.GetComponent<TextMesh>().text = playerScore.ToString();
		}
	}

	///***********************************************************************
	/// Process and show player's health on screen
	///***********************************************************************
	void monitorPlayerHealth (){

		//Limiters
		if(playerHealth > 3) 
			playerHealth = 3;
		if(playerHealth < 0) 
			playerHealth = 0;
			
		//show health icons
		for(int i = 0; i < playerHealth; i++) {
			healthIcons[i].GetComponent<Renderer>().enabled = true;
		}
		
		//hide lost health icons
		for(int j = playerHealth; j < 3; j++) {
			healthIcons[j].GetComponent<Renderer>().enabled = false;
		}
		
		//check for gameover state
		if(playerHealth <= 0) {
			GameController_pilot.gameOver = true;
			return;
		}
	}

	///***********************************************************************
	/// Blink the ship after a collision occured
	///***********************************************************************
	IEnumerator blinkAfterhit (){
		//activate blink state
		reborn = true;
		GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r,
											GetComponent<Renderer>().material.color.g,
											GetComponent<Renderer>().material.color.b,
											0.25f);									
		for(int i = 0; i < 10; i++) {
			yield return new WaitForSeconds(0.1f);
			GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r,
											GetComponent<Renderer>().material.color.g,
											GetComponent<Renderer>().material.color.b,
											0.25f);
			yield return new WaitForSeconds(0.1f);
			GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r,
											GetComponent<Renderer>().material.color.g,
											GetComponent<Renderer>().material.color.b,
											0.85f);
		}
		reborn = false;
		yield break;
	}

	///***********************************************************************
	/// Collision managements
	///***********************************************************************
	void OnTriggerEnter ( Collider other  ){
		//Normal state
		if(!reborn) {
			switch(other.gameObject.tag) {		
				case "helperPlusLive":
					playSfx(eatSfx);
					playerHealth++;
					monitorPlayerHealth();
					Destroy(other.gameObject);
					break;

				default:
					playSfx(hitSfx);
					makeSparks();
					playerHealth--;
					monitorPlayerHealth();
					
					//just for Android & iOS
					#if UNITY_IPHONE || UNITY_ANDROID
					Handheld.Vibrate();
					#endif
					
					StartCoroutine(blinkAfterhit());
					break;
			}
		} else { //If we collide with something while blinking...
			//always eat good things ;)
			switch(other.gameObject.tag) {						
				case "helperPlusLive":
					playSfx(eatSfx);
					playerHealth++;
					monitorPlayerHealth();
					Destroy(other.gameObject);
					break;
			}
		}
	}

		
	///***********************************************************************
	/// Make some hit particle effects
	///***********************************************************************
	void makeSparks (){

        if(sparksFX)
		    Instantiate(sparksFX, transform.position + new Vector3(Random.value/3,0,Random.value/3), Quaternion.Euler(new Vector3(-90, 0, 0)));
	}

	void playSfx ( AudioClip _sfx  ){
		GetComponent<AudioSource>().clip = _sfx;
		if(!GetComponent<AudioSource>().isPlaying)
			GetComponent<AudioSource>().Play();
	}

}