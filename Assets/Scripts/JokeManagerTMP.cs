using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class JokeManagerTMP : MonoBehaviour
{
    [Header("UI (TextMeshPro only)")]
    public Button fetchButton;

    public TextMeshProUGUI setupTMP;
    public TextMeshProUGUI punchlineTMP;
    public TextMeshProUGUI statusTMP;

    [Header("API")]
    [Tooltip("Default: Official Joke API")]
    public string apiUrl = "https://official-joke-api.appspot.com/random_joke";

    [Header("Options")]
    public bool fetchOnStart = false;
    public int requestTimeoutSeconds = 10;

    void Start()
    {
        if (fetchButton != null)
            fetchButton.onClick.AddListener(() => StartCoroutine(FetchJokeCoroutine()));

        if (fetchOnStart)
            StartCoroutine(FetchJokeCoroutine());
    }

    IEnumerator FetchJokeCoroutine()
    {
        SetStatus("Loading...");
        ClearJoke();

        using (UnityWebRequest www = UnityWebRequest.Get(apiUrl))
        {
            www.timeout = requestTimeoutSeconds;
            yield return www.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            bool hasError = (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError);
#else
            bool hasError = www.isNetworkError || www.isHttpError;
#endif

            if (hasError)
            {
                SetStatus("Error: " + www.error);
                yield break;
            }

            string json = www.downloadHandler.text;
            try
            {
                JokeResponse joke = JsonUtility.FromJson<JokeResponse>(json);
                if (joke != null)
                {
                    DisplayJoke(joke);
                    SetStatus("OK");
                }
                else
                {
                    SetStatus("No joke in response");
                }
            }
            catch (System.Exception ex)
            {
                SetStatus("Parse error: " + ex.Message);
            }
        }
    }

    void DisplayJoke(JokeResponse joke)
    {
        if (setupTMP != null) setupTMP.text = joke.setup;
        if (punchlineTMP != null) punchlineTMP.text = joke.punchline;
    }

    void ClearJoke()
    {
        if (setupTMP != null) setupTMP.text = "";
        if (punchlineTMP != null) punchlineTMP.text = "";
    }

    void SetStatus(string s)
    {
        if (statusTMP != null) statusTMP.text = s;
    }

    [System.Serializable]
    private class JokeResponse
    {
        public int id;
        public string type;
        public string setup;
        public string punchline;
    }
}
