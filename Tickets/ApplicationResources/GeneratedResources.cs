
namespace AppData
{
    public static class Resources
    {
        public static System.String ConnectionString
        {
            get
            {
                return ResourceRetriever.GetResourceByName<System.String>("ConnectionString");
            }
            set
            {
                ResourceRetriever.SetResourceByName<System.String>("ConnectionString", value);
            }
        }
        
        public static System.String DBFileName
        {
            get
            {
                return ResourceRetriever.GetResourceByName<System.String>("DBFileName");
            }
            set
            {
                ResourceRetriever.SetResourceByName<System.String>("DBFileName", value);
            }
        }
        
	}
}