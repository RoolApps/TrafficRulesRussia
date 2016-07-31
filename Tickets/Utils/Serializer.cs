using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Utils {
    public static class Serializer {
        public static string SerializeToString( object toSerialize ) {
            string str = "";
            try {
                using(MemoryStream mStream = new MemoryStream()) {
                    DataContractSerializer serializer = new DataContractSerializer(toSerialize.GetType());
                    serializer.WriteObject(mStream, toSerialize);
                    mStream.Position = 0;
                    using(StreamReader sStream = new StreamReader(mStream)) {
                        str = sStream.ReadToEnd();
                    }
                }
            } catch(Exception e) {
                System.Diagnostics.Debug.WriteLine("Can't Serialize: {0}", e.Message);
            }
            return str;
        }

        public static T DeserializeFromString<T>( string toDeserialize ) {
            var bytes = Encoding.UTF8.GetBytes(toDeserialize);
            try {
                using(MemoryStream mStream = new MemoryStream(bytes)) {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                    mStream.Position = 0;
                    return (T)serializer.ReadObject(mStream);
                }
            } catch(Exception e) {
                System.Diagnostics.Debug.WriteLine("Can't Deserialize: {0}", e.Message);
            }
            return default(T);
        }
    }
}
