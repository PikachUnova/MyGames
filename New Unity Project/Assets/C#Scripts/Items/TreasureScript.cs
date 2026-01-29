using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureScript : MonoBehaviour
{
    private bool isOpened = false;
    public GameObject item;

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Open") && !isOpened)
        {
            StartCoroutine(RevealItem(0.8f));
            isOpened = true;
        }  
    }


    IEnumerator RevealItem(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        if (item != null)
        {
            Vector3 position = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
            Instantiate(item, position, transform.rotation);
        }
    }

    public bool IsOpened()
    {
        return isOpened;
    }
}
