using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Test : MonoBehaviour
{

    [SerializeField] GameObject c;
    int j;
    float i;

 

   public List<Piece> Circles = new List<Piece>();

    [SerializeField] Piece piece; 

    // Start is called before the first frame update
   public void onclick()
    {
        Vector2 v = new Vector2(0, 1);
        Piece ob = Instantiate(piece, v, Quaternion.identity);
        j++;
        c.name = "checker "+j;
        Circles.Add(ob);
 

         i += 0.7f;

        Debug.Log(i);

       Circles.LastOrDefault().transform.position = new Vector2(Circles.LastOrDefault().transform.position.x, Circles.LastOrDefault().transform.position.y + i);
 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
