using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UserProfile : MonoBehaviour
{
    [SerializeField] RawImage ProfileImage;
    [SerializeField] Text Username;

    // Start is called before the first frame update
    void Start()
    {

        Username.text = PassData.Username;
        StartCoroutine(GetTexture());
        
    }

  

    IEnumerator GetTexture()
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(PassData.ImageURL);
        Debug.Log("image url " + www);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;

            ProfileImage.texture = myTexture;
        }

    }
}
