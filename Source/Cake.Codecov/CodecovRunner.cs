// <copyright file="CodecovRunner.cs" company="Cake Contrib">
// Copyright (c) 2017-2021 Larz White, Kim J. Nordmo and Cake Contrib.
// Licensed under the MIT license. See LICENSE in the project.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

using Cake.Codecov.Internals;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;

namespace Cake.Codecov
{
    internal sealed class CodecovRunner : Tool<CodecovSettings>
    {
        private readonly IPlatformDetector _platformDetector;

        internal CodecovRunner(IFileSystem fileSystem, ICakeEnvironment environment, IProcessRunner processRunner, IToolLocator tools)
            : this(new PlatformDetector(), fileSystem, environment, processRunner, tools)
        {
        }

        internal CodecovRunner(IPlatformDetector platformDetector, IFileSystem fileSystem, ICakeEnvironment environment, IProcessRunner processRunner, IToolLocator tools)
            : base(fileSystem, environment, processRunner, tools)
        {
            _platformDetector = platformDetector ?? throw new ArgumentNullException(nameof(platformDetector));
        }

        internal void Run(CodecovSettings settings)
        {
            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            Run(settings, GetArguments(settings));
        }

        protected override IEnumerable<string> GetToolExecutableNames()
        {
            if (_platformDetector.IsLinuxPlatform())
            {
                yield return "codecov-linux";
                yield return "linux-x64/codecov";
                yield return "codecov";
            }
            else if (_platformDetector.IsOsxPlatform())
            {
                yield return "codecov-macos";
                yield return "osx-x64/codecov";
                yield return "codecov";
            }
            else
            {
                // The official version of Codecov CLI can not run with the exe
                // on anything other than Windows. As such we only return this
                // in the else statement.
                yield return "codecov.exe";
            }
        }

        protected override string GetToolName() => "Codecov";

        private static void AddValue(ProcessArgumentBuilder builder, string key, IEnumerable<string> value)
        {
            if (key.Equals("--file", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Select(v => NormalizePath(v));

                var combinedValues = '"' + string.Join(
                    "\",\"",
                    value) + '"';

                // In this case we know it isn't a secret value,
                // so no need to do any additional handling.
                builder.AppendSwitch(
                    key,
                    " ",
                    combinedValues);
                return;
            }

            var allValues = value.ToList();
            AddValue(builder, key, allValues.FirstOrDefault());
            foreach (var subValue in allValues.Skip(1))
            {
                AddValue(builder, key, subValue, true);
            }
        }

        private static void AddValue(ProcessArgumentBuilder builder, string key, Uri value)
        {
            if (value.IsWellFormedOriginalString())
            {
                AddValue(builder, key, value.ToString());
            }
        }

        private static void AddValue(ProcessArgumentBuilder builder, KeyValuePair<string, object> argument, bool value)
        {
            if (value)
            {
                builder.Append(argument.Key);
            }
        }

        private static void AddValue(ProcessArgumentBuilder builder, string key, string? value, bool onlyAppendValue = false)
        {
            if (value == null)
            {
                return;
            }

            if (key.Equals("--file", StringComparison.OrdinalIgnoreCase))
            {
                value = NormalizePath(value);
            }

            if (key.StartsWith("!", StringComparison.OrdinalIgnoreCase))
            {
                if (onlyAppendValue)
                {
                    builder.AppendQuoted(value);
                }
                else
                {
                    builder.AppendSwitchQuotedSecret(key.TrimStart('!'), " ", value);
                }
            }
            else
            {
                if (onlyAppendValue)
                {
                    builder.AppendQuoted(value);
                }
                else
                {
                    builder.AppendSwitchQuoted(key, " ", value);
                }
            }
        }

        private static string NormalizePath(ReadOnlySpan<char> value)
        {
            Span<char> normalizedPath = stackalloc char[value.Length];

            for (var i = 0; i < value.Length; i++)
            {
                if (value[i] == System.IO.Path.AltDirectorySeparatorChar)
                {
                    normalizedPath[i] = System.IO.Path.DirectorySeparatorChar;
                }
                else
                {
                    normalizedPath[i] = value[i];
                }
            }

            return normalizedPath.ToString();
        }

        private static ProcessArgumentBuilder GetArguments(CodecovSettings settings)
        {
            var builder = new ProcessArgumentBuilder();

            foreach (var argument in settings.GetAllArguments())
            {
                switch (argument.Value)
                {
                    case bool value:
                        AddValue(builder, argument, value);
                        break;

                    case Uri value:
                        AddValue(builder, argument.Key, value);
                        break;

                    case IEnumerable<string> value:
                        AddValue(builder, argument.Key, value);
                        break;

                    default:
                        AddValue(builder, argument.Key, argument.Value.ToString());
                        break;
                }
            }

            return builder;
        }
    }
}
