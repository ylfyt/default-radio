using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using NativeUI;

namespace default_radio
{
    public class Main : Script
    {
        private int timeToCheck = Game.GameTime + 500;
        private bool IsPrevOnVehicle { get; set; }
        private readonly ScriptSettings settings;
        private int radioIdx;
        private readonly MenuPool pool;
        private readonly UIMenu menu;

        public Main()
        {
            pool = new MenuPool();
            menu = new UIMenu("Default Radio", "~b~Choose Radio Station");
            pool.Add(menu);
            createMenu(menu);
            pool.RefreshIndex();

            IsPrevOnVehicle = Game.Player.Character.IsInVehicle();
            settings = ScriptSettings.Load("./default_radio.ini");
            radioIdx = settings.GetValue("", "radioIdx", 255);

            Tick += OnTick;
            KeyDown += OnKeyDown;
        }

        public void OnTick(object o, EventArgs e)
        {
            pool.ProcessMenus();
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
            if (e.KeyCode == Keys.D9) {
                menu.Visible = !pool.IsAnyMenuOpen();
            }
        }

        private void createMenu(UIMenu menu)
        {
            var radios = new List<object>{
                "Blue Ark",
                "Worldwide FM",
                "FlyLo FM",
                "The Lowdown 91.1",
                "The Lab",
                "Radio Mirror Park",
                "Space 103.2",
                "Vinewood Boulevard Radio",
                "Blonded Los Santos 97.8 FM",
                "Los Santos Undeground Radio",
                "iFruit Radio",
                "Self Radio",
                "Los Santos Rock Radio",
                "Non-Stop-Pop FM",
                "Radio Los Santos",
                "Channel X",
                "West Coast Talk Radio",
                "Rabel Radio",
                "Soulwax FM",
                "East Los FM",
                "West Cost Classics",
                "MOTOMAMI Los Santos",
                "Radio Off"
            };


            var newitem = new UIMenuListItem("", radios, 0);
            menu.AddItem(newitem);
            menu.OnListChange += (sender, item, index) =>
            {
                if (item != newitem)
                    return;
                
                var radio = item.Items[index] as string;
                UI.Notify(radio);
            };
        }

        private void OnEnterVehicle()
        {
            Function.Call(Hash.SET_RADIO_TO_STATION_INDEX, radioIdx);
        }
    }
}
