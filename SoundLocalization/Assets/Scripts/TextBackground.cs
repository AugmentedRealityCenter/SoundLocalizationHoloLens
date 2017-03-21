using UnityEngine;
using System.Collections;

public class TextBackground : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Color backgroundColor = Color.blue;
        GetComponent<Renderer>().material.color = backgroundColor;
	}
	
	// Update is called once per frame
	void Update () {
        updatePosition();

    }

    /// <summary>
    /// Updates the background of the text to be behind the SpeechText and facing the same direction
    /// </summary>
    void updatePosition()
    {
        Vector3 textPos =
            GameObject.FindGameObjectWithTag("SpeechText").GetComponent<SpeechText>().transform.position;
        Vector3 newPos = textPos * 1.2f;

        Quaternion textRot =
            GameObject.FindGameObjectWithTag("SpeechText").GetComponent<SpeechText>().transform.rotation;

        transform.position = newPos;
        transform.rotation = textRot;
    }
}
