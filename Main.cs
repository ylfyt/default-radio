using System;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using GTA.UI;

namespace default_radio
{
    public class Main : Script
    {
        private int timeToCheck = Game.GameTime + 500;
        private bool IsPrevOnVehicle { get; set; }

        public Main()
        {   
            Tick += OnTick;
            KeyDown += OnKeyDown;
            IsPrevOnVehicle = Game.Player.Character.IsInVehicle();
        }

        public void OnTick(object o, EventArgs e)
        {
            if (Game.GameTime > timeToCheck)
            {
                timeToCheck = Game.GameTime + 500;

                var isOnVehicle = Game.Player.Character.IsInVehicle();
                if (isOnVehicle && !IsPrevOnVehicle)
                {
                    Wait(500);
                    OnEnterVehicle();
                }
                IsPrevOnVehicle = isOnVehicle;
            }
        }

        public void OnKeyDown(object o, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D9)
            {                
                Notification.Show("Default Radio");
            }
        }

        private void OnEnterVehicle()
        {
            Function.Call(Hash.SET_RADIO_TO_STATION_INDEX, 11);
        }
    }
}
