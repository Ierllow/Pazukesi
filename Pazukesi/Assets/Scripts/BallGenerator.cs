using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallGenerator : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Sprite[] ballSprites;
    [SerializeField] private Sprite bombSprite;
    [SerializeField] private Button exchnageButton;

    public readonly int createLotbumbNum = 5;

    public IEnumerator CoCreateDropBalls(int count)
    {
        if (count == 50)
        {
            StartCoroutine(CoEnableInteractable()) ;
        }

        for (int i = 0; i < count; i++)
        {
            GameObject ball = Instantiate(ballPrefab, new Vector2(Random.Range(-0.2f, 0.2f), 8f), Quaternion.identity);
            int ballID = Random.Range(0, ballSprites.Length);
            if (Random.Range(0, 100) < createLotbumbNum)
            {
                ballID = -1;
                ball.GetComponent<SpriteRenderer>().sprite = bombSprite;
            }
            else
            {
                ball.GetComponent<SpriteRenderer>().sprite = ballSprites[ballID];

            }
            ball.GetComponent<Ball>().id = ballID;
            yield return new WaitForSeconds(0.04f);
        }
    }

    private IEnumerator CoEnableInteractable()
    {
        exchnageButton.interactable = false;
        yield return new WaitForSeconds(5.0f);
        exchnageButton.interactable = true;
    }
}