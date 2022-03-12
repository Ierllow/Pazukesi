using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;

public class GameController : MonoBehaviour
{
    [SerializeField] private BallGenerator ballGenerator;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI restTextText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private GameObject gameOverText;
    [SerializeField] private GameObject finishButton;
    [SerializeField] private Button ExchangeButton;
    [SerializeField] private GameObject pointEffectPrefab;

    private List<Ball> removeBalls = new List<Ball>();

    private Ball currentDraggingBall;
    private bool isDragging;
    private int currentScore = 0;

    private bool isGameOver = false;

    private readonly int firstCreateBallNum = 50;

    private void Awake()
    {
        finishButton.SetActive(false);
        gameOverText.SetActive(false);
    }

    private void Start()
    {
        AddScore(currentScore);
        StartCoroutine(ballGenerator.CoCreateDropBalls(firstCreateBallNum));
        StartCoroutine(CoCountDown());
    }

    private void Update()
    {
        if (isGameOver)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            OnDragBegin();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            OnDragEnd();
        }
        else if (isDragging)
        {
            OnDragging();
        }
    }

    private IEnumerator CoCountDown()
    {
        int restTime = 60;
        while (restTime > 0)
        {
            yield return new WaitForSeconds(1);
            restTime--;
            restTextText.text = string.Format("{0}", restTime);
        }
        Debug.Log("時間切れ");
        GameOver();
    }

    private void AddScore(int point)
    {
        currentScore += point;
        scoreText.text = string.Format("Score:{0}", currentScore);
    }

    private void OnDragBegin()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
        if (hit && hit.collider.GetComponent<Ball>())
        {
            Ball ball = hit.collider.GetComponent<Ball>();
            if (ball.IsBomb())
            {
                Bomb(ball);
            }
            else
            {
                AddRemoveBall(ball);
                isDragging = true;
            }
        }
    }

    private void OnDragging()
    {
        Debug.Log("ドラッグ中");
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
        if (hit && hit.collider.GetComponent<Ball>())
        {
            Ball ball = hit.collider.GetComponent<Ball>();

            if (ball.id == currentDraggingBall.id)
            {
                float distance = Vector2.Distance(ball.transform.position, currentDraggingBall.transform.position);
                if (distance < 1.5)
                {
                    AddRemoveBall(ball);
                }
            }
        }
    }

    private void OnDragEnd()
    {
        int removeCount = removeBalls.Count;

        if (removeCount >= 3)
        {
            removeBalls.ForEach(x => x.OnDestory());
            StartCoroutine(ballGenerator.CoCreateDropBalls(removeCount));
            int score = removeCount * 100;
            AddScore(score);
            SpawnPointEffect(removeBalls.Last(x => x).transform.position, score);
            removeBalls.ForEach(x => x.transform.DOScale(Vector3.one, 1.7f));
        }
        else
        {
            removeBalls.ForEach(x => x.transform.localScale = Vector3.one);
        }
        removeBalls.Clear();
        isDragging = false;
    }

    private void AddRemoveBall(Ball ball)
    {
        currentDraggingBall = ball;
        if (!removeBalls.Contains(ball))
        {
            ball.transform.DOScale(new Vector3(1.4f, 1.4f, 1.4f), 0.3f);
            removeBalls.Add(ball);
        }
    }

    private void Bomb(Ball bomb)
    {
        Collider2D[] hitObjArray = Physics2D.OverlapCircleAll(bomb.transform.position, 2);

        Ball ball;
        List<Ball> bombList = new List<Ball>();
        hitObjArray.ToList().ForEach(
            x =>
            {
                if (x.TryGetComponent(out ball)) bombList.Add(ball);
            });
        bombList.ForEach(
            x =>
            {
                x.OnDestory();
            });
        int removeCount = bombList.Count;

        StartCoroutine(ballGenerator.CoCreateDropBalls(removeCount));

        int score = removeCount * 100;
        AddScore(score);
        SpawnPointEffect(bomb.transform.position, score);
    }

    private void SpawnPointEffect(Vector2 pos, int score)
    {
        GameObject effectObj = Instantiate(pointEffectPrefab, pos, Quaternion.identity);
        PointEffect pointEffect = effectObj.GetComponent<PointEffect>();
        pointEffect.Show(score);
    }

    private void GameOver()
    {
        isGameOver = true;

        finishButton.SetActive(true);
        gameOverText.SetActive(true);

        gameObject.transform.DOScale(1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo);

        ExchangeButton.enabled = false;

        var highScore = PlayerPrefs.GetInt("Score", 0);
        highScoreText.text = string.Format("High Score:{0}", highScore);
        if (highScore < currentScore)
        {
            PlayerPrefs.SetInt("Score", currentScore);
            PlayerPrefs.Save();
        }
    }

    public void OnAllExchangeBalls()
    {
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Respawn");
        balls.ToList().ForEach(x => Destroy(x));
        StartCoroutine(ballGenerator.CoCreateDropBalls(firstCreateBallNum));
    }

    public void OnTapChangeScene()
    {
        SceneManager.LoadScene("Title");
    }
}