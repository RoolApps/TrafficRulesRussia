
namespace SharedApplication
{
    public static class Resources
    {
        private static System.Collections.Generic.Dictionary<string, object> ResourcesDictionary = new System.Collections.Generic.Dictionary<string, object>();

        public static System.String ConnectionString
        {
            get
            {
                return GetResourceByName<System.String>("ConnectionString");
            }
            set
            {
                SetResourceByName<System.String>("ConnectionString", value);
            }
        }
        
        public static System.String DBFileName
        {
            get
            {
                return GetResourceByName<System.String>("DBFileName");
            }
            set
            {
                SetResourceByName<System.String>("DBFileName", value);
            }
        }
        

        private static T GetResourceByName<T>(System.String name)
        {
            if(!ResourcesDictionary.ContainsKey(name))
            {
                return default(T);
            }
            else
            {
                return (T)ResourcesDictionary[name];
            }
        }

        private static void SetResourceByName<T>(System.String name, T value)
        {
            if(ResourcesDictionary.ContainsKey(name))
            {
                ResourcesDictionary.Remove(name);
            }
            ResourcesDictionary.Add(name, value);
        }
	}
}