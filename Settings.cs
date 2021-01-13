using System;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Dropdown;
using MCM.Abstractions.Settings.Base.Global;

namespace LightProsperity
{
	public class Settings : AttributeGlobalSettings<Settings>
	{
		public override string Id => "LightProsperity";

		public override string DisplayName => "Light Prosperity";

		public override string FolderName => "LightProsperity";

		public override string FormatType => "json2";

		[SettingPropertyDropdown("{=st3hNa}Bonus Slots For", Order = 0, RequireRestart = false, HintText = "{=FsrLvU}Who can receive extra recruitment slots from notables.")]
		[SettingPropertyGroup("{=nY8kyK}General Settings", GroupOrder = 0)]
		public DropdownDefault <string> BonusSlotsFor { get; set; } = new DropdownDefault<string>(new string[]
		{
			"Everyone",
			"Not at war",
			"Same faction",
			"Same clan"
		}, 2);

		[SettingPropertyFloatingInteger("{=Dt7T5Q}Prisoner Prosperity Value", 0f, 10f, "0.00", Order = 1, RequireRestart = false, HintText = "{=FZTOih}Prosperity increase for selling one prisoner.")]
		[SettingPropertyGroup("{=nY8kyK}General Settings")]
		public float PrisonerProsperityValue { get; set; } = 1f;

		[SettingPropertyFloatingInteger("{=wDiy1T}Militia Growth Bonus", 0f, 10f, "0.00", Order = 2, RequireRestart = true, HintText = "{=qbKYdD}Number of extra militia every day.")]
		[SettingPropertyGroup("{=nY8kyK}General Settings")]
		public float MilitiaGrowthBonus { get; set; } = 0f;

		[SettingPropertyFloatingInteger("{=r7PmHe}Prosperity Growth Multiplier", 0f, 20f, "0.00", Order = 3, RequireRestart = true, HintText = "{=WWmXeh}The multiplier to overall prosperity and hearth growth rate.")]
		[SettingPropertyGroup("{=nY8kyK}General Settings")]
		public float ProsperityGrowthMultiplier { get; set; } = 1f;

		[SettingPropertyBool("{=2w9Irb}Garrison Settings", Order = 0, RequireRestart = true, IsToggle = true, HintText = "{=iYJ2Aw}Modify food consumption and wages of garrison.")]
		[SettingPropertyGroup("{=2w9Irb}Garrison Settings", GroupOrder = 1)]
		public bool ModifyGarrisonConsumption { get; set; } = true;

		[SettingPropertyFloatingInteger("{=bYdOtk}Garrison Wages Multiplier", 0f, 1f, "0.00", Order = 1, RequireRestart = false, HintText = "{=zAi4b4}Multiplier for garrison wages.")]
		[SettingPropertyGroup("{=2w9Irb}Garrison Settings")]
		public float GarrisonWagesMultiplier { get; set; } = 0.5f;

		[SettingPropertyFloatingInteger("{=XFGXdV}Garrison Food Consumption Multiplier", 0f, 1f, "0.00", Order = 2, RequireRestart = false, HintText = "{=S75wJJ}Multplier for garrison food consumption, and how many troops at garrison are lost because of food shortage.")]
		[SettingPropertyGroup("{=2w9Irb}Garrison Settings")]
		public float GarrisonFoodConsumpetionMultiplier { get; set; } = 0f;

		[SettingPropertyBool("{=NDmBnT}New Prosperity Model", Order = 0, RequireRestart = true, IsToggle = true, HintText = "{=cboszz}Enable the new prosperity model. The new model gives settlements a natural new born growth rate and is affected by settlement capacity, trade, food storage and enemies around.")]
		[SettingPropertyGroup("{=NDmBnT}New Prosperity Model", GroupOrder = 2)]
		public bool NewProsperityModel { get; set; } = false;

		[SettingPropertyInteger("{=1DP3ee}Village Natural Growth Capacity", 0, 3000, "0", Order = 1, RequireRestart = false, HintText = "{=udES1n}Natural growth capacity for village. 0 for unlimited.")]
		[SettingPropertyGroup("{=NDmBnT}New Prosperity Model")]
		public int VillageGrowthCap { get; set; } = 800;

		[SettingPropertyInteger("{=8uSaMW}Town Natural Growth Capacity", 0, 20000, "0", Order = 2, RequireRestart = false, HintText = "{=taQ8PZ}Natural growth capacity for towns. Trade, buildings and other bonus can boost prosperity beyond natural capacity. 0 for unlimited.")]
		[SettingPropertyGroup("{=NDmBnT}New Prosperity Model")]
		public int TownGrowthCap { get; set; } = 7000;

		[SettingPropertyInteger("{=hkVfKG}Castle Natural Growth Capacity", 0, 5000, "0", Order = 3, RequireRestart = false, HintText = "{=hjvM5R}Natural growth capacity for castles. Buildings and other bonus can boost prosperity beyond natural capacity. 0 for unlimited.")]
		[SettingPropertyGroup("{=NDmBnT}New Prosperity Model")]
		public int CastleGrowthCap { get; set; } = 1500;

