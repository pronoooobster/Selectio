using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ChartAndGraph;

public class UICommands : MonoBehaviour
{
    public List<GraphChart> graphs = new List<GraphChart>();

    [SerializeField] private GameObject overlay;                                // hamburger overlay
    [SerializeField] private List<GameObject> charts = new List<GameObject>();
    [SerializeField] private Button burgerMenu;
    [SerializeField] private float hamburgerAnimationDuration_sec = 0.8f;
    [SerializeField] private GameObject loadingScreen;
    
    private simulationManager simulationManagerScript;
    private float _currentSpeed = 1.0f;
    private AsyncOperation sceneLoading;

    private IEnumerator LoadAsyncScene(int _sceneNumber)
    {
        loadingScreen.SetActive(true);
        sceneLoading = SceneManager.LoadSceneAsync(_sceneNumber);                 // starting scene loading

        while (!sceneLoading.isDone)
        {
            yield return null;
        }
        loadingScreen.SetActive(false);
    }

    private IEnumerator WaitAnim(float _sec)                                    // wait till the end of the animation
    {
        yield return new WaitForSeconds(_sec);
        overlay.SetActive(false);
    }

    private IEnumerator HamburgerDelay(float _sec)                              // local
    {
        burgerMenu.interactable = false;
        yield return new WaitForSeconds(_sec);
        burgerMenu.interactable = true;
    }

    public void TimeStopExit()                                                  // time pause in exit button
    {
        if (!(Time.timeScale == 0.00000001f))
        {
            _currentSpeed = Time.timeScale;
            Time.timeScale = 0.00000001f;
        }
    }

    public void TimeStartExit()
    {
        if (Time.timeScale == 0.00000001f)
        {
            Time.timeScale = _currentSpeed;
        }
    }

    public void TimeStop(Toggle _caller)
    {
        if (Time.timeScale == 0.00000001f)
        {
            _caller.isOn = true;
        }
        else
        {
            _caller.isOn = false;
        }
        if (Time.timeScale == 0.00000001f)
        {
            Time.timeScale = _currentSpeed;
        } else
        {
            _currentSpeed = Time.timeScale;
            Time.timeScale = 0.00000001f;
        }
        
    }

    public void showOverlay()
    {
        if(overlay.activeSelf)
        {
            overlay.transform.LeanMoveLocalX(-146.3f, hamburgerAnimationDuration_sec * Time.timeScale).setEaseInOutQuart();
            StartCoroutine(HamburgerDelay(hamburgerAnimationDuration_sec * Time.timeScale));
            StartCoroutine(WaitAnim(hamburgerAnimationDuration_sec * Time.timeScale));
            
        } else
        {
            overlay.SetActive(true);
            overlay.transform.LeanMoveLocalX(-5.7f, hamburgerAnimationDuration_sec * Time.timeScale).setEaseInOutQuart();
            StartCoroutine(HamburgerDelay(hamburgerAnimationDuration_sec * Time.timeScale));
        }
    }

    public void showCharts(int _chartNum)                                                                                               // show charts window
    {
        charts[_chartNum].SetActive(true);
        LeanTween.alphaCanvas(charts[_chartNum].GetComponent<CanvasGroup>(), 1.0f, hamburgerAnimationDuration_sec * Time.timeScale);
    }

    public void hideCharts(int _chartNum)                                                                                                // hide charts window
    {
        StartCoroutine(_chartsToInnactive(_chartNum));
    }

    private IEnumerator _chartsToInnactive(int _chartNum)                                                                                       // additional function for upper one
    {
        LeanTween.alphaCanvas(charts[_chartNum].GetComponent<CanvasGroup>(), 0.0f, hamburgerAnimationDuration_sec * Time.timeScale);
        yield return new WaitForSeconds(hamburgerAnimationDuration_sec * Time.timeScale);
        charts[_chartNum].SetActive(false);
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1.0f;
        StartCoroutine(LoadAsyncScene(0));
    }



    public void GraphUpdate(int _graphNum, string _itemName, float _x, float _y)
    {
        graphs[_graphNum].DataSource.StartBatch();
        graphs[_graphNum].DataSource.AddPointToCategoryRealtime(_itemName, _x, _y);
        graphs[_graphNum].DataSource.EndBatch();
    }

    public void HorizontalScrollToggle(Toggle _caller)
    {
        if (_caller.isOn)
            _caller.GetComponentInParent<GraphChart>().AutoScrollHorizontally = true;
        else
            _caller.GetComponentInParent<GraphChart>().AutoScrollHorizontally = false;
    }

    public void VerticalScrollToggle(Toggle _caller)
    {
        if (_caller.isOn)
            _caller.GetComponentInParent<GraphChart>().AutoScrollVertically = true;
        else
            _caller.GetComponentInParent<GraphChart>().AutoScrollVertically = false;
    }

    private void Start()
    {
        simulationManagerScript = GameObject.Find("Simulation Manager").GetComponent<simulationManager>();
    }
}
