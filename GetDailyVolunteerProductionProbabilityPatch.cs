using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;

namespace LightProsperity
{
	[HarmonyPatch(typeof(DefaultVolunteerProductionModel), "GetDailyVolunteerProductionProbability")]
	public class GetDailyVolunteerProductionProbabilityPatch
	{
		public static void Postfix(ref float __result, Hero hero, int index, Settlement settlement)
		{
			bool isTown = settlement.IsTown;
			if (isTown)
			{
				double num = (double)((settlement.Prosperity - (float)SubModule.Settings.TownMinProsperityForRecruit) / (float)(SubModule.Settings.TownProsperityThreshold - SubModule.Settings.TownMinProsperityForRecruit));
				num = Math.Max(num, 0.0);
				num = Math.Pow(num, 0.7);
				__result *= (float)num;
			}
			bool isVillage = settlement.IsVillage;
			if (isVillage)
			{
				double num2 = (double)((settlement.Village.Hearth - (float)SubModule.Settings.VillageMinProsperityForRecruit) / (float)(SubModule.Settings.VillageProsperityThreshold - SubModule.Settings.VillageMinProsperityForRecruit));
				num2 = Math.Max(num2, 0.0);
				num2 = Math.Pow(num2, 0.7);
				__result *= (float)num2;
			}
		}
	}
}
