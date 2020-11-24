using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private enum ESceneControlMode
    {
        Load, Unload
    }

    private struct SceneControlTask
    {
        public string SceneName { get; set; }
        public ESceneControlMode SceneControlMode { get; set; }
        public bool ActivateWhenReady { get; set; }
    }

    private Queue<SceneControlTask> m_taskQueue;
    private bool m_bExecutingTask;

    public bool IsActiveSceneChangedSuccessfully { get; private set; }

    public Scene ActiveScene
    {
        get { return SceneManager.GetActiveScene (); }
        set { IsActiveSceneChangedSuccessfully = SceneManager.SetActiveScene (value); }
    }

    public string AcitveSceneName
    {
        get { return ActiveScene.name; }
        set { ActiveScene = SceneManager.GetSceneByName (value); }
    }

    public int ActiveSceneBuildIndex
    {
        get { return ActiveScene.buildIndex; }
        set { ActiveScene = SceneManager.GetSceneByBuildIndex (value); }
    }

    // Event is invoked right after calling SceneManager.LoadSceneAsync
    public EventDictionary<string> OnLoadStartEventDictionary { get; private set; } = new EventDictionary<string> ();
    // Event is invoked while AsyncOperation of SceneManager.LoadSceneAsync is not done yet
    // Param0: progress, Param1: ready to activate
    public EventDictionary<string, float, bool> OnLoadingEventDictionary { get; private set; } = new EventDictionary<string, float, bool> ();
    // Event is invoked when SceneManager.sceneLoaded is called (before calling Start and first Update, not activated yet)
    public EventDictionary<string> OnLoadedEventDictionary { get; private set; } = new EventDictionary<string> ();
    // Event is invoked atfer activating scene (after calling Start and first Update)
    public EventDictionary<string> OnLoadEndEventDictionary { get; private set; } = new EventDictionary<string> ();

    // Event is invoked right after calling SceneManager.UnloadSceneAsync
    public EventDictionary<string> OnUnloadStartEventDictionary { get; private set; } = new EventDictionary<string> ();
    // Event is invoked while AsyncOperation of SceneManager.UnloadSceneAsync is not done yet
    // Param0: progress
    public EventDictionary<string, float> OnUnloadingEventDictionary { get; private set; } = new EventDictionary<string, float> ();
    // Event is invoked when SceneManager.sceneUnloaded is called (after calling OnDestroy)
    public EventDictionary<string> OnUnloadedEventDictionary { get; private set; } = new EventDictionary<string> ();

    private void Awake ()
    {
        m_taskQueue = new Queue<SceneControlTask> ();
        m_bExecutingTask = false;

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void Update ()
    {
        if (m_bExecutingTask || m_taskQueue.Any () == false)
        {
            return;
        }

        m_bExecutingTask = true;
        var task = m_taskQueue.Dequeue ();

        switch (task.SceneControlMode)
        {
            case ESceneControlMode.Load:
                StartCoroutine (SceneLoadCoroutine (task));
                break;
            case ESceneControlMode.Unload:
                StartCoroutine (SceneUnloadCoroutine (task));
                break;
        }
    }

    public void RequestLoadScene (string sceneName, bool activateWhenReady)
    {
        var task = new SceneControlTask ();
        task.SceneName = sceneName;
        task.SceneControlMode = ESceneControlMode.Load;
        task.ActivateWhenReady = activateWhenReady;

        m_taskQueue.Enqueue (task);
    }

    public void RequestUnloadScene (string sceneName)
    {
        var task = new SceneControlTask ();
        task.SceneName = sceneName;
        task.SceneControlMode = ESceneControlMode.Unload;
        task.ActivateWhenReady = true;

        m_taskQueue.Enqueue (task);
    }

    private IEnumerator SceneLoadCoroutine (SceneControlTask task)
    {
        var sceneName = task.SceneName;
        var async = SceneManager.LoadSceneAsync (sceneName, LoadSceneMode.Additive);
        async.allowSceneActivation = task.ActivateWhenReady;

        OnLoadStartEventDictionary.GetEvent (sceneName).Invoke ();

        while (async.isDone == false)
        {
            float progress = async.progress;
            bool bReady = progress >= 0.9f;

            OnLoadingEventDictionary.GetEvent (sceneName).Invoke (progress, bReady);

            yield return null;
        }

        OnLoadEndEventDictionary.GetEvent (sceneName).Invoke ();
    }

    private IEnumerator SceneUnloadCoroutine (SceneControlTask task)
    {
        var sceneName = task.SceneName;
        var async = SceneManager.UnloadSceneAsync (sceneName);
        async.allowSceneActivation = task.ActivateWhenReady;

        OnUnloadStartEventDictionary.GetEvent (sceneName).Invoke ();

        while (async.isDone == false)
        {
            float progress = async.progress;

            OnUnloadingEventDictionary.GetEvent (sceneName).Invoke (progress);

            yield return null;
        }
    }

    private void OnSceneLoaded (Scene scene, LoadSceneMode mode)
    {
        OnLoadedEventDictionary.GetEvent (scene.name).Invoke ();
        m_bExecutingTask = false;
    }

    private void OnSceneUnloaded (Scene scene)
    {
        OnUnloadedEventDictionary.GetEvent (scene.name).Invoke ();
        m_bExecutingTask = false;
    }
}
