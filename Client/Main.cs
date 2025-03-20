using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace RoundManager.Client
{
    internal class Main : BaseScript
    {
        public Main()
        {
        }

        [EventHandler("ROUNDMANAGER_CL_SPECTATE")]
        private void OnSpectate()
        {
            Debug.WriteLine("Entering spectator mode.");

            _ = Spectate();
        }


        private async Task Spectate()
        {
            await Delay(60000);

            API.NetworkResurrectLocalPlayer(248.86f, 207.23f, 106.96f, 22.34f, false, false);

            Debug.WriteLine("NetworkResurrectLocalPlayer");

            var camera = API.CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", 248.86f, 207.23f, 106.96f, 0f, 0f, 0f, 40f, true, (int)Game.Player.Character.Rotation.Z);

            Debug.WriteLine("CreateCamWithParams");

            API.RenderScriptCams(true, true, 2000, false, false);

            Debug.WriteLine("RenderScriptCams");

            API.SetCamActive(camera, true);

            Debug.WriteLine("SetCamActive");
        }
    }
}