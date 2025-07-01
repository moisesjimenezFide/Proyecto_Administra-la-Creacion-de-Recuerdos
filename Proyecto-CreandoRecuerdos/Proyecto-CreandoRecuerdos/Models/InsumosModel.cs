sing System.Collections.Generic;

namespace Proyecto_CreandoRecuerdos.Models
{
    public class InsumosModel
    {
        public List<MateriaPrima> MateriasPrimas { get; set; }
        public List<ProductoPreparado> ProductosPreparados { get; set; }
        public List<EmpaqueDecoracion> EmpaquesDecoraciones { get; set; }
        public List<Implemento> Implementos { get; set; }
        public List<Suministro> Suministros { get; set; }
        public List<RecetaCosto> CostosRecetas { get; set; }
        public List<ProductoFinal> ProductosFinales { get; set; }
        public ProductoFinal ProductoFinalEditado { get; set; }
    }

    public class MateriaPrima
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string marca { get; set; }
        public string presentacion { get; set; }
        public string proveedor { get; set; }
        public decimal costo { get; set; }
        public int peso { get; set; }
        public string unidad_de_medida { get; set; }
        public decimal costo_por_gramo { get; set; }
        public int merma_total_en_gramos { get; set; }
        public decimal porcentaje_de_merma { get; set; }
        public decimal costo_de_merma_total { get; set; }
        public decimal costo_total_mas_merma_total { get; set; }
        public decimal costo_por_gramo_con_merma { get; set; }
    }

    public class ProductoPreparado
    {
        public int id { get; set; }
        public string tipo { get; set; }
        public string nombre { get; set; }
        public string marca { get; set; }
        public string presentacion { get; set; }
        public string proveedor { get; set; }
        public int volumen_de_porcion { get; set; }
        public decimal costo { get; set; }
        public int peso { get; set; }
        public string unidad_de_medida { get; set; }
        public decimal costo_por_peso { get; set; }
        public decimal costo_por_porcion_con_merma { get; set; }
    }

    public class EmpaqueDecoracion
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string marca { get; set; }
        public string presentacion { get; set; }
        public string proveedor { get; set; }
        public decimal costo { get; set; }
        public int cantidad { get; set; }
        public string unidad_de_medida { get; set; }
        public decimal costo_por_cantidad { get; set; }
    }

    public class Implemento
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string marca { get; set; }
        public string presentacion { get; set; }
        public string proveedor { get; set; }
        public decimal costo { get; set; }
        public int cantidad { get; set; }
        public string unidad_de_medida { get; set; }
        public decimal costo_por_cantidad { get; set; }
    }

    public class Suministro
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string marca { get; set; }
        public string presentacion { get; set; }
        public string proveedor { get; set; }
        public decimal costo { get; set; }
        public int cantidad { get; set; }
        public string unidad_de_medida { get; set; }
        public decimal costo_por_cantidad { get; set; }
    }

    public class RecetaCosto
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public int porcion { get; set; }
        public decimal costo_total_receta { get; set; }
        public decimal costo_por_porcion { get; set; }
        public List<MateriaPrimaUtilizada> MateriasPrimasUtilizadas { get; set; }
        public List<ProductoPreparadoUtilizado> ProductosPreparadosUtilizados { get; set; }
        public string plataforma_de_envio { get; set; }
    }

    public class MateriaPrimaUtilizada
    {
        public int id { get; set; }
        public int id_receta { get; set; }
        public int id_materia_prima_utilizada { get; set; }
        public string nombre { get; set; }
        public int cantidad { get; set; }
        public string unidad_de_medida { get; set; }
        public decimal costo_por_cantidad { get; set; }
        public decimal total_costo { get; set; }
    }

    public class ProductoPreparadoUtilizado
    {
        public int id { get; set; }
        public int id_receta { get; set; }
        public int id_producto_preparado_utilizado { get; set; }
        public string nombre { get; set; }
        public int cantidad { get; set; }
        public string unidad_de_medida { get; set; }
        public decimal costo_por_cantidad { get; set; }
        public decimal total_costo { get; set; }
    }

    public class ProductoFinal
    {
        public int id { get; set; }
        public int id_receta { get; set; }
        public string nombre_receta { get; set; }
        public decimal costo_total_receta { get; set; }
        public decimal margen_de_utilidad { get; set; }
        public decimal costo_sin_margen_de_utilidad { get; set; }
        public decimal costo_con_margen_de_utilidad { get; set; }
        public decimal costo_empaque_decoracion_utilizado { get; set; }
        public decimal costo_implemento_utilizado { get; set; }
        public decimal costo_suministro_utilizado { get; set; }
        public decimal iva { get; set; }
        public decimal impuesto_de_servicio { get; set; }
        public decimal envio { get; set; }
        public string plataforma_de_envio { get; set; }
        public decimal precio_final_sugerido { get; set; }
        public List<EmpaqueDecoracionUtilizado> EmpaquesDecoracionesUtilizados { get; set; }
        public List<ImplementoUtilizado> ImplementosUtilizados { get; set; }
        public List<SuministroUtilizado> SuministrosUtilizados { get; set; }
    }

    public class EmpaqueDecoracionUtilizado
    {
        public int id { get; set; }
        public int id_precio_final_sugerido { get; set; }
        public int id_empaque_decoracion_utilizado { get; set; }
        public string nombre { get; set; }
        public int cantidad { get; set; }
        public string unidad_de_medida { get; set; }
        public decimal costo_por_cantidad { get; set; }
        public decimal total_costo { get; set; }
    }

    public class ImplementoUtilizado
    {
        public int id { get; set; }
        public int id_precio_final_sugerido { get; set; }
        public int id_implemento_utilizado { get; set; }
        public string nombre { get; set; }
        public int cantidad { get; set; }
        public string unidad_de_medida { get; set; }
        public decimal costo_por_cantidad { get; set; }
        public decimal total_costo { get; set; }
    }

    public class SuministroUtilizado
    {
        public int id { get; set; }
        public int id_precio_final_sugerido { get; set; }
        public int id_suministro_utilizado { get; set; }
        public string nombre { get; set; }
        public int cantidad { get; set; }
        public string unidad_de_medida { get; set; }
        public decimal costo_por_cantidad { get; set; }
        public decimal total_costo { get; set; }
    }
}
