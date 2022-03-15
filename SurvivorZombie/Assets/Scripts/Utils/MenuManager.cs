using System;
using System.Collections;
using System.Collections.Generic;
using SurvivorZombies.Utils.Sound;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SurvivorZombies.Utils {
    
    public class MenuManager : MonoBehaviour {
        public AudioClip ClickButton;
        public AudioClip SplashMusic;
        
        public Button PlayGameBtn;
        public Button QuitGameBtn;

        private void Start() {
            AudioController.Instance.Play(SplashMusic, AudioController.SoundType.Music, 0.1f, true);
            
            PlayGameBtn.onClick.AddListener(() => {
                AudioController.Instance.Play(ClickButton, AudioController.SoundType.SoundEffect2D);
                SceneManager.LoadScene("Lobby");
            });
            
            QuitGameBtn.onClick.AddListener(() => {
                AudioController.Instance.Play(ClickButton, AudioController.SoundType.SoundEffect2D);
                Application.Quit();
            });
        }
    }
}