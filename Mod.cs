using Colossal.IO.AssetDatabase;
using Game;
using Game.Modding;
using Game.SceneFlow;
using ConstructionAnimation.Systems;

namespace ConstructionAnimation
{
    public sealed class Mod : IMod
    {
        private Setting m_Setting;

        public void OnLoad(UpdateSystem updateSystem)
        {
            // Options first, so everything below already sees the saved choices.
            m_Setting = new Setting(this);
            Setting.Instance = m_Setting;
            m_Setting.RegisterInOptionsUI();
            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(m_Setting));
            AssetDatabase.global.LoadSettings("ConstructionAnimation", m_Setting, new Setting(this));

            ModLog.Initialize();

            ModLog.Notice(
                "Construction Animation v0.1 loaded; log level=" +
                ModLog.Level
            );

            ModLog.Checkpoint(
                "MOD OnLoad; diagnostic logging active; version=V1.43.47.4.3.10"
            );

            // V1.43.47.4.3.10: suppress the vanilla construction Sand Surface
            // during Modification2 so the game's own SubAreaReferencesSystem,
            // which runs in Modification2B, can detach the deleted Area before
            // AreaBatchSystem reaches PreCulling.
            updateSystem.UpdateAt<ConstructionSandSuppressionSystem>(
                SystemUpdatePhase.Modification2
            );

            updateSystem.UpdateAt<ConstructionDetectionSystem>(
                SystemUpdatePhase.ModificationEnd
            );
        }

        public void OnDispose()
        {
            ModLog.Checkpoint(
                "MOD OnDispose begin"
            );

            ModLog.Notice(
                "Construction Animation disposed."
            );

            ModLog.Shutdown();

            if (m_Setting != null)
            {
                m_Setting.UnregisterInOptionsUI();
                m_Setting = null;
                Setting.Instance = null;
            }
        }
    }
}
