using UnityEngine.InputSystem;

namespace Bonkers.Control
{
    public class KeyboardSetup
    {
        public bool keyboard1Taken { get; set; } = false;
        public bool keyboard2Taken { get; set; } = false;

        public Keyboard keyboard { get; set; }

        public KeyboardSetup(Keyboard keyboard)
        {
            this.keyboard = keyboard;
        }
    }
}
