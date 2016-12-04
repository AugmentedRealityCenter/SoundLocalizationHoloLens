using UnityEngine;
using System.Collections;

public class SpeechText : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward*5;

        transform.position = headPosition + gazeDirection;
        //transform.position += new Vector3(-1, 0, 0);
        transform.TransformDirection(gazeDirection);
    }

    public void updateText(string text)
    {
        GetComponent<TextMesh>().text = text;
    }
}
