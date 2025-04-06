using System;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace RoundManager.Server
{
    internal class Round : BaseScript
    {
        private Round currentRound;
        private Guid Id { get; set; }
        private DateTime StartTime { get; set; }
        private DateTime EndTime { get; set; }

        public Round(Guid id, DateTime startTime, DateTime endTime)
        {
            Id = id;
            StartTime = startTime;
            EndTime = endTime;
            currentRound = this;

            Debug.WriteLine($"^5[INFO] Round '{Id}' created. Start time '{StartTime}' End time '{EndTime}'^7");
        }

        private void CreateRound(DateTime startTime, DateTime endTime)
        {
            try
            {
                if (Players.Count() == 0)
                {
                    Debug.WriteLine($"^3[WARNING] No players online. Round not created.^7");
                    return;
                }

                currentRound = new Round(Guid.NewGuid(), startTime, endTime);

                foreach (var player in Players)
                {
                    player.State.Set("endTime", endTime, true);
                    player.State.Set("isPlaying", true, true);
                    player.State.Set("isSpectating", false, true);

                    player.TriggerEvent("CORE_CL_ROUND_STARTED");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"^1[ERROR] in CreateRound. Reason: {ex.Message}^7");
            }
        }

        [Tick]
        internal async Task RoundTick()
        {
            await Delay(1000);

            if (Players.Count() == 0)
            {
                return;
            }

            if (currentRound == null)
            {
                CreateRound(DateTime.Now, DateTime.Now.AddMinutes(10));
                return;
            }

            if (DateTime.Now == currentRound.StartTime)
            {
                /**
                 * Notify all teams that the round has started.
                 * Set the round as the current round.
                 */
                return;
            }

            if (DateTime.Now < currentRound.EndTime)
            {
                foreach (var player in Players)
                {
                    if (player.State.Get("isPlaying") == true)
                    {
                        continue;
                    }

                    if (player.State.Get("isSpectating") == true)
                    {
                        Debug.WriteLine($"Player '{player.Name}' is spectating. Updated EndTime to '{currentRound.EndTime}'");

                        player.State.Set("endTime", currentRound.EndTime, true);

                        continue;
                    }

                    if (player.State.Get("isConnected") == true)
                    {
                        player.State.Set("isInRound", false, true);
                        player.State.Set("isSpectating", true, true);

                        player.TriggerEvent("CORE_CL_ROUND_SPECTATE");
                    }
                }

                return;
            }

            if (DateTime.Now >= currentRound.EndTime)
            {
                Debug.WriteLine($"^5[INFO] Round '{Id}' ended.^7");
                Debug.WriteLine($"^5[INFO] Starting next round...^7");

                currentRound = null;

                foreach (uint ped in API.GetAllPeds())
                {
                    if (!API.IsPedAPlayer((int)ped))
                    {
                        continue;
                    }

                    API.DeleteEntity((int)ped);
                }

                TriggerEvent("BANK_SV_RESTART_BY_SERVER");
                TriggerEvent("BANK_SV_DOOR_CLOSE_BY_SERVER", 0, 1);
                TriggerEvent("BANK_SV_DOOR_CLOSE_BY_SERVER", 0, 2);
                TriggerEvent("BANK_SV_DOOR_CLOSE_BY_SERVER", 0, 3);

                return;
            }
        }
    }
}