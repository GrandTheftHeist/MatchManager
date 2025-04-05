using System;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace RoundManager.Server
{
    internal class Main : BaseScript
    {
        private readonly Round round;

        public Main()
        {
            round = new Round(Guid.NewGuid(), DateTime.Now, DateTime.Now.AddSeconds(1));

            Tick += round.RoundTick;
        }

        #region EventHandlers
        [EventHandler("playerConnecting")]
        private void OnPlayerConnecting([FromSource] Player player)
        {
            player.State.Set("isConnected", false, true);

            Debug.WriteLine($"Player '{player.Name}' connecting. State '{player.State.Get("isConnected")}'");
        }

        [EventHandler("ROUNDMANAGER_SV_CONNECTED")]
        private void OnConnected([FromSource] Player player)
        {
            _ = ConnectAsync(player);
        }
        #endregion

        #region Methods
        private async Task ConnectAsync(Player player)
        {
            await Delay(6000);

            player.State.Set("isConnected", true, true);

            Debug.WriteLine($"Player '{player.Name}' connected. State '{player.State.Get("isConnected")}'");
        }
        #endregion

        #region Commands
        [Command("round")]
        private void RoundPlayersAndSpectators()
        {
            Debug.WriteLine("Playing:");
            foreach (var player in Players)
            {
                if (player.State.Get("isInRound") == true)
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
        #endregion
    }
}