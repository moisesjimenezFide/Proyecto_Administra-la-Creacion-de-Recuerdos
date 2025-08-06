namespace Proyecto_CreandoRecuerdos.ViewModels
{
    public class CostosOperativosMensualesViewModel
    {
        public decimal PromedioCostosRecetas { get; set; }
        public decimal PromedioCostosEmpaques { get; set; }
        public decimal PromedioCostosImplementos { get; set; }
        public decimal PromedioCostosSuministros { get; set; }

        public decimal TotalCostoOperativo =>
            PromedioCostosRecetas + PromedioCostosEmpaques + PromedioCostosImplementos + PromedioCostosSuministros;
    }
}
