using System;
using HarmonyLib;
using MCM.Abstractions.Settings.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using System.Windows.Forms;

namespace LightProsperity
{
	public class SubModule : MBSubModuleBase
	{
		private static Harmony? harmony = null;
		protected override void OnBeforeInitialModuleScreenSetAsRoot()
		{
			/*
			if (!this.Patched)
			{
				SubModule.Settings = GlobalSettings<Settings>.Instance;
				Harmony harmony = new Harmony("mod.bannerlord.lightprosperity");
				harmony.PatchAll();
				this.Patched = true;
			}
			*/

			if (harmony == null)
			{
				try
				{
					harmony = new Harmony("mod.bannerlord.lightprosperity");
					harmony.PatchAll();

					if (GlobalSettings<Settings>.Instance is not null)
						SubModule.Settings = GlobalSettings<Settings>.Instance;
					
					InformationManager.DisplayMessage(new InformationMessage("LightProsperity Loaded", Color.ConvertStringToColor("#42FF00FF")));
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Error Initialising Bannerlord Tweaks:\n\n{ex.ToString()}");
				}
			}
		}

		protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
		{
			base.OnGameStart(game, gameStarterObject);
			//this.AddModels(gameStarterObject as CampaignGameStarter);
			AddModels((CampaignGameStarter)gameStarterObject);
		}

		private void AddModels(CampaignGameStarter gameStarter)
		{
			if (SubModule.Settings is { } settings && gameStarter is not null)
            {
				if (settings.ModifyGarrisonConsumption)
				{
					gameStarter.AddModel(new LightSettlementGarrisonModel());
				}
				if (settings.NewProsperityModel)
				{
					gameStarter.AddModel(new LightSettlementProsperityModel());
				}
			}
		}

		//private bool Patched = false;

		public static Settings? Settings;
	}
}
