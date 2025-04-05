using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

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

        #region Methods
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
                    player.State.Set("endTime", endTime, true);

                    player.State.Set("isInRound", true, true);
                    player.State.Set("isSpectating", false, true);

                    player.TriggerEvent("CORE_CL_ROUND_STARTED");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"\u001b[41;37m[ERROR] {ex.Message}^7");
            }
        }
        #endregion

        #region Tick
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

                this.CreateRound(DateTime.Now, DateTime.Now.AddMinutes(10));
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
                    if (player.State.Get("isInRound") == true)
                    {
                        continue;
                    }

                    if (player.State.Get("isSpectating") == true)
                    {
                        Debug.WriteLine($"Player '{player.Name}' is spectating. Updated EndTime to '{round.EndTime}'.");

                        player.State.Set("endTime", round.EndTime, true);

                        continue;
                    }

                    if (player.State.Get("isConnected") == true)
                    {
                        player.State.Set("isInRound", false, true);
                        player.State.Set("isSpectating", true, true);

                        player.TriggerEvent("CORE_CL_ROUND_SPECTATE");
                    }
                }
            }

            if (DateTime.Now >= this.round.EndTime)
            {
                Debug.WriteLine($"\u001b[44;37m[INFO] Round '{this.Id}' ended.^7");
                Debug.WriteLine("\u001b[44;37m[INFO] Selecting next round...^7");

                this.round = null;

                foreach (uint ped in API.GetAllPeds())
                {
                    // If ped is not a player, skip it.
                    if (!API.IsPedAPlayer((int)ped))
                    {
                        continue;
                    }

                    Debug.WriteLine($"Deleting ped '{ped}'.");

                    API.DeleteEntity((int)ped);
                }

                TriggerEvent("BANK_SV_RESTART_BY_SERVER");

                TriggerEvent("BANK_SV_DOOR_CLOSE_BY_SERVER", 0, 1);
                TriggerEvent("BANK_SV_DOOR_CLOSE_BY_SERVER", 0, 2);
                TriggerEvent("BANK_SV_DOOR_CLOSE_BY_SERVER", 0, 3);
                return;
            }
        }
        #endregion
    }
}