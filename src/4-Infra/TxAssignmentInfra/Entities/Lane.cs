﻿namespace TxAssignmentInfra.Entities
{
    public class Lane : BaseEntity
    {
        public int Number { get; set; }
        public string JanCode { get; set; }
        public int PositionX { get; set; }
        public int Quantity { get; set; }
    }
}
