using System;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace RoundManager.Server
{
    internal class Round : BaseScript
    {
        private Round round;

        private Guid Id { get; set; }
        private DateTime StartTime { get; set; }
        private DateTime EndTime { get; set; }

        public Round(Guid id, DateTime startTime, DateTime endTime)
        {
            this.Id = id;
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.round = this;

            Debug.WriteLine($"\u001b[44;37m[INFO] Round '{Id}' created. Start time '{StartTime}', end time '{EndTime}'^7");
        }

        private void CreateRound(DateTime startTime, DateTime endTime)
        {
            try
            {
                this.round = new Round(Guid.NewGuid(), startTime, endTime);

                var players = Players;
                if (players == null)
                {
                    return;
                }

                foreach (var player in Players)
                {
                    player.State.Set("isPlaying", true, true);
                    player.State.Set("isSpectating", false, true);

                    player.TriggerEvent("CORE_CL_ROUND_STARTED");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"\u001b[41;37m[ERROR] {ex.Message}^7");
            }
        }

        [Tick]
        internal async Task RoundTick()
        {
            await Delay(1000);

            var players = Players;
            if (players == null)
            {
                return;
            }

            if (this.round == null)
            {
                /**
                 * Create a new round.
                 * Set the new round as the current round.
                 */

                this.CreateRound(DateTime.Now, DateTime.Now.AddMinutes(5));
                return;
            }

            if (DateTime.Now == this.round.StartTime)
            {
                /**
                 * Notify all teams that the round has started.
                 * Set the round as the current round.
                 */
                return;
            }

            if (DateTime.Now < this.round.EndTime)
            {
                /**
                 * Notify all citizens that the round is in progress.
                 */

                foreach (var player in players)
                {
                    if (player.State.Get("isPlaying") == true)
                    {
                        continue;
                    }

                    if (player.State.Get("isSpectating") == true)
                    {
                        continue;
                    }

                    player.TriggerEvent("ROUNDMANAGER_CL_SPECTATE");

                    player.State.Set("isPlaying", false, true);
                    player.State.Set("isSpectating", true, true);
                }

                return;
            }

            if (DateTime.Now >= this.round.EndTime)
            {
                Debug.WriteLine($"\u001b[44;37m[INFO] Round '{this.Id}' ended.^7");
                Debug.WriteLine("\u001b[44;37m[INFO] Selecting next round...^7");

                this.round = null;

                TriggerEvent("BANK_SV_DOOR_CLOSE_BY_SERVER", 0, 1);
                TriggerEvent("BANK_SV_DOOR_CLOSE_BY_SERVER", 0, 2);
                TriggerEvent("BANK_SV_DOOR_CLOSE_BY_SERVER", 0, 3);

                return;
            }
        }
    }
}