using TMPro;
using UnityEngine;

public class GameUIBehaviour : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text grazeText;

    void Update()
    {
           scoreText.text = $"Score: {GameManager.Instance.Score}";
           grazeText.text = $"Graze: {GameManager.Instance.GrazeCount}";
    }
}
