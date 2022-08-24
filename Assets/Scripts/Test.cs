using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class Test : MonoBehaviour
{

    [SerializeField] GameObject c;
    [SerializeField] GameObject Friend;
    [SerializeField] GameObject FriendHolderUI;
    [SerializeField] Button testbtn;
    int j;
    float i;

    float amount = 2;



    

 
    [SerializeField] GameObject img;

    public bool controller=false;
 

   public List<GameObject> Circles = new List<GameObject>();

    [SerializeField] GameObject piece;


    public void Start()
    {

    }

    public void Update()
    {
 
    }

 

    // Start is called before the first frame update
    public void onclick()
    {
        Vector2 v = new Vector2(0, -3);
        GameObject ob = Instantiate(piece, v, Quaternion.identity);
        j++;
        c.name = "checker "+j;
        Circles.Add(ob);

         i += 0.7f;

        Debug.Log(i);

       Circles.Last().transform.position = new Vector2(Circles.Last().transform.position.x, Circles.Last().transform.position.y + i);
 
    }

    public void OnRemove()
    {
        i -= 0.7f;
        DestroyObject(Circles.LastOrDefault().gameObject);
        Circles.Remove(Circles.Last());

        

    }
 
 

    }
