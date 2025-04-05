using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace RoundManager.Client
{
    internal class Main : BaseScript
    {
        public Main()
        {
            TriggerServerEvent("ROUNDMANAGER_SV_CONNECTED", Game.Player.Handle);
        }
    }
}