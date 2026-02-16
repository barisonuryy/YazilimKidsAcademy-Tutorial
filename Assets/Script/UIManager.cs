using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private Button restartButton;

    [SerializeField] private Image[] healths;
    [SerializeField] private Image health;
  
    public void SetState(string state)
    {
        stateText.text=state;
    }
    public void SetRestart()
    {
        restartButton.gameObject.SetActive(true);
    }
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
    public void SetHealths(int currentHealth)
    {
        for(int i = 0; i < healths.Length; i++)
        {
            bool isActive=i<=currentHealth;
            healths[i].gameObject.SetActive(isActive);
        }
    }
}
