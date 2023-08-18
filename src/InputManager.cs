namespace JumpDiveClock
{
    public class InputManager
    {
        private readonly Keybindings _keybindings;
        // Used for checking the last time a key was pressed.
        private double _timer;

        public InputManager(Keybindings keybindings)
        {
            _keybindings = keybindings;
        }

        public void Update(float delta)
        {

        }
    }
}