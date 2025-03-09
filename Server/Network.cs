using System;
using System.Collections.Generic;
using CitizenFX.Core;

namespace RoundManager.Server
{
    internal static class Network
    {
        private static readonly List<Player> network = new List<Player>();
        private static int Count => network.Count;

        public static void Add(Player player)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            network.Add(player);
        }

        public static void Remove(Player player)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            network.Remove(player);
        }

        public static List<Player> Get()
        {
            return network;
        }

        public static int GetCount()
        {
            return Count;
        }
    }
}