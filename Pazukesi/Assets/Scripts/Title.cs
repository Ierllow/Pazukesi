using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pazukesi.Title
{
    public class Title : MonoBehaviour
    {
        public void OnTapStart()
        {
            SceneManager.LoadSceneAsync("Main");
        }
    }
}