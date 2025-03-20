using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace RoundManager.Client
{
    internal class Main : BaseScript
    {
        public Main()
        {
        }

        [EventHandler("ROUNDMANAGER_CL_SPECTATE")]
        private void OnSpectate()
        {
            Debug.WriteLine("Entering spectator mode.");

            API.NetworkSetInSpectatorMode(true, Game.Player.Handle);
        }
    }
}