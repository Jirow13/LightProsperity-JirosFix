using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;

namespace LightProsperity
{
	[HarmonyPatch(typeof(DefaultSettlementProsperityModel), "CalculateProsperityChange")]
	public class CalculateProsperityChangePatch
	{
		public static void Postfix(ref float __result, Town fortification, StatExplainer explanation)
		{
			bool flag = explanation != null;
			if (flag)
			{
				foreach (StatExplainer.ExplanationLine explanationLine in explanation.Lines)
				{
					explanationLine.Number *= SubModule.Settings.ProsperityGrowthMultiplier;
				}
			}
			__result *= SubModule.Settings.ProsperityGrowthMultiplier;
		}

		public static bool Prepare()
		{
			return !SubModule.Settings.NewProsperityModel && SubModule.Settings.ProsperityGrowthMultiplier != 1f;
		}
	}
}
