using System;
using System.Resources;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Microsoft.Win32;
using System.Reflection;

namespace Witcher2SaveRestorer
{
    class SaveRestorer
    {
        public List<string> Data;
        public bool Loaded = false;
        public event EventHandler OnChange;

    private ResourceManager loc = new ResourceManager("Witcher2SaveRestorer.strings", Assembly.GetExecutingAssembly());

        private const string C_Key = "Software\\CD Projekt Red\\The Witcher 2";
        private const string C_Value = "GameData";
        
        public void CheckAccess()
        {
            Loaded = false;            
            RegistryKey hk = Registry.CurrentUser.OpenSubKey(C_Key);
            if (hk == null)
            {
                throw new Exception(loc.GetString("strGameNotFound"));
            }
            hk.Close();
            Loaded = true;            
        }

        public bool Check()
        {
            if (!Loaded)
                return false;

            RegistryKey hk = Registry.CurrentUser.OpenSubKey(C_Key);
            if (hk == null)
                throw new Exception(loc.GetString("strRegKeyNotFound"));
                        
            byte[] bytes = (byte[])hk.GetValue(C_Value);
            hk.Close();
            if (bytes == null)
                return false;

            string current = "";
            for (int i = 0; i < bytes.Length; i++)
                current += string.Format(" {0:X2}", bytes[i]);

            if ((Data.Count == 0) || !string.Equals(current, Data.Last()))
            {
                Data.Add(current);
                Save();
                OnChange(this, null);
                return true;
            }
            return false;
        }

        public bool Restore()
        {
            if (!Loaded)
                return false;

            Check();
            if (Data.Count > 1)
            {
                Data.RemoveAt(Data.Count - 1);
                Save();
            }

            if (Data.Count == 0)
                throw new Exception(loc.GetString("strNoData"));

            string current = Data.Last();
            byte[] bytes = new byte[current.Length / 3];
            for (int i = 0; i < bytes.Length; i++)
            {
                string part = current.Substring(i * 3, 3);
                bytes[i] = byte.Parse(part, System.Globalization.NumberStyles.HexNumber);
            }

            RegistryKey hk = Registry.CurrentUser.OpenSubKey(C_Key, true);
            if (hk == null)
                throw new Exception(loc.GetString("strRegKeyNotFound"));
            hk.SetValue(C_Value, bytes);
            hk.Close();

            OnChange(null, null);

            return true;
        }

        public void Load()
        {            
            Properties.Settings.Default.Upgrade();
            Data = Properties.Settings.Default.GameData.Cast<string>().ToList();
            OnChange(this, null);
        }

        public void Save()
        {
            if (Data == null)
                return;

            Properties.Settings.Default.GameData.Clear();            
            Properties.Settings.Default.GameData.AddRange(Data.ToArray());
            Properties.Settings.Default.Save();
        }        
    }
}
