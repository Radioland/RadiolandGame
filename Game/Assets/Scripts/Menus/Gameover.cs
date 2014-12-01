using UnityEngine;
using System.Collections;

public class Gameover : MonoBehaviour {

	[SerializeField] private GameObject newGameButton;
	[SerializeField] private GameObject exitButton;
	
	void Start () {
		
	}
	
	void Update () {

	}
	
	public void GoNewGame() {
		Application.LoadLevel(1);
	}
	
	public void GoExit() {
		Application.Quit();
	}
}
