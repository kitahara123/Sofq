using System;
using System.Collections;
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
        [SerializeField] private ScrollSnapBase slotScroll1;
        [SerializeField] private ScrollSnapBase slotScroll2;
        [SerializeField] private ScrollSnapBase slotScroll3;
        private TextMeshProUGUI betLabel;
        private IServer server;
        private SlotMachineModel model;

        private int stopSlot1At = -1;
        private int stopSlot2At = -1;
        private int stopSlot3At = -1;

        private bool isWaitForUpdate = false;

        private void Start()
        {
            server = GetComponentInChildren<TestServer>();
            view.AddSpinButtonListener(DoSpin);
            betLabel = view.AddBetButtonListener(ChangeBet);
            view.AddRestartButtonListener(Restart);
            UpdateCurrentStats();
            StartCoroutine(TimerUpdater());
        }

        private IEnumerator TimerUpdater()
        {
            yield return new WaitUntil(() => model != null);
            while (true)
            {
                if (model.SpinsRenewInTime <= 0)
                {
                    model.SpinsRenewInTime = 0;
                    UpdateCurrentStats();
                }
                else
                {
                    model.SpinsRenewInTime--;
                    view.SetSpinsRenew(model.SpinsRenewCount, model.SpinsRenewInTime);
                }

                yield return new WaitForSeconds(1);
            }
        }

        private async void UpdateCurrentStats()
        {
            if (isWaitForUpdate) return;
            isWaitForUpdate = true;
            var res = await server.GetCurrentStats();
            model = JsonUtility.FromJson<SlotMachineModel>(res);
            isWaitForUpdate = false;
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
            if (isWaitForUpdate) return;
            isWaitForUpdate = true;

            // Update this only for user to see, after taking info from server, we update it again 
            view.SetSpinLeft(model.SpinsLeft - 1, model.MaxSpins);

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
            isWaitForUpdate = false;
        }

        private void ChangeBet()
        {
            if (isWaitForUpdate) return;
            if (model.Score < betDelta) return;
            var betLimit = maxBet < model.Score ? maxBet : model.Score;
            var newBet = (model.CurrentBet + betDelta) % betLimit;
            model.CurrentBet = newBet == 0 ? betLimit : newBet;
            betLabel.text = $"Bet x{model.CurrentBet}";
        }

        private void Restart()
        {
            server.Restart();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void UpdateInterface()
        {
            view.SetScore(model.Score);
            view.SetSpinLeft(model.SpinsLeft, model.MaxSpins);
            view.SetSpinsRenew(model.SpinsRenewCount, model.SpinsRenewInTime);

            if (model.Score < maxBet && model.CurrentBet > model.Score)
            {
                model.CurrentBet = model.Score;
                betLabel.text = $"Bet x{model.CurrentBet}";
            }

            if (model.Score <= 0)
            {
                view.ShowRestartButton(true);
            }

            if (model.SpinsLeft <= 0)
            {
                view.ShowRestartButtonAndScore(true);
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