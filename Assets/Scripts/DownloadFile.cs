using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class DownloadFile : MonoBehaviour
{
    private static DownloadFile instance;
    public static DownloadFile _Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("DownLoad").AddComponent<DownloadFile>();
            }
            return instance;
        }
    }
    public Dictionary<string, UnityWebRequest> listRequest = new Dictionary<string, UnityWebRequest>();
    public _DownloadHandler StartDownload(string url,string savePath)
    {
        if (listRequest.ContainsKey(url))
        {
            Debug.Log("下载列表已经存在路径 =>" + url);
            return null;
        }
        _DownloadHandler loadHandler = new _DownloadHandler(savePath);
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.chunkedTransfer = true;
        request.disposeDownloadHandlerOnDispose = true;
        request.SetRequestHeader("Range", "bytes=" + loadHandler.DownedLength + "-");
        request.downloadHandler = loadHandler;
        request.Send();
        listRequest.Add(url, request);
        return loadHandler;
    }
    public void StopDownload(string url)
    {
        UnityWebRequest request = null;
        if(!listRequest.TryGetValue(url,out request))
        {
            Debug.Log("不存在下载请求=>" + url);
            return;
        }
        listRequest.Remove(url);
        (request.downloadHandler as _DownloadHandler).OnDispose();
        request.Abort();
        request.Dispose();
    }
    private void Start()
    {
        StartDownload("http://10.11.12.207/Test.txt", @"D:\新建文件夹\test.txt");
    }
    private void Update()
    {
        List<string> removeList = new List<string>();
        foreach (var url in listRequest.Keys)
        {
            UnityWebRequest request = listRequest[url];
            if (request.isDone)
            {
                Debug.Log(request.responseCode);
                request.Dispose();
                removeList.Add(url);
            }
        }
        for(int i = 0; i < removeList.Count; i++)
        {
            listRequest.Remove(removeList[i]);

        }
        removeList.Clear();
    }
    private void OnApplicationQuit()
    {
        foreach (var url in listRequest.Keys)
        {
            (listRequest[url].downloadHandler as _DownloadHandler).OnDispose();
            listRequest[url].Dispose();
        }
        listRequest.Clear();
    }
}
