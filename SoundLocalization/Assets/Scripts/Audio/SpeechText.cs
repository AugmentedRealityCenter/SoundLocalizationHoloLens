using UnityEngine;
using System.Collections;

public class SpeechText : MonoBehaviour {
    public CreateObjects createObjects;

    private const string BOTTOM_TEXT = "put text on bottom";
    private const string MIDDLE_TEXT = "put text on middle";
    private const string TOP_TEXT = "put text on top";
    private const string SPEECH_BUBBLE_TEXT = "put text on speech bubble";

    private bool bottom;
    private bool middle;
    private bool top;
    private bool speechBubble;
    private string currentText;

    private SoundObject soundObject;
    //Use this for initialization

   void Start () {
        currentText = "Microphone is Recording";
        bottom = false;
        middle = false;
        top = false;
        speechBubble = true;
        createObjects = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CreateObjects>();
	}
	
	// Update is called once per frame
	void Update () {
        updateLocation(GetComponent<TextMesh>().text);
        scale();
        faceUser();
    }

    
    

    /// <summary>
    /// Makes sure that the text is always facing the user
    /// </summary>
    private void faceUser()
    {
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward * 5;

        if (bottom)
        {

            Vector3 temp = new Vector3(headPosition.x, headPosition.y - 0.5f, headPosition.z);
            moveTowardsPosition(temp + gazeDirection);
            transform.TransformDirection(gazeDirection);
        }
        else if (middle)
        {
            moveTowardsPosition(headPosition + gazeDirection);
            transform.TransformDirection(gazeDirection);
        }
        else if (top)
        {
            Vector3 temp = new Vector3(headPosition.x, headPosition.y + 0.75f, headPosition.z);
            moveTowardsPosition(temp + gazeDirection);
            transform.TransformDirection(gazeDirection);
        }
        else if(speechBubble) //If speech bubble is selected and a sound object exists
        {
            if (!(createObjects.getBestPosition() == (new Vector3(0, 0, 0))))
            {
                transform.position = createObjects.getBestPosition();
            }
            else
            {
                moveTowardsPosition(headPosition + gazeDirection);
                transform.TransformDirection(gazeDirection);
            }

        }
    }

    /// <summary>
    /// Update the 3D text
    /// </summary>
    /// <param name="text"></param>
    public void updateText(string text)
    {
        GetComponent<TextMesh>().text = text;
        updateLocation(text);
      
    }

    /// <summary>
    /// Updates the boolean variables representing where the text should be located
    /// </summary>
    /// <param name="text"></param>
    private void updateLocation(string text)
    {
        if (text.ToLower().Contains(BOTTOM_TEXT))
        {
            bottom = true;
            middle = false;
            top = false;
            speechBubble = false;
        }
        else if (text.ToLower().Contains(MIDDLE_TEXT))
        {
            bottom = false;
            middle = true;
            top = false;
            speechBubble = false;
        }
        else if (text.ToLower().Contains(TOP_TEXT))
        {
            bottom = false;
            middle = false;
            top = true;
            speechBubble = false;
        }
        else if (text.ToLower().Contains(SPEECH_BUBBLE_TEXT))
        {
            bottom = false;
            middle = false;
            top = false;
            speechBubble = true;
        }
    }

    private void scale()
    {
         GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
         float distance = Vector3.Distance(mainCamera.transform.position, transform.position);
         transform.localScale = new Vector3((0.01f * distance), (0.01f * distance), (0.01f * distance));
    }

    private void moveTowardsPosition(Vector3 target)
    {
        float distance = Vector3.Distance(transform.position, target);
        if (distance > 0.5)
        {
            float speed = distance * distance + 1;
            transform.position = Vector3.MoveTowards(transform.position, target, speed* Time.deltaTime);
        }
    }

}