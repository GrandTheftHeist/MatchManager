using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace MatchManager.Server
{
    internal class ServerMain : BaseScript
    {
        public ServerMain()
        {
            EventHandlers ["MATCHMANAGER_GOLDTROLLEY_ROB"] += new Action<Player, int, int>(Match.RobGoldTrolley);

            Match.Create(Guid.NewGuid(), DateTime.Now.AddMinutes(1), DateTime.Now.AddSeconds(1));
        }

        [Tick]
        internal async Task OnTick()
        {
            if(DateTime.Now < Match.EndTime)
            {
                foreach(var player in Players)
                {
                    if(player.State.Get("isPlaying") == true)
                    {
                        continue;
                    }

                    if(player.State.Get("isSpectating") == true)
                    {
                        player.State.Set("endTime", Match.EndTime, true);
                        continue;
                    }

                    if(player.State.Get("isPlaying") != true && player.State.Get("isSpectating") != true)
                    {
                        player.State.Set("isPlaying", false, true);
                        player.State.Set("isSpectating", true, true);
                    }
                }
            }

            if(DateTime.Now >= Match.EndTime)
            {
                Debug.WriteLine($"^6[DEBUG] {Match.EndTime}");
                Debug.WriteLine($"^5[INFO] Match '{Match.Id}' has ended.^7");
                Debug.WriteLine($"^5[INFO] Creating next match...^7");

                foreach(uint ped in API.GetAllPeds())
                {
                    if(API.IsPedAPlayer((int)ped))
                    {
                        continue;
                    }

                    API.DeleteEntity((int)ped);
                }

                TriggerEvent("BANK_SV_RESTART_BY_SERVER");
                TriggerEvent("BANK_SV_DOOR_STATE", 0, 1, true);
                TriggerEvent("BANK_SV_DOOR_STATE", 0, 2, true);
                TriggerEvent("BANK_SV_DOOR_STATE", 0, 3, true);

                Match.Create(Guid.NewGuid(), DateTime.Now, DateTime.Now.AddMinutes(5));

                foreach(var player in Players)
                {
                    player.State.Set("endTime", Match.EndTime, true);
                    player.State.Set("isPlaying", true, true);
                    player.State.Set("isSpectating", false, true);

                    player.TriggerEvent("CORE_CL_MATCH_STARTED");
                }
            }

            await Delay(1000);
        }
    }
}