using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class ManageScene : MonoBehaviour
{
    public PlayableDirector director;
    public Playable timeline;
    PlayableGraph graph;

    private bool isPlaying = false;
    private float x = 0;

    // Start is called before the first frame update
    void Start()
    {
        //director.RebuildGraph();
        //director.RebindPlayableGraphOutputs();
        //DontDestroyOnLoad(director);
        //print("start");
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        x += Time.deltaTime;
        print(x);
        if (isPlaying)
        {
            director.Play();
            director.DeferredEvaluate();
            director.playableGraph.Evaluate(Time.deltaTime);
        }
    }

    private void Awake()
    {
        director.RebuildGraph();
        //director.RebindPlayableGraphOutputs();
        //print("awake");
    }

    void OnEnable()
    {
        //director.RebuildGraph();
        //director.RebindPlayableGraphOutputs();
        //print("enable");
        SceneManager.sceneLoaded += onSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= onSceneLoaded;
    }

    void onSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //director.RebuildGraph();
        director.Play();
        print(director.playableGraph.IsPlaying());
        //director.Resume();
        print(director.state);
        director.playableGraph.GetRootPlayable(0).SetSpeed(1);
        print(director.playableGraph.GetRootPlayable(0).GetSpeed());
        //isPlaying = true;
        
    }
}
