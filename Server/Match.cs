using System;
using CitizenFX.Core;

namespace MatchManager.Server
{
    internal static class Match
    {
        internal static Guid Id;
        internal static DateTime StartTime;
        internal static DateTime EndTime;

        internal static void Create(Guid id, DateTime startTime, DateTime endTime)
        {
            Id = id;
            StartTime = startTime;
            EndTime = endTime;

            Debug.WriteLine($"^5[INFO] Match '{Id}' has been created. Start time '{StartTime}' End time '{EndTime}'^7");
        }
    }
}