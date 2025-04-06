using System;
using CitizenFX.Core;

namespace RoundManager.Server
{
    internal class ServerMain : BaseScript
    {
        private readonly Round round;

        public ServerMain()
        {
            round = new Round(Guid.NewGuid(), DateTime.Now, DateTime.Now.AddSeconds(1));

            Tick += round.RoundTick;
        }
    }
}