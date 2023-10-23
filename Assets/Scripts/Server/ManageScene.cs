using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class ManageScene : MonoBehaviour
{
    public PlayableDirector director;
    public Playable timeline;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
    }

    private void Awake()
    {
        director.RebuildGraph();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += onSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= onSceneLoaded;
    }

    void onSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        director.Play();
        director.playableGraph.GetRootPlayable(0).SetSpeed(1);
    }
}
