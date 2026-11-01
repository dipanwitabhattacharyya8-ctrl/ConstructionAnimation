using Colossal.Logging;
using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace ConstructionAnimation
{
    /// <summary>How much ModLog writes (Options > Diagnostics).</summary>
    public enum ModLogLevel
    {
        Off = 0,
        PerfOnly = 1,
        Full = 2
    }

    internal static class ModLog
    {
        private static readonly ILog s_Log =
            LogManager.GetLogger(
                "ConstructionAnimation"
            );

        private static readonly object s_FileLock =
            new object();

        private static StreamWriter s_DiagnosticWriter;
        private static string s_DiagnosticPath;
        private static bool s_Initialized;

        public static string DiagnosticPath =>
            s_DiagnosticPath;

        /// <summary>Off until the options are loaded.</summary>
        internal static ModLogLevel Level =>
            Setting.Instance != null
                ? Setting.Instance.LogLevel
                : ModLogLevel.Off;

        /// <summary>
        /// Called when the option changes. Off closes the diagnostic file;
        /// it is reopened lazily by the next write when the level is raised.
        /// </summary>
        internal static void OnLevelChanged()
        {
            if (
                Level == ModLogLevel.Off
            )
            {
                Shutdown();
            }
        }

        public static void Initialize()
        {
            // Off: do not even create or open the diagnostic file.
            if (
                Level == ModLogLevel.Off
            )
            {
                return;
            }

            lock (s_FileLock)
            {
                if (s_Initialized)
                {
                    return;
                }

                s_Initialized = true;

                try
                {
                    string logDirectory =
                        Path.Combine(
                            Application.persistentDataPath,
                            "Logs"
                        );

                    Directory.CreateDirectory(
                        logDirectory
                    );

                    s_DiagnosticPath =
                        Path.Combine(
                            logDirectory,
                            "ConstructionAnimation-diagnostic.log"
                        );

                    FileStream stream =
                        new FileStream(
                            s_DiagnosticPath,
                            FileMode.Append,
                            FileAccess.Write,
                            FileShare.ReadWrite,
                            4096,
                            FileOptions.WriteThrough
                        );

                    s_DiagnosticWriter =
                        new StreamWriter(
                            stream,
                            new UTF8Encoding(
                                false
                            )
                        );

                    s_DiagnosticWriter.AutoFlush =
                        true;

                    s_DiagnosticWriter.WriteLine();
                    s_DiagnosticWriter.WriteLine(
                        "================ NEW SESSION ================"
                    );

                    WriteDiagnosticLineUnsafe(
                        "SESSION",
                        "ConstructionAnimation diagnostic session started."
                    );
                }
                catch (Exception ex)
                {
                    s_DiagnosticWriter =
                        null;

                    TryWrite(
                        () => s_Log.Error(
                            "Failed to initialize diagnostic file: " +
                            ex
                        )
                    );
                }
            }
        }

        public static void Shutdown()
        {
            lock (s_FileLock)
            {
                try
                {
                    if (s_DiagnosticWriter != null)
                    {
                        WriteDiagnosticLineUnsafe(
                            "SESSION",
                            "ConstructionAnimation diagnostic session ended normally."
                        );

                        s_DiagnosticWriter.Flush();
                        s_DiagnosticWriter.Dispose();
                    }
                }
                catch
                {
                }
                finally
                {
                    s_DiagnosticWriter =
                        null;

                    s_Initialized =
                        false;
                }
            }
        }

        public static void Info(
            string message
        )
        {
            if (
                Level != ModLogLevel.Full
            )
            {
                return;
            }

            TryWrite(
                () => s_Log.Info(
                    message
                )
            );
        }

        /// <summary>
        /// Game log only, at every level: for the few one-time lines (mod
        /// loaded/unloaded) that show the mod is running even when logging
        /// is Off.
        /// </summary>
        public static void Notice(
            string message
        )
        {
            TryWrite(
                () => s_Log.Info(
                    message
                )
            );
        }

        public static void Error(
            string message
        )
        {
            TryWrite(
                () => s_Log.Error(
                    message
                )
            );

            WriteDiagnosticLine(
                "ERROR",
                message
            );
        }

        public static void Diagnostic(
            string message
        )
        {
            WriteDiagnosticLine(
                "DIAG",
                message
            );
        }

        public static void Checkpoint(
            string message
        )
        {
            WriteDiagnosticLine(
                "CHECKPOINT",
                message
            );
        }

        private static void WriteDiagnosticLine(
            string category,
            string message
        )
        {
            ModLogLevel level =
                Level;

            // Off: nothing, and no lock or file I/O. PerfOnly: only the
            // [PERF] report lines and errors. Full: everything, as before.
            if (
                level == ModLogLevel.Off ||
                (
                    level == ModLogLevel.PerfOnly &&
                    category != "ERROR" &&
                    (
                        message == null ||
                        !message.StartsWith(
                            "[PERF]",
                            StringComparison.Ordinal
                        )
                    )
                )
            )
            {
                return;
            }

            lock (s_FileLock)
            {
                if (!s_Initialized)
                {
                    Initialize();
                }

                try
                {
                    WriteDiagnosticLineUnsafe(
                        category,
                        message
                    );
                }
                catch
                {
                }
            }
        }

        private static void WriteDiagnosticLineUnsafe(
            string category,
            string message
        )
        {
            if (s_DiagnosticWriter == null)
            {
                return;
            }

            s_DiagnosticWriter.WriteLine(
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] " +
                $"[{category}] {message}"
            );
        }

        private static void TryWrite(
            Action writeAction
        )
        {
            try
            {
                writeAction?.Invoke();
            }
            catch
            {
            }
        }
    }
}
