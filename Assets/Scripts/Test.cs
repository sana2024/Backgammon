using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("other " + PassData.OtherUserId);
        Debug.Log("Mine " + PassData.Match.Self.UserId);
        Debug.Log("host" + PassData.hostPresence.UserId);
        Debug.Log("second" + PassData.SecondPresence.UserId);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
