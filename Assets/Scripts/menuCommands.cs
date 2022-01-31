using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Michsky.UI.ModernUIPack;

public class menuCommands : MonoBehaviour
{
    private AsyncOperation sceneLoading;                                // loading Operation
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private List<string> webUrl = new List<string>();                             // URL to my page
    [SerializeField] private GameObject settings;
    [SerializeField] private float animationDuration;

            // SETTINGS FIELDS
    [SerializeField] private TMP_InputField _C1Spawn;
    [SerializeField] private TMP_InputField _C2Spawn;
    [SerializeField] private TMP_InputField _F1Spawn;
    [SerializeField] private TMP_InputField _F2Spawn;
    [SerializeField] private TMP_InputField _C1Mut;
    [SerializeField] private TMP_InputField _C2Mut;

    [SerializeField] private CustomDropdown _C1Eats;
    [SerializeField] private CustomDropdown _C2Eats;
    [SerializeField] private CustomDropdown _C1Creates;
    [SerializeField] private CustomDropdown _C2Creates;

    public static int C1Spawn_i;
    public static int C2Spawn_i;
    public static int F1Spawn_i;
    public static int F2Spawn_i;
    public static float C1Mut_f;
    public static float C2Mut_f;
    public static int C1Eats_i;
    public static int C2Eats_i;
    public static int C1Creates_i;
    public static int C2Creates_i;

    public void StartSimulation()                                       // start the simylation
    {
        StartCoroutine(LoadAsyncScene(1));
    }

    public void ExitApp()                                               // exit application
    {
        // paste saving 
        Application.Quit();
        
    }

    public void OpenUrl(int _urlNumber)                                               // open my webPage
    {
        Application.OpenURL(webUrl[_urlNumber]);
    }

    private IEnumerator LoadAsyncScene(int _sceneNumber)
    {
        loadingScreen.SetActive(true);
        sceneLoading = SceneManager.LoadSceneAsync(_sceneNumber);                 // starting scene loading

        while(!sceneLoading.isDone)
        {
            yield return null;
        }
        loadingScreen.SetActive(false);
    }

    public void showSettings()                                                                                               // show settings window
    {
        settings.SetActive(true);
        LeanTween.alphaCanvas(settings.GetComponent<CanvasGroup>(), 1.0f, animationDuration * Time.timeScale);
    }

    public void hideSettings()                                                                                                // hide settings window
    {
        StartCoroutine(_settingsToInnactive());
    }

    private IEnumerator _settingsToInnactive()                                                                                       // additional function for upper one
    {
        LeanTween.alphaCanvas(settings.GetComponent<CanvasGroup>(), 0.0f, animationDuration * Time.timeScale);
        yield return new WaitForSeconds(animationDuration * Time.timeScale);
        settings.SetActive(false);
    }

    public void SaveProperties()
    {
        C1Spawn_i = int.Parse(_C1Spawn.text);
        C2Spawn_i = int.Parse(_C2Spawn.text);
        F1Spawn_i = int.Parse(_F1Spawn.text);
        F2Spawn_i = int.Parse(_F2Spawn.text);

        C1Mut_f = float.Parse(_C1Mut.text);
        C2Mut_f = float.Parse(_C2Mut.text);

        C1Eats_i = _C1Eats.selectedItemIndex;
        C2Eats_i = _C2Eats.selectedItemIndex;
        C1Creates_i = _C1Creates.selectedItemIndex;
        C2Creates_i = _C2Creates.selectedItemIndex;
    }

    private void Start()
    {
        Time.timeScale = 1.0f;
    }
}
