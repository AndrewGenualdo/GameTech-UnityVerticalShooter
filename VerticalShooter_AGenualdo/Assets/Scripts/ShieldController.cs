using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.W)) { SetDirection(ROTATION_UP); }
        else if (Input.GetKeyDown(KeyCode.D)) { SetDirection(ROTATION_RIGHT); }
        else if(Input.GetKeyDown(KeyCode.S)) { SetDirection(ROTATION_DOWN); }
        else if (Input.GetKeyDown(KeyCode.A)) { SetDirection(ROTATION_LEFT); }
    }

    const int ROTATION_UP = 0;
    const int ROTATION_RIGHT = 1;
    const int ROTATION_DOWN = 2;
    const int ROTATION_LEFT = 3;

    const float distance = 1.5f;
    void SetDirection(int rotation)
    {
        switch(rotation)
        {
            case ROTATION_UP:
                {
                    gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                    gameObject.transform.position = new Vector3(0, distance, 0);
                    break;
                }
            case ROTATION_RIGHT:
                {
                    gameObject.transform.rotation = Quaternion.Euler(0, 0, 90);
                    gameObject.transform.position = new Vector3(distance, 0, 0);
                    break;
                }
            case ROTATION_DOWN:
                {
                    gameObject.transform.rotation = Quaternion.Euler(0, 0, 180);
                    gameObject.transform.position = new Vector3(0, -distance, 0);
                    break;
                }
            case ROTATION_LEFT:
                {
                    gameObject.transform.rotation = Quaternion.Euler(0, 0, 270);
                    gameObject.transform.position = new Vector3(-distance, 0, 0);
                    break;
                }
        }
    }
}
