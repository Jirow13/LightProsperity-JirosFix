using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace LightProsperity
{
	public class LightSettlementProsperityModel : SettlementProsperityModel
	{
		internal static void AddDefaultDailyBonus(Town fortification, ref ExplainedNumber result)
		{
			float num = (float)((double)fortification.Construction * (double)fortification.CurrentBuilding.BuildingType.Effects[0].Level1Effect * 0.00999999977648258);
			result.Add(num * SubModule.Settings.ProsperityGrowthMultiplier, fortification.CurrentBuilding.BuildingType.Name, null);
		}

		public override float CalculateProsperityChange(Town fortification, StatExplainer? explanation = null)
		{
			return this.CalculateProsperityChangeInternal(fortification, explanation);
		}

		public override float CalculateHearthChange(Village village, StatExplainer? explanation = null)
		{
			return this.CalculateHearthChangeInternal(village, explanation);
		}

		private float CalculateHearthChangeInternal(Village village, StatExplainer? explanation = null)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, explanation, null);
			bool flag = village.VillageState == Village.VillageStates.Normal;
			if (flag)
			{
				float hearth = Math.Max(village.Hearth * LightSettlementProsperityModel._hearthMultiplier, 0.5f);
				float hearthCoef = this._hearthCoeff * village.Hearth * village.Hearth * LightSettlementProsperityModel._hearthMultiplier;
				float enemiesSpotted = hearth - hearthCoef;

				if (enemiesSpotted > 0f)
				{
					explainedNumber.Add(enemiesSpotted * SubModule.Settings.ProsperityGrowthMultiplier, this._newBornText, null);
				}
				else
				{
					explainedNumber.Add(enemiesSpotted * SubModule.Settings.ProsperityGrowthMultiplier, this._populationLossText, null);
				}

				float townFoodStockChange = Math.Min(0.8f * village.Settlement.NumberOfEnemiesSpottedAround, 1f);
				float enemyAroundPenalty = -townFoodStockChange * village.Hearth * LightSettlementProsperityModel._hearthMultiplier;
				explainedNumber.Add(enemyAroundPenalty * SubModule.Settings.ProsperityGrowthMultiplier, this._enemyText, null);

				if (village.Bound != null)
				{
					bool flag4 = village.Bound.Town.CurrentBuilding != null && village.Bound.Town.CurrentBuilding.BuildingType == DefaultBuildingTypes.IrrigationDaily;
					if (flag4)
					{
						LightSettlementProsperityModel.AddDefaultDailyBonus(village.Bound.Town, ref explainedNumber);
					}
					LightSettlementProsperityModel.AddPerkBonusForTown(DefaultPerks.Medicine.BushDoctor, village.Bound.Town, ref explainedNumber, hearth);
				}
			}
			else
			{
				if (village.VillageState == Village.VillageStates.Looted)
				{
					explainedNumber.Add(-village.Hearth * 0.01f * SubModule.Settings.ProsperityGrowthMultiplier, this._raidedText, null);
				}
			}
			return explainedNumber.ResultNumber;
		}

		private float CalculateProsperityChangeInternal(Town fortification, StatExplainer? explanation = null)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, explanation, null);
			float townProsperity = 0f, townCoeff =0f, enemiesSpotted = 0f, foodChange = 0f;

			float townFoodStockChange = Campaign.Current.Models.SettlementFoodModel.CalculateTownFoodStocksChange(fortification, null);

			float num5 = fortification.FoodStocks / -townFoodStockChange;

			if (fortification.Settlement.Culture.ProsperityBonus > 0 && fortification.IsTown)
			{
				townProsperity = fortification.Prosperity * LightSettlementProsperityModel._townProsperityMultiplier;
				townCoeff = this._townCoeff * fortification.Prosperity * fortification.Prosperity * LightSettlementProsperityModel._townProsperityMultiplier;
				enemiesSpotted = Math.Min(0.6f * fortification.Settlement.NumberOfEnemiesSpottedAround, 1f);
				foodChange = ((num5 < 0f) ? 0f : (Math.Max(1f - num5 / 21f, 0f) * townProsperity));
			}

			if (fortification.Settlement.Culture.ProsperityBonus > 0 && fortification.IsCastle)
			{
				townProsperity = fortification.Prosperity * LightSettlementProsperityModel._castleProsperityMultiplier;
				townCoeff = this._castleCoeff * fortification.Prosperity * fortification.Prosperity * LightSettlementProsperityModel._castleProsperityMultiplier;
				enemiesSpotted = Math.Min(0.4f * fortification.Settlement.NumberOfEnemiesSpottedAround, 1f);
				foodChange = ((num5 < 0f) ? 0f : (Math.Max(1f - num5 / 42f, 0f) * townProsperity));
			}

			/* To Do: Make this a setting and configurable */
			if (fortification.IsTown)
			{
				if (fortification.Prosperity < 500f)
				{
					explainedNumber.Add(6f, _housingCostsText, null);
				}
				else if (fortification.Prosperity < 1000f)
				{
					explainedNumber.Add(4f, _housingCostsText, null);
				}
				else if (fortification.Prosperity < 1500f)
				{
					explainedNumber.Add(2f, _housingCostsText, null);
				}
			}

			float prosperityChange = townProsperity - townCoeff;

			if (prosperityChange > 0f)
			{
				explainedNumber.Add(prosperityChange * SubModule.Settings.ProsperityGrowthMultiplier, this._newBornText, null);
			}

			else
			{
				explainedNumber.Add(prosperityChange * SubModule.Settings.ProsperityGrowthMultiplier, this._populationLossText, null);
			}

			float num8 = enemiesSpotted * townProsperity;

			explainedNumber.Add(-num8 * SubModule.Settings.ProsperityGrowthMultiplier, this._enemyText, null);

			float num9 = (!fortification.Owner.IsStarving || townFoodStockChange >= 0f) ? 0f : townFoodStockChange;

			if (num9 < 0f)
			{
				explainedNumber.Add(num9 * SubModule.Settings.ProsperityGrowthMultiplier, this._foodShortageText, null);
			}
			if (foodChange > 0f)
			{
				explainedNumber.Add(-foodChange * SubModule.Settings.ProsperityGrowthMultiplier, this._foodWorriesText, null);
			}

			/* Look Hard at this one, as it doesn't exist in the newer models. */			
			if (fortification.Settlement.Culture.ProsperityBonus > 0)
			{
				float num10 = (float)fortification.Settlement.Culture.ProsperityBonus * LightSettlementProsperityModel._vanillaToRatio * townProsperity;
				explainedNumber.Add(num10 * SubModule.Settings.ProsperityGrowthMultiplier, this._empireProsperityBonus, null);
			}
			if (fortification.IsTown)
			{
				int num11 = fortification.SoldItems.Sum((Town.SellLog x) => (x.Category.Properties != ItemCategory.Property.BonusToProsperity) ? 0 : x.Number);
				if (num11 > 0)
				{
					float num12 = (float)num11 * 2f * LightSettlementProsperityModel._vanillaToRatio * townProsperity;
					explainedNumber.Add(num12 * SubModule.Settings.ProsperityGrowthMultiplier, this._prosperityFromMarketText, null);
				}
			}
			
			LightSettlementProsperityModel.AddPerkBonusForTown(DefaultPerks.Medicine.PristineStreets, fortification, ref explainedNumber, townProsperity);
			LightSettlementProsperityModel.AddPerkBonusForTown(DefaultPerks.Riding.Veterinary, fortification, ref explainedNumber, townProsperity);
			
			if (fortification.CurrentBuilding.BuildingType == DefaultBuildingTypes.BuildHouseDaily)
			{
				LightSettlementProsperityModel.AddDefaultDailyBonus(fortification, ref explainedNumber);
			}
			
			foreach (Building building in fortification.Buildings)
			{
				float buildingEffectAmount = building.GetBuildingEffectAmount(BuildingEffectEnum.Prosperity);
				if (!building.BuildingType.IsDefaultProject && buildingEffectAmount > 0f)
				{
					float num13 = buildingEffectAmount * LightSettlementProsperityModel._vanillaToRatio * townProsperity;
					explainedNumber.Add(num13 * SubModule.Settings.ProsperityGrowthMultiplier, building.Name, null);
				}
				if (building.BuildingType == DefaultBuildingTypes.SettlementAquaducts)
				{
					LightSettlementProsperityModel.AddPerkBonusForTown(DefaultPerks.Medicine.CleanInfrastructure, fortification, ref explainedNumber, townProsperity);
				}
			}
			
			//if (fortification.Loyalty > 75.0)			
			if (fortification.Loyalty > (float)Campaign.Current.Models.SettlementLoyaltyModel.ThresholdForProsperityBoost)				
			{
				float num14 = Campaign.Current.Models.SettlementLoyaltyModel.HighLoyaltyProsperityEffect * LightSettlementProsperityModel._vanillaToRatio * townProsperity;
				explainedNumber.Add(num14 * SubModule.Settings.ProsperityGrowthMultiplier, this._loyaltyText, null);
			}
			else
			{
				if (fortification.Loyalty <= (float)Campaign.Current.Models.SettlementLoyaltyModel.ThresholdForProsperityPenalty)
				{
					float num15 = Campaign.Current.Models.SettlementLoyaltyModel.LowLoyaltyProsperityEffect * LightSettlementProsperityModel._vanillaToRatio * townProsperity;
					explainedNumber.Add(num15 * SubModule.Settings.ProsperityGrowthMultiplier, this._loyaltyText, null);
				}
			}

			if (fortification.IsTown && !fortification.CurrentBuilding.IsCurrentlyDefault && fortification.Governor != null && fortification.Governor.GetPerkValue(DefaultPerks.Trade.TrickleDown))
			{
				explainedNumber.Add(DefaultPerks.Trade.TrickleDown.SecondaryBonus, DefaultPerks.Trade.TrickleDown.Name, null);
			}

			if (fortification.Settlement.OwnerClan.Kingdom != null)
			{
				if (fortification.Settlement.OwnerClan.Kingdom.ActivePolicies.Contains(DefaultPolicies.Serfdom))
				{
					float num16 = -1f * LightSettlementProsperityModel._vanillaToRatio * townProsperity;
					explainedNumber.Add(num16 * SubModule.Settings.ProsperityGrowthMultiplier, DefaultPolicies.Serfdom.Name, null);
				}
				if (fortification.Settlement.OwnerClan.Kingdom.ActivePolicies.Contains(DefaultPolicies.RoadTolls))
				{
					float num17 = -0.2f * LightSettlementProsperityModel._vanillaToRatio * townProsperity;
					explainedNumber.Add(num17 * SubModule.Settings.ProsperityGrowthMultiplier, DefaultPolicies.RoadTolls.Name, null);
				}
				if (fortification.Settlement.OwnerClan.Kingdom.RulingClan == fortification.Settlement.OwnerClan && fortification.Settlement.OwnerClan.Kingdom.ActivePolicies.Contains(DefaultPolicies.ImperialTowns))
				{
					float num18 = 1f * LightSettlementProsperityModel._vanillaToRatio * townProsperity;
					explainedNumber.Add(num18 * SubModule.Settings.ProsperityGrowthMultiplier, DefaultPolicies.ImperialTowns.Name, null);
				}
				if (fortification.Settlement.OwnerClan.Kingdom.ActivePolicies.Contains(DefaultPolicies.CrownDuty))
				{
					float num19 = -1f * LightSettlementProsperityModel._vanillaToRatio * townProsperity;
					explainedNumber.Add(num19 * SubModule.Settings.ProsperityGrowthMultiplier, DefaultPolicies.CrownDuty.Name, null);
				}
				if (fortification.Settlement.OwnerClan.Kingdom.ActivePolicies.Contains(DefaultPolicies.WarTax))
				{
					float num20 = -1f * LightSettlementProsperityModel._vanillaToRatio * townProsperity;
					explainedNumber.Add(num20 * SubModule.Settings.ProsperityGrowthMultiplier, DefaultPolicies.WarTax.Name, null);
				}
			}
			GetSettlementProsperityChangeDueToIssues(fortification.Settlement, ref explainedNumber, townProsperity);
			return explainedNumber.ResultNumber;
		}

		/*
		private void GetSettlementProsperityChangeDueToIssues(Settlement settlement, ref ExplainedNumber result, float newBorn)
		{
			float num;
			bool flag = !IssueManager.DoesSettlementHasIssueEffect(DefaultIssueEffects.SettlementProsperity, settlement, ref num);
			if (!flag)
			{
				float num2 = num * LightSettlementProsperityModel._vanillaToRatio * newBorn;
				result.Add(num2 * SubModule.Settings.ProsperityGrowthMultiplier, this._issueText, null);
			}
		}
		*/

		private void GetSettlementProsperityChangeDueToIssues(Settlement settlement, ref ExplainedNumber result, float newBorn)
		{
			Campaign.Current.Models.IssueModel.GetIssueEffectsOfSettlement(DefaultIssueEffects.SettlementProsperity, settlement, ref result);
			float num = result.ResultNumber;
			float num2 = num * LightSettlementProsperityModel._vanillaToRatio * newBorn;
			result.Add(num2 * SubModule.Settings.ProsperityGrowthMultiplier, this._issueText, null);
		}


		private static void AddPerkBonusForTown(PerkObject perk, Town town, ref ExplainedNumber bonuses, float newBorn)
		{
			bool flag = perk.PrimaryRole == SkillEffect.PerkRole.Governor;
			bool flag2 = perk.SecondaryRole == SkillEffect.PerkRole.Governor;
			if (flag || flag2)
			{
				Hero governor = town.Governor;
				bool flag3 = governor == null || !governor.GetPerkValue(perk) || governor.CurrentSettlement == null || governor.CurrentSettlement != town.Settlement;
				if (governor != null && governor.GetPerkValue(perk) && governor.CurrentSettlement != null && governor.CurrentSettlement == town.Settlement)
				{
					float number = (perk.PrimaryRole == SkillEffect.PerkRole.Governor) ? perk.PrimaryBonus : perk.SecondaryBonus;
					if (flag)
                    {
						LightSettlementProsperityModel.AddToStat(ref bonuses, perk.PrimaryIncrementType, number, LightSettlementProsperityModel._textGovernor, newBorn);
						return;
					}
					LightSettlementProsperityModel.AddToStat(ref bonuses, perk.SecondaryIncrementType, number, LightSettlementProsperityModel._textGovernor, newBorn);
				}
			}
		}

		private static void AddToStat(ref ExplainedNumber stat, SkillEffect.EffectIncrementType effectIncrementType, float number, TextObject text, float newBorn)
		{
			bool flag = effectIncrementType == SkillEffect.EffectIncrementType.Add;
			if (flag)
			{
				stat.Add(number * LightSettlementProsperityModel._vanillaToRatio * newBorn * SubModule.Settings.ProsperityGrowthMultiplier, text, null);
			}
			else
			{
				bool flag2 = effectIncrementType != SkillEffect.EffectIncrementType.AddFactor;
				if (!flag2)
				{
					stat.AddFactor(number * 0.01f, text);
				}
			}
		}

		private readonly TextObject _loyaltyText = GameTexts.FindText("str_loyalty", null);

		private readonly TextObject _foodShortageText = new TextObject("{=qTFKvGSg}Food Shortage", null);

		private readonly TextObject _prosperityFromMarketText = new TextObject("{=RNT5hMVb}Goods From Market", null);

		private readonly TextObject _surplusFoodText = GameTexts.FindText("str_surplus_food", null);

		private readonly TextObject _issueText = GameTexts.FindText("str_issues", null);

		private readonly TextObject _empireProsperityBonus = new TextObject("{=3Ditaq1M}Empire Prosperity Bonus", null);

		private readonly TextObject _governor = new TextObject("{=Fa2nKXxI}Governor", null);

		private readonly TextObject _issues = new TextObject("{=D7KllIPI}Issues", null);

		private readonly TextObject _newBornText = new TextObject("{=RVas571P}New Born", null);

		private readonly TextObject _raidedText = new TextObject("{=RVas572P}Raided", null);

		private readonly TextObject _populationLossText = new TextObject("{=iz6aMY}Population Loss", null);

		private readonly TextObject _enemyText = new TextObject("{=YpDrMf}Enemy Around", null);

		private readonly TextObject _foodWorriesText = new TextObject("{=PBMzr8}Food Running Out", null);

		private readonly TextObject _housingCostsText = new TextObject("{=ByRAgJy4}Housing Costs", null);

		private static readonly TextObject _textGovernor = new TextObject("{=Fa2nKXxI}Governor", null);

		private static readonly float _hearthMultiplier = 0.01f;

		private static readonly float _townProsperityMultiplier = 0.0075f;

		private static readonly float _castleProsperityMultiplier = 0.0075f;

		private static readonly float _vanillaToRatio = 0.05f;

		private readonly float _hearthCoeff = (SubModule.Settings.VillageGrowthCap == 0) ? 0f : (1f / (float)SubModule.Settings.VillageGrowthCap);

		private readonly float _townCoeff = (SubModule.Settings.TownGrowthCap == 0) ? 0f : (1f / (float)SubModule.Settings.TownGrowthCap);

		private readonly float _castleCoeff = (SubModule.Settings.CastleGrowthCap == 0) ? 0f : (1f / (float)SubModule.Settings.CastleGrowthCap);
	}
}
