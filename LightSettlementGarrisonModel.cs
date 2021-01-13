using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace LightProsperity
{
	internal class LightSettlementGarrisonModel : SettlementGarrisonModel
	{
		public override int CalculateGarrisonChange(Settlement settlement, StatExplainer? explanation = null)
		{
			return LightSettlementGarrisonModel.CalculateGarrisonChangeInternal(settlement, explanation);
		}

		private static int CalculateGarrisonChangeInternal(Settlement settlement, StatExplainer? explanation = null)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, explanation, null);
			bool flag = settlement.IsTown || settlement.IsCastle;
			if (flag)
			{
				double num = (double)settlement.Town.Loyalty;
				bool isStarving = settlement.IsStarving;
				if (isStarving)
				{
					float foodChange = settlement.Town.FoodChange;
					int num2 = (!settlement.Town.Owner.IsStarving || (double)foodChange >= -19.0) ? 0 : ((int)(((double)foodChange + 10.0) * (double)SubModule.Settings.GarrisonFoodConsumpetionMultiplier / 10.0));
					explainedNumber.Add((float)num2, LightSettlementGarrisonModel._foodShortageText, null);
				}
				bool flag2 = settlement.Town.GarrisonParty != null && ((double)settlement.Town.GarrisonParty.Party.NumberOfHealthyMembers + (double)explainedNumber.ResultNumber) / (double)settlement.Town.GarrisonParty.Party.PartySizeLimit > (double)settlement.Town.GarrisonParty.PaymentRatio;
				if (flag2)
				{
					int num3 = 0;
					do
					{
						num3++;
					}
					while ( (settlement is not null && settlement.Town.GarrisonParty is not null) && 
						( ((double)settlement.Town.GarrisonParty.Party.NumberOfHealthyMembers + (double)explainedNumber.ResultNumber - (double)num3)  / (double)settlement.Town.GarrisonParty.Party.PartySizeLimit >= 
							(double)settlement.Town.GarrisonParty.PaymentRatio && (double)settlement.Town.GarrisonParty.Party.NumberOfHealthyMembers + (double)explainedNumber.ResultNumber - (double)num3 > 0.0 && num3 < 20) );
					explainedNumber.Add((float)(-(float)num3), LightSettlementGarrisonModel._paymentIsLess, null);
				}
			}

			if (settlement is not null)
            {
				LightSettlementGarrisonModel.GetSettlementGarrisonChangeDueToIssues(settlement, ref explainedNumber);
			}
			
			return (int)explainedNumber.ResultNumber;
		}

		/*
		private static void GetSettlementGarrisonChangeDueToIssues(Settlement settlement, ref ExplainedNumber result)
		{
			float value;
			bool flag = !IssueManager.DoesSettlementHasIssueEffect(DefaultIssueEffects.SettlementGarrison, settlement, ref value);
			if (!flag)
			{
				result.Add(value, LightSettlementGarrisonModel._issues, null);
			}
		}
		*/

		private static void GetSettlementGarrisonChangeDueToIssues(Settlement settlement, ref ExplainedNumber result)
		{
			Campaign.Current.Models.IssueModel.GetIssueEffectsOfSettlement(DefaultIssueEffects.SettlementGarrison, settlement, ref result);
			result.Add(result.ResultNumber, LightSettlementGarrisonModel._issues, null);
		}


		public override int FindNumberOfTroopsToTakeFromGarrison(MobileParty mobileParty, Settlement settlement, float defaultIdealGarrisonStrengthPerWalledCenter = 0f)
		{
			MobileParty garrisonParty = settlement.Town.GarrisonParty;
			int result;
			if (garrisonParty is null)
			{
				result = 0;
			}
			else
			{
				float totalStrength = garrisonParty.Party.TotalStrength;
				float num = (((double)defaultIdealGarrisonStrengthPerWalledCenter > 0.100000001490116) ? defaultIdealGarrisonStrengthPerWalledCenter : FactionHelper.FindIdealGarrisonStrengthPerWalledCenter(mobileParty.MapFaction as Kingdom, settlement.OwnerClan)) * FactionHelper.OwnerClanEconomyEffectOnGarrisonSizeConstant(settlement.OwnerClan) * (settlement.IsTown ? 2f : 1f);
				float num2 = (float)mobileParty.Party.PartySizeLimit * mobileParty.PaymentRatio / (float)mobileParty.Party.NumberOfAllMembers;
				double num3 = Math.Min(11.0, (double)num2 * Math.Sqrt((double)num2)) - 1.0;
				float num4 = (float)Math.Pow((double)totalStrength / (double)num, 1.5);
				float num5 = (mobileParty.LeaderHero.Clan.Leader == mobileParty.LeaderHero) ? 2f : 1f;
				double num6 = (double)num4;
				int num7 = MBRandom.RoundRandomized((float)(num3 * num6) * num5);
				int num8 = 25 * (settlement.IsTown ? 2 : 1);
				bool flag2 = num7 > garrisonParty.Party.MemberRoster.TotalRegulars - num8;
				if (flag2)
				{
					num7 = garrisonParty.Party.MemberRoster.TotalRegulars - num8;
				}
				result = num7;
			}
			return result;
		}

		public override int FindNumberOfTroopsToLeaveToGarrison(MobileParty mobileParty, Settlement settlement)
		{
			MobileParty garrisonParty = settlement.Town.GarrisonParty;
			float garrisonStrength = 0f;
			if (garrisonParty is not null)
			{
				garrisonStrength = garrisonParty.Party.TotalStrength;
			}
			float num2 = FactionHelper.FindIdealGarrisonStrengthPerWalledCenter(mobileParty.MapFaction as Kingdom, settlement.OwnerClan) * FactionHelper.OwnerClanEconomyEffectOnGarrisonSizeConstant(settlement.OwnerClan) * (settlement.IsTown ? 2f : 1f);
			int result;

			if ((settlement.OwnerClan.Leader == Hero.MainHero && (mobileParty.LeaderHero == null || mobileParty.LeaderHero.Clan != Clan.PlayerClan)) || (double)garrisonStrength >= (double)num2)
			{
				result = 0;
			}
			else
			{
				int numberOfRegularMembers = mobileParty.Party.NumberOfRegularMembers;
				float woundedRatio = (float)(1.0 + (double)mobileParty.Party.NumberOfWoundedRegularMembers / (double)mobileParty.Party.NumberOfRegularMembers);
				float num4 = (float)mobileParty.Party.PartySizeLimit * mobileParty.PaymentRatio;
				float num5 = (float)(Math.Pow((double)Math.Min(2f, (float)numberOfRegularMembers / num4), 1.20000004768372) * 0.75);
				float num6 = (float)((1.0 - (double)garrisonStrength / (double)num2) * (1.0 - (double)garrisonStrength / (double)num2));
				bool flag3 = mobileParty.Army != null;
				if (flag3)
				{
					num6 = Math.Min(num6, 0.5f);
				}
				float num7 = 0.5f;
				bool flag4 = settlement.OwnerClan == mobileParty.Leader.HeroObject.Clan || settlement.OwnerClan == mobileParty.Party.Owner.MapFaction.Leader.Clan;
				if (flag4)
				{
					num7 = 1f;
				}
				float num8 = (mobileParty.Army != null) ? 1.25f : 1f;
				float num9 = 1f;
				List<float> list = new List<float>(5);
				for (int i = 0; i < 5; i++)
				{
					list.Add(Campaign.MapDiagonal * Campaign.MapDiagonal);
				}
				foreach (Kingdom kingdom in Kingdom.All)
				{
					bool flag5 = kingdom.IsKingdomFaction && mobileParty.MapFaction.IsAtWarWith(kingdom);
					if (flag5)
					{
						foreach (Settlement settlement2 in kingdom.Settlements)
						{
							float num10 = settlement2.Position2D.DistanceSquared(mobileParty.Position2D);
							for (int j = 0; j < 5; j++)
							{
								bool flag6 = (double)num10 < (double)list[j];
								if (flag6)
								{
									for (int k = 4; k >= j + 1; k--)
									{
										list[k] = list[k - 1];
									}
									list[j] = num10;
									break;
								}
							}
						}
					}
				}
				float num11 = 0f;
				for (int l = 0; l < 5; l++)
				{
					num11 += (float)Math.Sqrt((double)list[l]);
				}
				float num12 = num11 / 5f;
				double num13 = (double)Math.Max(0f, Math.Min((float)((double)Campaign.MapDiagonal / 15.0 - (double)Campaign.MapDiagonal / 30.0), num12 - Campaign.MapDiagonal / 30f)) / ((double)Campaign.MapDiagonal / 15.0 - (double)Campaign.MapDiagonal / 30.0);
				float num14 = Math.Min(0.7f, num9 * num5 * num6 * num7 * num8 * woundedRatio);
				result = MBRandom.RoundRandomized((float)numberOfRegularMembers * num14);
			}
			return result;
		}

		private static readonly TextObject _townWallsText = new TextObject("{=SlmhqqH8}Town Walls", null);

		private static readonly TextObject _moraleText = new TextObject("{=UjL7jVYF}Morale", null);

		private static readonly TextObject _foodShortageText = new TextObject("{=qTFKvGSg}Food Shortage", null);

		private static readonly TextObject _surplusFoodText = GameTexts.FindText("str_surplus_food", null);

		private static readonly TextObject _recruitFromCenterNotablesText = GameTexts.FindText("str_center_notables", null);

		private static readonly TextObject _recruitFromVillageNotablesText = GameTexts.FindText("str_village_notables", null);

		private static readonly TextObject _villageBeingRaided = GameTexts.FindText("str_village_being_raided", null);

		private static readonly TextObject _villageLooted = GameTexts.FindText("str_village_looted", null);

		private static readonly TextObject _townIsUnderSiege = GameTexts.FindText("str_villages_under_siege", null);

		private static readonly TextObject _retiredText = GameTexts.FindText("str_retired", null);

		private static readonly TextObject _paymentIsLess = GameTexts.FindText("str_payment_is_less", null);

		private static readonly TextObject _issues = new TextObject("{=D7KllIPI}Issues", null);
	}
}
