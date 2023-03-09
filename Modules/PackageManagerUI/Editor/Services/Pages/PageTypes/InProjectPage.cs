// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEditor.PackageManager.UI.Internal
{
    [Serializable]
    internal class InProjectPage : SimplePage
    {
        public const string k_Id = "InProject";

        public override string id => k_Id;
        public override string displayName => L10n.Tr("In Project");

        public override RefreshOptions refreshOptions => RefreshOptions.UpmList | RefreshOptions.ImportedAssets;

        public override IEnumerable<PageFilters.Status> supportedStatusFilters
        {
            get
            {
                yield return PageFilters.Status.UpdateAvailable;
                if (visualStates.Any(v => m_PackageDatabase.GetPackage(v.packageUniqueId)?.hasEntitlements == true))
                    yield return PageFilters.Status.SubscriptionBased;
            }
        }

        public InProjectPage(PackageDatabase packageDatabase) : base(packageDatabase) {}

        public override bool ShouldInclude(IPackage package)
        {
            return package != null
                   && !package.versions.All(v => v.HasTag(PackageTag.BuiltIn))
                   && (package.progress == PackageProgress.Installing || package.versions.installed != null || package.versions.Any(v => v.importedAssets?.Any() == true));
        }

        public override string GetGroupName(IPackage package)
        {
            if (package.product != null)
                return L10n.Tr("Packages - Asset Store");
            var version = package.versions.primary;
            if (version.HasTag(PackageTag.Unity))
                return version.HasTag(PackageTag.Feature) ? L10n.Tr("Features") : L10n.Tr("Packages - Unity");
            return string.IsNullOrEmpty(version.author) ? L10n.Tr("Packages - Other") : string.Format(L10n.Tr("Packages - {0}"), version.author);
        }
    }
}
