using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace LightProsperity
{
	[HarmonyPatch(typeof(SellPrisonersAction), "ApplyInternal")]
	public class ApplyInternalPatch
	{
		public static void Prefix(MobileParty sellerParty, TroopRoster prisoners, Settlement currentSettlement, bool applyGoldChange)
		{
			if (currentSettlement != null)
			{
				currentSettlement.Prosperity += (float)prisoners.TotalRegulars * SubModule.Settings.PrisonerProsperityValue;
			}
		}
	}
}
