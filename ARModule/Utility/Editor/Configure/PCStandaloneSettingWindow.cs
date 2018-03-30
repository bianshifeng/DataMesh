using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DataMesh.AR.Utility
{
    public class PCStandaloneSettingWindow : AbstractConfigureWindow<PCStandaloneSettingWindow.PCSetting>
    {
        public enum PCSetting
        {
            BuildForPC,
            ChangeAPILevelToDotNet2,
            CloseVSync //
        }

        protected override void ApplySettings()
        {
            if (Values[PCSetting.BuildForPC])
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
            }

            if (Values[PCSetting.ChangeAPILevelToDotNet2])
            {
                if (PlayerSettings.scriptingRuntimeVersion != ScriptingRuntimeVersion.Latest)
                    PlayerSettings.scriptingRuntimeVersion = ScriptingRuntimeVersion.Latest;
                PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Standalone, ApiCompatibilityLevel.NET_4_6);
            }

            if(Values[PCSetting.CloseVSync])
            {
                string[] names = QualitySettings.names;
                for(int i = 0;i < names.Length;i++)
                {
                    QualitySettings.SetQualityLevel(i);
                    QualitySettings.vSyncCount = 0;
                }
            }
        }

        protected override void LoadSettings()
        {
            for (int i = (int)PCSetting.BuildForPC; i <= (int)PCSetting.CloseVSync; i++)
            {
                Values[(PCSetting)i] = true;
            }
        }

        protected override void LoadStrings()
        {
            Names[PCSetting.BuildForPC] = "Build For PC";
            Descriptions[PCSetting.BuildForPC] = "Set the build target for PC";

            Names[PCSetting.ChangeAPILevelToDotNet2] = "Change API Level To DotNet 4.6";
            Descriptions[PCSetting.ChangeAPILevelToDotNet2] = "Change API Compatibility level to .Net 4.6";

            Names[PCSetting.CloseVSync] = "Close All Level VSync";
            Descriptions[PCSetting.CloseVSync] = "Close All Level VSync";

        }

        protected override void OnEnable()
        {
            // Pass to base first
            base.OnEnable();

            // Set size
            this.minSize = new Vector2(350, 240);
            this.maxSize = this.minSize;
        }


    }

}
