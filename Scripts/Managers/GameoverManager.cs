using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameoverManager : MonoBehaviour {

	///***********************************************************************
	/// GameOver Manager Class. 
	///***********************************************************************

	public GameObject scoreText;			//reference to score gameobject to modify its text
	public AudioClip menuTap;
	private bool canTap;
	private float buttonAnimationSpeed = 9;

	void Awake (){
		canTap = true;
	}

	void Update (){	

		//Set the new score on the screen
		scoreText.GetComponent<TextMesh>().text = PlayerManager_pilot.playerScore.ToString();
		
		if(canTap)
			StartCoroutine(tapManager());
	}
	///***********************************************************************
	/// Manage user taps on gameover buttons
	///***********************************************************************
	private RaycastHit hitInfo;
	private Ray ray;
	IEnumerator tapManager (){

		//Mouse of touch?
		if(	Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Ended)  
			ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
		else if(Input.GetMouseButtonUp(0))
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		else
			yield break;
			
		if (Physics.Raycast(ray, out hitInfo)) {
			GameObject objectHit = hitInfo.transform.gameObject;
			print(objectHit.name);
			switch(objectHit.name) {
				case "retryButton":
					playSfx(menuTap);			//play audioclip
					saveScore();				//save players best and last score
					StartCoroutine(animateButton(objectHit));	//animate the button
					yield return new WaitForSeconds(0.4f);	//Wait
					SceneManager.LoadScene(SceneManager.GetActiveScene().name); //reload
					break;
				case "menuButton":
					playSfx(menuTap);
					saveScore();
					StartCoroutine(animateButton(objectHit));
					yield return new WaitForSeconds(1.0f);
					SceneManager.LoadScene("Menu-c#");
					break;
			}	
		}
	}
	///***********************************************************************
	/// Save player score
	///***********************************************************************
	void saveScore (){
		
		PlayerPrefs.SetInt("lastScore_SPilot", PlayerManager_pilot.playerScore);
		//check if this new score is higher than saved bestScore.
		//if so, save this new score into playerPrefs. otherwise keep the last bestScore intact.
		int lastBestScore;
		lastBestScore = PlayerPrefs.GetInt("bestScore_SPilot");
		if(PlayerManager_pilot.playerScore > lastBestScore)
			PlayerPrefs.SetInt("bestScore_SPilot", PlayerManager_pilot.playerScore);
	}

	///***********************************************************************
	/// Animate buttons on touch
	///***********************************************************************
	IEnumerator animateButton ( GameObject _btn  ){
		canTap = false;
		Vector3 startingScale = _btn.transform.localScale;
		Vector3 destinationScale = startingScale * 1.1f;
		//yield return new WaitForSeconds(0.1f);
		float t = 0.0f; 
		while (t <= 1.0f) {
			t += Time.deltaTime * buttonAnimationSpeed;
			_btn.transform.localScale = new Vector3(Mathf.SmoothStep(startingScale.x, destinationScale.x, t),
			                                        _btn.transform.localScale.y,
			                                        Mathf.SmoothStep(startingScale.z, destinationScale.z, t));
			yield return 0;
		}
		
		float r = 0.0f; 
		if(_btn.transform.localScale.x >= destinationScale.x) {
			while (r <= 1.0f) {
				r += Time.deltaTime * buttonAnimationSpeed;
				_btn.transform.localScale = new Vector3(Mathf.SmoothStep(destinationScale.x, startingScale.x, r),
				                                        _btn.transform.localScale.y,
				                                        Mathf.SmoothStep(destinationScale.z, startingScale.z, r));
				yield return 0;
			}
		}
		
		if(r >= 1)
			canTap = true;
	}

	///***********************************************************************
	/// IPlay audioclip
	///***********************************************************************
	void playSfx ( AudioClip _sfx  ){
		GetComponent<AudioSource>().clip = _sfx;
		if(!GetComponent<AudioSource>().isPlaying)
			GetComponent<AudioSource>().Play();
	}

}