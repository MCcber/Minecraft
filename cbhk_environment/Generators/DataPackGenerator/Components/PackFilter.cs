namespace cbhk_environment.Generators.DataPackGenerator.Components
{
    public class PackFilter
    {
        private string nameSpace = "";
        public string NameSpace
        {
            get { return nameSpace; }
            set
            {
                nameSpace = value;
            }
        }

        private string path = "";
        public string Path
        {
            get { return path; }
            set
            {
                path = value;
            }
        }
    }
}
