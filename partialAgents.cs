using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UP_11
{
    partial class Agents
    {
        public byte[] LogoTip
        {
            get
            {
                if (Logo != null)
                {
                    return Logo;
                }
                return null;
            }
        }
        public string CountRealization
        {
            get
            {
                int sum = ProductRealization.Sum(s => s.Count);

                return sum.ToString() + " продаж за год";
            }
        }
        public string TypeNameAgents
        {
            get
            {
                return $"{AgentTypes.TypeName} | {NameCompany}";
            }
        }
        public string PriorityZnach
        {
            get
            {
                return $"Приоритетность: {Priority}";

            }
        }
        public string Skidka
        {
            get
            {
                decimal sum = (decimal)ProductRealization.Sum(s => s.Count * s.Products.PriceMin);
                if (sum < 10000) { return "Скидки нет"; }
                if (sum >= 10000 && sum < 50000) { return "5%"; }
                if (sum >= 50000 && sum < 150000) { return "10%"; }
                if (sum >= 150000 && sum < 500000) { return "20%"; }
                return "25%";
            }
        }
        public string AType
        {
            get
            {
                return $"{AgentTypes.TypeName}";
            }
        }
        public int YearSales
        {
            get
            {
                int sum = ProductRealization.Sum(s => s.Count);

                return sum;
            }
        }
    }
}
