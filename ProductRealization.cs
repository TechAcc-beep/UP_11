//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UP_11
{
    using System;
    using System.Collections.Generic;
    
    public partial class ProductRealization
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int AgentId { get; set; }
        public System.DateTime DateRealization { get; set; }
        public int Count { get; set; }
    
        public virtual Agents Agents { get; set; }
        public virtual Products Products { get; set; }
    }
}
