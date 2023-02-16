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
        private UIMenuListItem radioMenuList;
        private bool isPrevHasSelfRadio;

        public Main()
        {
            settings = ScriptSettings.Load("scripts/default-radio.ini");
            radioIdx = settings.GetValue("DEFAULT_RADIO", "RADIO_IDX", 255);

            pool = new MenuPool();
            menu = new UIMenu("Default Radio", "~b~Choose Radio Station");
            pool.Add(menu);
            createMenu(menu);
            pool.RefreshIndex();

            IsPrevOnVehicle = Game.Player.Character.IsInVehicle();

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
            if (e.KeyCode == Keys.NumPad0) {
                if (!menu.Visible)
                {
                    var current = IsHasSelfRadio();
                    if (!current && isPrevHasSelfRadio)
                    {
                        radioMenuList.Items.RemoveAt(radioMenuList.Items.Count - 1);
                        UI.Notify("User doesn't have self radio, DefaultRadio disabled");
                        if (radioIdx == 253)
                        {
                            radioIdx = 254;
                            radioMenuList.Index = radioMenuList.Items.Count - 1;
                        }
                    }
                    else if (current && !isPrevHasSelfRadio)
                    {
                        UI.Notify("Adding self radio");
                        radioMenuList.Items.Add("Self Radio");
                    }
                    isPrevHasSelfRadio = current;
                    OnEnterVehicle();
                }
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
                "Radio Off",
                "Disable DefaultRadio"
            };

            isPrevHasSelfRadio = IsHasSelfRadio();
            if (isPrevHasSelfRadio)
                radios.Add("Self Radio");
            
            if (radioIdx == 255)
                radioMenuList = new UIMenuListItem("", radios, radios.Count - 2);
            else if (radioIdx == 254)
                radioMenuList = new UIMenuListItem("", radios, radios.Count - 1);
            else if (radioIdx == 253)
            {
                if (!isPrevHasSelfRadio)
                    UI.Notify("User doesn't have self radio, DefaultRadio disabled");      
                
                radioMenuList = new UIMenuListItem("", radios, radios.Count - 1);
            }
            else
            {
                radioIdx %= 21;
                radioMenuList = new UIMenuListItem("", radios, radioIdx);
            }
            
            menu.AddItem(radioMenuList);
            radioMenuList.OnListChanged += OnRadioSelected;
        }

        private void OnRadioSelected(UIMenuListItem source, int idx)
        {
            if (isPrevHasSelfRadio)
            {
                if (idx == source.Items.Count - 1)
                    radioIdx = 253;
                else if (idx == source.Items.Count - 2)
                    radioIdx = 254;
                else if (idx == source.Items.Count - 3)
                    radioIdx = 255;
                else
                    radioIdx = idx;
            }
            else
            {
                radioIdx = idx;
            }
            settings.SetValue("DEFAULT_RADIO", "RADIO_IDX", radioIdx);
            var success = settings.Save();
            if (!success)
                UI.Notify("Failed to set default radio!");

            if (Game.Player.Character.IsInVehicle())
                OnEnterVehicle();
        }

        private void OnEnterVehicle()
        {
            if (radioIdx == 255)
            {
                Game.Player.LastVehicle.IsRadioEnabled = false;
                return;
            }
            Game.Player.LastVehicle.IsRadioEnabled = true;
            if (radioIdx == 254) return;

            if (IsHasSelfRadio())
            {
                if (radioIdx == 253)
                {
                    Function.Call(Hash.SET_RADIO_TO_STATION_INDEX, 11);
                    return;
                }
                Function.Call(Hash.SET_RADIO_TO_STATION_INDEX, radioIdx < 11 ? radioIdx : radioIdx + 1);
                return;
            }
            if (radioIdx == 253) return;

            Function.Call(Hash.SET_RADIO_TO_STATION_INDEX, radioIdx < 11 ? radioIdx : radioIdx + 1);
        }

        private bool IsHasSelfRadio()
        {
            return Function.Call<string>(Hash.GET_RADIO_STATION_NAME, 11) == "RADIO_19_USER";
        }
    }
}
