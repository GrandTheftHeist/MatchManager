using System;
using CitizenFX.Core;

namespace RoundManager.Server
{
    internal class Round : BaseScript
    {
        internal static Guid Id { get; set; }
        internal static DateTime StartTime { get; set; }
        internal static DateTime EndTime { get; set; }

        internal static void Create(Guid id, DateTime startTime, DateTime endTime)
        {
            Id = id;
            StartTime = startTime;
            EndTime = endTime;

            Debug.WriteLine($"^5[INFO] Round '{Id}' created. Start time '{StartTime}' End time '{EndTime}'^7");
        }
    }
}