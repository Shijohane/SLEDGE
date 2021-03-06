﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class MainMenu : MonoBehaviour {

    [SerializeField] private Animator mainMenuAnimator;
    [SerializeField] private Animator fadeAnimator;

    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float infoY = 14f;
    [SerializeField] private float landingY = 3f;
    [SerializeField] private float levelSelectorY = -8f;

    [SerializeField] private Button[] levelButtons;

    [SerializeField] private GameObject SplashScreenUI;
    [SerializeField] private AudioSource BGM;

    private LevelLockData levelLockData;

    private void Awake()
    {
        LoadLevelLockData();

        for(int i = 2; i >= levelLockData.currentLevelUnlocked; i--) {
            levelButtons[i].interactable = false;
            levelButtons[i].transform.GetChild(0).gameObject.SetActive(false);
            levelButtons[i].transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    private void Start()
    {
        StartCoroutine(WaitSplashScreen());
    }

    public void Landing()
    {
        cameraTarget.position = new Vector3(cameraTarget.position.x, landingY, cameraTarget.position.z);

        mainMenuAnimator.SetTrigger("landing");
    }

	public void LevelSelector()
    {
        cameraTarget.position = new Vector3(cameraTarget.position.x, levelSelectorY, cameraTarget.position.z);

        mainMenuAnimator.SetTrigger("levelSelector");
    }

    public void Info()
    {
        cameraTarget.position = new Vector3(cameraTarget.position.x, infoY, cameraTarget.position.z);

        mainMenuAnimator.SetTrigger("info");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadLevel(int level)
    {
        StartCoroutine(FadeLoadLevel(level));
    }

    private IEnumerator FadeLoadLevel(int level)
    {
        fadeAnimator.SetTrigger("fadeOut");

        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene(level);
    }

    private IEnumerator WaitSplashScreen()
    {
        SplashScreenUI.SetActive(true);

        yield return new WaitForSeconds(8f);

        SplashScreenUI.SetActive(false);
        BGM.Play();
    }

    private void LoadLevelLockData()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file;

        if (File.Exists(Application.persistentDataPath + "/levelLockData.dat")) {
            file = File.Open(Application.persistentDataPath + "/levelLockData.dat", FileMode.Open);
            levelLockData = (LevelLockData)binaryFormatter.Deserialize(file);
            Debug.Log(levelLockData.currentLevelUnlocked);
        } else {
            file = File.Open(Application.persistentDataPath + "/levelLockData.dat", FileMode.OpenOrCreate);

            LevelLockData data = new LevelLockData();
            binaryFormatter.Serialize(file, data);

            levelLockData = data;
        }

        file.Close();
    }
}

[Serializable]
public class LevelLockData
{
    public int currentLevelUnlocked;

    public LevelLockData()
    {
        currentLevelUnlocked = 1;
    }

    public LevelLockData(int level)
    {
        currentLevelUnlocked = level;
    }
}