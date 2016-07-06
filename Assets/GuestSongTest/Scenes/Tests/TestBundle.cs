using UnityEngine;
using System.Collections;
using AssetBundles;
using System.Collections.Generic;
using UnityEngine.UI;

public class TestBundle : MonoBehaviour
{
    public GameObject PanelLoading;
    public GameObject PanelGuestSong;
    public GameObject PanelResult;

    public AudioSource audioSource;
    public List<AudioClip> clips = new List<AudioClip>();

    public Button[] Buttons;

    void Awake()
    {
        this.audioSource = gameObject.GetComponent<AudioSource>();
        clips = new List<AudioClip>();
        SetPanel();
    }

    // Use this for initialization
    IEnumerator Start()
    {
        // Initialize bundle
        yield return StartCoroutine(Initialize());

        // download bundle
        yield return StartCoroutine(InstantiateGameObjectAsync("2016-song", "Don't Let Me Down -The Chainsmokers, Daya"));
        yield return StartCoroutine(InstantiateGameObjectAsync("2016-song", "This One’s For You -David Guetta, Zara Larsson"));
        yield return StartCoroutine(StartGame());
    }

    public List<int> corrent = new List<int>();
    public int CurrentIndexSong = 0;
    public int CurrentCorrectIndex = 0;

    IEnumerator StartGame()
    {
        SetPanel(Panel.GUESTSONG);

        corrent = new List<int>();
        BindSong(0);

        yield return null;
    }

    public void BindSong(int index)
    {
        for (int i = 0; i < Buttons.Length; i++)
        {
            Button btn = Buttons[i];
            if (index == 0 && i == 2)
            {
                btn.GetComponentInChildren<Text>().text = "Don't Let Me Down";
                CurrentCorrectIndex = i;
            }
            else if (index == 1 && i == 3)
            {
                btn.GetComponentInChildren<Text>().text = "This One’s For You";
                CurrentCorrectIndex = i;
            }
            else btn.GetComponentInChildren<Text>().text = "la di da " + i;

            var j = i;

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => { Select(j); });
        }

        audioSource.clip = clips[index];
        audioSource.Play();
    }

    public void Select(int index)
    {
        Debug.Log("Select");
        audioSource.Stop();

        if (CurrentCorrectIndex == index) corrent.Add(1);
        else corrent.Add(0);

        Next();
    }

    public void Next()
    {
        CurrentIndexSong += 1;
        if (CurrentIndexSong > clips.Count) return;
        if (CurrentIndexSong == clips.Count)
        {
            End();
        }
        else {
            BindSong(CurrentIndexSong);
        }
    }

    public void End()
    {
        SetPanel(Panel.RESULT);

        string txtResult = "";

        string[] sArr = new string[corrent.Count];
        for (int i = 0; i < corrent.Count; i++) { sArr[i] = corrent[i] == 0 ? "WRONG" : "CORRECT"; }
        txtResult = string.Join(" - ", sArr);

        PanelResult.GetComponentInChildren<Text>().text = txtResult;
        Debug.Log("END");
    }

    public enum Panel { LOADING, GUESTSONG, RESULT }
    public void SetPanel(Panel active = Panel.LOADING)
    {
        PanelLoading.SetActive(false);
        PanelGuestSong.SetActive(false);
        PanelResult.SetActive(false);

        switch (active)
        {
            case Panel.GUESTSONG:
                PanelGuestSong.SetActive(true);
                break;

            case Panel.RESULT:
                PanelResult.SetActive(true);
                break;

            default:
                PanelLoading.SetActive(true);
                break;
        }
    }

    // Initialize the downloading url and AssetBundleManifest object.
    protected IEnumerator Initialize()
    {
        // Don't destroy this gameObject as we depend on it to run the loading script.
        DontDestroyOnLoad(gameObject);

        // With this code, when in-editor or using a development builds: Always use the AssetBundle Server
        // (This is very dependent on the production workflow of the project. 
        // 	Another approach would be to make this configurable in the standalone player.)
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        AssetBundleManager.SetDevelopmentAssetBundleServer();
#else
		// Use the following code if AssetBundles are side-by-side with web deployment:
		AssetBundleManager.SetSourceAssetBundleURL(Application.dataPath + "/");
		// Or customize the URL based on your deployment or configuration
		//AssetBundleManager.SetSourceAssetBundleURL("http://www.MyWebsite/MyAssetBundles");
#endif

        // Initialize AssetBundleManifest which loads the AssetBundleManifest object.
        var request = AssetBundleManager.Initialize();

        if (request != null)
            yield return StartCoroutine(request);
    }

    protected IEnumerator InstantiateGameObjectAsync(string assetBundleName, string assetName, int loadpercent = 100)
    {
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        // Load asset from assetBundle.
        AssetBundleLoadAssetOperation www = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(GameObject));

        if (www == null) yield break;
        yield return StartCoroutine(www);

        // Get the asset.
        AudioClip clip = www.GetAsset<AudioClip>();
        clips.Add(clip);

        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log("Loaded successfully in " + elapsedTime + " seconds");
    }
}
