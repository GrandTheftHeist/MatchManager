using System;
using System.Collections.Generic;
using CitizenFX.Core;

namespace RoundManager.Server
{
    internal class Network
    {
        private static readonly List<Player> network = new List<Player>();

        #region Methods
        internal static void Add(Player player)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            network.Add(player);
        }

        internal static void Remove(Player player)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            network.Remove(player);
        }

        internal static List<Player> Get()
        {
            return network;
        }

        internal static int GetCount()
        {
            return network.Count;
        }
        #endregion
    }
}