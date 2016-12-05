﻿using UnityEngine;
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

    void Start()
    {
        //This needs to be scraped from server at some point
        url = "http://172.25.52.54:8000/sounds.json";
        //StartCoroutine(getURL());
        soundObjects = new List<GameObject>();
        soundThreshold = 1000;

        //These three components are needed to record speech
        dictationAudio = gameObject.GetComponent<AudioSource>();
        microphoneManager = GetComponent <MicrophoneManager>();
        dictationAudio.clip = microphoneManager.StartRecording();
    }

    void Update()
    {
        connectAndCreateObjects();
        //This will check to see if there is a hologram out of view
        //If there is, it will cause the notification object to point towards the hologram
        checkObjects();
        if (tempSphere != null && notificationObject != null)
        {
            notificationObject.transform.LookAt(tempSphere.transform);
        }
        //Go through JSON information and generate holograms
        generateSoundObjects();

        //This will reset the microphone and dictationAudio if 5 seconds have passed without the user speaking. 
        if (microphoneManager.speechText.GetComponent<TextMesh>().text.Equals("Speech has ended."))
        {
            microphoneManager = GetComponent<MicrophoneManager>();
            dictationAudio.clip = microphoneManager.StartRecording();
        }

    }

    /// <summary>
    /// Retrieves the url address of the raspberry pi
    /// </summary>
    /// <returns>string representing the url</returns>
    IEnumerator getURL()
    {
        string shelvURL = "http://shelvar.com/ip.php";
        WWWForm form = new WWWForm();
        //form.AddField("command", "sounds.json");
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
                    if (!checkForSound(pos, firstFrameID))
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

       // Debug.Log("Sphere at POS: " + soundPos + "| Original Pos: " + pos);
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
    /// <param name="pos">Position of sound</param>
    /// <returns>
    /// True if sound is found in list of existing sounds
    /// False otherwise
    /// </returns>
    private bool checkForSound(Vector3 pos, int firstFrameID)
    {

        foreach (GameObject o in soundObjects)
        {
            //If object is found to already exist
            if(firstFrameID == o.GetComponent<SoundObject>().getFirstFrameID())
            {
                return true;
            }

            //Vector3 originalPosition = o.GetComponent<SoundObject>().getOriginlPosition();
            //var x = originalPosition.x;
            //var z = originalPosition.z;


            ////Do not need to check 'y' because we set it to 0
            //if ((x + 0.3f) >= pos.x && (x - 0.3f) <= pos.x)
            //{
            //    if ((z >= 0 && pos.z >= 0) || (z  <= 0 && pos.z <= 0))
            //    {
            //        //If found, reset timer on object.
            //        o.GetComponent<SoundObject>().resetTimer();
            //        return true;
            //    }
            //}
        }


        return false;
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
}