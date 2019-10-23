
using System;

namespace Sofq
{
    [Serializable]
    public class SlotMachineModel
    {
        public int Score;
        public int SpinsLeft;
        public int MaxSpins;
        public int CurrentBet;
        
        public SlotModel Slot1;
        public SlotModel Slot2;
        public SlotModel Slot3;

    }
}