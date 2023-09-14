using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1.334f;

    /// <summary>
    /// Called on to start loading the new scene with the animation
    /// </summary>
    /// <param name="buildIndex"></param>
    public void LoadNewScene(int buildIndex)
    {
        StartCoroutine(LoadScene(buildIndex));
    }

    IEnumerator LoadScene(int buildIndex)
    {
        // Play Start animation
        transition.SetTrigger("Start");

        // Wait
        yield return new WaitForSeconds(transitionTime);

        // Start Loading Scene
        AsyncOperation operation = SceneManager.LoadSceneAsync(buildIndex);

        // Play loading animation
        //transition.SetTrigger("Loading");

        //while (!operation.isDone)
        //{
        //    float progress = Mathf.Clamp01(operation.progress / 0.9f);
        //    Debug.Log(progress);

        //    yield return null;
        //}
    }
}
