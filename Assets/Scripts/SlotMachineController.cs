using System;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Task = System.Threading.Tasks.Task;

namespace Sofq
{
    public class SlotMachineController : MonoBehaviour
    {
        [Tooltip("Delay between first, second and third slots to stop scrolling")] [SerializeField]
        private int delayBetweenSlots = 2000;

        [SerializeField] private int betDelta = 100;
        [SerializeField] private int maxBet = 500;
        [SerializeField] private SlotMachineView view;
        [SerializeField] private SlotMachineModel model = new SlotMachineModel();
        [SerializeField] private ScrollSnapBase slotScroll1;
        [SerializeField] private ScrollSnapBase slotScroll2;
        [SerializeField] private ScrollSnapBase slotScroll3;
        private TextMeshProUGUI betLabel;
        private IServer server = new TestServer();

        private int stopSlot1At = -1;
        private int stopSlot2At = -1;
        private int stopSlot3At = -1;

        private bool isSpin = false;

        private void Start()
        {
            view.AddSpinButtonListener(DoSpin);
            betLabel = view.AddBetButtonListener(ChangeBet);
            view.AddRestartButtonListener(Restart);
            UpdateInterface();
        }

        private void FixedUpdate()
        {
            if (stopSlot1At > slotScroll1.TargetPage)
            {
                slotScroll1.NextScreen();
            }

            if (stopSlot2At > slotScroll2.TargetPage)
            {
                slotScroll2.NextScreen();
            }

            if (stopSlot3At > slotScroll3.TargetPage)
            {
                slotScroll3.NextScreen();
            }
        }

        private async void DoSpin()
        {
            if (isSpin) return;
            isSpin = true;
            ToggleSpinAndWait(true);
            var result = await server.DoSpin(model.CurrentBet);
            model = JsonUtility.FromJson<SlotMachineModel>(result);
//            await Task.Run(() => server.DoSpin());

            stopSlot1At = GetPageIndexBySlotIndex(slotScroll1, model.Slot1.WinningItemIndex);
            await Task.Delay(delayBetweenSlots);
            stopSlot2At = GetPageIndexBySlotIndex(slotScroll2, model.Slot2.WinningItemIndex);
            await Task.Delay(delayBetweenSlots);
            stopSlot3At = GetPageIndexBySlotIndex(slotScroll3, model.Slot3.WinningItemIndex);

            UpdateInterface();
            isSpin = false;
        }

        private void ChangeBet()
        {
            if (model.Score < betDelta) return;
            var betLimit = maxBet < model.Score ? maxBet : model.Score;
            var newBet = (model.CurrentBet + betDelta) % betLimit;
            model.CurrentBet = newBet == 0 ? betLimit : newBet;
            betLabel.text = $"Bet x{model.CurrentBet}";
        }

        private void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void UpdateInterface()
        {
            view.SetScore(model.Score);
            view.SetSpinLeft(model.SpinsLeft, model.MaxSpins);
            view.SetSpinsRenew(4, 1725);
            if (model.Score < maxBet)
            {
                model.CurrentBet = model.Score;
                betLabel.text = $"Bet x{model.CurrentBet}";
            }

            if (model.Score <= 0 || model.SpinsLeft <= 0)
            {
                view.ShowRestartButton(true);
            }
        }

        private void ToggleSpinAndWait(bool enable = false)
        {
            if (enable)
            {
                stopSlot1At = int.MaxValue;
                stopSlot2At = int.MaxValue;
                stopSlot3At = int.MaxValue;
            }
            else
            {
                stopSlot1At = -1;
                stopSlot2At = -1;
                stopSlot3At = -1;
            }
        }

        private int GetPageIndexBySlotIndex(ScrollSnapBase slotScroll, int itemIndex)
        {
            if (itemIndex < 0) return 0;

            int stopAtIndex;
            var currentSlotIndex = slotScroll.TargetPage % slotScroll._screens;

            if (currentSlotIndex <= itemIndex)
            {
                stopAtIndex = slotScroll.TargetPage - currentSlotIndex + itemIndex;
            }
            else
            {
                stopAtIndex = slotScroll.TargetPage - currentSlotIndex + slotScroll._screens + itemIndex;
            }

            return stopAtIndex;
        }
    }
}