using UnityEngine;
using System.Collections;

public class SpeechText : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        faceUser();
    }

    /// <summary>
    /// Makes sure that the text is always facing the user
    /// </summary>
    private void faceUser()
    {
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward * 5;

        transform.position = headPosition + gazeDirection;
        transform.TransformDirection(gazeDirection);
    }

    /// <summary>
    /// Update the 3D text
    /// </summary>
    /// <param name="text"></param>
    public void updateText(string text)
    {
        GetComponent<TextMesh>().text = text;
    }
}
