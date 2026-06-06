namespace DAL
{
    public partial class Seoul_StayDataContext
    {
        partial void OnCreated()
        {
            string customConn = DAL_Settings.ConnectionString;
            if (!string.IsNullOrWhiteSpace(customConn))
            {
                this.Connection.ConnectionString = customConn;
            }
        }
    }
}