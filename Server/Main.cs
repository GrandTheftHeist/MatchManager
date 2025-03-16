using System;
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
    }
}