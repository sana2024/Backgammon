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
 

   public List<Piece> Circles = new List<Piece>();

    [SerializeField] Piece piece;


    public void Start()
    {
        bool s = true;

        Debug.Log(s.ToString());

        SpawnList();
 
    }

    public void Update()
    {
 
    }

    IEnumerator checkInternetConnection()
    {

        UnityWebRequest request = new UnityWebRequest("http://google.com");
        yield return request.SendWebRequest();

        if(request.error != null)
        {
           
        }
    }



    // Start is called before the first frame update
    public void onclick()
    {
        Vector2 v = new Vector2(0, -3);
        Piece ob = Instantiate(piece, v, Quaternion.identity);
        j++;
        c.name = "checker "+j;
        Circles.Add(ob);
 

         i += 0.7f;

        Debug.Log(i);

       Circles.LastOrDefault().transform.position = new Vector2(Circles.LastOrDefault().transform.position.x, Circles.LastOrDefault().transform.position.y + i);
 
    }

    public void OnRemove()
    {
        Circles.Remove(Circles.LastOrDefault());

    }

    public void dissable()
    {
        controller = !controller;
        img.SetActive(controller);
    }



 
    public void SpawnList()
    {
  
        string[] a = { "a", "b", "c" };

        foreach(var index in a)
        {
         GameObject f=  Instantiate(Friend, Vector3.zero, Quaternion.identity) as GameObject;
            f.transform.parent = FriendHolderUI.transform;
        }

    }


    public void onTestClicked()
    {
        amount *= 2;
        Debug.Log(amount);
     
 
    }


    }
