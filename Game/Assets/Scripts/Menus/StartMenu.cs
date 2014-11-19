using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartMenu : MonoBehaviour {
	[SerializeField] private GameObject continueButton;
	[SerializeField] private GameObject newGameButton;
	[SerializeField] private GameObject exitButton;
	[SerializeField] private GameObject initialText;

	void Start () {
	
	}

	void Update () {
		if (Input.GetButtonDown(buttonName:"Submit")) {
			continueButton.SetActive(true);
			continueButton.GetComponent<Button>();
			newGameButton.SetActive(true);
			exitButton.SetActive(true);
			initialText.SetActive(false);
		}
	}

	public void GoContinue() {
		if (PlayerPrefs.GetInt("level") != 0) {
			Application.LoadLevel(PlayerPrefs.GetInt("level"));
		}
		else {
			Application.LoadLevel(1);
		}
	}

	public void GoNewGame() {
		Application.LoadLevel(1);
	}

	public void GoExit() {
		Application.Quit();
	}
}
