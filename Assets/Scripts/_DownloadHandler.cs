using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Networking;

public class _DownloadHandler : DownloadHandlerScript
{
    private FileStream fs;
    //File total length
    private int _contentLength = 0;
    private int ContentLength
    {
        get { return _contentLength; }
    }
    //done data length
    private int _downedLength = 0;
    public int DownedLength
    {
        get { return _downedLength; }
    }
    //filename to save
    private string _fileName;
    public string FileName
    {
        get { return _fileName; }
    }
    public string FileNameTemp
    {
        get
        {
            return _fileName + ".temp";
        }
    }
    //store path
    private string savePath = null;
    //file directory
    public string DirectoryPath
    {
        get { return savePath.Substring(0, savePath.LastIndexOf('/')); }

    }
    #region
    private event Action<int> _eventTotalLength = null;//the total length of file;
    private event Action<float> _eventProgress = null;//progress event
    private event Action<string> _eventComplete = null;//finished event

    #endregion
    #region 
    public void RegisteReceiveTotalLengthBack(Action<int> back)
    {
        if (back != null)
            _eventTotalLength += back;
    }
    public void RegisteProgressBack(Action<float> back)
    {
        if (back != null)
            _eventProgress += back;
    }
    public void RegisteCompleteBack(Action<string> back)
    {
        if (back != null)
            _eventComplete += back;
    }
    #endregion
    public _DownloadHandler(string filePath):base(new byte[1024 * 1024])
    {
        savePath = filePath.Replace('\\', '/');
        _fileName = savePath.Substring(savePath.LastIndexOf('/') + 1);
        this.fs = new FileStream(savePath + ".temp", FileMode.Append, FileAccess.Write);
        _downedLength = (int)fs.Length;
        //fs.Position=_downedLength;
    }
    protected override bool ReceiveData(byte[] data, int dataLength)
    {
        if (data == null || data.Length == 0)
        {
            Debug.Log("没有获取到数据缓存！");
            return false;
        }
        fs.Write(data, 0, dataLength);
        _downedLength += dataLength;
        if (_eventProgress != null)
            _eventProgress.Invoke((float)_downedLength / _contentLength);
        return true; 
    }
    protected override void CompleteContent()
    {
        string CompleteFilePath = DirectoryPath + "/" + FileName;
        string TempFilePath = fs.Name;
        OnDispose();
        if (File.Exists(TempFilePath))
        {
            if (File.Exists(CompleteFilePath))
            {
                File.Delete(CompleteFilePath);
            }
            File.Move(TempFilePath, CompleteFilePath);
        }
        else
        {
            Debug.Log("生成文件失败=>下载的文件不存在！");
        }
        if (_eventComplete != null)
            _eventComplete.Invoke(CompleteFilePath);
    }
    public void OnDispose()
    {
       
        fs.Dispose();
    }
    protected override void ReceiveContentLength(int contentLength)
    {
        _contentLength = contentLength + _downedLength;
        if (_eventTotalLength != null)
            _eventTotalLength.Invoke(_contentLength);
    }
}
