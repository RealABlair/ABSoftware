using ABSoftware.ServerFiles.Utils;
using System;
using System.IO;

namespace ABSoftware.ServerFiles
{
    public class Config
    {
        public static int Port = 8921;
        public static int StartHealth = 10;
        public static int LobbySize = 2;

        public static void LoadConfig()
        {
            if(File.Exists(FilePaths.CONFIG_PATH))
            {
                KLIN k = new KLIN();
                k.Parse(File.ReadAllText(FilePaths.CONFIG_PATH));
                Port = int.Parse(k.Get("Port").ToString());
                StartHealth = int.Parse(k.Get("StartHealth").ToString());
                LobbySize = int.Parse(k.Get("LobbySize").ToString());
            }
            else
            {
                KLIN k = new KLIN();
                Port = 8921;
                StartHealth = 10;
                LobbySize = 2;
                k.Add("Port", Port);
                k.Add("StartHealth", StartHealth);
                k.Add("LobbySize", LobbySize);
                File.WriteAllText(FilePaths.CONFIG_PATH, k.ToString());
            }
        }
    }
}
