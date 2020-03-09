﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip levelComplete;
    [SerializeField] AudioClip rocketCrash;
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem levelCompleteParticles;
    [SerializeField] ParticleSystem rocketCrashParticles;
    Rigidbody rigidBody;
    AudioSource audioSource;
    enum State { Alive, Dying, Transcending }
    State state = State.Alive;


    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive)
        {
            return;
        }
        switch (collision.gameObject.tag)
        {
            case "Friendly":

                break;

            case "Finish":
                startLevelComplete();
                break;

            default:
                startDeath();
                break;
        }

    }

    private void startDeath()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(rocketCrash);
        rocketCrashParticles.Play();
        Invoke("BackToStart", 2f);
    }

    private void startLevelComplete()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(levelComplete);
        levelCompleteParticles.Play();
        Invoke("LoadNextLevel", 1f);
    }

    private void BackToStart()
    {
        SceneManager.LoadScene(0);
      
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(1);
    }

    private void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true;
        
        float rotationThisFrame = rcsThrust * Time.deltaTime;
       

        if (Input.GetKey(KeyCode.A))
        {
            
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }

        else if (Input.GetKey(KeyCode.D))
        {
            
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        rigidBody.freezeRotation = false;
    }

    private void RespondToThrustInput()
    {
        float thrustThisFrame = mainThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust(thrustThisFrame);
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust(float thrustThisFrame)
    {
        rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }
}
