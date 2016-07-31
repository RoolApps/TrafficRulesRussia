using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Utils {
    public static class SettingSaver {
        public static async Task SaveSettingToFile( string fileName, string text ) {
            try {
                StorageFolder sFolder = ApplicationData.Current.LocalFolder;
                StorageFile sFile = await sFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(sFile, text);
            } catch(Exception e) {
                System.Diagnostics.Debug.WriteLine("Can't SaveSettingToFile: {0}", e.Message);
            }
        }

        public static async Task<string> GetSettingFromFile( string fileName ) {
            try {
                StorageFolder sFolder = ApplicationData.Current.LocalFolder;
                StorageFile sFile = await sFolder.GetFileAsync(fileName);
                return await FileIO.ReadTextAsync(sFile);
            } catch(Exception e){
                System.Diagnostics.Debug.WriteLine("Can't TakeSettingFromFile: {0}", e.Message);
            }
            return "";
        }

        public static void SaveSetting( string key, string obj ) {
            ApplicationDataContainer appData = ApplicationData.Current.LocalSettings;
            appData.Values[key] = obj;
        }

        public static string GetSetting( string key ) {
            string setting = "";
            ApplicationDataContainer appData = ApplicationData.Current.LocalSettings;
            if(appData.Values.ContainsKey(key)) {
                setting = appData.Values[key].ToString();
            }
            return setting;
        }
    }
}