		[SettingPropertyInteger("{=vmWeg5}Town Minimum Prosperity for Recruit", 0, 5000, "0", Order = 0, RequireRestart = false, HintText = "{=5lAFED}If prosperity is below this value, a town will stop generating new recruits.")]
		[SettingPropertyGroup("{=rRxcGb}Town Settings", GroupOrder = 3)]
		public int TownMinProsperityForRecruit { get; set; } = 1000;

		[SettingPropertyInteger("{=zfnNmH}Town Prosperity Threshold", 0, 10000, "0", Order = 1, RequireRestart = false, HintText = "{=DCIx2W}The required prosperity for a town to generate more recruits.")]
		[SettingPropertyGroup("{=rRxcGb}Town Settings")]
		public int TownProsperityThreshold { get; set; } = 3000;

		[SettingPropertyInteger("{=SSS3Uk}Town Prosperity Per Bonus Slot", 0, 10000, "0", Order = 2, RequireRestart = false, HintText = "{=hdv7FT}Amount of prosperity past the threshold required for one extra recruitment slot.")]
		[SettingPropertyGroup("{=rRxcGb}Town Settings")]
		public int TownProsperityPerBonusSlot { get; set; } = 2000;

		[SettingPropertyFloatingInteger("{=VvQ1N7}Town Recruit Prosperity Cost", 0f, 10f, "0.00", Order = 3, RequireRestart = false, HintText = "{=Fua09c}The prosperity cost for one recuit.")]
		[SettingPropertyGroup("{=rRxcGb}Town Settings")]
		public float TownRecruitProsperityCost { get; set; } = 2f;

		[SettingPropertyInteger("{=f0PBIT}Village Minimum Hearth for Recruit", 0, 500, "0", Order = 0, RequireRestart = false, HintText = "{=6J40z3}If hearth is below this value, a village will stop generating new recruits.")]
		[SettingPropertyGroup("{=Sbl08S}Village Settings", GroupOrder = 4)]
		public int VillageMinProsperityForRecruit { get; set; } = 100;

		[SettingPropertyInteger("{=5gY7RW}Village Hearth Threshold", 0, 1000, "0", Order = 1, RequireRestart = false, HintText = "{=XRf31f}The required hearth for a village to generate more recruits.")]
		[SettingPropertyGroup("{=Sbl08S}Village Settings")]
		public int VillageProsperityThreshold { get; set; } = 300;

		[SettingPropertyInteger("{=TdEny1}Village Hearth Per Bonus Slot", 0, 1000, "0", Order = 2, RequireRestart = false, HintText = "{=Pa373a}Amount of hearth past the threshold required for one extra recruitment slot.")]
		[SettingPropertyGroup("{=Sbl08S}Village Settings")]
		public int VillageProsperityPerBonusSlot { get; set; } = 200;

		[SettingPropertyFloatingInteger("{=zkkSzM}Village Recruit Hearth Cost", 0f, 10f, "0.00", Order = 3, RequireRestart = false, HintText = "{=GnGQnu}The hearth cost for one recuit.")]
		[SettingPropertyGroup("{=Sbl08S}Village Settings")]
		public float VillageRecruitProsperityCost { get; set; } = 1f;

		[SettingPropertyInteger("{=0A9Xwm}Castle Minimum Prosperity for Recruit", 0, 2000, "0", Order = 0, RequireRestart = false, HintText = "{=QH4Z4u}If prosperity is below this value, a castle will stop generating new noble recruits.")]
		[SettingPropertyGroup("{=DACgrG}Castle Settings", GroupOrder = 5)]
		public int CastleMinProsperityForRecruit { get; set; } = 500;

		[SettingPropertyInteger("{=2Qgisr}Castle Prosperity Threshold", 0, 20000, "0", Order = 1, RequireRestart = false, HintText = "{=5mN2sb}Controls the chance of castles getting noble recruits. If prosperity reaches the threshold, the castle is guaranteed to get one noble recruit every day.")]
		[SettingPropertyGroup("{=DACgrG}Castle Settings")]
		public int CastleProsperityThreshold { get; set; } = 7500;

		[SettingPropertyFloatingInteger("{=3eTGOO}Castle Recruit Prosperity Cost", 0f, 10f, "0.00", Order = 2, RequireRestart = false, HintText = "{=Fua09c}The prosperity cost for one recuit.")]
		[SettingPropertyGroup("{=DACgrG}Castle Settings")]
		public float CastleRecruitProsperityCost { get; set; } = 2f;

		[SettingPropertyInteger("{=izsqza}Notable Power Threshold For Noble Recruit", 0, 1000, "0", Order = 0, RequireRestart = false, HintText = "{=bh2GwT}Controls the chance of rural notables getting noble recruits. If power reaches the threshold, the notable is guaranteed to get notable recruits at every chance.")]
		[SettingPropertyGroup("{=JZ32iR}Notable Settings", GroupOrder = 6)]
		public int NotablePowerThresholdForNobleRecruit { get; set; } = 600;

		[SettingPropertyFloatingInteger("{=CpOhvD}Notable Noble Recruit Power Cost", 0f, 10f, "0.00", Order = 1, RequireRestart = false, HintText = "{=45urKB}The power cost for one noble recuit.")]
		[SettingPropertyGroup("{=JZ32iR}Notable Settings")]
		public float NotableNobleRecruitPowerCost { get; set; } = 1f;
	}
}
