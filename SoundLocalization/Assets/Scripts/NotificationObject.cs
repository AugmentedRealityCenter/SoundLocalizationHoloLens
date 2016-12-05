using UnityEngine;
using System.Collections;

//This object is used to notify the user that there is a sound outside of their vision
public class NotificationObject : MonoBehaviour
{


    void Start()
    {
        GetComponent<Renderer>().
        transform.localScale = new Vector3(0.05F, 0.05F, 0.05F);
    }

    // Update is called once per frame
    void Update()
    {
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        transform.position = headPosition + gazeDirection;
        transform.TransformDirection(gazeDirection);
    }

}
