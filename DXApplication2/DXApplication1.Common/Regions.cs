namespace DXApplication1.Common
{
    public static class Regions
    {
        public static string MainWindow { get { return "MainWindow"; } }
        public static string Documents { get { return "Documents"; } }
        public static string Navigation { get { return "Navigation"; } }
    }

    public  class DaoService
    {
        public string Name { get; set; }
        public string Documents { get; set; }
        public int Id { get; set; }
    }
}
