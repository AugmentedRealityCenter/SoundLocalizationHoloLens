using UnityEngine;
using System.Collections;

public class ColorChanging : MonoBehaviour {
    public Color lerpedColor = Color.blue;
    public int distance;
    public int x;
    public int y;
    public int z;
    // Use this for initialization
    void Start () {
        GetComponent<Renderer>().material.color = lerpedColor;
        distance = 15;
        x = 0;
        y = 0;
        z = 10;
    }
	
	// Update is called once per frame
	void Update () {
        //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        //sphere.transform.position = Camera.main.transform.forward + new Vector3(x++,y++,z++);
        ////var dir = new Vector3();
        ////dir = Camera.main.transform.forward;
        ////dir.y = transform.position.y;
        ////dir.x = transform.position.x;
        ////dir.z = transform.position.z;
        ////dir.Normalize();

        ////transform.Translate(dir * 1 * Time.deltaTime);
    
        //transform.position += Camera.main.transform.forward * 2.0f * Time.deltaTime;
        ////transform.position = Camera.main.transform.position + new Vector3(0, 0 ,distance);// * 10 * 1;
        ////transform.rotation = Camera.main.transform.rotation;
        ////Camera.main.transform.position = Camera.main.transform.position + new Vector3(1, 0, 1) * Time.deltaTime;
        //lerpedColor = Color.Lerp(Color.white, Color.green, Mathf.PingPong(Time.time, 1));
        //GetComponent<Renderer>().material.color = lerpedColor;
        //transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);




    }
}
