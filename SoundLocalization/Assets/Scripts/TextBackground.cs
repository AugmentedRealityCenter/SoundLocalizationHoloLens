using UnityEngine;
using System.Collections;

public class TextBackground : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Color backgroundColor = Color.blue;
        backgroundColor.a = 0.5f;
        GetComponent<Renderer>().material.color = backgroundColor;
	}
	
	// Update is called once per frame
	void Update () {
        updatePosition();
        scale();
    }

    /// <summary>
    /// Updates the background of the text to be behind the SpeechText and facing the same direction
    /// </summary>
    void updatePosition()
    {
        Vector3 textPos =
            GameObject.FindGameObjectWithTag("SpeechText").GetComponent<SpeechText>().transform.position;
        Vector3 newPos = textPos * 1.05f;

        Quaternion textRot =
            GameObject.FindGameObjectWithTag("SpeechText").GetComponent<SpeechText>().transform.rotation;

        transform.position = newPos;
        transform.rotation = textRot;
    }

    /// <summary>
    /// Scales the object so that it is the proper size based on the distance from the user.
    /// </summary>
    void scale()
    {
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        float distance = Vector3.Distance(mainCamera.transform.position, transform.position);
        transform.localScale = new Vector3((distance), (0.1f * distance), (0.005f * distance));
    }
}
