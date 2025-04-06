using System;
using CitizenFX.Core;

namespace RoundManager.Server
{
    internal class Round : BaseScript
    {
        internal static Guid Id;
        internal static DateTime StartTime;
        internal static DateTime EndTime;

        internal static void Create(Guid id, DateTime startTime, DateTime endTime)
        {
            Id = id;
            StartTime = startTime;
            EndTime = endTime;

            Debug.WriteLine($"^5[INFO] Round '{Id}' created. Start time '{StartTime}' End time '{EndTime}'^7");
        }
    }
}