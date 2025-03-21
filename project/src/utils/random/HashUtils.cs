
using System;
using Godot;

namespace Game.Utils
{
    public static class HashUtils
    {
        public static string RandomMd5()
        {
            var randInt = GD.Randi();
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(randInt.ToString());
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                return Convert.ToHexString(hashBytes);
            }
        }
    }
}