using System;
using System.Net.Sockets;
using BestHTTP;
using IW.SimpleJSON;

namespace Backend
{
    public class ResponseParser
    {
        public static BackendResponse Parse(HTTPRequest req, HTTPResponse res)
        {
            BackendResponse br = new BackendResponse();
            br.Request = req;
            br.Response = res;

            try
            {
                if (req.Exception != null)
                {
                    if (req.Exception is TimeoutException || req.Exception is SocketException)
                    {
                        br.IsConnectionError = true;
                        throw req.Exception;
                    }
                    throw req.Exception;
                }

                if (req.State != HTTPRequestStates.Finished)
                {
                    br.IsConnectionError = true;
                    throw new Exception("ResponseParser: Http request failed: " + req.State);
                }

                br.ResponseAsText = res.DataAsText;
                br.ResponseAsJson = JSON.Parse(br.ResponseAsText);

                if (br.ResponseAsText == null && br.ResponseAsJson == null)
                {
                    br.IsOk = false;
                    br.IsError = true;
                }
                else
                {
                    if (br.ResponseAsJson["error"] != null)
                    {
                        br.ErrorAsJson = br.ResponseAsJson["error"]["message"];
                        br.ErrorAsText = br.ResponseAsJson["error"]["message"].Value;

                        if (string.IsNullOrEmpty(br.ErrorAsText))
                        {
                            br.ErrorAsText = br.ResponseAsJson["error"]["message"].ToString();
                        }

                        if (br.ResponseAsJson["error"]["code"] != null)
                        {
                            br.Code = br.ResponseAsJson["error"]["code"].AsInt;
                        }

                        br.IsOk = false;
                        br.IsError = true;
                    }
                    else if (br.ResponseAsJson["result"] != null)
                    {
                        br.ResultAsJson = br.ResponseAsJson["result"]["message"];
                        br.ResultAsText = br.ResponseAsJson["result"]["message"].Value;

                        if (string.IsNullOrEmpty(br.ResultAsText))
                        {
                            br.ResultAsText = br.ResponseAsJson["result"]["message"].ToString();
                        }

                        if (br.ResponseAsJson["result"]["code"] != null)
                        {
                            br.Code = br.ResponseAsJson["result"]["code"].AsInt;
                        }

                        br.IsOk = true;
                        br.IsError = false;
                    }
                    else
                    {
                        throw new Exception("ResponseParser: Can't parse response: not found 'result' or 'error' nodes");
                    }
                }

                // // timestamp parsing
                // List<string> dateHeaders;
                // if (res.Headers.TryGetValue("date", out dateHeaders))
                // {
                //     if (dateHeaders.Count != 1)
                //     {
                //         throw new Exception("ResponseParser: multiply 'date' headers found!");
                //     }
                //
                //     DateTime local = DateTime.Parse(dateHeaders[0]);
                //     DateTime universal = local.ToUniversalTime();
                //     TimeController.Instance.ServerTimeRecieved(universal);
                // }
                // else
                // {
                //     throw new Exception("ResponseParser: 'date' header not found");
                // }
            }
            catch (Exception e)
            {
                IW.Logger.LogWarning($"ResponseParser: error: {e.Message}\n response:{(res != null ? res.DataAsText : "is null")}\n request:{(req != null ? req.Uri.ToString() : "is null")}");
                br.IsOk = false;
                br.IsError = true;
            }

            return br;
        }
    }
}

