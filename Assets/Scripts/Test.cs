using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    [SerializeField] GameObject c;
 

 

    List<GameObject> Circles = new List<GameObject>();

    

    // Start is called before the first frame update
   public void onclick()
    {
        Vector2 v = new Vector2(0, -2);
        GameObject ob = Instantiate(c, v, Quaternion.identity);

        Circles.Add(ob);
 


        for(int i = 0; i<Circles.Count; i++)
        {
            Circles[i].transform.position = new Vector2(0,-2+i);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
