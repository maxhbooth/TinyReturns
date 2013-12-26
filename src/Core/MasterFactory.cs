namespace Dimensional.TinyReturns.Core
{
    public static class MasterFactory
    {
        public static ISystemLog SystemLog { get; set; }
        public static ITinyReturnsDatabaseSettings TinyReturnsDatabaseSettings { get; set; }
    }
}