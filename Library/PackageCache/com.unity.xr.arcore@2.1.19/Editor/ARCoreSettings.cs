using UnityEngine;

namespace UnityEditor.XR.ARCore
{
    /// <summary>
    /// Holds settings that are used to configure the Unity ARCore Plugin.
    /// </summary>
    public class ARCoreSettings : ScriptableObject
    {
        /// <summary>
        /// Enum which defines whether ARCore is optional or required.
        /// </summary>
        public enum Requirement
        {
            /// <summary>
            /// ARCore is required, which means the app cannot be installed on devices that do not support ARCore.
            /// </summary>
            Required,

            /// <summary>
            /// ARCore is optional, which means the the app can be installed on devices that do not support ARCore.
            /// </summary>
            Optional
        }

        [SerializeField, Tooltip("Toggles whether ARCore is required for this app. Will make app only downloadable by devices with ARCore support if set to 'Required'.")]
        Requirement m_Requirement;

        /// <summary>
        /// Determines whether ARCore is required for this app: will make app only downloadable by devices with ARCore support if set to <see cref="Requirement.Required"/>.
        /// </summary>
        public Requirement requirement
        {
            get => m_Requirement;
            set
            {
                if (value != m_Requirement)
                {
                    m_Requirement = value;
                    EditorUtility.SetDirty(this);
                }
            }
        }

        [SerializeField, Tooltip("Toggles whether the Gradle version is validated during Player build.")]
        bool m_IgnoreGradleVersion;

        /// <summary>
        /// Whether the Gradle version is validated during Player build.
        /// </summary>
        internal bool ignoreGradleVersion
        {
            get => m_IgnoreGradleVersion;
            set
            {
                if (value != m_IgnoreGradleVersion)
                {
                    m_IgnoreGradleVersion = value;
                    EditorUtility.SetDirty(this);
                }
            }
        }

        /// <summary>
        /// Gets the currently selected settings, or create a default one if no <see cref="ARCoreSettings"/> has been set in Player Settings.
        /// </summary>
        /// <returns>The ARCore settings to use for the current Player build.</returns>
        public static ARCoreSettings GetOrCreateSettings()
        {
            var settings = currentSettings;
            if (settings != null)
                return settings;

            return CreateInstance<ARCoreSettings>();
        }

        /// <summary>
        /// Get or set the <see cref="ARCoreSettings"/> that will be used for the player build.
        /// </summary>
        public static ARCoreSettings currentSettings
        {
            get
            {
                ARCoreSettings settings = null;
                EditorBuildSettings.TryGetConfigObject(k_ConfigObjectName, out settings);
                return settings;
            }

            set
            {
                if (value == null)
                {
                    EditorBuildSettings.RemoveConfigObject(k_ConfigObjectName);
                }
                else
                {
                    EditorBuildSettings.AddConfigObject(k_ConfigObjectName, value, true);
                }
            }
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }

        internal static bool TrySelect()
        {
            var settings = currentSettings;
            if (settings == null)
                return false;

            Selection.activeObject = settings;
            return true;
        }

        static readonly string k_ConfigObjectName = "com.unity.xr.arcore.PlayerSettings";
    }
}
