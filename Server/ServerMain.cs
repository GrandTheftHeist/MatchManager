using System;
using System.Reflection;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace RoundManager.Server
{
    internal class ServerMain : BaseScript
    {
        private readonly Round round;

        public ServerMain()
        {
            round = new Round(Guid.NewGuid(), DateTime.Now, DateTime.Now.AddMinutes(5));

            Tick += RoundTick;
        }

        private async Task RoundTick()
        {
            await round.RoundTick();
        }

        #region EventHandlers
        [EventHandler("playerJoining")]
        private void OnPlayerJoining([FromSource] Player player)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            Network.Add(player);
            Debug.WriteLine($"Player '{player.Name}' is added to the network.");
        }

        [EventHandler("playerDropped")]
        private void OnPlayerDropped([FromSource] Player player, string reason)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            Network.Remove(player);
            Debug.WriteLine($"Player '{player.Name}' is removed from the network. Reason '{reason}'.");
        }
        #endregion

        #region Commands
        [Command("listnetwork")]
        private void ListNetwork()
        {
            if (Network.GetCount() == 0)
            {
                Debug.WriteLine("No players in the network.");
                return;
            }

            foreach (Player player in Network.Get())
            {
                foreach (PropertyInfo property in player.GetType().GetProperties())
                {
                    try
                    {
                        var value = property.GetValue(player, null);
                        if (property.Name == "Identifiers" && value is IdentifierCollection identifiers)
                        {
                            Debug.WriteLine($"Identifiers: {string.Join(", ", identifiers)}");
                        }
                        else if (property.Name == "Character" && value is Ped character)
                        {
                            Debug.WriteLine($"Character: {character}");
                        }
                        else
                        {
                            Debug.WriteLine($"{property.Name}: {value}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"{property.Name}: Error retrieving value - {ex.Message}");
                    }
                }
            }
        }

        [Command("listround")]
        private void ListRound()
        {
            if (round == null)
            {
                Debug.WriteLine("No active round.");
                return;
            }

            foreach (PropertyInfo property in round.GetType().GetProperties())
            {
                try
                {
                    var value = property.GetValue(round, null);
                    Debug.WriteLine($"{property.Name}: {value}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{property.Name}: Error retrieving value - {ex.Message}");
                }
            }
        }
        #endregion
    }
}