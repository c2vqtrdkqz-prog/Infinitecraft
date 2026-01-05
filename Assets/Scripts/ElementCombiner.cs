using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class Element
{
    public string id; // unique, e.g., "fire"
    public string name;
    public string description;
}

[Serializable]
public class Recipe
{
    public string a;
    public string b;
    public string resultId;
}

public class ElementCombiner : MonoBehaviour
{
    [Header("Initial Elements")]
    public List<Element> startingElements = new List<Element>()
    {
        new Element(){ id="fire", name="Fire", description="A hot, burning element." },
        new Element(){ id="water", name="Water", description="Fluid and flowing." },
        new Element(){ id="earth", name="Earth", description="Solid and steady." },
        new Element(){ id="air", name="Air", description="Light and unseen." }
    };

    [Header("API (optional)")]
    [Tooltip("If empty, local generation will be used.")]
    public string apiEndpoint = "";

    [Tooltip("Timeout seconds for the API request.")]
    public int apiTimeout = 8;

    private Dictionary<string, Element> elementById = new Dictionary<string, Element>();
    private Dictionary<string, string> recipeMap = new Dictionary<string, string>();

    private const string SAVE_KEY = "Infinitecraft_Elements_v1";

    void Awake()
    {
        InitializeFromStart();
        LoadState();
    }

    void InitializeFromStart()
    {
        elementById.Clear();
        recipeMap.Clear();
        foreach (var e in startingElements)
        {
            elementById[e.id] = e;
        }
    }

    string PairKey(string a, string b)
    {
        return (string.CompareOrdinal(a, b) <= 0) ? $"{a}|{b}" : $"{b}|{a}";
    }

    public void Combine(string idA, string idB, Action<Element> onResult, Action<string> onError = null)
    {
        if (!elementById.ContainsKey(idA) || !elementById.ContainsKey(idB))
        {
            onError?.Invoke("One or both elements unknown");
            return;
        }

        string key = PairKey(idA, idB);
        if (recipeMap.TryGetValue(key, out var resId))
        {
            onResult?.Invoke(elementById[resId]);
            return;
        }

        if (!string.IsNullOrEmpty(apiEndpoint))
        {
            StartCoroutine(CombineViaApiCoroutine(idA, idB, onResult, (err) =>
            {
                Debug.LogWarning("API combine failed: " + err + ". Falling back to local generation.");
                var e = GenerateLocalElement(idA, idB);
                RegisterNewElement(idA, idB, e);
                onResult?.Invoke(e);
            }));
        }
        else
        {
            var e = GenerateLocalElement(idA, idB);
            RegisterNewElement(idA, idB, e);
            onResult?.Invoke(e);
        }
    }

    IEnumerator CombineViaApiCoroutine(string idA, string idB, Action<Element> onResult, Action<string> onError)
    {
        var requestBody = JsonUtility.ToJson(new { a = idA, b = idB });
        using (var uw = new UnityWebRequest(apiEndpoint, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestBody);
            uw.uploadHandler = new UploadHandlerRaw(bodyRaw);
            uw.downloadHandler = new DownloadHandlerBuffer();
            uw.SetRequestHeader("Content-Type", "application/json");
            uw.timeout = apiTimeout;

            yield return uw.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            bool hasError = (uw.result == UnityWebRequest.Result.ConnectionError || uw.result == UnityWebRequest.Result.ProtocolError);
#else
            bool hasError = uw.isNetworkError || uw.isHttpError;
#endif
            if (hasError)
            {
                onError?.Invoke(uw.error);
                yield break;
            }

            try
            {
                var json = uw.downloadHandler.text;
                var apiResp = JsonUtility.FromJson<ApiElementResponse>(json);
                if (apiResp == null || string.IsNullOrEmpty(apiResp.id))
                {
                    onError?.Invoke("Invalid API response");
                    yield break;
                }
                var el = new Element() { id = apiResp.id, name = apiResp.name, description = apiResp.description };
                RegisterNewElement(idA, idB, el);
                onResult?.Invoke(el);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex.Message);
            }
        }
    }

    Element GenerateLocalElement(string idA, string idB)
    {
        string key = PairKey(idA, idB);
        int hash = key.GetHashCode();
        string newId = $"el_{Math.Abs(hash)}";

        string nameA = elementById[idA].name;
        string nameB = elementById[idB].name;
        string newName = $"{nameA}-{nameB}";

        string description = $"A fusion of {nameA} and {nameB}. Unique properties depend on the combination.";

        return new Element() { id = newId, name = newName, description = description };
    }

    void RegisterNewElement(string idA, string idB, Element e)
    {
        elementById[e.id] = e;
        string key = PairKey(idA, idB);
        recipeMap[key] = e.id;
        SaveState();
    }

    [Serializable]
    private class SaveData
    {
        public List<Element> elements = new List<Element>();
        public List<Recipe> recipes = new List<Recipe>();
    }

    void SaveState()
    {
        var sd = new SaveData();
        sd.elements.AddRange(elementById.Values);
        foreach (var kv in recipeMap)
        {
            var parts = kv.Key.Split('|');
            if (parts.Length == 2)
                sd.recipes.Add(new Recipe() { a = parts[0], b = parts[1], resultId = kv.Value });
        }
        var json = JsonUtility.ToJson(sd);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    void LoadState()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY)) return;
        var json = PlayerPrefs.GetString(SAVE_KEY);
        try
        {
            var sd = JsonUtility.FromJson<SaveData>(json);
            if (sd == null) return;
            elementById.Clear();
            recipeMap.Clear();
            foreach (var e in sd.elements) elementById[e.id] = e;
            foreach (var r in sd.recipes)
            {
                var key = PairKey(r.a, r.b);
                recipeMap[key] = r.resultId;
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Failed to load element state: " + ex.Message);
        }
    }

    public List<Element> GetAllElements() => new List<Element>(elementById.Values);
    public bool HasRecipe(string a, string b) => recipeMap.ContainsKey(PairKey(a, b));
    public Element GetElement(string id) => elementById.TryGetValue(id, out var e) ? e : null;

    [Serializable]
    private class ApiElementResponse
    {
        public string id;
        public string name;
        public string description;
    }
}
