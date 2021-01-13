using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;

namespace LightProsperity
{
	[HarmonyPatch(typeof(RecruitmentCampaignBehavior), "OnTroopRecruited")]
	public class OnTroopRecruitedPatch
	{
		public static void Postfix(Hero arg1, Settlement settlement, Hero individual, CharacterObject troop, int count)
		{
			if (individual != null && settlement != null)
			{
				if (settlement.IsTown)
				{
					settlement.Prosperity -= SubModule.Settings.TownRecruitProsperityCost * (float)count;
					if (settlement.Prosperity < 0f)
					{
						settlement.Prosperity = 0f;
					}
				}
				if (settlement.IsVillage)
				{
					settlement.Village.Hearth -= SubModule.Settings.VillageRecruitProsperityCost * (float)count;
					if (settlement.Village.Hearth < 0f)
					{
						settlement.Village.Hearth = 0f;
					}
				}
			}
		}
	}
}
