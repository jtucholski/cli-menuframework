namespace MenuFramework
{
    public class Park 
    {
        public string Name { get; set; }
        public string State { get; set; }

        public override string ToString()
        {
            return $"{Name} ({State})";
        }
    }
}