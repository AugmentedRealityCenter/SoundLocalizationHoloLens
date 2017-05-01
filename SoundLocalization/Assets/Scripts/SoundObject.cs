using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;
using System.Collections.Generic;

public class SoundObject : MonoBehaviour
{
    //live 5 seconds after last time you heard it
    private float timer; //time since last reset
    private int timeToLive; //how long the object is allowed to live
    private bool snappedToMask; //True if sound is placed on mask. False otherwise
    private float timeAlive; //How long the sound has been alive
    private bool placed; //true if sound is placed, false otherwise
    private Vector3 originalPosition;
    private int firstFrameID;

    // Use this for initialization
    void Start()
    {
        timer = 0;
        GetComponent<Renderer>().transform.localScale = new Vector3(0.25F, 0.25F, 0.25F);
        timeToLive = 15;
        snappedToMask = false;
        timeAlive = 0;
        placed = false;
        originalPosition = new Vector3(0, 0, 0);
        GetComponent<Renderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Update timers with how much time has passed since last update
        timer += Time.deltaTime;
        timeAlive += Time.deltaTime;

        //Check to see if object should be destroyed
        destroyObject();

        //Place the object in the real world if it has not yet been placed
        if (!placed)
        {
            if(!snapToMask())
            {
                placeWithoutCast();
            }
        }
    }

    /// <summary>
    /// Resets the timer of the object to 0
    /// </summary>
    public void resetTimer()
    {
        timer = 0;
    }

    /// <summary>
    /// Places the sound object at the best guess of where the sound came from.
    /// Does not attach the object to the layer mask
    /// </summary>
    private void placeWithoutCast()
    {
        var pos = transform.position;
        transform.position = Camera.main.transform.position + new Vector3(pos.x * 3, Camera.main.transform.position.y, pos.z * 3);
        GetComponent<Renderer>().material.color = Color.red;
        placed = true;
    }

    //TODO: Make this more accurate
    /// <summary>
    /// Casts a ray from sound to layer mask in attempts to place the sphere
    /// </summary>
    /// <returns>True if the object has been snapped to layer mask</returns>
    private bool snapToMask()
    {
        //Consider making Y value 0 before casting the ray so that everything is at eye level for the user
        Vector3 raycastDirection = (transform.position - Camera.main.transform.position).normalized;
        Vector3 position;
        int maximumPlacementDistance = 5;
        // Cast a ray from the center of the box collider face to the surface.
        RaycastHit centerHit;
        if (!Physics.Raycast(transform.position,
                        raycastDirection,
                        out centerHit,
                        maximumPlacementDistance,
                        SpatialMappingManager.Instance.LayerMask))
        {
            // If the ray failed to hit the surface, we are done.
            return false;
        }

        // We have found a surface.  Set position and surfaceNormal.
        position = centerHit.point;
        transform.position = position;
        // Uncomment next line for debugging purposes to see if a sound snaps to a mask
        // GetComponent<Renderer>().material.color = Color.green;
        CreateObjects createObjects = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CreateObjects>();
        createObjects.setBestPosition(position);
        placed = true;
        return true;
    }

    /// <summary>
    /// Sets the position of the sphere
    /// </summary>
    /// <param name="pos">Directional vector of the sound</param>
    public void setPos(Vector3 pos)
    {
        transform.position = new Vector3(pos.x, Camera.main.transform.position.y, pos.z);
    }

    /// <summary>
    /// Sets the original position of the sphere so that it can be checked against incoming positions
    /// </summary>
    /// <param name="pos"></param>
    public void setOriginalPosition(Vector3 pos)
    {
        originalPosition = new Vector3(pos.x, pos.y, pos.z);
    }

    /// <summary>
    /// Gets the original position of the object that came from the JSON file
    /// </summary>
    /// <returns></returns>
    public Vector3 getOriginlPosition()
    {
        return originalPosition;
    }

    /// <summary>
    /// Sets the firstFrameID for the sound object
    /// </summary>
    /// <param name="firstFrame">ID of the first frame the sound object was heard</param>
    public void setFirstFrameID(int firstFrame)
    {
        firstFrameID = firstFrame;
    }

    /// <summary>
    /// Gets the firstFrameID of the object
    /// </summary>
    /// <returns>The first frame at which the object was heard</returns>
    public int getFirstFrameID()
    {
        return firstFrameID;
    }

    /// <summary>
    /// Destroys this object after timer has passed timeToLive
    /// </summary>
    private void destroyObject()
    {
        if (timer > timeToLive)
        {
            // Uncomment the following code if you wish to make the speech text more usable
            // CreateObjects createObjects = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CreateObjects>();
            // createObjects.setBestPosition(new Vector3(0, 0, 0), true);

            Destroy(gameObject);
        }
    }
}
