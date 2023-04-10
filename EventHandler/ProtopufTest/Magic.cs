using ProtoBuf;

namespace ProtopufTest
{
    public static class Magic
    {
        public static byte[] Serialize(object obj)
        {
            if (obj == null) return null;


            using (MemoryStream stream = new MemoryStream())
            {
                Serializer.Serialize(stream, obj);
                return stream.ToArray();
            }

        }

        public static T Deserialize<T>(byte[] data) where T : class
        {
            if (data == null) return null;
            using (MemoryStream stream = new MemoryStream(data))
            {
                return Serializer.Deserialize<T>(stream);
            }


        }
    }
}
