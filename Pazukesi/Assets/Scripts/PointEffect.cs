using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Pazukesi.Game
{
    public class PointEffect : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI point;

        public void Show(int score)
        {
            point.SetText(string.Format("{0}", score));
            StartCoroutine(CoMovePointEffectUp());
        }

        private IEnumerator CoMovePointEffectUp()
        {
            yield return new WaitForSeconds(1);
            transform.DOMoveY(2, 0.7f).OnComplete(() => Destroy(gameObject, 0.2f));
        }
    }
}