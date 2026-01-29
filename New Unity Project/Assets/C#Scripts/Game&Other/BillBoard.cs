using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    public Transform cam;

    // Start is called before the first frame update

    private void Start()
    {
        cam = GameObject.FindWithTag("MainCamera").transform;
    }


    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);

    }
}
