﻿using System;
using System.Runtime.InteropServices;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace SJP.ProcessRedux.Tests
{
    public static class TestPlatform
    {
        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
        public sealed class Windows : NUnitAttribute, IApplyToTest
        {
            public void ApplyToTest(Test test)
            {
                if (test.RunState == RunState.NotRunnable || _isWindows)
                    return;

                test.RunState = RunState.Ignored;

                var reason = "This test is ignored because the current platform is non-Windows and the test is for Windows platforms only.";
                test.Properties.Set(PropertyNames.SkipReason, reason);
            }

            private readonly static bool _isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
        public sealed class NonWindows : NUnitAttribute, IApplyToTest
        {
            public void ApplyToTest(Test test)
            {
                if (test.RunState == RunState.NotRunnable || _isNotWindows)
                    return;

                test.RunState = RunState.Ignored;

                var reason = "This test is ignored because the current platform is Windows and the test is for non-Windows platforms only.";
                test.Properties.Set(PropertyNames.SkipReason, reason);
            }

            private readonly static bool _isNotWindows = !RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }
    }
}
