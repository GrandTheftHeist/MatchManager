using System;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace RoundManager.Server
{
    internal class Round : BaseScript
    {
        private Round round;

        private Guid Id { get; set; }
        private DateTime StartTime { get; set; }
        private DateTime EndTime { get; set; }

        public Round(Guid id, DateTime startTime, DateTime endTime)
        {
            this.Id = id;
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.round = this;

            Debug.WriteLine($"\u001b[44;37m[INFO] Round '{Id}' created. Start time '{StartTime}', end time '{EndTime}'^7");
        }

        #region Tick
        [Tick]
        internal async Task RoundTick()
        {
            await Delay(1000);

            if (this.round == null)
            {
                /**
                 * Create a new round.
                 * Set the new round as the current round.
                 */

                this.CreateRound(DateTime.Now, DateTime.Now.AddMinutes(2));
                return;
            }

            if (DateTime.Now == this.round.StartTime)
            {
                /**
                 * Notify all teams that the round has started.
                 * Set the round as the current round.
                 */
                return;
            }

            if (DateTime.Now < this.round.EndTime)
            {
                /**
                 * Notify all citizens that the round is in progress.
                 */
                return;
            }

            if (DateTime.Now >= this.round.EndTime)
            {
                Debug.WriteLine($"\u001b[44;37m[INFO] Round '{this.Id}' ended.^7");
                Debug.WriteLine("\u001b[44;37m[INFO] Selecting next round...^7");
                this.round = null;
                return;
            }
        }
        #endregion

        #region Methods
        private void CreateRound(DateTime startTime, DateTime endTime)
        {
            this.round = new Round(Guid.NewGuid(), startTime, endTime);

            foreach (var player in Players)
            {
                player.TriggerEvent("CORE_CL_ROUND_STARTED");
            }
        }
        #endregion
    }
}