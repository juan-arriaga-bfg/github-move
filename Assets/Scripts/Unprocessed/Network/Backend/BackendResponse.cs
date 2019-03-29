using UnityEngine;
using System.Collections;
using BestHTTP;
using IW.SimpleJSON;

public class BackendResponse
{
    public bool IsOk;
    public bool IsError = true;

    public bool IsConnectionError = false;

    public int Code = -1;

    public HTTPRequest Request;
    public HTTPResponse Response;

    public string ResponseAsText = "";
    public JSONNode ResponseAsJson;

    public string ErrorAsText = "";
    public JSONNode ErrorAsJson;

    public string ResultAsText = "";
    public JSONNode ResultAsJson;
}
