using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Sofq
{
    public class TestServer : IServer
    {
        private int Score = 1000;
        private int MaxSpins = 50;
        private int SpinsLeft = 30;
        private int CurrentBet = 100;

        public async Task<string> DoSpin(int bet)
        {
            await Task.Delay(2000);

            var test = TestData(bet);

            return JsonUtility.ToJson(test);
        }

        private SlotMachineModel TestData(int bet)
        {
            CurrentBet = bet;
            --SpinsLeft;
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

            return model;
        }
    }
}