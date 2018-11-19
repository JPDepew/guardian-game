using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    AudioSource[] audioSource;
    public Animator Instructions;

    // Use this for initialization
    void Start()
    {
        audioSource = GetComponents<AudioSource>();
    }

    public void Play()
    {
        audioSource[0].Play();
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(1);
    }

    public void ShowInstructions()
    {
        audioSource[0].Play();
        StartCoroutine(CycleInstructions());
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

    IEnumerator CycleInstructions()
    {
        Instructions.gameObject.SetActive(true);
        Instructions.Play("Instructions1");
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                break;
            }
            yield return null;
        }
        Instructions.Play("Instructions2");
        Input.ResetInputAxes();
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                break;
            }
            yield return null;
        }
        Instructions.Play("Instructions3");
        Input.ResetInputAxes();
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                break;
            }
            yield return null;
        }
        Instructions.Play("FadeOut");
        yield return new WaitForSeconds(Instructions.GetCurrentAnimatorStateInfo(0).length);
        Instructions.gameObject.SetActive(false);
    }
}
