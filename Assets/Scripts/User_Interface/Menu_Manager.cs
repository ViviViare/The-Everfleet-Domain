using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace viviviare
{
    public class Menu_Manager : MonoBehaviour
    {
        public static Menu_Manager _i;
        private void Awake()
        {
            if (_i == null)
            {
                _i = this;
            }
            else
            {
                Destroy(this);
            }
        }

        [Header("Win Screen")]
        [SerializeField] private Canvas _winScreen;
        #region Win Screen Variables
        [SerializeField] private TextMeshProUGUI _winHealth;
        [SerializeField] private TextMeshProUGUI _winGold;
        [SerializeField] private Button _winReplay;
        [SerializeField] private Button _winQuit;

        #endregion

        [Header("Lose Screen")]
        [SerializeField] private Canvas _loseScreen;
        #region Lose Screen Variables
        [SerializeField] private Button _loseRestart;
        [SerializeField] private Button _loseQuit;
        #endregion
        
        [Header("Pause Screen")]
        [SerializeField] private Canvas _pauseScreen;
        #region Pause Screen Variables
        [SerializeField] private Button _pauseRestart;
        [SerializeField] private Button _pauseQuit;
        [SerializeField] private Button _pauseResume;
        #endregion

        public bool _isMenuUp;
        private bool _isPaused;


        private void Start()
        {
            //Quit buttons setup
            _winQuit.onClick.AddListener(QuitGame);
            _loseQuit.onClick.AddListener(QuitGame);
            _pauseQuit.onClick.AddListener(QuitGame);

            //Restart / Replay buttons setup
            _winReplay.onClick.AddListener(Restart);
            _loseRestart.onClick.AddListener(Restart);
            _pauseRestart.onClick.AddListener(Restart);

            //Resume Button setup
            _pauseResume.onClick.AddListener(Resume);
        }

        private void Update()
        {
            //Guard Clause: Only run if the player has pressed Escape
            if (!Input.GetKeyDown(KeyCode.Escape)) return;

            //Guard Clause: Only run if no menus are already up
            if (_isMenuUp) return;

            //Only run if the player is not currently selecting a tower
            if (TowerPlacement._i._currentTower == null)
            {
                if (_isPaused)
                {
                    Resume();
                }
                else
                {
                    PlayerHasPaused();
                }
            }
        }

        public void PlayerHasLost()
        {
            Time.timeScale = 0f;
            _loseScreen.enabled = true;
            _isMenuUp = true;
        }

        public void PlayerHasWon()
        {
            Time.timeScale = 0f;
            _winScreen.enabled = true;
            _isMenuUp = true;

            _winGold.text = "Remaining Gold: " + Player_Stats._i._currency;
            _winHealth.text = "Remaning Health: " + Player_Stats._i._health;

        }

        public void PlayerHasPaused()
        {
            Time.timeScale = 0f;
            _pauseScreen.enabled = true;
            _isPaused = true;
        }
    

        private void QuitGame()
        {
            Application.Quit();
        }

        private void Resume()
        {
            Time.timeScale = 1f;
            _loseScreen.enabled = false;
            _winScreen.enabled = false;
            _pauseScreen.enabled = false;
            _isPaused = false;
        }
        
        //Reload the current scene (Level_Tutorial)
        private void Restart()
        {
            PoolManager.ClearPools();
            
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
            Waypoints._waypoints.Clear();
            Time.timeScale = 1f;
        }
    }
}
