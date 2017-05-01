using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateObjects : MonoBehaviour
{

    private List<GameObject> soundObjects;
    private GameObject notificationObject;
    private ReadJSON objs;
    private WWW www;
    private string url;
    private float soundThreshold;
    private GameObject tempSphere;
    private MicrophoneManager microphoneManager;
    private AudioSource dictationAudio;
    private Vector3 speechBubblePos;
    private Vector3 bestPosition;

    void Start()
    {
        speechBubblePos = Camera.main.transform.position;
        //This needs to be scraped from server at some point
        url = "http://172.25.53.167:8000/sounds.json";
        soundObjects = new List<GameObject>();
        soundThreshold = 1000;
        bestPosition = new Vector3(0, 0, 0);
        //These three components are needed to record speech
        dictationAudio = gameObject.GetComponent<AudioSource>();
        microphoneManager = GetComponent<MicrophoneManager>();
        dictationAudio.clip = microphoneManager.StartRecording();
    }

    void Update()
    {
        //Connects to the server and creates sound objects
        connectAndCreateObjects();

        //This will check to see if there is a hologram out of view
        //If there is, it will cause the notification object to point towards the hologram
        if (isTextOutOfView() && notificationObject != null)
        {
            GameObject textBackground = GameObject.FindGameObjectWithTag("TextBackground");
            notificationObject.transform.LookAt(textBackground.transform);
        }

        //Go through JSON information and generate holograms
        generateSoundObjects();

        //This will reset the microphone and dictationAudio if 5 seconds have passed without the user speaking. 
        resetMicrophone();
    }

    //TODO: Make this method word. Currently causes errors.
    /// <summary>
    /// Retrieves the url address of the raspberry pi
    /// </summary>
    /// <returns>string representing the url</returns>
    IEnumerator getURL()
    {
        string shelvURL = "http://shelvar.com/ip.php";
        www = new WWW(shelvURL);

        yield return www;

        if (www.error == null)
        {
            url = www.text;
            Debug.Log(url);
        }
        else
        {
            Debug.Log("SHELVAR WWW Error: " + www.error);
        }

    }

    /// <summary>
    /// Connects to the web server
    /// </summary>
    public void connectAndCreateObjects()
    {
        WWWForm form = new WWWForm();
        form.AddField("command", "sounds.json");
        www = new WWW(url, form);
        StartCoroutine(WaitForRequest(www));
    }

    /// <summary>
    /// Create holograms representing sounds in real world based on information from microphones
    /// </summary>
    public void generateSoundObjects()
    {
        //If there are objects to parse, then we continue
        if (objs != null)
        {
            //lists of all sounds, loudness values, and first frame IDs
            List<Vector3> soundLocations = objs.getPositions();
            List<double> loudnessValues = objs.getLoudness();
            List<int> firstFrameIDs = objs.getFirstFrameIDs();

            //Loop through all sounds and loudness values
            for (int i = 0; i < soundLocations.Count; i++)
            {
                Vector3 pos = soundLocations[i];
                double loudness = loudnessValues[i];
                int firstFrameID = firstFrameIDs[i];
                //A sphere will not be created unless there is enough noise coming from that position
                if (loudness > soundThreshold)
                {
                    if (!checkForSound(firstFrameID))
                    {
                        createSphere(pos, firstFrameID);
                    }
                }
            }
            //Remove all elements in the JSON list
            objs = null;
        }
    }

    /// <summary>
    /// Checks all spheres to see if any are out of view. If so, generate an arrow
    /// </summary>
    private void checkObjects()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        bool deleteObject = true;

        //Removes all deleted holograms from list
        soundObjects.RemoveAll(item => item == null);
        foreach (GameObject o in soundObjects)
        {
            //Used to get bounds of the sphere
            Collider objCollider = o.GetComponent<Collider>();


            //If sphere is not visible, do not delete the arrow
            if (!GeometryUtility.TestPlanesAABB(planes, objCollider.bounds))
            {
                deleteObject = false;
                //If notificationObject does not exist, generate arrow that points to sphere out of view
                if (notificationObject == null)
                {
                    tempSphere = o;
                    generateNotification(o);
                }
            }
        }
        //If there are no holograms out of view, delete arrow
        if (deleteObject) Destroy(notificationObject);

    }

    /// <summary>
    /// Determines if the text is out of view of the user
    /// If it is out of the view, a notification object will be created
    /// </summary>
    /// <returns>True if out of view. False Otherwise</returns>
    private bool isTextOutOfView()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        //Used to get bounds of the sphere
        GameObject obj = GameObject.FindGameObjectWithTag("TextBackground");
        if (obj == null) return false;
        Collider objCollider = obj.GetComponent<Collider>();
        //If text is not visible, do not delete the arrow
        if (!GeometryUtility.TestPlanesAABB(planes, objCollider.bounds))
        {
            //If notificationObject does not exist, generate arrow that points to sphere out of view
            if (notificationObject == null)
            {
                generateNotification(obj);
            }
            return true;
        }

        Destroy(notificationObject);
        return false;

    }

    /// <summary>
    /// Creates a sphere at position pos relative to user
    /// </summary>
    /// <param name="pos">position of the sound in the real world</param>
    /// <returns>GameObject hologram representing real world sound</returns>
    private void createSphere(Vector3 pos, int firstFrameID)
    {
        //forward, up, and right vectors
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.AddComponent<SoundObject>();
        Vector3 soundPos = Camera.main.transform.position + pos;
        sphere.GetComponent<SoundObject>().setPos(soundPos);
        sphere.GetComponent<SoundObject>().setOriginalPosition(pos);
        sphere.GetComponent<SoundObject>().setFirstFrameID(firstFrameID);
        soundObjects.Add(sphere);
    }

    /// <summary>
    /// Generates the notification object if a sound is out of view of the user
    /// </summary>
    /// <param name="o">Sound hologram</param>
    private void generateNotification(GameObject o)
    {
        notificationObject = Instantiate(Resources.Load("arrow"), new Vector3(0, 0, 0), new Quaternion()) as GameObject;
        notificationObject.tag = "Arrow";
        notificationObject.AddComponent<NotificationObject>();
    }

    /// <summary>
    /// Returns true if sound is found in list of existing sounds
    /// </summary>
    /// <param name="firstFrameID">First frame sound was heard</param>
    /// <returns>
    /// True if sound is found in list of existing sounds
    /// False otherwise
    /// </returns>
    private bool checkForSound(int firstFrameID)
    {
        soundObjects.RemoveAll(item => item == null);
        foreach (GameObject o in soundObjects)
        {
            //If object is found to already exist
            if (firstFrameID == o.GetComponent<SoundObject>().getFirstFrameID())
            {
                return true;
            }
        }


        return false;
    }

    /// <summary>
    /// Resets the microphone if conditions require it to be reset
    /// </summary>
    private void resetMicrophone()
    {
        if(microphoneManager != null)
        {
            if (microphoneManager.speechText.GetComponent<TextMesh>().text.Equals("Speech has ended."))
            {
                microphoneManager = GetComponent<MicrophoneManager>();
                dictationAudio.clip = microphoneManager.StartRecording();
            }
        } else
        {
            dictationAudio = gameObject.GetComponent<AudioSource>();
            microphoneManager = GetComponent<MicrophoneManager>();
            dictationAudio.clip = microphoneManager.StartRecording();
        }
        
    }

    ///<summary>
    ///Receives the data from the server and puts it into objs
    ///</summary>
    IEnumerator WaitForRequest(WWW www)
    {
        yield return www;
        // check for errors
        if (www.error == null)
        {
            objs = new ReadJSON(www.text);
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }

    /// <summary>
    /// Gets the location of the first object in the soundLocations array
    /// </summary>
    /// <returns></returns>
    public Vector3 getFirstPosition()
    {
        Vector3 returnPosition = Camera.main.transform.position;
        if (objs != null)
        {
            //lists of all sounds, loudness values, and first frame IDs
            List<Vector3> soundLocations = objs.getPositions();
            if(soundLocations.ToArray().Length > 0)
                returnPosition = soundLocations[0];
           
        }
        return returnPosition;
    }

    public SoundObject getFirstObject()
    {
        if(soundObjects.Count>0)
        {
            return soundObjects[0].GetComponent<SoundObject>();
        }
        return null;
    }

    public Vector3 getBestPosition()
    {
        return bestPosition;
    }
    public void setBestPosition(Vector3 pos)
    {
        // if(bestPosition == (new Vector3(0,0,0)))
        // {
            bestPosition = pos;
        // }
    }

    public void setBestPosition(Vector3 pos, bool onDestroy)
    {
        if (!(bestPosition == (new Vector3(0, 0, 0)) && onDestroy))
        {
            bestPosition = pos;
        }
    }
}