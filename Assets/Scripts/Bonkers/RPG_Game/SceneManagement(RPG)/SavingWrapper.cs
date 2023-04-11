using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace Bonkers.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField] float fadeInTime = 0.2f;
        [SerializeField] float waitBeforeInitialFadeIn = 2f;

        const string defaultSaveFile = "save";
        SavingSystem savingSystem;

        IEnumerator Start()
        {
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
            savingSystem = GetComponent<SavingSystem>();
            yield return savingSystem.LoadLastScene(defaultSaveFile);
            yield return new WaitForSeconds(waitBeforeInitialFadeIn);
            yield return fader.FadeIn(fadeInTime);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L)) Load();
            if (Input.GetKeyDown(KeyCode.S)) Save();
        }

        public void Save()
        {
            savingSystem.Save(defaultSaveFile);
        }

        public void Load()
        {
            //call to saving system load
            savingSystem.Load(defaultSaveFile);
            Debug.Log("Loading save!");
        }
    }

}