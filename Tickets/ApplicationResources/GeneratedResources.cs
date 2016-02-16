
namespace AppData
{
    public static class Resources
    {
        internal static System.String ConnectionStringDefaultValue = "";
        public static System.String ConnectionString
        {
            get
            {
                return ResourceRetriever.GetResourceByName<System.String>("ConnectionString") ?? ConnectionStringDefaultValue;
            }
            set
            {
                ResourceRetriever.SetResourceByName<System.String>("ConnectionString", value);
            }
        }
        
        internal static System.String DBFileNameDefaultValue = "tickets.db";
        public static System.String DBFileName
        {
            get
            {
                return ResourceRetriever.GetResourceByName<System.String>("DBFileName") ?? DBFileNameDefaultValue;
            }
            set
            {
                ResourceRetriever.SetResourceByName<System.String>("DBFileName", value);
            }
        }
        
	}
}