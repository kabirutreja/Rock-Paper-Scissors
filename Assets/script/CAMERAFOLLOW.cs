
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAMERAFOLLOW : MonoBehaviour
{
    public Transform cameraposition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void LateUpdate()
    {
       transform.position = cameraposition.position;
    }
}
