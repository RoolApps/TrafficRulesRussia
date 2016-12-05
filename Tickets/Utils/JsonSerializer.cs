using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Utils {
    public class JsonSerializer {
        public static string Serialize(object objToSerialize) {
            string jsonString = "";
            try {
                using ( MemoryStream mStream = new MemoryStream() ) {
                    DataContractSerializer jsonSerializer = new DataContractSerializer(objToSerialize.GetType());
                    jsonSerializer.WriteObject(mStream, objToSerialize);
                    mStream.Position = 0;
                    using ( StreamReader srStream = new StreamReader(mStream) ) {
                        jsonString = srStream.ReadToEnd();
                    }
                }
            } catch ( Exception e ) {
                System.Diagnostics.Debug.WriteLine("Cant Serialize to xml format: {0}", e.Message);
            }
            return jsonString;
        }

        public static T Deserialize<T>(string jsonString) {
            T jsonObject = default(T);
            try {
                var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
                using ( MemoryStream mStream = new MemoryStream(jsonBytes) ) {
                    DataContractSerializer jsonSerializer = new DataContractSerializer(typeof(T));
                    mStream.Position = 0;
                    jsonObject = (T)jsonSerializer.ReadObject(mStream);
                }
            } catch ( Exception e ) {
                System.Diagnostics.Debug.WriteLine("Cant Deserialize from xml format: {0}", e.Message);
            }
            return jsonObject;
        }
    }
}
