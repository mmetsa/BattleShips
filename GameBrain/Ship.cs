namespace GameBrain
{
    public class Ship
    {
        public int ID { get; set; }
        public int Size { get; set; }
        public string Name { get; set; }
        public Ship(int id, string name, int size)
        {
            ID = id;
            Size = size;
            Name = name;
        }

        public Ship() : this(0, "Ship", 1) { }

        public override string ToString()
        {
            return Name + " (1 x " + Size + ")";
        }
    }
}