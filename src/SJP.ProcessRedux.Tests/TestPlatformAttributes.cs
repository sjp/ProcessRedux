using System;
using System.Runtime.InteropServices;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using OSPlatform = System.Runtime.InteropServices.OSPlatform;

namespace SJP.ProcessRedux.Tests
{
    internal static class TestPlatform
    {
        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
        internal sealed class Windows : NUnitAttribute, IApplyToTest
        {
            public void ApplyToTest(Test test)
            {
                if (test.RunState == RunState.NotRunnable || _isWindows)
                    return;

                test.RunState = RunState.Ignored;

                const string reason = "This test is ignored because the current platform is non-Windows and the test is for Windows platforms only.";
                test.Properties.Set(PropertyNames.SkipReason, reason);
            }

            private readonly static bool _isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
        internal sealed class Osx : NUnitAttribute, IApplyToTest
        {
            public void ApplyToTest(Test test)
            {
                if (test.RunState == RunState.NotRunnable || _isOsx)
                    return;

                test.RunState = RunState.Ignored;

                const string reason = "This test is ignored because the current platform is non-OSX and the test is for OSX platforms only.";
                test.Properties.Set(PropertyNames.SkipReason, reason);
            }

            private readonly static bool _isOsx = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        }

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
        internal sealed class Linux : NUnitAttribute, IApplyToTest
        {
            public void ApplyToTest(Test test)
            {
                if (test.RunState == RunState.NotRunnable || _isLinux)
                    return;

                test.RunState = RunState.Ignored;

                const string reason = "This test is ignored because the current platform is non-Linux and the test is for Linux platforms only.";
                test.Properties.Set(PropertyNames.SkipReason, reason);
            }

            private readonly static bool _isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        }
    }
}
