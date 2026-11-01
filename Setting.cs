using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using System.Collections.Generic;

namespace ConstructionAnimation
{
    // Options (Options > Mods > Construction Animation).
    [FileLocation("ConstructionAnimation")]
    [SettingsUIGroupOrder(kPerformanceGroup)]
    [SettingsUIShowGroupName(kPerformanceGroup)]
    public class Setting : ModSetting
    {
        public const string kSection = "Main";

        public const string kPerformanceGroup = "Performance";

        /// <summary>The loaded options (the second instance LoadSettings creates for defaults is never stored here).</summary>
        public static Setting Instance { get; set; }

        /// <summary>True (default) while the option is on or the options are not loaded yet.</summary>
        public static bool CompanyLookupOwnRentersOnly =>
            Instance == null ||
            Instance.OptimizeCompanyLookup;

        public Setting(IMod mod) : base(mod)
        {
        }

        [SettingsUISection(kSection, kPerformanceGroup)]
        public bool OptimizeCompanyLookup { get; set; } = true;

        public override void SetDefaults()
        {
            OptimizeCompanyLookup = true;
        }
    }

    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "Construction Animation" },
                { m_Setting.GetOptionTabLocaleID(Setting.kSection), "Main" },

                { m_Setting.GetOptionGroupLocaleID(Setting.kPerformanceGroup), "Performance" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OptimizeCompanyLookup)), "Cheaper company banner lookup" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OptimizeCompanyLookup)), "On: a construction site looks for its company in the building's own renter list every 5 seconds, and one shared scan of every renter in the city runs at most every 30 seconds for all sites still waiting. Off: the previous behavior, which scans every renter in the city (households included) for each waiting site every second." },
            };
        }

        public void Unload()
        {
        }
    }
}
