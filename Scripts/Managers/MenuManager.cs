using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

	///*************************************************************************///
	/// Main Menu Buttons Controller.
	///*************************************************************************///

	private int bestScore;
	private int lastScore;
	public GameObject bestScoreText;	 //we just need the textmesh component
	public GameObject lastScoreText;	 // ""       ""        ""       ""
	private int controlType = 0;		 // 0=Tilt , 1=Touch
	public GameObject controlTypeText;

	public AudioClip menuTap;

	private bool canTap;
	private float buttonAnimationSpeed = 9;
	public GameObject panel_loading;

	void Awake (){

		canTap = true; //player can tap on buttons
		
		bestScore = PlayerPrefs.GetInt("bestScore_SPilot");
		bestScoreText.GetComponent<TextMesh>().text = bestScore.ToString();
		
		lastScore = PlayerPrefs.GetInt("lastScore_SPilot");
		lastScoreText.GetComponent<TextMesh>().text = lastScore.ToString();
		
		//fetch previous controlType set by player, instead of resetting it everytime
		PlayerPrefs.SetInt("controlType", 1);
		controlType = PlayerPrefs.GetInt("controlType");
		/*if(controlType == 0) {
			controlTypeText.GetComponent<TextMesh>().text = "Control: Tilt";
		} else {
			controlTypeText.GetComponent<TextMesh>().text = "Control: Touch";
		}*/
		
	}

	void Start (){
		//prevent screenDim in handheld devices
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}

	void Update (){

		if(canTap)	
			StartCoroutine(tapManager());
	}
	///***********************************************************************
	/// Process user inputs
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
			switch(objectHit.name) {
				case "btnPlay":
					canTap = false;
					playSfx(menuTap);
					StartCoroutine(animateButton(objectHit));
					yield return new WaitForSeconds(1.0f);
					SceneManager.LoadScene("Game-c#");
					break;
					
				case "btnExit":
					canTap = false;
					playSfx(menuTap);
					StartCoroutine(animateButton(objectHit));
					yield return new WaitForSeconds(0.1f);
					panel_loading.SetActive(true);
                    SceneManager.LoadSceneAsync(0 , LoadSceneMode.Single);
					break;
				
				case "ControlTypeLabel":
					playSfx(menuTap);
					StartCoroutine(animateButton(objectHit));
					
					if(controlType == 0) {
						controlType = 1;
						PlayerPrefs.SetInt("controlType", controlType);
						controlTypeText.GetComponent<TextMesh>().text = "Control: Touch";
					} else {
						controlType = 0;
						PlayerPrefs.SetInt("controlType", controlType);
						controlTypeText.GetComponent<TextMesh>().text = "Control: Tilt";
					}
					
					yield return new WaitForSeconds(0.6f);
					break;
			}	
		}
	}

	///***********************************************************************
	/// Animate button by modifying it's scale
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
	/// play audio clip
	///***********************************************************************
	void playSfx ( AudioClip _sfx  ){
		GetComponent<AudioSource>().clip = _sfx;
		if(!GetComponent<AudioSource>().isPlaying)
			GetComponent<AudioSource>().Play();
	}

}