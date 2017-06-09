using UnityEngine;
using System.Collections;

//This object is used to notify the user that there is a sound outside of their vision
public class NotificationObject : MonoBehaviour
{


    void Start()
    {
        transform.SetParent(GameObject.FindGameObjectWithTag("MainCamera").transform);
        transform.localScale = new Vector3(0.025F, 0.025F, 0.025F);

    }

    // Update is called once per frame
    void Update()
    {
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        //Ensure the object is always in the middle of the user's screen
        Vector3 v3Pos = new Vector3(0, -0.4f, 3);
        transform.localPosition = v3Pos;
        //transform.position = headPosition + gazeDirection + new Vector3(0.025f, 0.025f, 0); 
        //transform.position = headPosition + new Vector3(0, 1f, 0.25f);
        transform.TransformDirection(gazeDirection);
    }

}
