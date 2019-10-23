using System.Threading.Tasks;
using UnityEngine;

namespace Sofq
{
    // Server is monobeh only for test, because we need update SpinsRenewInTime
    public class TestServer : MonoBehaviour, IServer
    {
        [SerializeField] private int Score = 1000;
        [SerializeField] private int MaxSpins = 50;
        [SerializeField] private int SpinsLeft = 30;
        [SerializeField] private int CurrentBet = 100;
        [SerializeField] private int SpinsRenewCount = 5;
        [SerializeField] private float SpinsRenewInTime = 15f;
        private SlotMachineModel currentModel;

        private void Awake()
        {
            currentModel = new SlotMachineModel
            {
                Score = Score, MaxSpins = MaxSpins, SpinsLeft = SpinsLeft, CurrentBet = CurrentBet,
                SpinsRenewCount = SpinsRenewCount, SpinsRenewInTime = (int) SpinsRenewInTime
            };
        }

        private void Update()
        {
            SpinsRenewInTime -= Time.deltaTime;
            if (SpinsRenewInTime <= 0)
            {
                SpinsLeft += SpinsRenewCount;
                SpinsRenewInTime = 15f;
            }
        }

        public async Task<string> DoSpin(int bet)
        {
            await Task.Delay(2000);

            var test = TestData(bet);
            currentModel = test;

            return JsonUtility.ToJson(test);
        }

        public async Task<string> GetCurrentStats()
        {
            await Task.Delay(1000);
            currentModel.SpinsRenewInTime = (int) SpinsRenewInTime;
            currentModel.CurrentBet = CurrentBet;
            currentModel.SpinsLeft = SpinsLeft;

            return JsonUtility.ToJson(currentModel);
        }

        public async void Restart()
        {
            // Reset all stats, not needed for test because we reload scene
        }

        private SlotMachineModel TestData(int bet)
        {
            CurrentBet = bet;
            SpinsLeft = SpinsLeft - 1 < 0 ? 0 : SpinsLeft - 1;
            var model = new SlotMachineModel();

            switch (SpinsLeft % 5)
            {
                case 4:
                    model.Slot1 = new SlotModel {WinningItemIndex = 3};
                    model.Slot2 = new SlotModel {WinningItemIndex = 2};
                    model.Slot3 = new SlotModel {WinningItemIndex = 0};
                    model.Score = Score -= bet;
                    break;
                case 3:
                    model.Slot1 = new SlotModel {WinningItemIndex = 1};
                    model.Slot2 = new SlotModel {WinningItemIndex = 1};
                    model.Slot3 = new SlotModel {WinningItemIndex = 3};
                    model.Score = Score -= bet;
                    break;
                case 2:
                    model.Slot1 = new SlotModel {WinningItemIndex = 2};
                    model.Slot2 = new SlotModel {WinningItemIndex = 3};
                    model.Slot3 = new SlotModel {WinningItemIndex = 1};
                    model.Score = Score -= bet;
                    break;
                case 1:
                    model.Slot1 = new SlotModel {WinningItemIndex = 4};
                    model.Slot2 = new SlotModel {WinningItemIndex = 4};
                    model.Slot3 = new SlotModel {WinningItemIndex = 4};
                    model.Score = Score += bet;
                    break;
                case 0:
                    model.Slot1 = new SlotModel {WinningItemIndex = 2};
                    model.Slot2 = new SlotModel {WinningItemIndex = 2};
                    model.Slot3 = new SlotModel {WinningItemIndex = 2};
                    model.Score = Score += bet;
                    break;
            }

            model.MaxSpins = MaxSpins;
            model.SpinsLeft = SpinsLeft;
            model.CurrentBet = CurrentBet;
            model.SpinsRenewCount = SpinsRenewCount;
            model.SpinsRenewInTime = (int) SpinsRenewInTime;
            currentModel = model;

            return model;
        }
    }
}