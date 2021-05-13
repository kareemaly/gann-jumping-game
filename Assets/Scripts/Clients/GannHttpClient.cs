using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class GannHttpClient
{
    private static string baseUrl = "http://localhost:8080";

    public static IEnumerator FirstPopulation(Action<PopulationDto> onSuccess)
    {
        return PostRequest($"{baseUrl}/first-population", new EmptyDto(), onSuccess);
    }

    public static IEnumerator NextPopulation(Action<PopulationDto> onSuccess)
    {
        return PostRequest($"{baseUrl}/next-population", new EmptyDto(), onSuccess);
    }

    public static IEnumerator Act(string id, ActInputDto inputDto, Action<ActOutputDto> onSuccess)
    {
        return PostRequest($"{baseUrl}/act/{id}", inputDto, onSuccess);
    }

    public static IEnumerator UpdateFitness(string id, UpdateFitnessDto inputDto, Action<EmptyDto> onSuccess)
    {
        return PostRequest($"{baseUrl}/update-fitness/{id}", inputDto, onSuccess);
    }

    public static IEnumerator PostRequest<I, O>(string url, I inputDto, Action<O> onSuccess)
    {
        string body = JsonUtility.ToJson(inputDto);
        var www = new UnityWebRequest(url, "POST");
        www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body));
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            O dto = JsonUtility.FromJson<O>(www.downloadHandler.text);
            onSuccess(dto);
        }
    }
}
