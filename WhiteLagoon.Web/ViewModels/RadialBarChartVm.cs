namespace WhiteLagoon.Web.ViewModels
{
    public class RadialBarChartVm
    {
        public string[] Labels { get; set; } = [];
       public string TotalCountLabel{ get; set; }
        public int ValueChangeLabel{  get; set; }
       // public bool RatioValueChange { get; set; }
        public decimal[] Series {  get; set; } = [];
    }
}
