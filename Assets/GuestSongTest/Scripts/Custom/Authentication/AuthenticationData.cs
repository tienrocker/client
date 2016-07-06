using System.IO;
namespace Photon.LoadBalancing.Custom
{
    public class AuthenticationData
    {
        public enum Type : int { DIRECT, }
        public string username { get; set; }
        public string password { get; set; }
        public string nickname { get; set; }
        public bool register { get; set; }
        public Type type { get; set; }

        public byte[] Serialize()
        {
            using (MemoryStream m = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(m))
                {
                    writer.Write(username);
                    writer.Write(password);
                    writer.Write(nickname);
                    writer.Write(register);
                    writer.Write((int)type);
                }
                return m.ToArray();
            }
        }

        public static AuthenticationData Desserialize(byte[] data)
        {
            AuthenticationData result = new AuthenticationData();
            using (MemoryStream m = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(m))
                {
                    result.username = reader.ReadString();
                    result.password = reader.ReadString();
                    result.nickname = reader.ReadString();
                    result.register = reader.ReadBoolean();
                    result.type = (Type)reader.ReadInt16();
                }
            }
            return result;
        }
    }
}