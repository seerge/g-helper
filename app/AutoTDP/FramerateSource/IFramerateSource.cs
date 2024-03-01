namespace GHelper.AutoTDP.FramerateSource
{

    internal class GameInstance
    {
        public string? ProcessName { get; set; }

        public int ProcessID { get; set; }
    }

    internal interface IFramerateSource
    {
        public double GetFramerate(string processName);

        public List<GameInstance> GetRunningGames();
    }
}
