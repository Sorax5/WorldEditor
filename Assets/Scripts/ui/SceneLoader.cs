using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private Animator animator;

    // Update is called once per frame
    void Update()
    {
        // if right mouse button is pressed
        /*if(Input.GetMouseButtonDown(1))
        {
            // verify if current scene is the last scene
            if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings - 1)
            {
                return;
            }
            // load next scene
            LoadNextScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
        }*/
        
        // if left mouse button is pressed
        /*if(Input.GetMouseButtonDown(0))
        {
            // verify if current scene is the first scene
            if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0)
            {
                return;
            }
            // load previous scene
            LoadNextScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 1);
        }*/
        
    }
    
    public void LoadNextScene(int sceneIndex)
    {
        StartCoroutine(LoadScene(sceneIndex));
    }

    IEnumerator LoadScene(int sceneIndex)
    {
        animator.SetTrigger("End");
        yield return new WaitForSeconds(1f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }
}
