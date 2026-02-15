using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stateText;
  
    public void SetState(string state)
    {
        stateText.text=state;
    }
}
