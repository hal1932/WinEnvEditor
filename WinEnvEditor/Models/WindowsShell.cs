using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace WinEnvEditor.Models
{
    public class WindowsShell
    {
        #region プロセス
        public static int StartProcessSync(
            string command, string[] args = null,
            Dictionary<string, string> env = null,
            Action<string> onReceiveOutput = null, Action<string> onReceiveError = null,
            Func<string> onRequestInput = null
            )
        {
            var process = StartProcess(command, args, env, onReceiveOutput, onReceiveError, onRequestInput);
            if (process == null) return -1;

            process.WaitForExit();

            var returnCode = process.ExitCode;
            process.Dispose();

            return returnCode;
        }

        public static Process StartProcess(
            string command, string[] args = null,
            Dictionary<string, string> env = null,
            Action<string> onReceiveOutput = null, Action<string> onReceiveError = null,
            Func<string> onRequestInput = null
            )
        {
            var process = new Process();

            process.StartInfo.FileName = command;

            if (args != null)
            {
                process.StartInfo.Arguments = string.Join(" ", args);
            }

            if (env != null)
            {
                foreach(KeyValuePair<string, string> item in env)
                {
                    process.StartInfo.EnvironmentVariables.Add(item.Key, item.Value);
                }
            }

            var useShellExecute = true;
            if (onReceiveOutput != null)
            {
                process.StartInfo.RedirectStandardOutput = true;
                useShellExecute = false;
            }
            if (onReceiveError != null)
            {
                process.StartInfo.RedirectStandardError = true;
                useShellExecute = false;
            }
            if (onRequestInput != null)
            {
                process.StartInfo.RedirectStandardInput = true;
                useShellExecute = false;
            }
            process.StartInfo.UseShellExecute = useShellExecute;

            if(!process.Start())
            {
                return null;
            }

            if (onReceiveOutput != null) process.BeginOutputReadLine();
            if (onReceiveError != null) process.BeginErrorReadLine();
            if (onRequestInput != null)
            {
                using (var input = process.StandardInput)
                {
                    input.Write(onRequestInput());
                    input.Close();
                }
            }

            return process;
        }
        #endregion

        #region 環境変数
        public static bool SetEnvironmentVariableSync(string key, string value, bool user = true)
        {
            if (user)
            {
                SetRegistryUser(@"Environment", key, value);
            }
            else
            {
                SetRegistryMachine(@"SYSTEM\CurrentControlSet\Control\Session Manager\Environment", key, value);
            }
            return true;
        }

        public static bool DeleteEnvironmentVariableSync(string key, bool user = true)
        {
            if (user)
            {
                DeleteRegistryUser(@"Environment", key);
            }
            else
            {
                DeleteRegistryMachine(@"SYSTEM\CurrentControlSet\Control\Session Manager\Environment", key);
            }
            return true;
        }

        public static bool AddEnvironmentVariableSync(string key, string value, bool user = true)
        {
            return true;
        }

        public static bool SubstructEnvironmentVariableSync(string key, string value, bool user = true)
        {
            return true;
        }
        #endregion

        #region レジストリ
        public static void SetRegistryUser(string path, string key, string value)
        {
            RegistryKey regkey = Registry.CurrentUser.OpenSubKey(path, true);
            regkey.SetValue(key, value);
            regkey.Close();
        }

        public static void SetRegistryMachine(string path, string key, string value)
        {
            RegistryKey regkey = Registry.LocalMachine.OpenSubKey(path, true);
            regkey.SetValue(key, value);
            regkey.Close();
        }

        public static void DeleteRegistryUser(string path, string key)
        {
            RegistryKey regkey = Registry.CurrentUser.OpenSubKey(path, true);
            regkey.DeleteValue(key, false);
            regkey.Close();
        }

        public static void DeleteRegistryMachine(string path, string key)
        {
            RegistryKey regkey = Registry.LocalMachine.OpenSubKey(path, true);
            regkey.DeleteValue(key, false);
            regkey.Close();
        }
        #endregion
    }
}
