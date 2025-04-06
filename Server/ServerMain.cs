using System;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace RoundManager.Server
{
    public class ServerMain : BaseScript
    {
        public ServerMain()
        {
            Round.Create(Guid.NewGuid(), DateTime.Now.AddMinutes(1), DateTime.Now.AddSeconds(1));
        }

        [Tick]
        public async Task OnTick()
        {
            await Delay(1000);

            if (Players.Count() == 0)
            {
                Debug.WriteLine($"^3[WARNING] No Players online.^7");
                return;
            }

            if (DateTime.Now < Round.EndTime)
            {
                Debug.WriteLine($"^6[DEBUG] DateTime.Now < Round.EndTime...^7");

                foreach (var player in Players)
                {
                    if (player.State.Get("isPlaying") == true)
                    {
                        Debug.WriteLine($"^6[DEBUG] {player.Name} is playing {Round.StartTime} - {Round.EndTime}^7");

                        continue;
                    }

                    if (player.State.Get("isSpectating") == true)
                    {
                        Debug.WriteLine($"^6[DEBUG] {player.Name} is spectating {Round.StartTime} - {Round.EndTime}^7");

                        player.State.Set("endTime", Round.EndTime, true);

                        continue;
                    }

                    if (player.State.Get("isPlaying") != true && player.State.Get("isSpectating") != true)
                    {
                        Debug.WriteLine($"^6[DEBUG] {player.Name} is now spectating {Round.StartTime} - {Round.EndTime}^7");

                        player.State.Set("isPlaying", false, true);
                        player.State.Set("isSpectating", true, true);

                        player.TriggerEvent("CORE_CL_ROUND_SPECTATE");
                    }
                }
            }

            if (DateTime.Now >= Round.EndTime)
            {
                Debug.WriteLine($"^6[DEBUG] {Round.EndTime}");
                Debug.WriteLine($"^5[INFO] Round '{Round.Id}' ended.^7");
                Debug.WriteLine($"^5[INFO] Creating next round...^7");

                foreach (uint ped in API.GetAllPeds())
                {
                    if (API.IsPedAPlayer((int)ped))
                    {
                        continue;
                    }

                    API.DeleteEntity((int)ped);
                }

                TriggerEvent("BANK_SV_RESTART_BY_SERVER");
                TriggerEvent("BANK_SV_DOOR_CLOSE_BY_SERVER", 0, 1);
                TriggerEvent("BANK_SV_DOOR_CLOSE_BY_SERVER", 0, 2);
                TriggerEvent("BANK_SV_DOOR_CLOSE_BY_SERVER", 0, 3);

                Round.Create(Guid.NewGuid(), DateTime.Now, DateTime.Now.AddMinutes(10));

                foreach (var player in Players)
                {
                    player.State.Set("endTime", Round.EndTime, true);

                    player.State.Set("isPlaying", true, true);
                    player.State.Set("isSpectating", false, true);

                    player.TriggerEvent("CORE_CL_ROUND_STARTED");
                }
            }
        }
    }
}