using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using System.Collections.Generic;

namespace ConstructionAnimation
{
    // Options (Options > Mods > Construction Animation).
    [FileLocation("ConstructionAnimation")]
    [SettingsUIGroupOrder(kPerformanceGroup, kDiagnosticsGroup)]
    [SettingsUIShowGroupName(kPerformanceGroup, kDiagnosticsGroup)]
    public class Setting : ModSetting
    {
        public const string kSection = "Main";

        public const string kPerformanceGroup = "Performance";

        public const string kDiagnosticsGroup = "Diagnostics";

        private ModLogLevel m_LogLevel = ModLogLevel.Off;

        /// <summary>The loaded options (the second instance LoadSettings creates for defaults is never stored here).</summary>
        public static Setting Instance { get; set; }

        /// <summary>True (default) while the option is on or the options are not loaded yet.</summary>
        public static bool CompanyLookupOwnRentersOnly =>
            Instance == null ||
            Instance.OptimizeCompanyLookup;

        /// <summary>True (default) while the option is on or the options are not loaded yet.</summary>
        public static bool NearSiteThrottleEnabled =>
            Instance == null ||
            Instance.ThrottleNearSites;

        /// <summary>True (default) while the option is on or the options are not loaded yet.</summary>
        public static bool RecutPerFloorEnabled =>
            Instance == null ||
            Instance.RecutPerFloor;

        public Setting(IMod mod) : base(mod)
        {
        }

        [SettingsUISection(kSection, kPerformanceGroup)]
        public bool OptimizeCompanyLookup { get; set; } = true;

        [SettingsUISection(kSection, kPerformanceGroup)]
        public bool ThrottleNearSites { get; set; } = true;

        [SettingsUISection(kSection, kPerformanceGroup)]
        public bool RecutPerFloor { get; set; } = true;

        [SettingsUISection(kSection, kDiagnosticsGroup)]
        public ModLogLevel LogLevel
        {
            get { return m_LogLevel; }
            set
            {
                m_LogLevel = value;
                ModLog.OnLevelChanged();
            }
        }

        public override void SetDefaults()
        {
            OptimizeCompanyLookup = true;
            ThrottleNearSites = true;
            RecutPerFloor = true;
            LogLevel = ModLogLevel.Off;
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
                { m_Setting.GetOptionGroupLocaleID(Setting.kDiagnosticsGroup), "Diagnostics" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OptimizeCompanyLookup)), "Cheaper company banner lookup" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OptimizeCompanyLookup)), "On: a construction site looks for its company in the building's own renter list every 5 seconds, and one shared scan of every renter in the city runs at most every 30 seconds for all sites still waiting. Off: the previous behavior, which scans every renter in the city (households included) for each waiting site every second." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ThrottleNearSites)), "Throttle nearby sites (10 Hz)" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ThrottleNearSites)), "On: construction sites within 650 m run their heavy visual update (building cut, scaffold, crane) about 10 times per second instead of every frame. Off: the previous every-frame update." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RecutPerFloor)), "Re-cut building once per floor" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RecutPerFloor)), "On: the rising building is re-cut (its mesh rebuilt) only when it reaches the next floor, so it rises floor by floor, with cuts on the building's own floor lines (floors closer than 2 m are merged, gaps over 5 m are split, every 3 m if floors were not detected). Off: the previous behavior, re-cut every 0.3-2.5 m depending on camera distance." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LogLevel)), "Log level" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LogLevel)), "Off (default): no diagnostic log file; errors and the load/unload lines still go to the game log. PERF lines only: writes only the [PERF] report (every 10 s) and errors to Logs/ConstructionAnimation-diagnostic.log. Full: the previous, very verbose logging." },
                { m_Setting.GetEnumValueLocaleID(ModLogLevel.Off), "Off" },
                { m_Setting.GetEnumValueLocaleID(ModLogLevel.PerfOnly), "PERF lines only" },
                { m_Setting.GetEnumValueLocaleID(ModLogLevel.Full), "Full" },
            };
        }

        public void Unload()
        {
        }
    }
}
