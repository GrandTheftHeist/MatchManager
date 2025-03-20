using System;
using CitizenFX.Core;

namespace RoundManager.Server
{
    internal class Main : BaseScript
    {
        private readonly Round round;

        public Main()
        {
            round = new Round(Guid.NewGuid(), DateTime.Now, DateTime.Now.AddSeconds(10));

            Tick += round.RoundTick;
        }

        [Command("round")]
        private void RoundPlayersAndSpectators()
        {
            Debug.WriteLine("Playing:");
            foreach (var player in Players)
            {
                if (player.State.Get("isPlaying") == true)
                {
                    Debug.WriteLine(player.Name);
                }
            }

            Debug.WriteLine("Spectating:");
            foreach (var player in Players)
            {
                if (player.State.Get("isSpectating") == true)
                {
                    Debug.WriteLine(player.Name);
                }
            }
        }
    }
}