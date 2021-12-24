using System;




namespace Sam.Domain.Entity
{
    [Serializable]
    public class FuncionalidadeSistemaEntity : BaseEntity
    {
        public string Descricao { get; set; }


        public FuncionalidadeSistemaEntity() { }
        public FuncionalidadeSistemaEntity(byte? _id) 
        { 
            this.Id = _id; 
            base.Id = _id; 
        }

        public byte? Id
        {
            get { return (byte)base.Id; }
            set { base.Id = value; }
        }
    }
}
