
namespace GHelper.AutoTDP
{
    public class GameProfile
    {
        public string GameTitle { get; set; }
        public string ProcessName { get; set; }
        public int TargetFPS { get; set; }
        public int MinTdp { get; set; }
        public int MaxTdp { get; set; }
        public bool Enabled { get; set; }

        public int GetTDPFPS()
        {
            return TargetFPS - 1;
        }
    }
}
