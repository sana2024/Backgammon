using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeBoard : MonoBehaviour
{

    [SerializeField] SpriteRenderer BoardBackground;
    [SerializeField] Sprite Oslo;
    [SerializeField] Sprite tokyo;
    [SerializeField] Sprite boston;
    [SerializeField] Sprite london;
    [SerializeField] Sprite paris;
    [SerializeField] Sprite newyork;
    // Start is called before the first frame update
    void Start()
    {
        switch (PassData.BoardType)
        {
            case "Oslo":

                BoardBackground.sprite = Oslo;
                break;

            case "Tokyo":
                BoardBackground.sprite = tokyo;

                break;

            case "Boston":
                BoardBackground.sprite = boston;

                break;

            case "London":
                BoardBackground.sprite = london;

                break;

            case "Paris":
                BoardBackground.sprite = paris;

                break;

            case "Newyork":
                BoardBackground.sprite = newyork;

                break;
        }
    }
 
}
