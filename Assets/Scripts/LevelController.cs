using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.JuniorProgrammer.Breakout
{
    public class LevelController : MonoBehaviour
    {
        public Action onGameOver;
        public Action onRestartGame;
        public Action<int> onAddPoints;
        public Rigidbody Ball;
        public Brick BrickPrefab;
        public int LineCount = 6;

        bool isGameStarted = false;
        bool isGameOver = false;

        // Start is called before the first frame update
        void Start()
        {
            const float step = 0.6f;
            int perLine = Mathf.FloorToInt(4.0f / step);

            int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
            for (int i = 0; i < LineCount; ++i)
            {
                for (int x = 0; x < perLine; ++x)
                {
                    Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                    var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                    brick.PointValue = pointCountArray[i];
                    brick.onDestroyed.AddListener(AddPoint);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!isGameStarted)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    isGameStarted = true;
                    float randomDirection = Random.Range(-1.0f, 1.0f);
                    Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                    forceDir.Normalize();

                    Ball.transform.SetParent(null);
                    Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
                }
            }
            else if (isGameOver)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    onRestartGame?.Invoke();
                }
            }
        }

        void AddPoint(int point)
        {
            onAddPoints?.Invoke(point);
        }

        public void GameOver()
        {
            isGameOver = true;
            onGameOver?.Invoke();
        }
    }
}
