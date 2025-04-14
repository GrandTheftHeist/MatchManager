using System;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace MatchManager.Server
{
    internal class Object
    {
        internal int Id;
        internal string Model;
        internal Vector3 Location;
        internal float Heading;

        internal Object(string model, Vector3 location, float heading = 0f)
        {
            Id = API.CreateObject((int)API.GetHashKey(model), location.X, location.Y, location.Z, true, true, false);
            Model = model;
            Location = location;
            Heading = heading;

            API.SetEntityHeading(Id, Heading);
        }
    }

    internal class GoldTrolley
    {
        internal int Id;
        internal int Value;
        internal bool IsRobbed;
        internal Vector3 Location;
        internal float Heading;

        internal GoldTrolley(int value, Vector3 location, float heading)
        {
            Id = API.CreateObject((int)API.GetHashKey("prop_gold_trolly_full"), location.X, location.Y, location.Z, true, true, false);
            Value = value;
            IsRobbed = false;
            Location = location;
            Heading = heading;

            API.SetEntityHeading(Id, Heading);
        }
    }

    internal static class Match
    {
        internal static Guid Id;
        internal static DateTime StartTime;
        internal static DateTime EndTime;
        internal static List<GoldTrolley> GoldTrolleys = new List<GoldTrolley>();
        internal static List<Object> Objects = new List<Object>();

        internal static void Create(Guid id, DateTime startTime, DateTime endTime)
        {
            Id = id;
            StartTime = startTime;
            EndTime = endTime;

            DeleteExistingGoldTrolleys();
            DeleteExistingObjects();

            GoldTrolleys.Add(new GoldTrolley(2_250_000, new Vector3(262.114f, 212.761f, 100.680f), -18.99f));
            GoldTrolleys.Add(new GoldTrolley(2_250_000, new Vector3(261.46f, 213.813f, 100.680f), 70.249f));
            GoldTrolleys.Add(new GoldTrolley(2_250_000, new Vector3(262.336f, 216.252f, 100.680f), 70.249f));
            GoldTrolleys.Add(new GoldTrolley(2_250_000, new Vector3(263.523f, 216.628f, 100.680f), 18.991f));

            Objects.Add(new Object("sm_prop_smug_heli", new Vector3(580.114f, 12.358f, 102.213f), 79.994f));
            Objects.Add(new Object("ex_cash_scatter_01", new Vector3(262.26f, 214.862f, 100.683f), -21.769f));
            Objects.Add(new Object("m23_2_prop_m32_laptop_01a", new Vector3(264.118f, 213.612f, 101.528f), 0f));

            Debug.WriteLine($"^5[INFO] Match '{Id}' has been created. Start time '{StartTime}' End time '{EndTime}'^7");
            Debug.WriteLine($"^5[INFO] World has been initialized with {GoldTrolleys.Count} gold trolleys and {Objects.Count} objects^7");
        }

        internal static void DeleteExistingGoldTrolleys()
        {
            foreach(var item in GoldTrolleys)
            {
                if(API.DoesEntityExist(item.Id))
                {
                    API.DeleteEntity(item.Id);
                }
            }

            GoldTrolleys.Clear();
        }

        internal static void DeleteExistingObjects()
        {
            foreach(var item in Objects)
            {
                if(API.DoesEntityExist(item.Id))
                {
                    API.DeleteEntity(item.Id);
                }
            }

            GoldTrolleys.Clear();
        }
    }
}