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
            try
            {
                /**
                 * RhapidFyre: you can’t spawn objects/vehicles/etc server side when there are no players 
                 * on the server, which is why mine was always returning zero. Basically you can’t spawn objects 
                 * on the server and then just have them “be there” when players join.
                 * https://forum.cfx.re/t/createobject-returns-model-hash/1934711
                 */
                int id = Id = API.CreateObject((int)API.GetHashKey(model), location.X, location.Y, location.Z, true, true, false);
                if(id == 0)
                {
                    throw new Exception($"Object with id '{id}' failed to be created!");
                }

                Model = model;
                Location = location;
                Heading = heading;

                API.SetEntityHeading(Id, Heading);
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"^1[ERROR] {ex}^7");
            }
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
            try
            {
                /**
                 * RhapidFyre: you can’t spawn objects/vehicles/etc server side when there are no players 
                 * on the server, which is why mine was always returning zero. Basically you can’t spawn objects 
                 * on the server and then just have them “be there” when players join.
                 * https://forum.cfx.re/t/createobject-returns-model-hash/1934711
                 */
                int id = API.CreateObject((int)API.GetHashKey("prop_gold_trolly_full"), location.X, location.Y, location.Z, true, true, false);
                if(id == 0)
                {
                    throw new Exception($"GoldTrolley with id '{id}' failed to be created!");
                }

                Id = id;
                Value = value;
                IsRobbed = false;
                Location = location;
                Heading = heading;

                API.SetEntityHeading(Id, Heading);
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"^1[ERROR] {ex}^7");
            }
        }
    }

    internal static class Match
    {
        internal static Guid Id;
        internal static DateTime StartTime;
        internal static DateTime EndTime;
        internal static List<GoldTrolley> GoldTrolleys = new List<GoldTrolley>();
        internal static List<Object> Objects = new List<Object>();
        internal static int StolenGoldTrolleys = 0;

        internal static void Create(Guid id, DateTime startTime, DateTime endTime)
        {
            Id = id;
            StartTime = startTime;
            EndTime = endTime;
            StolenGoldTrolleys = 0;

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

        internal static void RobGoldTrolley([FromSource] Player player, int _, int netId)
        {
            try
            {
                int id = API.NetworkGetEntityFromNetworkId(netId);

                Debug.WriteLine($"{id}");
                Debug.WriteLine($"{netId}");

                foreach(var item in GoldTrolleys)
                {
                    Debug.WriteLine($"{item.Id}");
                }

                GoldTrolley target = GoldTrolleys.Find(goldTrolley => goldTrolley.Id == id);

                if(target == null)
                {
                    throw new Exception($"GoldTrolley with id '{id}' failed to be found!");
                }

                if(target.IsRobbed)
                {
                    throw new Exception($"GoldTrolley with id '{id}' failed to be robbed again!");
                }

                target.IsRobbed = true;
                StolenGoldTrolleys++;
                API.DeleteEntity(target.Id);
                int trolley = API.CreateObject((int)API.GetHashKey("hei_prop_gold_trolly_empty"), target.Location.X, target.Location.Y, target.Location.Z, true, true, false);
                API.SetEntityHeading(trolley, target.Heading);

                Debug.WriteLine($"^5[INFO] GoldTrolley '{target.Id}' has been robbed by {player.Name}.^7");
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"^1[ERROR] {ex}^7");
            }
        }

        internal static void DeleteExistingObjects()
        {
            foreach(int item in API.GetAllObjects())
            {
                if(API.DoesEntityExist(item))
                {
                    API.DeleteEntity(item);
                }
            }

            GoldTrolleys.Clear();
        }
    }
}