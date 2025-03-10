using System;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace RoundManager.Server
{
    internal class Round : BaseScript
    {
        private Round currentRound;

        private Guid Id { get; set; }
        private DateTime StartTime { get; set; }
        private DateTime EndTime { get; set; }

        public Round(Guid id, DateTime startTime, DateTime endTime)
        {
            Id = id;
            StartTime = startTime;
            EndTime = endTime;

            currentRound = this;

            Debug.WriteLine($"Round '{Id}' created. Start time '{StartTime}', end time '{EndTime}'.");
        }

        #region Tick
        [Tick]
        public async Task RoundTick()
        {
            await Delay(1000);

            if (currentRound == null)
            {
                /**
                 * Create a new round.
                 * Set the new round as the current round.
                 */

                CreateRound(DateTime.Now, DateTime.Now.AddSeconds(30));
                return;
            }

            if (DateTime.Now == currentRound.StartTime)
            {
                Debug.WriteLine("Round has started.");

                /**
                 * Notify all teams that the round has started.
                 * Set the round as the current round.
                 */
                return;
            }

            if (DateTime.Now < currentRound.EndTime)
            {
                Debug.WriteLine("Round is in progress.");

                /**
                 * Notify all citizens that the round is in progress.
                 */
                return;
            }

            if (DateTime.Now >= currentRound.EndTime)
            {
                Debug.WriteLine("Round has ended.");
                Debug.WriteLine("Calculating round results...");
                currentRound = null;
                Debug.WriteLine("Selecting next round...");
                Debug.WriteLine("currentRound is null.");
                return;
            }
        }
        #endregion

        #region Methods
        public Round GetRound()
        {
            return currentRound;
        }

        private Round CreateRound(DateTime startTime, DateTime endTime)
        {
            currentRound = new Round(Guid.NewGuid(), startTime, endTime);

            Network.Get().ForEach((player) =>
            {
                player.TriggerEvent("CORE_CL_ROUND_STARTED");
            });

            return currentRound;
        }
        #endregion
    }
}