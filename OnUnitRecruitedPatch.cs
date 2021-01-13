using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;

namespace LightProsperity
{
	[HarmonyPatch(typeof(RecruitmentCampaignBehavior), "OnUnitRecruited")]
	public class OnUnitRecruitedPatch
	{
		public static void Postfix(CharacterObject troop, int count)
		{
			Settlement currentSettlement = Hero.MainHero.CurrentSettlement;
			if (currentSettlement != null)
			{
				if (currentSettlement.IsTown)
				{
					currentSettlement.Prosperity -= SubModule.Settings.TownRecruitProsperityCost * (float)count;
					if (currentSettlement.Prosperity < 0f)
					{
						currentSettlement.Prosperity = 0f;
					}
				}
				if (currentSettlement.IsVillage)
				{
					currentSettlement.Village.Hearth -= SubModule.Settings.VillageRecruitProsperityCost * (float)count;
					if (currentSettlement.Village.Hearth < 0f)
					{
						currentSettlement.Village.Hearth = 0f;
					}
				}
			}
		}
	}
}
