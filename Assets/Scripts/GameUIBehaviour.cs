using TMPro;
using UnityEngine;

public class GameUIBehaviour : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;

    void Update()
    {
           scoreText.text = $"Score: {GameManager.Instance.Score}";
    }
}
