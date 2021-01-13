using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;

namespace LightProsperity
{
	[HarmonyPatch(typeof(DefaultSettlementFoodModel), "CalculateTownFoodStocksChange")]
	public class CalculateTownFoodStocksChangePatch
	{
		public static void Postfix(ref float __result, Town town, StatExplainer explanation)
		{
			MobileParty garrisonParty = town.GarrisonParty;
			int num = -((garrisonParty != null) ? garrisonParty.Party.NumberOfAllMembers : 0) / 20;
			int num2 = (int)((float)num * SubModule.Settings.GarrisonFoodConsumpetionMultiplier);
			__result = __result - (float)num + (float)num2;
			bool flag = explanation != null && explanation.Lines.Count > 1;
			if (flag)
			{
				explanation.Lines[1].Number = (float)num2;
			}
		}

		public static bool Prepare()
		{
			return SubModule.Settings.ModifyGarrisonConsumption;
		}
	}
}
