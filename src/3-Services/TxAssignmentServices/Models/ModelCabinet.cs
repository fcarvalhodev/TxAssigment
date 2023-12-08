﻿namespace TxAssignmentServices.Models
{
    public class ModelCabinet : ModelBase
    {
        public int Number { get; set; }
        public List<ModelRow> Rows { get; set; }
        public ModelPosition Position { get; set; }
        public ModelSize Size { get; set; }

        public ModelCabinet()
        {
            Rows = new List<ModelRow>();
            Position = new ModelPosition();
            Size = new ModelSize();
        }
    }
}
