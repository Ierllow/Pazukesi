using UnityEngine;

namespace Pazukesi.Game
{
    public class Ball : MonoBehaviour
    {
        public int Id { get; set; }

        public void OnDestroy()
        {
            Destroy(gameObject);
        }
    }
}