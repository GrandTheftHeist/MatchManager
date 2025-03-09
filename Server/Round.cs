using System;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace RoundManager.Server
{
    internal class Round : BaseScript
    {
        private Guid Id { get; set; }
        private DateTime StartTime { get; set; }
        private DateTime EndTime { get; set; }

        private Round CurrentRound { get; set; }

        public Round(Guid id, DateTime startTime, DateTime endTime)
        {
            Id = id;
            StartTime = startTime;
            EndTime = endTime;

            Debug.WriteLine($"Round '{Id}' created. Start time '{StartTime}', end time '{EndTime}'.");
        }

        #region Tick
        [Tick]
        public async Task RoundTick()
        {
            await Delay(5000);

            if (Network.GetCount() == 0)
            {
                Debug.WriteLine("No players in the network.");

                /**
                 * If there are no players in the network, we can skip the round check.
                 */
                return;
            }

            if (CurrentRound == null)
            {
                Debug.WriteLine("No active round.");

                /**
                 * Create a new round.
                 * Set the new round as the current round.
                 */

                CreateRound(DateTime.Now, DateTime.Now.AddMinutes(5));
                return;
            }

            if (DateTime.Now == CurrentRound.StartTime)
            {
                Debug.WriteLine("Round has started.");

                /**
                 * Notify all teams that the round has started.
                 * Set the round as the current round.
                 */
                return;
            }

            if (DateTime.Now < CurrentRound.EndTime)
            {
                Debug.WriteLine("Round is still active.");

                /**
                 * Notify all citizens that the round is still active.
                 */
                return;
            }

            if (DateTime.Now >= CurrentRound.EndTime)
            {
                Debug.WriteLine("Round has ended.");

                /**
                 * Calculate the round results.
                 * Create a new round.
                 * Set the new round as the current round.
                 */

                CreateRound(DateTime.Now, DateTime.Now.AddMinutes(5));
                return;
            }
        }
        #endregion

        #region Methods
        private Round CreateRound(DateTime startTime, DateTime endTime)
        {
            CurrentRound = new Round(Guid.NewGuid(), startTime, endTime);
            return CurrentRound;
        }
        #endregion
    }
}