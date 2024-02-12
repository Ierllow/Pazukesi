using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Pazukesi.Game
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private BallGenerator ballGenerator;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI restTextText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private GameObject gameOverRoot;
        [SerializeField] private GameObject finishButton;
        [SerializeField] private Button exchangeButton;
        [SerializeField] private GameObject pointEffectPrefab;

        private readonly List<Ball> removeBallList = new();

        private Ball currentDraggingBall;
        private bool isDragging;
        private int currentScore = 0;

        private bool isGameOver = false;

        private void Start()
        {
            StartCoroutine(ballGenerator.CoCreateDropBalls(50, () => StartCoroutine(CoEnableInteractable())));
            StartCoroutine(CoCountDown());
        }

        private void Update()
        {
            if (isGameOver) return;
            if (Input.GetMouseButtonDown(0)) OnDragBegin();
            else if (Input.GetMouseButtonUp(0)) OnDragEnd();
            else if (isDragging) OnDragging();
        }

        private IEnumerator CoEnableInteractable()
        {
            exchangeButton.interactable = false;
            yield return new WaitForSeconds(5.0f);
            exchangeButton.interactable = true;
        }

        private IEnumerator CoCountDown()
        {
            var restTime = 60;
            while (restTime > 0)
            {
                yield return new WaitForSeconds(1);
                restTime--;
                restTextText.SetText(string.Format("{0}", restTime));
            }
            OnGameOver();
        }

        private void AddScore(int point)
        {
            currentScore += point;
            scoreText.SetText(string.Format("Score:{0}", currentScore));
        }

        private void OnDragBegin()
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit && hit.collider.GetComponent<Ball>())
            {
                var ball = hit.collider.GetComponent<Ball>();
                if (ball.Id == -1)
                {
                    Bomb(ball);
                    return;
                }
                AddRemoveBall(ball);
                isDragging = true;
            }
        }

        private void OnDragging()
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit && hit.collider.GetComponent<Ball>())
            {
                var ball = hit.collider.GetComponent<Ball>();
                if (ball.Id == currentDraggingBall.Id)
                {
                    var distance = Vector2.Distance(ball.transform.position, currentDraggingBall.transform.position);
                    if (distance < 1.5)
                    {
                        AddRemoveBall(ball);
                    }
                }
            }
        }

        private void OnDragEnd()
        {
            var removeCount = removeBallList.Count;
            if (removeCount >= 3)
            {
                removeBallList.ForEach(x => x.OnDestroy());
                StartCoroutine(ballGenerator.CoCreateDropBalls(removeCount, () => StartCoroutine(CoEnableInteractable())));
                var score = removeCount * 100;
                AddScore(score);
                SpawnPointEffect(removeBallList.Last(x => x).transform.position, score);
                removeBallList.ForEach(x => x.transform.DOScale(Vector3.one, 1.7f));
            }
            else
            {
                removeBallList.ForEach(x => x.transform.localScale = Vector3.one);
            }
            removeBallList.Clear();
            isDragging = false;
        }

        private void AddRemoveBall(Ball ball)
        {
            currentDraggingBall = ball;
            if (removeBallList.Contains(ball)) return;
            ball.transform.DOScale(new Vector3(1.4f, 1.4f, 1.4f), 0.3f);
            removeBallList.Add(ball);

        }

        private void Bomb(Ball bomb)
        {
            var hitObjArray = Physics2D.OverlapCircleAll(bomb.transform.position, 2);
            var bombList = new List<Ball>();
            hitObjArray.Select(x => x.TryGetComponent(out Ball ball) ? ball : null).Where(x => x != null).ToList().ForEach(x => bombList.Add(x));
            bombList.ForEach(x => x.OnDestroy());
            var removeCount = bombList.Count;

            StartCoroutine(ballGenerator.CoCreateDropBalls(removeCount, () => StartCoroutine(CoEnableInteractable())));

            var score = removeCount * (bomb.Id == -1 ? 200 : 100);
            AddScore(score);
            SpawnPointEffect(bomb.transform.position, score);
        }

        private void SpawnPointEffect(Vector2 pos, int score)
        {
            var effectObj = Instantiate(pointEffectPrefab, pos, Quaternion.identity);
            var pointEffect = effectObj.GetComponent<PointEffect>();
            pointEffect.Show(score);
        }

        private void OnGameOver()
        {
            isGameOver = true;

            finishButton.SetActive(true);
            gameOverRoot.SetActive(true);

            gameObject.transform.DOScale(1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo);

            exchangeButton.enabled = false;

            var highScore = PlayerPrefs.GetInt("Score", 0);
            highScoreText.SetText(string.Format("High Score:{0}", highScore));
            if (highScore < currentScore)
            {
                PlayerPrefs.SetInt("Score", currentScore);
                PlayerPrefs.Save();
            }
        }

        public void OnExchangeBallAll()
        {
            var balls = GameObject.FindGameObjectsWithTag("Respawn");
            balls.ToList().ForEach(x => Destroy(x));
            StartCoroutine(ballGenerator.CoCreateDropBalls(50, () => StartCoroutine(CoEnableInteractable())));
        }

        public void OnTapChangeScene()
        {
            SceneManager.LoadSceneAsync("Title");
        }
    }
}