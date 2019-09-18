using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UnityWebRequestTest : MonoBehaviour
{
	void Start ()
    {
        StartCoroutine(GetText("http://10.11.12.207/Test.txt"));
	}
	IEnumerator GetText(string url)
    {

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
        }
        byte[] results = request.downloadHandler.data;
             
    }
	

}
