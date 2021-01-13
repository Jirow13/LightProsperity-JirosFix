using System;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;

namespace LightProsperity
{
	[HarmonyPatch(typeof(HeroHelper), "MaximumIndexHeroCanRecruitFromHero")]
	public class MaximumIndexHeroCanRecruitFromHeroPatch
	{
		public static bool Prefix(ref int __result, Hero buyerHero, Hero sellerHero, int useValueAsRelation)
		{
			int num = 1;
			int num2 = (buyerHero == Hero.MainHero) ? Campaign.Current.Models.DifficultyModel.GetPlayerRecruitSlotBonus() : 0;
			int num3 = (sellerHero.CurrentSettlement == null || buyerHero.MapFaction != sellerHero.CurrentSettlement.MapFaction) ? 0 : 1;
			int num4 = (sellerHero.CurrentSettlement == null || !buyerHero.MapFaction.IsAtWarWith(sellerHero.CurrentSettlement.MapFaction)) ? 0 : -1;
			int num5 = (useValueAsRelation < -100) ? buyerHero.GetRelation(sellerHero) : useValueAsRelation;
			int num6 = (num5 >= 100) ? 7 : ((num5 >= 80) ? 6 : ((num5 >= 60) ? 5 : ((num5 >= 40) ? 4 : ((num5 >= 20) ? 3 : ((num5 >= 10) ? 2 : ((num5 >= 5) ? 1 : ((num5 >= 0) ? 0 : -1)))))));
			int num7 = (sellerHero.CurrentSettlement == null || buyerHero.Clan != sellerHero.CurrentSettlement.OwnerClan) ? 0 : 1;
			int num8 = 0;
			Settlement currentSettlement = sellerHero.CurrentSettlement;
			if (currentSettlement != null)
			{
				bool isTown = currentSettlement.IsTown;
				if (isTown)
				{
					float prosperity = currentSettlement.Prosperity;
					num8 = (int)Math.Floor((double)((prosperity - (float)SubModule.Settings.TownProsperityThreshold) / (float)SubModule.Settings.TownProsperityPerBonusSlot));
				}
				bool isVillage = currentSettlement.IsVillage;
				if (isVillage)
				{
					float hearth = currentSettlement.Village.Hearth;
					num8 = (int)Math.Floor((double)((hearth - (float)SubModule.Settings.VillageProsperityThreshold) / (float)SubModule.Settings.VillageProsperityPerBonusSlot));
				}
			}
			int num9;
			if (num8 > 0 && sellerHero.CurrentSettlement is not null)
			{
				switch (SubModule.Settings.BonusSlotsFor.SelectedIndex)
				{
				case 0:
					num9 = num8;
					break;
				case 1:
					num9 = (!buyerHero.MapFaction.IsAtWarWith(sellerHero.CurrentSettlement.MapFaction)) ? num8 : 0;
					break;
				case 2:
					num9 = (buyerHero.MapFaction == sellerHero.CurrentSettlement.MapFaction) ? num8 : 0;
					break;
				default:
					num9 = (buyerHero.Clan == sellerHero.CurrentSettlement.OwnerClan) ? num8 : 0;
					break;
				}
			}
			else
			{
				num9 = num8;
			}
			__result = Math.Max(0, num + num6 + num2 + num3 + num4 + num7 + num9);
			return false;
		}
	}
}
