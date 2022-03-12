using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class PointEffect : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI point;

    public void Show(int score)
    {
        point.text = string.Format("{0}", score);
        StartCoroutine(CoMovePointEffectUp());
    }

    private IEnumerator CoMovePointEffectUp()
    {
        yield return new WaitForSeconds(1);
        this.transform.DOMoveY(2, 0.7f).OnComplete(() => Destroy(gameObject, 0.2f));
    }
}