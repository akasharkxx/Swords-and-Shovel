using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    // keep track of the game state
    // generate other persistent systems
    public GameObject[] SystemPrefabs;

    private List<GameObject> _instancedSystemPrefabs;
    private string _currentLevelName = string.Empty;

    List<AsyncOperation> _loadOperations;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        _instancedSystemPrefabs = new List<GameObject>();        
        _loadOperations = new List<AsyncOperation>();

        InstantiateSystemPrefabs();

        LoadLevel("Main");
    }

    void OnLoadOperationComplete(AsyncOperation ao)
    {
        if (_loadOperations.Contains(ao))
        {
            _loadOperations.Remove(ao);

            // dispatch message
            // transition between scenes
        }
        Debug.Log("Load complete");
    }

    void OnUnloadOperationComplete(AsyncOperation ao)
    {
        Debug.Log("Unload complete");
    }

    void InstantiateSystemPrefabs()
    {
        GameObject prefabInstances;
        for (int i = 0; i < SystemPrefabs.Length; i++)
        {
            prefabInstances = Instantiate(SystemPrefabs[i]);
            _instancedSystemPrefabs.Add(prefabInstances);
        }
    }

    public void LoadLevel(string levelName)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        if(ao == null)
        {
            Debug.LogError("[GameManager] unable to load level: " + levelName);
            return;
        }
        ao.completed += OnLoadOperationComplete;
        _loadOperations.Add(ao);
        _currentLevelName = levelName;
    }

    public void UnloadLevel(string levelName)
    {
        AsyncOperation ao = SceneManager.UnloadSceneAsync(levelName);
        ao.completed += OnUnloadOperationComplete;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        for(int i = 0; i < _instancedSystemPrefabs.Count; i++)
        {
            Destroy(_instancedSystemPrefabs[i]);
        }
        _instancedSystemPrefabs.Clear();
    }
}
