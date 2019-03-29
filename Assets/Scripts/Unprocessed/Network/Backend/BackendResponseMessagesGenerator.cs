using UnityEngine;
using System.Collections;

public class BackendResponseMessagesGenerator
{
    private BackendResponse m_response;

    public BackendResponseMessagesGenerator(BackendResponse response)
    {
        m_response = response;
    }

    public string GetText()
    {
        if (m_response == null)
        {
            return null;
        }

        if (m_response.IsConnectionError)
        {
            return GetNetworkError();
        }
        
        return null;
    }

    protected virtual string GetNetworkError()
    {
        return LocalizationService.Get("common.network.error.message");
    }
}
