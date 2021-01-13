using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;

namespace LightProsperity
{
	[HarmonyPatch(typeof(DefaultPartyWageModel), "GetTotalWage")]
	public class GetTotalWagePatch
	{
		public static void Postfix(ref int __result, MobileParty mobileParty, StatExplainer? explanation = null)
		{
			if (mobileParty.IsGarrison && SubModule.Settings is { } settings)
			{
				__result = (int)((float)__result * settings.GarrisonWagesMultiplier);
			}
		}

 		public static bool Prepare() => SubModule.Settings is { } settings && settings.ModifyGarrisonConsumption;
	}
}
