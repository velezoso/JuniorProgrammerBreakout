using System;
using System.Collections;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Scripts.JuniorProgrammer.Breakout
{
    public class MainManager : MonoBehaviour
    {
        public static MainManager Instance;
        public GameObject menuPanel;
        public GameObject gamePanel;
        public TMP_InputField menuNameInputField;
        public TMP_Text menuBestScoreText;
        public Text gameScoreText;
        public Text gameBestScoreText;
        public GameObject gameOverText;

        int points;
        string playerName = "";
        int bestPlayerScore = 0;
        string bestPlayerName = "";
        LevelController levelController;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (menuNameInputField == null) throw new Exception("ERROR olvidaste asignar Inputfield");
            if (menuPanel == null) throw new Exception("ERROR olvidaste asignar menuPanel");
            if (gamePanel == null) throw new Exception("ERROR olvidaste asignar gamePanel");

            menuNameInputField.onEndEdit.AddListener(SetPlayerName);

            LoadGameData();
            UpdateTexts();

            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            menuPanel.SetActive(true);
            gamePanel.SetActive(false);
        }

        void SetPlayerName(string name)
        {
            playerName = name;
        }

        public void StartGame()
        {
            StartCoroutine(LoadScene());
        }

        IEnumerator LoadScene()
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(Constants.GAMESCENE);

            while (!asyncOperation.isDone)
            {
                yield return null;
            }

            points = 0;
            gameScoreText.text = $"Score : {points}";

            menuPanel.SetActive(false);
            gamePanel.SetActive(true);
            gameOverText.SetActive(false);

            levelController = GameObject.Find(Constants.LEVELCONTROLLER_GAMEOBJECT_NAME).GetComponent<LevelController>();
            levelController.onGameOver += GameOver;
            levelController.onAddPoints += AddPoints;
            levelController.onRestartGame += RestartGame;
        }

        void GameOver()
        {
            gameOverText.SetActive(true);

            if (points > bestPlayerScore)
            {
                bestPlayerScore = points;
                bestPlayerName = playerName;
                SaveGameData();
            }
            UpdateTexts();
        }

        void UpdateTexts()
        {
            if (!bestPlayerName.Equals(string.Empty))
            {
                menuBestScoreText.text = $"Best Score: {bestPlayerName} : {bestPlayerScore}";
            }
            else
            {
                menuBestScoreText.text = $"Best Score: {bestPlayerScore}";
            }
            gameBestScoreText.text = menuBestScoreText.text;
        }

        void AddPoints(int pointsToAdd)
        {
            points += pointsToAdd;
            gameScoreText.text = $"Score : {points}";
        }

        void RestartGame()
        {
            levelController.onGameOver -= GameOver;
            levelController.onAddPoints -= AddPoints;
            levelController.onRestartGame -= RestartGame;
            StartGame();
        }

        void SaveGameData()
        {
            SaveData data = new SaveData();
            data.currentPlayerName = playerName;
            data.bestPlayerScore = bestPlayerScore;
            data.bestPlayerName = bestPlayerName;

            string json = JsonUtility.ToJson(data);

            File.WriteAllText(Application.persistentDataPath + Constants.GAMEINFO_FILE, json);
        }

        void LoadGameData()
        {
            string path = Application.persistentDataPath + Constants.GAMEINFO_FILE;
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                SaveData data = JsonUtility.FromJson<SaveData>(json);

                playerName = data.currentPlayerName;
                menuNameInputField.text = playerName;

                bestPlayerScore = data.bestPlayerScore;
                bestPlayerName = data.bestPlayerName;
            }
        }

        public void QuitGame()
        {
            menuNameInputField.onEndEdit.RemoveListener(SetPlayerName);
            SaveGameData();

            if (levelController != null)
            {
                levelController.onGameOver -= GameOver;
                levelController.onAddPoints -= AddPoints;
                levelController.onRestartGame -= RestartGame;
            }


#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
        Application.Quit(); // original code to quit Unity player
#endif
        }

        [Serializable]
        class SaveData
        {
            public string currentPlayerName;
            public string bestPlayerName;
            public int bestPlayerScore;
        }

    }
}