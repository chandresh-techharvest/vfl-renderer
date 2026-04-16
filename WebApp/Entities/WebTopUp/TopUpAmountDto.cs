using System.Collections.Generic;

namespace VFL.Renderer.Entities.WebTopUp
{
    public class TopUpAmountDto
    {
        public int Value { get; set; }
        public bool IsDefault { get; set; }
    }
    public class TopUpAmountResponse
    {
        public List<TopUpAmountDto> AllTopUpAmounts { get; set; }
    }
}
