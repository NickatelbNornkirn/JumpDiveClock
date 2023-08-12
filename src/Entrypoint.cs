namespace JumpDiveClock
{
    public class Entrypoint
    {
        public static void Main(string[] args)
        {
            var app = new App();

            Result r = app.Init();

            if (!r.Success)
            {
                Console.WriteLine("Failed to read initialize app.");
                Console.WriteLine(r.Error);
            }
            app.Loop();
            app.Exit();
        }
    }
}