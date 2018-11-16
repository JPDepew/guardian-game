using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour {

    AudioSource[] audioSource;

	// Use this for initialization
	void Start () {
        audioSource = GetComponents<AudioSource>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Play()
    {
        audioSource[0].Play();
        SceneManager.LoadScene(1);
    }

    public void ShowInstructions()
    {
        audioSource[0].Play();
    }
    
    public void CloseInstructions()
    {
        audioSource[0].Play();
    }

    public void Exit()
    {
        audioSource[0].Play();
        Application.Quit();
    }
}
