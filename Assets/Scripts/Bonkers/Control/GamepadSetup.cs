using UnityEngine.InputSystem;

namespace Bonkers.Control
{
    public class GamepadSetup
    {
        public Gamepad gamepad { get; set; }
        public GamepadSetup(Gamepad gamepad)
        {
            this.gamepad = gamepad;
        }
    }
}
