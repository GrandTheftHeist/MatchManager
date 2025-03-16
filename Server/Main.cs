using System;
using System.Reflection;
using CitizenFX.Core;

namespace RoundManager.Server
{
    internal class Main : BaseScript
    {
        public Main()
        {
            var round = new Round(Guid.NewGuid(), DateTime.Now, DateTime.Now.AddSeconds(30));

            Tick += round.RoundTick;
        }

        #region EventHandlers
        [EventHandler("playerJoining")]
        private void OnPlayerJoining([FromSource] Player player)
        {
            try
            {
                if (player == null)
                {
                    throw new ArgumentNullException(nameof(player));
                }

                Network.Add(player);
                Debug.WriteLine($"\u001b[45;37m[INFO] Player '{player.Name}' is added to the network.^7");
            }
            catch (ArgumentNullException ex)
            {
                Debug.WriteLine($"\"\\u001b[41;37m[ERROR] OnPlayerJoining. {ex.Message}");
                throw;
            }
        }

        [EventHandler("playerDropped")]
        private void OnPlayerDropped([FromSource] Player player, string reason)
        {
            try
            {
                if (player == null)
                {
                    throw new ArgumentNullException(nameof(player));
                }

                Network.Remove(player);
                Debug.WriteLine($"\u001b[45;37m[INFO] Player '{player.Name}' is removed from the network. Reason '{reason}'^7");
            }
            catch (ArgumentNullException ex)
            {
                Debug.WriteLine($"\"\\u001b[41;37m[ERROR] OnPlayerDropped. {ex.Message}");
                throw;
            }
        }
        #endregion

        #region Commands
        [Command("listnetwork")]
        private void ListNetwork()
        {
            if (Network.GetCount() == 0)
            {
                Debug.WriteLine("\u001b[45;37m[INFO] dead network.^7");
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