using System.Collections;
using UnityEngine;

namespace Pazukesi.Game
{
    public class BallGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject ballPrefab;
        [SerializeField] private Sprite[] ballSprites;
        [SerializeField] private Sprite bombSprite;

        public IEnumerator CoCreateDropBalls(int count, System.Action action)
        {
            if (count == 50) action.Invoke();

            for (var _ = 0; _ < count; _++)
            {
                var ball = Instantiate(ballPrefab, new Vector2(Random.Range(-0.2f, 0.2f), 8f), Quaternion.identity);
                var ballID = Random.Range(0, ballSprites.Length);
                if (Random.Range(0, 100) < 5)
                {
                    ballID = -1;
                    ball.GetComponent<SpriteRenderer>().sprite = bombSprite;
                }
                else
                {
                    ball.GetComponent<SpriteRenderer>().sprite = ballSprites[ballID];

                }
                ball.GetComponent<Ball>().Id = ballID;
                yield return new WaitForSeconds(0.04f);
            }
        }
    }
}