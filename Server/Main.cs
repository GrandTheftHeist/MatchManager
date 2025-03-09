using System;
using System.Reflection;
using CitizenFX.Core;

namespace RoundManager.Server
{
    internal class Main : BaseScript
    {
        public Main()
        {
            var currentRound = new Round(Guid.NewGuid(), DateTime.Now, DateTime.Now.AddMinutes(5));

            Tick += currentRound.RoundTick;
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
                Debug.WriteLine("dead network.");
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
        #endregion
    }
}