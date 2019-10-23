using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Sofq
{
    public class SlotMachineView : MonoBehaviour
    {
        [SerializeField] private Button spinButton;
        [SerializeField] private Button betButton;
        [SerializeField] private GameObject loserTable;
        [SerializeField] private TextMeshProUGUI scoreLabel;
        [SerializeField] private Image spinLeftSlider;
        [SerializeField] private TextMeshProUGUI spinLeftLabel;
        [SerializeField] private TextMeshProUGUI spinsRenewInLabel;

        public void AddSpinButtonListener(UnityAction listener)
        {
            spinButton.onClick.AddListener(listener);
        }

        public TextMeshProUGUI AddBetButtonListener(UnityAction listener)
        {
            betButton.onClick.AddListener(listener);
            return betButton.GetComponentInChildren<TextMeshProUGUI>();
        }

        public void AddRestartButtonListener(UnityAction listener)
        {
            loserTable.GetComponentInChildren<Button>().onClick.AddListener(listener);
        }

        public void ShowRestartButton(bool value)
        {
            loserTable.SetActive(value);
        }

        public void ShowRestartButtonAndScore(bool value)
        {
            loserTable.GetComponentInChildren<TextMeshProUGUI>().text =
                $"GAME OVER YOUR SCORE IS <color=\"red\">{scoreLabel.text}</color>";
            loserTable.SetActive(value);
        }

        public void SetScore(int value)
        {
            scoreLabel.text = $"{value}";
        }

        public void SetSpinLeft(int left, int max)
        {
            spinLeftSlider.fillAmount = left / (float) max;
            spinLeftLabel.text = $"{left}/{max}";
        }

        public void SetSpinsRenew(int spinsCount, int time)
        {
            spinsRenewInLabel.text = $"{spinsCount} spins in {time / 60}:{time % 60}";
        }
    }
}