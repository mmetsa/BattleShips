namespace GameBrain
{
    public struct CellState
    {
        public int? ShipId { get; set; }
        public bool Bomb { get; set; }

        public override string ToString()
        {
            return ShipId == null ? "No Ship" : ShipId + " " + (Bomb ? "& has bomb" : "& no bomb");
        }
    }
}