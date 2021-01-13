using System;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;

namespace LightProsperity
{
	[HarmonyPatch(typeof(RecruitmentCampaignBehavior), "UpdateVolunteersOfNotables")]
	public class UpdateVolunteersOfNotablesPatch
	{
		private static bool IsBitSet(Hero hero, int bit)
		{
			return hero.VolunteerTypes[bit] != null;
		}

		private static bool GenerateBasicTroop(Hero notable, int index)
		{
			CultureObject cultureObject = (notable.CurrentSettlement != null) ? notable.CurrentSettlement.Culture : notable.Clan.Culture;
			bool flag = HeroHelper.HeroShouldGiveEliteTroop(notable);
			if (flag)
			{
				double num = Math.Pow((double)notable.Power / (double)SubModule.Settings.NotablePowerThresholdForNobleRecruit, 1.5);
				bool flag2 = (double)MBRandom.RandomFloat < num;
				if (!flag2)
				{
					return false;
				}
				notable.VolunteerTypes[index] = cultureObject.EliteBasicTroop;
				float num2 = Math.Min(notable.Power - 1, (int)SubModule.Settings.NotableNobleRecruitPowerCost);
				notable.AddPower(-num2);
			}
			else
			{
				notable.VolunteerTypes[index] = cultureObject.BasicTroop;
			}
			return true;
		}

		private static double GetTroopUpgradeChance(Hero notable, int index)
		{
			double num = 0.1;
			double num2 = 200.0;
			double num3 = num2 * num2 / (double)(Math.Max(50f, (float)notable.Power) * Math.Max(50f, (float)notable.Power));
			double x = (double)notable.VolunteerTypes[index].Level;
			return 1.0 / (Math.Pow(x, 2.0) * num3 * num);
		}

		private static void UpgradeTroop(Hero notable, int index)
		{
			bool flag = notable.VolunteerTypes[index].UpgradeTargets != null;
			if (flag)
			{
				notable.VolunteerTypes[index] = notable.VolunteerTypes[index].UpgradeTargets[MBRandom.RandomInt(notable.VolunteerTypes[index].UpgradeTargets.Length)];
			}
		}

		private static void SortNotableVolunteers(Hero notable)
		{
			for (int i = 0; i < 6; i++)
			{
				for (int j = 0; j < 6; j++)
				{
					bool flag = notable.VolunteerTypes[j] != null;
					if (flag)
					{
						int k = j + 1;
						while (k < 6)
						{
							bool flag2 = notable.VolunteerTypes[k] != null;
							if (flag2)
							{
								bool flag3 = (double)notable.VolunteerTypes[j].Level + (notable.VolunteerTypes[j].IsMounted ? 0.5 : 0.0) > (double)notable.VolunteerTypes[k].Level + (notable.VolunteerTypes[k].IsMounted ? 0.5 : 0.0);
								if (flag3)
								{
									CharacterObject characterObject = notable.VolunteerTypes[j];
									notable.VolunteerTypes[j] = notable.VolunteerTypes[k];
									notable.VolunteerTypes[k] = characterObject;
									break;
								}
								break;
							}
							else
							{
								k++;
							}
						}
					}
				}
			}
		}

		private static int GetDailyCastleNobleRecruitCount(Settlement settlement)
		{
			double num = (double)((settlement.Prosperity - (float)SubModule.Settings.CastleMinProsperityForRecruit) / (float)(SubModule.Settings.CastleProsperityThreshold - SubModule.Settings.CastleMinProsperityForRecruit));
			int num2 = (int)Math.Floor(num);
			return num2 + (((double)MBRandom.RandomFloat < num - (double)num2) ? 1 : 0);
		}

		private static void UpdateCastleNobleRecruit(Settlement settlement)
		{
			bool isUnderSiege = settlement.IsUnderSiege;
			if (!isUnderSiege)
			{
				int dailyCastleNobleRecruitCount = UpdateVolunteersOfNotablesPatch.GetDailyCastleNobleRecruitCount(settlement);
				if (dailyCastleNobleRecruitCount > 0)
				{
					CharacterObject eliteBasicTroop = settlement.Culture.EliteBasicTroop;
					if (settlement.Town.GarrisonParty == null)
					{
						settlement.AddGarrisonParty(false);
					}
					if (settlement.Town.GarrisonParty is not null)
                    {
						int val = settlement.Town.GarrisonParty.Party.PartySizeLimit - settlement.Town.GarrisonParty.Party.NumberOfAllMembers;
						int num = Math.Min(dailyCastleNobleRecruitCount, val);
						if (num > 0)
						{
							settlement.Town.GarrisonParty.MemberRoster.AddToCounts(eliteBasicTroop, num, false, 0, 0, true, -1);
							settlement.Prosperity -= SubModule.Settings.CastleRecruitProsperityCost * (float)num;
						}
					}
				}
			}
		}

		public static bool Prefix(ref bool initialRunning)
		{
			foreach (Settlement settlement in Campaign.Current.Settlements)
			{
				if ((settlement.IsTown && !settlement.Town.InRebelliousState) || (settlement.IsVillage && !settlement.Village.Bound.Town.InRebelliousState))
				{
					foreach (Hero hero in settlement.Notables)
					{
						try
						{
							if (hero.CanHaveRecruits)
							{
								bool flag2 = false;
								int i = 0;
								while (i < 6)
								{
									if ((double)MBRandom.RandomFloat < (double)Campaign.Current.Models.VolunteerProductionModel.GetDailyVolunteerProductionProbability(hero, i, settlement))
									{
										flag2 = true;
										if (!UpdateVolunteersOfNotablesPatch.IsBitSet(hero, i))
										{
											bool flag5 = UpdateVolunteersOfNotablesPatch.GenerateBasicTroop(hero, i);
											bool flag6 = !flag5;
											if (!flag6)
											{
												for (int j = 0; j < 3; j++)
												{
													if ((double)MBRandom.RandomFloat < UpdateVolunteersOfNotablesPatch.GetTroopUpgradeChance(hero, i))
													{
														UpdateVolunteersOfNotablesPatch.UpgradeTroop(hero, i);
													}
												}
											}
										}
										else
										{
											if ((double)MBRandom.RandomFloat < UpdateVolunteersOfNotablesPatch.GetTroopUpgradeChance(hero, i))
											{
												UpdateVolunteersOfNotablesPatch.UpgradeTroop(hero, i);
											}
										}
									}
									i++;
								}
								if (flag2)
								{
									UpdateVolunteersOfNotablesPatch.SortNotableVolunteers(hero);
								}
							}
						}
						catch (IndexOutOfRangeException)
						{
						}
					}
				}
				if (settlement.IsCastle && !settlement.InRebelliousState)
				{
					UpdateVolunteersOfNotablesPatch.UpdateCastleNobleRecruit(settlement);
				}
			}
			return false;
		}
	}
}
