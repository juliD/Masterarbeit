using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosRotLock : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject target;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = target.transform.position;
        
        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, target.transform.rotation.eulerAngles.y, 0));
    }
}
