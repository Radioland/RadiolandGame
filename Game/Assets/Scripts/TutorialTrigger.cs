using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialTrigger : MonoBehaviour {

	[SerializeField] private string messageText;
	[SerializeField] private GameObject textObject;

	private bool visited;

	void Start () {
		visited = false;
	}

	void OnTriggerEnter(Collider c) {
		if (c.gameObject.tag == "Player") {
			if (visited == false) {
				textObject.GetComponent<Text>().text = messageText;
				visited = true;
			}
		}
	}
	
	void Update () {
	
	}
}
