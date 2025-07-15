using Proyecto_CreandoRecuerdos.base_de_datos;
using Proyecto_CreandoRecuerdos.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

public class InsumosController : Controller
{
    private BD_CREANDO_RECUERDOSEntities4 db = new BD_CREANDO_RECUERDOSEntities4();

    // ----------- Materias Primas -----------

    //Listar y buscar materias primas
    public ActionResult materias_primas(string search)
    {
        // Verificar si el usuario es administrador
        if (Session["Rol"] == null || (int)Session["Rol"] != 1)
        {
            return RedirectToAction("registro_usuarios", "Registro_Usuarios");
        }

        // Obtener las materias primas y aplicar el filtro de búsqueda
        var query = db.tabla_materias_primas.AsQueryable();
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(m =>
                m.nombre.Contains(search) ||
                m.marca.Contains(search) ||
                m.presentacion.Contains(search) ||
                m.proveedor.Contains(search) ||
                m.unidad_de_medida.Contains(search) ||
                m.costo.ToString().Contains(search) ||
                m.peso.ToString().Contains(search) ||
                m.costo_por_gramo.ToString().Contains(search) ||
                m.merma_total_en_gramos.ToString().Contains(search) ||
                m.porcentaje_de_merma.ToString().Contains(search) ||
                m.costo_de_merma_total.ToString().Contains(search) ||
                m.costo_total_mas_merma_total.ToString().Contains(search) ||
                m.costo_por_gramo_con_merma.ToString().Contains(search)
            );
        }
        var materia_prima = new InsumosModel
        {
            MateriasPrimas = query.Select(m => new MateriaPrima
            {
                id = m.id,
                nombre = m.nombre,
                marca = m.marca,
                presentacion = m.presentacion,
                proveedor = m.proveedor,
                costo = (decimal)(m.costo ?? 0),
                peso = (int)(m.peso ?? 0),
                unidad_de_medida = m.unidad_de_medida,
                costo_por_gramo = (decimal)(m.costo_por_gramo ?? 0),
                merma_total_en_gramos = (int)(m.merma_total_en_gramos ?? 0),
                porcentaje_de_merma = (decimal)(m.porcentaje_de_merma ?? 0),
                costo_de_merma_total = (decimal)(m.costo_de_merma_total ?? 0),
                costo_total_mas_merma_total = (decimal)(m.costo_total_mas_merma_total ?? 0),
                costo_por_gramo_con_merma = (decimal)(m.costo_por_gramo_con_merma ?? 0)
            }).ToList()
        };
        ViewBag.Search = search;
        return View(materia_prima);
    }

    // Crear una nueva materia prima
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CrearMateriaPrima(MateriaPrima materia_prima)
    {
        var errores = new List<string>();

        // 1. Validar si existe la combinación exacta de los 4 campos
        bool existeExacto = db.tabla_materias_primas.Any(m =>
            m.nombre.ToLower() == materia_prima.nombre.ToLower() &&
            m.marca.ToLower() == materia_prima.marca.ToLower() &&
            m.presentacion.ToLower() == materia_prima.presentacion.ToLower() &&
            m.proveedor.ToLower() == materia_prima.proveedor.ToLower()
        );
        if (existeExacto)
        {
            errores.Add("Ya existe una materia prima con el mismo nombre, marca, presentación y proveedor.");
        }
        else
        {
            // 2. Validar individualmente cada campo
            if (db.tabla_materias_primas.Any(m => m.nombre.ToLower() == materia_prima.nombre.ToLower()))
                errores.Add("El nombre ya existe en otra materia prima.");
            if (db.tabla_materias_primas.Any(m => m.marca.ToLower() == materia_prima.marca.ToLower()))
                errores.Add("La marca ya existe en otra materia prima.");
            if (db.tabla_materias_primas.Any(m => m.presentacion.ToLower() == materia_prima.presentacion.ToLower()))
                errores.Add("La presentación ya existe en otra materia prima.");
            if (db.tabla_materias_primas.Any(m => m.proveedor.ToLower() == materia_prima.proveedor.ToLower()))
                errores.Add("El proveedor ya existe en otra materia prima.");
        }

        // Validar campos obligatorios y valores numéricos
        if (string.IsNullOrWhiteSpace(materia_prima.nombre) ||
                                      string.IsNullOrWhiteSpace(materia_prima.marca) ||
                                      string.IsNullOrWhiteSpace(materia_prima.presentacion) ||
                                      string.IsNullOrWhiteSpace(materia_prima.proveedor) ||
                                      string.IsNullOrWhiteSpace(materia_prima.unidad_de_medida))
               errores.Add("Todos los campos son obligatorios.");

        if (materia_prima.costo <= 0 || materia_prima.peso <= 0 || materia_prima.merma_total_en_gramos < 0)
            errores.Add("Los valores numéricos deben ser mayores a cero.");

        if (errores.Any())
        {
            ViewBag.Errores = errores;
            var lista = db.tabla_materias_primas.Select(mp => new MateriaPrima
            {
                id = mp.id,
                nombre = mp.nombre,
                marca = mp.marca,
                presentacion = mp.presentacion,
                proveedor = mp.proveedor,
                costo = (decimal)mp.costo,
                peso = (int)mp.peso,
                unidad_de_medida = mp.unidad_de_medida,
                costo_por_gramo = (decimal)mp.costo_por_gramo,
                merma_total_en_gramos = (int)mp.merma_total_en_gramos,
                porcentaje_de_merma = (decimal)mp.porcentaje_de_merma,
                costo_de_merma_total = (decimal)mp.costo_de_merma_total,
                costo_total_mas_merma_total = (decimal)mp.costo_total_mas_merma_total,
                costo_por_gramo_con_merma = (decimal)mp.costo_por_gramo_con_merma
            }).ToList();
            return View("materias_primas", new InsumosModel
            {
                MateriaPrimaEditado = materia_prima,
                MateriasPrimas = lista
            });
        }

        // Cálculos de campos derivados
        decimal costoPorGramo = (materia_prima.peso > 0) ? (materia_prima.costo / materia_prima.peso) : 0;
        decimal porcentajeMerma = (materia_prima.peso > 0) ? ((decimal)materia_prima.merma_total_en_gramos / materia_prima.peso) * 100 : 0;
        decimal costoMermaTotal = costoPorGramo * materia_prima.merma_total_en_gramos;
        decimal costoTotalMasMerma = materia_prima.costo + costoMermaTotal;
        decimal costoPorGramoConMerma = (materia_prima.peso > 0) ? (costoTotalMasMerma / materia_prima.peso) : 0;

        db.tabla_materias_primas.Add(new tabla_materias_primas
        {
            nombre = materia_prima.nombre,
            marca = materia_prima.marca,
            presentacion = materia_prima.presentacion,
            proveedor = materia_prima.proveedor,
            costo = materia_prima.costo,
            peso = materia_prima.peso,
            unidad_de_medida = materia_prima.unidad_de_medida,
            costo_por_gramo = costoPorGramo,
            merma_total_en_gramos = materia_prima.merma_total_en_gramos,
            porcentaje_de_merma = porcentajeMerma,
            costo_de_merma_total = costoMermaTotal,
            costo_total_mas_merma_total = costoTotalMasMerma,
            costo_por_gramo_con_merma = costoPorGramoConMerma
        });
        db.SaveChanges();
        TempData["SuccessMessage"] = "¡Materia prima agregada con éxito!";
        return RedirectToAction("materias_primas");
    }

    // Editar una materia prima existente
    public ActionResult EditarMateriaPrima(int id)
    {
        var m = db.tabla_materias_primas.Find(id);
        if (m == null) return HttpNotFound();
        var materia_prima = new MateriaPrima
        {
            id = m.id,
            nombre = m.nombre,
            marca = m.marca,
            presentacion = m.presentacion,
            proveedor = m.proveedor,
            costo = (decimal)(m.costo ?? 0),
            peso = (int)(m.peso ?? 0),
            unidad_de_medida = m.unidad_de_medida,
            costo_por_gramo = (decimal)(m.costo_por_gramo ?? 0),
            merma_total_en_gramos = (int)(m.merma_total_en_gramos ?? 0),
            porcentaje_de_merma = (decimal)(m.porcentaje_de_merma ?? 0),
            costo_de_merma_total = (decimal)(m.costo_de_merma_total ?? 0),
            costo_total_mas_merma_total = (decimal)(m.costo_total_mas_merma_total ?? 0),
            costo_por_gramo_con_merma = (decimal)(m.costo_por_gramo_con_merma ?? 0)
        };

        //Obtén el listado de materias primas
        var lista = db.tabla_materias_primas.Select(mp => new MateriaPrima
        {
            id = mp.id,
            nombre = mp.nombre,
            marca = mp.marca,
            presentacion = mp.presentacion,
            proveedor = mp.proveedor,
            costo = (decimal)(mp.costo ?? 0),
            peso = (int)(mp.peso ?? 0),
            unidad_de_medida = mp.unidad_de_medida,
            costo_por_gramo = (decimal)(mp.costo_por_gramo ?? 0),
            merma_total_en_gramos = (int)(mp.merma_total_en_gramos ?? 0),
            porcentaje_de_merma = (decimal)(mp.porcentaje_de_merma ?? 0),
            costo_de_merma_total = (decimal)(mp.costo_de_merma_total ?? 0),
            costo_total_mas_merma_total = (decimal)(mp.costo_total_mas_merma_total ?? 0),
            costo_por_gramo_con_merma = (decimal)(mp.costo_por_gramo_con_merma ?? 0)
        }).ToList();

        ViewBag.Editando = true;
        return View("materias_primas", new InsumosModel
        {
            MateriaPrimaEditado = materia_prima,
            MateriasPrimas = lista
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditarMateriaPrima(MateriaPrima materia_prima)
    {
        var errores = new List<string>();

        // Buscar si existe un registro con la misma combinación de los 4 campos (excepto el actual)
        var existe = db.tabla_materias_primas.Any(mp =>
            mp.id != materia_prima.id &&
            mp.nombre.ToLower() == materia_prima.nombre.ToLower() &&
            mp.marca.ToLower() == materia_prima.marca.ToLower() &&
            mp.presentacion.ToLower() == materia_prima.presentacion.ToLower() &&
            mp.proveedor.ToLower() == materia_prima.proveedor.ToLower()
        );
        if (existe)
        {
            errores.Add("Ya existe una materia prima con el mismo nombre, marca, presentación y proveedor.");
        }
        else
        {
            // Validar individualmente cada campo, pero solo si hay coincidencia con los otros campos
            var repetidos = db.tabla_materias_primas.Where(mp => mp.id != materia_prima.id);

            if (repetidos.Any(mp =>
                mp.nombre.ToLower() == materia_prima.nombre.ToLower() &&
                mp.presentacion.ToLower() == materia_prima.presentacion.ToLower() &&
                mp.proveedor.ToLower() == materia_prima.proveedor.ToLower()))
            {
                errores.Add("Ya existe una materia prima con el mismo nombre, presentación y proveedor.");
            }
            if (repetidos.Any(mp =>
                mp.nombre.ToLower() == materia_prima.nombre.ToLower() &&
                mp.marca.ToLower() == materia_prima.marca.ToLower() &&
                mp.presentacion.ToLower() == materia_prima.presentacion.ToLower()))
            {
                errores.Add("Ya existe una materia prima con el mismo nombre, marca y presentación.");
            }
            // Puedes agregar más combinaciones si lo deseas
        }

        // Validar campos obligatorios y valores numéricos
        if (string.IsNullOrWhiteSpace(materia_prima.nombre) ||
                                      string.IsNullOrWhiteSpace(materia_prima.marca) ||
                                      string.IsNullOrWhiteSpace(materia_prima.presentacion) ||
                                      string.IsNullOrWhiteSpace(materia_prima.proveedor) ||
                                      string.IsNullOrWhiteSpace(materia_prima.unidad_de_medida))
            errores.Add("Todos los campos son obligatorios.");


        if (materia_prima.costo <= 0 || materia_prima.peso <= 0 || materia_prima.merma_total_en_gramos < 0)
            errores.Add("Los valores numéricos deben ser mayores a cero.");

        if (errores.Any())
        {
            ViewBag.Errores = errores;
            ViewBag.Editando = true;
            var lista = db.tabla_materias_primas.Select(mp => new MateriaPrima
            {
                id = mp.id,
                nombre = mp.nombre,
                marca = mp.marca,
                presentacion = mp.presentacion,
                proveedor = mp.proveedor,
                costo = (decimal)(mp.costo ?? 0),
                peso = (int)(mp.peso ?? 0),
                unidad_de_medida = mp.unidad_de_medida,
                costo_por_gramo = (decimal)(mp.costo_por_gramo ?? 0),
                merma_total_en_gramos = (int)(mp.merma_total_en_gramos ?? 0),
                porcentaje_de_merma = (decimal)(mp.porcentaje_de_merma ?? 0),
                costo_de_merma_total = (decimal)(mp.costo_de_merma_total ?? 0),
                costo_total_mas_merma_total = (decimal)(mp.costo_total_mas_merma_total ?? 0),
                costo_por_gramo_con_merma = (decimal)(mp.costo_por_gramo_con_merma ?? 0)
            }).ToList();
            return View("materias_primas", new InsumosModel
            {
                MateriaPrimaEditado = materia_prima,
                MateriasPrimas = lista
            });
        }

        var m = db.tabla_materias_primas.Find(materia_prima.id);
        if (m != null)
        {
            // Cálculos de campos derivados
            decimal costoPorGramo = (materia_prima.peso > 0) ? (materia_prima.costo / materia_prima.peso) : 0;
            decimal porcentajeMerma = (materia_prima.peso > 0) ? ((decimal)materia_prima.merma_total_en_gramos / materia_prima.peso) * 100 : 0;
            decimal costoMermaTotal = costoPorGramo * materia_prima.merma_total_en_gramos;
            decimal costoTotalMasMerma = materia_prima.costo + costoMermaTotal;
            decimal costoPorGramoConMerma = (materia_prima.peso > 0) ? (costoTotalMasMerma / materia_prima.peso) : 0;

            m.nombre = materia_prima.nombre;
            m.marca = materia_prima.marca;
            m.presentacion = materia_prima.presentacion;
            m.proveedor = materia_prima.proveedor;
            m.costo = materia_prima.costo;
            m.peso = materia_prima.peso;
            m.unidad_de_medida = materia_prima.unidad_de_medida;
            m.merma_total_en_gramos = materia_prima.merma_total_en_gramos;
            m.costo_por_gramo = costoPorGramo;
            m.porcentaje_de_merma = porcentajeMerma;
            m.costo_de_merma_total = costoMermaTotal;
            m.costo_total_mas_merma_total = costoTotalMasMerma;
            m.costo_por_gramo_con_merma = costoPorGramoConMerma;
        }
        db.SaveChanges();
        TempData["SuccessMessage"] = "¡Materia prima actualizada con éxito!";
        return RedirectToAction("materias_primas");
    }

    // Eliminar una materia prima
    public ActionResult EliminarMateriaPrima(int id)
    {
        var m = db.tabla_materias_primas.Find(id);
        if (m != null)
        {
            db.tabla_materias_primas.Remove(m);
        }
        db.SaveChanges();
        TempData["SuccessMessage"] = "¡Materia prima eliminada con éxito!";
        return RedirectToAction("materias_primas");
    }

    // ----------- Productos Preparados -----------

    // Listar y buscar productos preparados
    public ActionResult productos_preparados(string search)
    {
        // Verificar si el usuario es administrador
        if (Session["Rol"] == null || (int)Session["Rol"] != 1)
        {
            return RedirectToAction("registro_usuarios", "Registro_Usuarios");
        }

        // Obtener los productos preparados y aplicar el filtro de búsqueda
        var query = db.tabla_productos_preparados.AsQueryable();
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p =>
                p.tipo.Contains(search) ||
                p.nombre.Contains(search) ||
                p.marca.Contains(search) ||
                p.presentacion.Contains(search) ||
                p.proveedor.Contains(search) ||
                p.volumen_de_porcion.ToString().Contains(search) ||
                p.unidad_de_medida.Contains(search) ||
                p.costo.ToString().Contains(search) ||
                p.peso.ToString().Contains(search) ||
                p.costo_por_peso.ToString().Contains(search) ||
                p.costo_por_porcion_con_merma.ToString().Contains(search)

            );
        }
        var producto_preparado = new InsumosModel
        {
            ProductosPreparados = query.Select(p => new ProductoPreparado
            {
                id = p.id,
                tipo = p.tipo,
                nombre = p.nombre,
                marca = p.marca,
                presentacion = p.presentacion,
                proveedor = p.proveedor,
                volumen_de_porcion = (int)p.volumen_de_porcion,
                costo = (decimal)p.costo,
                peso = (int)p.peso,
                unidad_de_medida = p.unidad_de_medida,
                costo_por_peso = (decimal)(p.costo_por_peso ?? 0),
                costo_por_porcion_con_merma = (decimal)(p.costo_por_porcion_con_merma ?? 0)
            }).ToList()
        };
        ViewBag.Search = search;
        return View(producto_preparado);
    }

    // Crear un nuevo producto preparado
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CrearProductoPreparado(ProductoPreparado producto_preparado)
    {
        var errores = new List<string>();

        // Validar que no exista otro producto preparado con el mismo nombre
        if (db.tabla_productos_preparados.Any(p => p.nombre.ToLower() == producto_preparado.nombre.ToLower()))
            errores.Add("Ya existe un producto preparado con ese nombre.");

        // Validar campos obligatorios y valores numéricos
        if (string.IsNullOrWhiteSpace(producto_preparado.tipo) ||
                                      string.IsNullOrWhiteSpace(producto_preparado.nombre) ||
                                      string.IsNullOrWhiteSpace(producto_preparado.marca) ||
                                      string.IsNullOrWhiteSpace(producto_preparado.presentacion) ||
                                      string.IsNullOrWhiteSpace(producto_preparado.proveedor) ||
                                      string.IsNullOrWhiteSpace(producto_preparado.unidad_de_medida))
            errores.Add("Todos los campos son obligatorios.");

        if (producto_preparado.costo <= 0 || producto_preparado.peso <= 0 || producto_preparado.volumen_de_porcion <= 0)
            errores.Add("Los valores numéricos deben ser mayores a cero.");

        if (errores.Any())
        {
            ViewBag.Errores = errores;
            var lista = db.tabla_productos_preparados.Select(p => new ProductoPreparado
            {
                id = p.id,
                tipo = p.tipo,
                nombre = p.nombre,
                marca = p.marca,
                presentacion = p.presentacion,
                proveedor = p.proveedor,
                volumen_de_porcion = (int)p.volumen_de_porcion,
                costo = (decimal)p.costo,
                peso = (int)p.peso,
                unidad_de_medida = p.unidad_de_medida,
                costo_por_peso = (decimal)p.costo_por_peso,
                costo_por_porcion_con_merma = (decimal)p.costo_por_porcion_con_merma
            }).ToList();
            return View("productos_preparados", new InsumosModel
            {
                ProductoPreparadoEditado = producto_preparado,
                ProductosPreparados = lista
            });
        }

        decimal costoPorPeso = (producto_preparado.peso > 0) ? (producto_preparado.costo / producto_preparado.peso) : 0;
        decimal costoPorPorcionConMerma = (producto_preparado.volumen_de_porcion > 0) ? (producto_preparado.costo / producto_preparado.volumen_de_porcion) : 0;

        db.tabla_productos_preparados.Add(new tabla_productos_preparados
        {
            tipo = producto_preparado.tipo,
            nombre = producto_preparado.nombre,
            marca = producto_preparado.marca,
            presentacion = producto_preparado.presentacion,
            proveedor = producto_preparado.proveedor,
            volumen_de_porcion = producto_preparado.volumen_de_porcion,
            costo = producto_preparado.costo,
            peso = producto_preparado.peso,
            unidad_de_medida = producto_preparado.unidad_de_medida,
            costo_por_peso = costoPorPeso,
            costo_por_porcion_con_merma = costoPorPorcionConMerma
        });
        db.SaveChanges();
        TempData["SuccessMessage"] = "¡Producto preparado agregado con éxito!";
        return RedirectToAction("productos_preparados");
    }

    // Editar un producto preparado existente
    public ActionResult EditarProductoPreparado(int id)
    {
        var p = db.tabla_productos_preparados.Find(id);
        if (p == null) return HttpNotFound();
        var producto_preparado = new ProductoPreparado
        {
            id = p.id,
            tipo = p.tipo,
            nombre = p.nombre,
            marca = p.marca,
            presentacion = p.presentacion,
            proveedor = p.proveedor,
            volumen_de_porcion = (int)p.volumen_de_porcion,
            costo = (decimal)p.costo,
            peso = (int)p.peso,
            unidad_de_medida = p.unidad_de_medida,
            costo_por_peso = (decimal)p.costo_por_peso,
            costo_por_porcion_con_merma = (decimal)p.costo_por_porcion_con_merma
        };

        //Obtén el listado de productos preparados
        var lista = db.tabla_productos_preparados.Select(prodprep => new ProductoPreparado
        {
            id = prodprep.id,
            tipo = prodprep.tipo,
            nombre = prodprep.nombre,
            marca = prodprep.marca,
            presentacion = prodprep.presentacion,
            proveedor = prodprep.proveedor,
            volumen_de_porcion = (int)prodprep.volumen_de_porcion,
            costo = (decimal)prodprep.costo,
            peso = (int)prodprep.peso,
            unidad_de_medida = prodprep.unidad_de_medida,
            costo_por_peso = (decimal)prodprep.costo_por_peso,
            costo_por_porcion_con_merma = (decimal)prodprep.costo_por_porcion_con_merma
        }).ToList();

        ViewBag.Editando = true;
        return View("productos_preparados", new InsumosModel
        {
            ProductoPreparadoEditado = producto_preparado,
            ProductosPreparados = lista
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditarProductoPreparado(ProductoPreparado producto_preparado)
    {
        var errores = new List<string>();

        // Validar que no exista otro producto preparado con el mismo nombre
        if (db.tabla_productos_preparados.Any(prodprep => prodprep.nombre.ToLower() == producto_preparado.nombre.ToLower() && prodprep.id != producto_preparado.id))
            errores.Add("Ya existe un producto preparado con ese nombre.");

        // Validar campos obligatorios y valores numéricos
        if (string.IsNullOrWhiteSpace(producto_preparado.tipo) ||
                                      string.IsNullOrWhiteSpace(producto_preparado.nombre) ||
                                      string.IsNullOrWhiteSpace(producto_preparado.marca) ||
                                      string.IsNullOrWhiteSpace(producto_preparado.presentacion) ||
                                      string.IsNullOrWhiteSpace(producto_preparado.proveedor) ||
                                      string.IsNullOrWhiteSpace(producto_preparado.unidad_de_medida))
            errores.Add("Todos los campos son obligatorios.");

        if (producto_preparado.costo <= 0 || producto_preparado.peso <= 0 || producto_preparado.volumen_de_porcion <= 0)
            errores.Add("Los valores numéricos deben ser mayores a cero.");

        if (errores.Any())
        {
            ViewBag.Errores = errores;
            var lista = db.tabla_productos_preparados.Select(prodprep => new ProductoPreparado
            {
                id = prodprep.id,
                tipo = prodprep.tipo,
                nombre = prodprep.nombre,
                marca = prodprep.marca,
                presentacion = prodprep.presentacion,
                proveedor = prodprep.proveedor,
                volumen_de_porcion = (int)prodprep.volumen_de_porcion,
                costo = (decimal)prodprep.costo,
                peso = (int)prodprep.peso,
                unidad_de_medida = prodprep.unidad_de_medida,
                costo_por_peso = (decimal)prodprep.costo_por_peso,
                costo_por_porcion_con_merma = (decimal)prodprep.costo_por_porcion_con_merma
            }).ToList();
            return View("productos_preparados", new InsumosModel
            {
                ProductoPreparadoEditado = producto_preparado,
                ProductosPreparados = lista
            });
        }

        var pp = db.tabla_productos_preparados.Find(producto_preparado.id);
        if (pp != null)
        {
            decimal costoPorPeso = (producto_preparado.peso > 0) ? (producto_preparado.costo / producto_preparado.peso) : 0;
            decimal costoPorPorcionConMerma = (producto_preparado.volumen_de_porcion > 0) ? (producto_preparado.costo / producto_preparado.volumen_de_porcion) : 0;

            pp.tipo = producto_preparado.tipo;
            pp.nombre = producto_preparado.nombre;
            pp.marca = producto_preparado.marca;
            pp.presentacion = producto_preparado.presentacion;
            pp.proveedor = producto_preparado.proveedor;
            pp.volumen_de_porcion = producto_preparado.volumen_de_porcion;
            pp.costo = producto_preparado.costo;
            pp.peso = producto_preparado.peso;
            pp.unidad_de_medida = producto_preparado.unidad_de_medida;
            pp.costo_por_peso = costoPorPeso;
            pp.costo_por_porcion_con_merma = costoPorPorcionConMerma;
        }
        db.SaveChanges();
        TempData["SuccessMessage"] = "¡Producto preparado actualizado con éxito!";
        return RedirectToAction("productos_preparados");
    }

    // Eliminar un producto preparado
    public ActionResult EliminarProductoPreparado(int id)
    {
        var pp = db.tabla_productos_preparados.Find(id);
        if (pp != null)
        {
            db.tabla_productos_preparados.Remove(pp);
        }
        db.SaveChanges();
        TempData["SuccessMessage"] = "¡Producto preparado eliminado con éxito!";
        return RedirectToAction("productos_preparados");
    }

    // ----------- Empaques y/o Decoraciones -----------

    // Listar y buscar empaques y/o decoraciones
    public ActionResult empaques_decoraciones(string search)
    {
        // Verificar si el usuario es administrador
        if (Session["Rol"] == null || (int)Session["Rol"] != 1)
        {
            return RedirectToAction("registro_usuarios", "Registro_Usuarios");
        }

        // Obtener los empaques o las decoraciones y aplicar el filtro de búsqueda
        var query = db.tabla_empaques_decoraciones.AsQueryable();
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(ed =>
                ed.nombre.Contains(search) ||
                ed.marca.Contains(search) ||
                ed.presentacion.Contains(search) ||
                ed.proveedor.Contains(search) ||
                ed.unidad_de_medida.Contains(search) ||
                ed.costo.ToString().Contains(search) ||
                ed.cantidad.ToString().Contains(search) ||
                ed.costo_por_cantidad.ToString().Contains(search)
            );
        }
        var empaque_decoracion = new InsumosModel
        {
            EmpaquesDecoraciones = query.Select(ed => new EmpaqueDecoracion
            {
                id = ed.id,
                nombre = ed.nombre,
                marca = ed.marca,
                presentacion = ed.presentacion,
                proveedor = ed.proveedor,
                costo = (decimal)ed.costo,
                cantidad = (int)ed.cantidad,
                unidad_de_medida = ed.unidad_de_medida,
                costo_por_cantidad = (decimal)(ed.costo_por_cantidad ?? 0)
            }).ToList()
        };
        ViewBag.Search = search;
        return View(empaque_decoracion);
    }

    // Crear un nuevo empaque o decoración
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CrearEmpaqueDecoracion(EmpaqueDecoracion empaque_decoracion)
    {
        var errores = new List<string>();

        // Validar que no exista otro empaque/decoración con el mismo nombre
        if (db.tabla_empaques_decoraciones.Any(ed => ed.nombre.ToLower() == empaque_decoracion.nombre.ToLower()))
            errores.Add("Ya existe un empaque o decoración con ese nombre.");

        // Validar campos obligatorios y valores numéricos
        if (string.IsNullOrWhiteSpace(empaque_decoracion.nombre) ||
                                      string.IsNullOrWhiteSpace(empaque_decoracion.marca) ||
                                      string.IsNullOrWhiteSpace(empaque_decoracion.presentacion) ||
                                      string.IsNullOrWhiteSpace(empaque_decoracion.proveedor) ||
                                      string.IsNullOrWhiteSpace(empaque_decoracion.unidad_de_medida))
            errores.Add("Todos los campos son obligatorios.");

        if (empaque_decoracion.costo <= 0 || empaque_decoracion.cantidad <= 0)
            errores.Add("El costo y la cantidad deben ser mayores a cero.");

        if (errores.Any())
        {
            ViewBag.Errores = errores;
            var lista = db.tabla_empaques_decoraciones.Select(ed => new EmpaqueDecoracion
            {
                id = ed.id,
                nombre = ed.nombre,
                marca = ed.marca,
                presentacion = ed.presentacion,
                proveedor = ed.proveedor,
                costo = (decimal)ed.costo,
                cantidad = (int)ed.cantidad,
                unidad_de_medida = ed.unidad_de_medida,
                costo_por_cantidad = (decimal)ed.costo_por_cantidad
            }).ToList();
            return View("empaques_decoraciones", new InsumosModel
            {
                EmpaqueDecoracionEditado = empaque_decoracion,
                EmpaquesDecoraciones = lista
            });
        }

        // Calcular el campo derivado
        decimal costoPorCantidad = (empaque_decoracion.cantidad > 0) ? (empaque_decoracion.costo / empaque_decoracion.cantidad) : 0;
        db.tabla_empaques_decoraciones.Add(new tabla_empaques_decoraciones
        {
            nombre = empaque_decoracion.nombre,
            marca = empaque_decoracion.marca,
            presentacion = empaque_decoracion.presentacion,
            proveedor = empaque_decoracion.proveedor,
            costo = empaque_decoracion.costo,
            cantidad = empaque_decoracion.cantidad,
            unidad_de_medida = empaque_decoracion.unidad_de_medida,
            costo_por_cantidad = costoPorCantidad
        });
        db.SaveChanges();
        TempData["SuccessMessage"] = "¡Empaque o decoración agregado con éxito!";
        return RedirectToAction("empaques_decoraciones");
    }

    // Editar un empaque o decoración existente (GET id)
    public ActionResult EditarEmpaqueDecoracion(int id)
    {
        var ed = db.tabla_empaques_decoraciones.Find(id);
        if (ed == null) return HttpNotFound();
        var empaque_decoracion = new EmpaqueDecoracion
        {
            id = ed.id,
            nombre = ed.nombre,
            marca = ed.marca,
            presentacion = ed.presentacion,
            proveedor = ed.proveedor,
            costo = (decimal)ed.costo,
            cantidad = (int)ed.cantidad,
            unidad_de_medida = ed.unidad_de_medida,
            costo_por_cantidad = (decimal)ed.costo_por_cantidad
        };

        //Obtén el listado de empaques y decoraciones
        var lista = db.tabla_empaques_decoraciones.Select(empdec => new EmpaqueDecoracion
        {
            id = empdec.id,
            nombre = empdec.nombre,
            marca = empdec.marca,
            presentacion = empdec.presentacion,
            proveedor = empdec.proveedor,
            costo = (decimal)empdec.costo,
            cantidad = (int)empdec.cantidad,
            unidad_de_medida = empdec.unidad_de_medida,
            costo_por_cantidad = (decimal)empdec.costo_por_cantidad
        }).ToList();

        ViewBag.Editando = true;
        return View("empaques_decoraciones", new InsumosModel
        {
            EmpaqueDecoracionEditado = empaque_decoracion,
            EmpaquesDecoraciones = lista
        });
    }

    // Editar un empaque o decoración existente (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditarEmpaqueDecoracion(EmpaqueDecoracion empaque_decoracion)
    {
        var errores = new List<string>();

        // Validar que no exista otro empaque/decoración con el mismo nombre
        if (db.tabla_empaques_decoraciones.Any(empdec => empdec.nombre.ToLower() == empaque_decoracion.nombre.ToLower() && empdec.id != empaque_decoracion.id))
            errores.Add("Ya existe un empaque/decoración con ese nombre.");

        // Validar campos obligatorios y valores numéricos
        if (string.IsNullOrWhiteSpace(empaque_decoracion.nombre) ||
                                      string.IsNullOrWhiteSpace(empaque_decoracion.marca) ||
                                      string.IsNullOrWhiteSpace(empaque_decoracion.presentacion) ||
                                      string.IsNullOrWhiteSpace(empaque_decoracion.proveedor) ||
                                      string.IsNullOrWhiteSpace(empaque_decoracion.unidad_de_medida))
            errores.Add("Todos los campos son obligatorios.");

        if (empaque_decoracion.costo <= 0 || empaque_decoracion.cantidad <= 0)
            errores.Add("Los valores numéricos deben ser mayores a cero.");

        if (errores.Any())
        {
            ViewBag.Errores = errores;
            var lista = db.tabla_empaques_decoraciones.Select(empdec => new EmpaqueDecoracion
            {
                id = empdec.id,
                nombre = empdec.nombre,
                marca = empdec.marca,
                presentacion = empdec.presentacion,
                proveedor = empdec.proveedor,
                costo = (decimal)empdec.costo,
                cantidad = (int)empdec.cantidad,
                unidad_de_medida = empdec.unidad_de_medida,
                costo_por_cantidad = (decimal)empdec.costo_por_cantidad
            }).ToList();
            return View("empaques_decoraciones", new InsumosModel
            {
                EmpaqueDecoracionEditado = empaque_decoracion,
                EmpaquesDecoraciones = lista
            });
        }

        // Calcular el campo derivado
        decimal costoPorCantidad = (empaque_decoracion.cantidad > 0) ? (empaque_decoracion.costo / empaque_decoracion.cantidad) : 0;
        var ed = db.tabla_empaques_decoraciones.Find(empaque_decoracion.id);
        if (ed != null)
        {
            ed.nombre = empaque_decoracion.nombre;
            ed.marca = empaque_decoracion.marca;
            ed.presentacion = empaque_decoracion.presentacion;
            ed.proveedor = empaque_decoracion.proveedor;
            ed.costo = empaque_decoracion.costo;
            ed.cantidad = empaque_decoracion.cantidad;
            ed.unidad_de_medida = empaque_decoracion.unidad_de_medida;
            ed.costo_por_cantidad = costoPorCantidad;
        }
        db.SaveChanges();
        TempData["SuccessMessage"] = "¡Empaque o decoración actualizado con éxito!";
        return RedirectToAction("empaques_decoraciones");
    }

    // Eliminar un empaque o decoración existente
    public ActionResult EliminarEmpaqueDecoracion(int id)
    {
        var ed = db.tabla_empaques_decoraciones.Find(id);
        if (ed != null)
        {
            db.tabla_empaques_decoraciones.Remove(ed);
        }
        db.SaveChanges();
        TempData["SuccessMessage"] = "¡Empaque o decoración eliminado con éxito!";
        return RedirectToAction("empaques_decoraciones");
    }

    // ----------- Implementos -----------

    // Listar y buscar implementos
    public ActionResult implementos(string search)
    {
        // Verificar si el usuario es administrador
        if (Session["Rol"] == null || (int)Session["Rol"] != 1)
        {
            return RedirectToAction("registro_usuarios", "Registro_Usuarios");
        }

        // Obtener los implementos y aplicar el filtro de búsqueda
        var query = db.tabla_implementos.AsQueryable();
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(i =>
                i.nombre.Contains(search) ||
                i.marca.Contains(search) ||
                i.presentacion.Contains(search) ||
                i.proveedor.Contains(search) ||
                i.costo.ToString().Contains(search) ||
                i.cantidad.ToString().Contains(search) ||
                i.unidad_de_medida.Contains(search) ||
                i.costo_por_cantidad.ToString().Contains(search)
            );
        }
        var implemento = new InsumosModel
        {
            Implementos = query.Select(i => new Implemento
            {
                id = i.id,
                nombre = i.nombre,
                marca = i.marca,
                presentacion = i.presentacion,
                proveedor = i.proveedor,
                costo = (decimal)i.costo,
                cantidad = (int)i.cantidad,
                unidad_de_medida = i.unidad_de_medida,
                costo_por_cantidad = (decimal)(i.costo_por_cantidad ?? 0)
            }).ToList()
        };
        ViewBag.Search = search;
        return View(implemento);
    }

    // Crear un nuevo implemento
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CrearImplemento(Implemento implemento)
    {
        var errores = new List<string>();

        // Validar que no exista otro implemento con el mismo nombre
        if (db.tabla_implementos.Any(i => i.nombre.ToLower() == implemento.nombre.ToLower()))
            errores.Add("Ya existe un implemento con ese nombre.");

        // Validar campos obligatorios y valores numéricos
        if (string.IsNullOrWhiteSpace(implemento.nombre) ||
                                      string.IsNullOrWhiteSpace(implemento.marca) ||
                                      string.IsNullOrWhiteSpace(implemento.presentacion) ||
                                      string.IsNullOrWhiteSpace(implemento.proveedor) ||
                                      string.IsNullOrWhiteSpace(implemento.unidad_de_medida))
            errores.Add("Todos los campos son obligatorios.");

        if (implemento.costo <= 0 || implemento.cantidad <= 0)
            errores.Add("Los valores numéricos deben ser mayores a cero.");

        if (errores.Any())
        {
            ViewBag.Errores = errores;
            var lista = db.tabla_implementos.Select(i => new Implemento
            {
                id = i.id,
                nombre = i.nombre,
                marca = i.marca,
                presentacion = i.presentacion,
                proveedor = i.proveedor,
                costo = (decimal)i.costo,
                cantidad = (int)i.cantidad,
                unidad_de_medida = i.unidad_de_medida,
                costo_por_cantidad = (decimal)i.costo_por_cantidad
            }).ToList();
            return View("implementos", new InsumosModel
            {
                ImplementoEditado = implemento,
                Implementos = lista
            });
        }

        // Calcular el campo derivado
        decimal costoPorCantidad = (implemento.cantidad > 0) ? (implemento.costo / implemento.cantidad) : 0;

        db.tabla_implementos.Add(new tabla_implementos
        {
            nombre = implemento.nombre,
            marca = implemento.marca,
            presentacion = implemento.presentacion,
            proveedor = implemento.proveedor,
            costo = implemento.costo,
            cantidad = implemento.cantidad,
            unidad_de_medida = implemento.unidad_de_medida,
            costo_por_cantidad = implemento.costo_por_cantidad
        });
        db.SaveChanges();
        TempData["SuccessMessage"] = "¡Implemento agregado con éxito!";
        return RedirectToAction("implementos");
    }

    // Editar un implemento existente (GET id)
    public ActionResult EditarImplemento(int id)
    {
        var i = db.tabla_implementos.Find(id);
        if (i == null) return HttpNotFound();
        var implemento = new Implemento
        {
            id = i.id,
            nombre = i.nombre,
            marca = i.marca,
            presentacion = i.presentacion,
            proveedor = i.proveedor,
            costo = (decimal)i.costo,
            cantidad = (int)i.cantidad,
            unidad_de_medida = i.unidad_de_medida,
            costo_por_cantidad = (decimal)i.costo_por_cantidad
        };

        //Obtén el listado de implementos
        var lista = db.tabla_implementos.Select(impl => new Implemento
        {
            id = impl.id,
            nombre = impl.nombre,
            marca = impl.marca,
            presentacion = impl.presentacion,
            proveedor = impl.proveedor,
            costo = (decimal)impl.costo,
            cantidad = (int)impl.cantidad,
            unidad_de_medida = impl.unidad_de_medida,
            costo_por_cantidad = (decimal)impl.costo_por_cantidad
        }).ToList();

        ViewBag.Editando = true;
        return View("implementos", new InsumosModel
        {
            ImplementoEditado = implemento,
            Implementos = lista
        });
    }

    // Editar un implemento existente (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditarImplemento(Implemento implemento)
    {
        var errores = new List<string>();

        // Validar que no exista otro implemento con el mismo nombre
        if (db.tabla_implementos.Any(impl => impl.nombre.ToLower() == implemento.nombre.ToLower() && impl.id != implemento.id))
            errores.Add("Ya existe un implemento con ese nombre.");

        // Validar campos obligatorios y valores numéricos
        if (string.IsNullOrWhiteSpace(implemento.nombre) ||
                                      string.IsNullOrWhiteSpace(implemento.marca) ||
                                      string.IsNullOrWhiteSpace(implemento.presentacion) ||
                                      string.IsNullOrWhiteSpace(implemento.proveedor) ||
                                      string.IsNullOrWhiteSpace(implemento.unidad_de_medida))
            errores.Add("Todos los campos son obligatorios.");

        if (implemento.costo <= 0 || implemento.cantidad <= 0)
            errores.Add("Los valores numéricos deben ser mayores a cero.");

        if (errores.Any())
        {
            ViewBag.Errores = errores;
            var lista = db.tabla_implementos.Select(impl => new Implemento
            {
                id = impl.id,
                nombre = impl.nombre,
                marca = impl.marca,
                presentacion = impl.presentacion,
                proveedor = impl.proveedor,
                costo = (decimal)impl.costo,
                cantidad = (int)impl.cantidad,
                unidad_de_medida = impl.unidad_de_medida,
                costo_por_cantidad = (decimal)impl.costo_por_cantidad
            }).ToList();
            return View("implementos", new InsumosModel
            {
                ImplementoEditado = implemento,
                Implementos = lista
            });
        }

        // Calcular el campo derivado
        decimal costoPorCantidad = (implemento.cantidad > 0) ? (implemento.costo / implemento.cantidad) : 0;
        var i = db.tabla_implementos.Find(implemento.id);
        if (i != null)
        {
            i.nombre = implemento.nombre;
            i.marca = implemento.marca;
            i.presentacion = implemento.presentacion;
            i.proveedor = implemento.proveedor;
            i.costo = implemento.costo;
            i.cantidad = implemento.cantidad;
            i.unidad_de_medida = implemento.unidad_de_medida;
            i.costo_por_cantidad = costoPorCantidad;
        }
        db.SaveChanges();
        TempData["SuccessMessage"] = "¡Implemento actualizado con éxito!";
        return RedirectToAction("implementos");
    }

    // Eliminar un implemento existente
    public ActionResult EliminarImplemento(int id)
    {
        var i = db.tabla_implementos.Find(id);
        if (i != null)
        {
            db.tabla_implementos.Remove(i);
        }
        db.SaveChanges();
        TempData["SuccessMessage"] = "¡Implemento eliminado con éxito!";
        return RedirectToAction("implementos");
    }

    // ----------- Suministros -----------

    // Listar y buscar suministros
    public ActionResult suministros(string search)
    {
        // Verificar si el usuario es administrador
        if (Session["Rol"] == null || (int)Session["Rol"] != 1)
        {
            return RedirectToAction("registro_usuarios", "Registro_Usuarios");
        }

        // Obtener los suministros y aplicar el filtro de búsqueda
        var query = db.tabla_suministros.AsQueryable();
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(s =>
                s.nombre.Contains(search) ||
                s.marca.Contains(search) ||
                s.presentacion.Contains(search) ||
                s.proveedor.Contains(search) ||
                s.costo.ToString().Contains(search) ||
                s.cantidad.ToString().Contains(search) ||
                s.unidad_de_medida.Contains(search) ||
                s.costo_por_cantidad.ToString().Contains(search)
            );
        }
        var suministro = new InsumosModel
        {
            Suministros = query.Select(s => new Suministro
            {
                id = s.id,
                nombre = s.nombre,
                marca = s.marca,
                presentacion = s.presentacion,
                proveedor = s.proveedor,
                costo = (decimal)s.costo,
                cantidad = (int)s.cantidad,
                unidad_de_medida = s.unidad_de_medida,
                costo_por_cantidad = (decimal)(s.costo_por_cantidad ?? 0)
            }).ToList()
        };
        ViewBag.Search = search;
        return View(suministro);
    }

    // Crear un nuevo suministro
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CrearSuministro(Suministro suministro)
    {
        var errores = new List<string>();

        // Validar que no exista otro suministro con el mismo nombre
        if (db.tabla_suministros.Any(s => s.nombre.ToLower() == suministro.nombre.ToLower()))
            errores.Add("Ya existe un suministro con ese nombre.");

        // Validar campos obligatorios y valores numéricos
        if (string.IsNullOrWhiteSpace(suministro.nombre) ||
                                      string.IsNullOrWhiteSpace(suministro.marca) ||
                                      string.IsNullOrWhiteSpace(suministro.presentacion) ||
                                      string.IsNullOrWhiteSpace(suministro.proveedor) ||
                                      string.IsNullOrWhiteSpace(suministro.unidad_de_medida))
            errores.Add("Todos los campos son obligatorios.");

        if (suministro.costo <= 0 || suministro.cantidad <= 0)
            errores.Add("Los valores numéricos deben ser mayores a cero.");

        if (errores.Any())
        {
            ViewBag.Errores = errores;
            var lista = db.tabla_suministros.Select(s => new Suministro
            {
                id = s.id,
                nombre = s.nombre,
                marca = s.marca,
                presentacion = s.presentacion,
                proveedor = s.proveedor,
                costo = (decimal)s.costo,
                cantidad = (int)s.cantidad,
                unidad_de_medida = s.unidad_de_medida,
                costo_por_cantidad = (decimal)s.costo_por_cantidad
            }).ToList();
            return View("suministros", new InsumosModel
            {
                SuministroEditado = suministro,
                Suministros = lista
            });
        }

        // Calcular el campo derivado
        decimal costoPorCantidad = (suministro.cantidad > 0) ? (suministro.costo / suministro.cantidad) : 0;

        db.tabla_suministros.Add(new tabla_suministros
        {
            nombre = suministro.nombre,
            marca = suministro.marca,
            presentacion = suministro.presentacion,
            proveedor = suministro.proveedor,
            costo = suministro.costo,
            cantidad = suministro.cantidad,
            unidad_de_medida = suministro.unidad_de_medida,
            costo_por_cantidad = costoPorCantidad
        });
        db.SaveChanges();
        TempData["SuccessMessage"] = "¡Suministro agregado con éxito!";
        return RedirectToAction("suministros");
    }

    // Editar un suministro existente (GET id)
    public ActionResult EditarSuministro(int id)
    {
        var s = db.tabla_suministros.Find(id);
        if (s == null) return HttpNotFound();
        var suministro = new Suministro
        {
            id = s.id,
            nombre = s.nombre,
            marca = s.marca,
            presentacion = s.presentacion,
            proveedor = s.proveedor,
            costo = (decimal)s.costo,
            cantidad = (int)s.cantidad,
            unidad_de_medida = s.unidad_de_medida,
            costo_por_cantidad = (decimal)s.costo_por_cantidad
        };

        // Obtén el listado de suministros
        var lista = db.tabla_suministros.Select(sumn => new Suministro
        {
            id = sumn.id,
            nombre = sumn.nombre,
            marca = sumn.marca,
            presentacion = sumn.presentacion,
            proveedor = sumn.proveedor,
            costo = (decimal)sumn.costo,
            cantidad = (int)sumn.cantidad,
            unidad_de_medida = sumn.unidad_de_medida,
            costo_por_cantidad = (decimal)sumn.costo_por_cantidad
        }).ToList();

        ViewBag.Editando = true;
        return View("suministros", new InsumosModel
        {
            SuministroEditado = suministro,
            Suministros = lista
        });
    }

    // Editar un suministro existente (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditarSuministro(Suministro suministro)
    {
        var errores = new List<string>();

        // Validar que no exista otro suministro con el mismo nombre
        if (db.tabla_suministros.Any(sumn => sumn.nombre.ToLower() == suministro.nombre.ToLower() && sumn.id != suministro.id))
            errores.Add("Ya existe un suministro con ese nombre.");


        // Validar campos obligatorios y valores numéricos
        if (string.IsNullOrWhiteSpace(suministro.nombre) ||
                                      string.IsNullOrWhiteSpace(suministro.marca) ||
                                      string.IsNullOrWhiteSpace(suministro.presentacion) ||
                                      string.IsNullOrWhiteSpace(suministro.proveedor) ||
                                      string.IsNullOrWhiteSpace(suministro.unidad_de_medida))
            errores.Add("Todos los campos son obligatorios.");


        if (suministro.costo <= 0 || suministro.cantidad <= 0)
            errores.Add("Los valores numéricos deben ser mayores a cero.");

        if (errores.Any())
        {
            ViewBag.Errores = errores;
            var lista = db.tabla_suministros.Select(sumn => new Suministro
            {
                id = sumn.id,
                nombre = sumn.nombre,
                marca = sumn.marca,
                presentacion = sumn.presentacion,
                proveedor = sumn.proveedor,
                costo = (decimal)sumn.costo,
                cantidad = (int)sumn.cantidad,
                unidad_de_medida = sumn.unidad_de_medida,
                costo_por_cantidad = (decimal)sumn.costo_por_cantidad
            }).ToList();
            return View("suministros", new InsumosModel
            {
                SuministroEditado = suministro,
                Suministros = lista
            });
        }

        // Calcular el campo derivado
        decimal costoPorCantidad = (suministro.cantidad > 0) ? (suministro.costo / suministro.cantidad) : 0;
        var s = db.tabla_suministros.Find(suministro.id);
        if (s != null)
        {
            s.nombre = suministro.nombre;
            s.marca = suministro.marca;
            s.presentacion = suministro.presentacion;
            s.proveedor = suministro.proveedor;
            s.costo = suministro.costo;
            s.cantidad = suministro.cantidad;
            s.unidad_de_medida = suministro.unidad_de_medida;
            s.costo_por_cantidad = costoPorCantidad;
        }
        db.SaveChanges();
        TempData["SuccessMessage"] = "¡Suministro actualizado con éxito!";
        return RedirectToAction("suministros");
    }

    // Eliminar un suministro existente
    public ActionResult EliminarSuministro(int id)
    {
        var s = db.tabla_suministros.Find(id);
        if (s != null)
        {
            db.tabla_suministros.Remove(s);
        }
        db.SaveChanges();
        TempData["SuccessMessage"] = "¡Suministro eliminado con éxito!";
        return RedirectToAction("suministros");
    }

   // ----------- Costos de Recetas -----------

    // Listar y buscar recetas
    public ActionResult costos_recetas(string search)
    {
        // Verificar si el usuario es administrador
        if (Session["Rol"] == null || (int)Session["Rol"] != 1)
        {
            return RedirectToAction("registro_usuarios", "Registro_Usuarios");
        }

        // Obtener las recetas y aplicar el filtro de búsqueda
        var query = db.tabla_costos_recetas.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(r =>
                r.nombre.Contains(search) ||
                r.porcion.ToString().Contains(search) ||
                r.costo_total_receta.ToString().Contains(search) ||
                r.costo_por_porcion.ToString().Contains(search) ||

                db.costos_receta_materias_primas_utilizadas.Any(mp =>
                    mp.id_receta == r.id &&
                    (
                        mp.tabla_materias_primas.nombre.Contains(search) ||
                        mp.cantidad.ToString().Contains(search) ||
                        mp.unidad_de_medida.Contains(search) ||
                        mp.costo_por_cantidad.ToString().Contains(search) ||
                        mp.total_costo.ToString().Contains(search)
                    )
                ) ||

                db.costos_receta_productos_preparados_utilizados.Any(pp =>
                    pp.id_receta == r.id &&
                    (
                        pp.tabla_productos_preparados.nombre.Contains(search) ||
                        pp.cantidad.ToString().Contains(search) ||
                        pp.unidad_de_medida.Contains(search) ||
                        pp.costo_por_cantidad.ToString().Contains(search) ||
                        pp.total_costo.ToString().Contains(search)
                    )
                )
            );
        }

        var receta = new InsumosModel
        {
            CostosRecetas = query.Select(r => new Receta
            {
                id = r.id,
                nombre = r.nombre,
                porcion = r.porcion ?? 0,
                costo_total_receta = r.costo_total_receta ?? 0,
                costo_por_porcion = r.costo_por_porcion ?? 0,
                MateriasPrimasUtilizadas = db.costos_receta_materias_primas_utilizadas
                    .Where(mp => mp.id_receta == r.id)
                    .Select(mp => new MateriaPrimaUtilizada
                    {
                        id = mp.id,
                        id_materia_prima_utilizada = mp.id_materia_prima_utilizada ?? 0,
                        nombre = mp.tabla_materias_primas.nombre,
                        cantidad = mp.cantidad ?? 0,
                        unidad_de_medida = mp.unidad_de_medida,
                        costo_por_cantidad = mp.costo_por_cantidad ?? 0,
                        total_costo = mp.total_costo ?? 0
                    }).ToList(),

                ProductosPreparadosUtilizados = db.costos_receta_productos_preparados_utilizados
                    .Where(pp => pp.id_receta == r.id)
                    .Select(pp => new ProductoPreparadoUtilizado
                    {
                        id = pp.id,
                        id_producto_preparado_utilizado = pp.id_producto_preparado_utilizado ?? 0,
                        nombre = pp.tabla_productos_preparados.nombre,
                        cantidad = pp.cantidad ?? 0,
                        unidad_de_medida = pp.unidad_de_medida,
                        costo_por_cantidad = pp.costo_por_cantidad ?? 0,
                        total_costo = pp.total_costo ?? 0
                    }).ToList()
            }).ToList()
        };
        ViewBag.Search = search;
        ViewBag.MateriasPrimas = new SelectList(db.tabla_materias_primas.ToList(), "nombre", "nombre");
        ViewBag.ProductosPreparados = new SelectList(db.tabla_productos_preparados.ToList(), "nombre", "nombre");
        return View(receta);
    }

    // Crear una nueva receta
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CrearReceta(Receta receta)
    {
        var errores = new List<string>();

        // Validar nombre único
        if (db.tabla_costos_recetas.Any(rec => rec.nombre.ToLower() == receta.nombre.ToLower()))
            errores.Add("Ya existe una receta con ese nombre.");

        // Validar campos principales
        if (string.IsNullOrWhiteSpace(receta.nombre))
            errores.Add("El nombre de la receta es obligatorio.");

        if (receta.porcion <= 0)
            errores.Add("La porción debe ser mayor a cero.");

        decimal costoTotalReceta = 0;

        // Validar filas de Materias Primas
        if (receta.MateriasPrimasUtilizadas != null)
        {
            var nombresMP = new HashSet<string>();
            for (int i = 0; i < receta.MateriasPrimasUtilizadas.Count; i++)
            {
                var mp = receta.MateriasPrimasUtilizadas[i];
                string nombre = mp?.nombre?.Trim().ToLower();

                if (string.IsNullOrWhiteSpace(mp.nombre) && mp.cantidad == 0 && string.IsNullOrWhiteSpace(mp.unidad_de_medida))
                {
                    errores.Add($"Fila {i + 1} de Materias Primas: No puede dejar filas vacías.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(mp.nombre))
                    errores.Add($"Fila {i + 1} de Materias Primas: Debe seleccionar una materia prima.");

                if (mp.cantidad <= 0)
                    errores.Add($"Fila {i + 1} de Materias Primas: La cantidad debe ser mayor a cero.");

                if (string.IsNullOrWhiteSpace(mp.unidad_de_medida) ||
                    (mp.unidad_de_medida.ToLower() != "unidad" && mp.unidad_de_medida.ToLower() != "unidades"))
                    errores.Add($"Fila {i + 1} de Materias Primas: La unidad de medida debe ser 'unidad' o 'unidades'.");

                if (mp.cantidad == 1 && mp.unidad_de_medida.ToLower() != "unidad")
                    errores.Add($"Fila {i + 1} de Materias Primas: Si la cantidad es 1, debe ser 'unidad'.");

                if (mp.cantidad > 1 && mp.unidad_de_medida.ToLower() != "unidades")
                    errores.Add($"Fila {i + 1} de Materias Primas: Si la cantidad es mayor a 1, debe ser 'unidades'.");

                if (!string.IsNullOrWhiteSpace(nombre))
                {
                    if (!nombresMP.Add(nombre))
                        errores.Add($"Fila {i + 1} de Materias Primas: Materia prima repetida: {mp.nombre}");

                    // Validar existencia en BD
                    var materia_prima = db.tabla_materias_primas.FirstOrDefault(m => m.nombre.ToLower() == nombre);
                    if (materia_prima == null)
                    {
                        errores.Add($"Fila {i + 1} de Materias Primas: La materia prima '{mp.nombre}' no existe en el sistema.");
                        continue;
                    }

                    // Si existe, asignar valores para cálculos
                    mp.id_materia_prima_utilizada = materia_prima.id;
                    mp.costo_por_cantidad = (decimal)(materia_prima.costo_por_gramo_con_merma ?? 0);
                    mp.total_costo = mp.cantidad * mp.costo_por_cantidad;
                    costoTotalReceta += mp.total_costo;
                }
                
            }
        }

        // Validar filas de Productos Preparados
        if (receta.ProductosPreparadosUtilizados != null)
        {
            var nombresPP = new HashSet<string>();
            for (int i = 0; i < receta.ProductosPreparadosUtilizados.Count; i++)
            {
                var pp = receta.ProductosPreparadosUtilizados[i];
                string nombre = pp?.nombre?.Trim().ToLower();

                if (string.IsNullOrWhiteSpace(pp.nombre) && pp.cantidad == 0 && string.IsNullOrWhiteSpace(pp.unidad_de_medida))
                {
                    errores.Add($"Fila {i + 1} de Productos Preparados: No puede dejar filas vacías.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(pp.nombre))
                    errores.Add($"Fila {i + 1} de Productos Preparados: Debe seleccionar un producto preparado.");

                if (pp.cantidad <= 0)
                    errores.Add($"Fila {i + 1} de Productos Preparados: La cantidad debe ser mayor a cero.");

                if (string.IsNullOrWhiteSpace(pp.unidad_de_medida) ||
                    (pp.unidad_de_medida.ToLower() != "unidad" && pp.unidad_de_medida.ToLower() != "unidades"))
                    errores.Add($"Fila {i + 1} de Productos Preparados: La unidad debe ser 'unidad' o 'unidades'.");

                if (pp.cantidad == 1 && pp.unidad_de_medida.ToLower() != "unidad")
                    errores.Add($"Fila {i + 1} de Productos Preparados: Si la cantidad es 1, debe ser 'unidad'.");

                if (pp.cantidad > 1 && pp.unidad_de_medida.ToLower() != "unidades")
                    errores.Add($"Fila {i + 1} de Productos Preparados: Si la cantidad es mayor a 1, debe ser 'unidades'.");

                if (!string.IsNullOrWhiteSpace(nombre))
                {
                    if (!nombresPP.Add(nombre))
                        errores.Add($"Fila {i + 1} de Productos Preparados: Producto preparado repetido: {pp.nombre}");

                    // Validar existencia en BD
                    var producto_preparado = db.tabla_productos_preparados.FirstOrDefault(p => p.nombre.ToLower() == nombre);
                    if (producto_preparado == null)
                    {
                        errores.Add($"Fila {i + 1} de Productos Preparados: El producto preparado '{pp.nombre}' no existe en el sistema.");
                        continue;
                    }

                    // Si existe, asignar valores para cálculos
                    pp.id_producto_preparado_utilizado = producto_preparado.id;
                    pp.costo_por_cantidad = (decimal)(producto_preparado.costo_por_peso ?? 0);
                    pp.total_costo = pp.cantidad * pp.costo_por_cantidad;
                    costoTotalReceta += pp.total_costo;

                }
            }
        }

        if (errores.Any())
        {
            ViewBag.Errores = errores;
            ViewBag.MateriasPrimas = new SelectList(db.tabla_materias_primas.ToList(), "nombre", "nombre");
            ViewBag.ProductosPreparados = new SelectList(db.tabla_productos_preparados.ToList(), "nombre", "nombre");

            var lista = db.tabla_costos_recetas.Select(rec => new Receta
            {
                id = rec.id,
                nombre = rec.nombre,
                porcion = rec.porcion ?? 0,
                costo_total_receta = rec.costo_total_receta ?? 0,
                costo_por_porcion = rec.costo_por_porcion ?? 0,

                MateriasPrimasUtilizadas = db.costos_receta_materias_primas_utilizadas
                .Where(mp => mp.id_receta == rec.id)
                .Select(mp => new MateriaPrimaUtilizada
                {
                    id = mp.id,
                    id_materia_prima_utilizada = mp.id_materia_prima_utilizada ?? 0,
                    nombre = mp.tabla_materias_primas.nombre,
                    cantidad = mp.cantidad ?? 0,
                    unidad_de_medida = mp.unidad_de_medida,
                    costo_por_cantidad = mp.costo_por_cantidad ?? 0,
                    total_costo = mp.total_costo ?? 0
                }).ToList(),

                ProductosPreparadosUtilizados = db.costos_receta_productos_preparados_utilizados
                .Where(pp => pp.id_receta == rec.id)
                .Select(pp => new ProductoPreparadoUtilizado
                {
                    id = pp.id,
                    id_producto_preparado_utilizado = pp.id_producto_preparado_utilizado ?? 0,
                    nombre = pp.tabla_productos_preparados.nombre,
                    cantidad = pp.cantidad ?? 0,
                    unidad_de_medida = pp.unidad_de_medida,
                    costo_por_cantidad = pp.costo_por_cantidad ?? 0,
                    total_costo = pp.total_costo ?? 0
                }).ToList()

            }).ToList();

            return View("costos_recetas", new InsumosModel
            {
                RecetaEditada = receta,
                CostosRecetas = lista
            });
        }

        // Calcular costos y guardar
        if (receta.MateriasPrimasUtilizadas != null)
        {
            foreach (var mp in receta.MateriasPrimasUtilizadas)
            {
                var materiaPrima = db.tabla_materias_primas.FirstOrDefault(m => m.nombre == mp.nombre);
                if (materiaPrima != null)
                {
                    mp.id_materia_prima_utilizada = materiaPrima.id;
                    mp.costo_por_cantidad = (decimal)(materiaPrima.costo_por_gramo_con_merma ?? 0);
                    mp.total_costo = mp.cantidad * mp.costo_por_cantidad;
                    costoTotalReceta += mp.total_costo;
                }
            }
        }

        if (receta.ProductosPreparadosUtilizados != null)
        {
            foreach (var pp in receta.ProductosPreparadosUtilizados)
            {
                var productoPreparado = db.tabla_productos_preparados.FirstOrDefault(p => p.nombre == pp.nombre);
                if (productoPreparado != null)
                {
                    pp.id_producto_preparado_utilizado = productoPreparado.id;
                    pp.costo_por_cantidad = (decimal)(productoPreparado.costo_por_peso ?? 0);
                    pp.total_costo = pp.cantidad * pp.costo_por_cantidad;
                    costoTotalReceta += pp.total_costo;
                }
            }
        }

        decimal costoPorPorcion = (receta.porcion > 0) ? (costoTotalReceta / receta.porcion) : 0;

        var r = new tabla_costos_recetas
        {
            nombre = receta.nombre,
            porcion = receta.porcion,
            costo_total_receta = costoTotalReceta,
            costo_por_porcion = costoPorPorcion
        };
        db.tabla_costos_recetas.Add(r);
        db.SaveChanges();

        if (receta.MateriasPrimasUtilizadas != null)
        {
            foreach (var mp in receta.MateriasPrimasUtilizadas)
            {
                db.costos_receta_materias_primas_utilizadas.Add(new costos_receta_materias_primas_utilizadas
                {
                    id_receta = r.id,
                    id_materia_prima_utilizada = mp.id_materia_prima_utilizada,
                    cantidad = mp.cantidad,
                    unidad_de_medida = mp.unidad_de_medida,
                    costo_por_cantidad = mp.costo_por_cantidad,
                    total_costo = mp.total_costo
                });
            }
        }

        if (receta.ProductosPreparadosUtilizados != null)
        {
            foreach (var pp in receta.ProductosPreparadosUtilizados)
            {
                db.costos_receta_productos_preparados_utilizados.Add(new costos_receta_productos_preparados_utilizados
                {
                    id_receta = r.id,
                    id_producto_preparado_utilizado = pp.id_producto_preparado_utilizado,
                    cantidad = pp.cantidad,
                    unidad_de_medida = pp.unidad_de_medida,
                    costo_por_cantidad = pp.costo_por_cantidad,
                    total_costo = pp.total_costo
                });
            }
        }

        db.SaveChanges();
        TempData["SuccessMessage"] = "¡Receta creada con éxito!";
        return RedirectToAction("costos_recetas");
    }

    // Editar receta existente (GET id)
    public ActionResult EditarReceta(int id)
    {
        var r = db.tabla_costos_recetas.Find(id);
        if (r == null) return HttpNotFound();

        // Receta a editar
        var receta = new Receta
        {
            id = r.id,
            nombre = r.nombre,
            porcion = r.porcion ?? 0,
            costo_total_receta = r.costo_total_receta ?? 0,
            costo_por_porcion = r.costo_por_porcion ?? 0,

            MateriasPrimasUtilizadas = db.costos_receta_materias_primas_utilizadas
                .Where(mp => mp.id_receta == r.id)
                .Select(mp => new MateriaPrimaUtilizada
                {
                    id = mp.id,
                    id_materia_prima_utilizada = mp.id_materia_prima_utilizada ?? 0,
                    nombre = mp.tabla_materias_primas.nombre,
                    cantidad = mp.cantidad ?? 0,
                    unidad_de_medida = mp.unidad_de_medida,
                    costo_por_cantidad = mp.costo_por_cantidad ?? 0,
                    total_costo = mp.total_costo ?? 0
                }).ToList(),

            ProductosPreparadosUtilizados = db.costos_receta_productos_preparados_utilizados
                .Where(pp => pp.id_receta == r.id)
                .Select(pp => new ProductoPreparadoUtilizado
                {
                    id = pp.id,
                    id_producto_preparado_utilizado = pp.id_producto_preparado_utilizado ?? 0,
                    nombre = pp.tabla_productos_preparados.nombre,
                    cantidad = pp.cantidad ?? 0,
                    unidad_de_medida = pp.unidad_de_medida,
                    costo_por_cantidad = pp.costo_por_cantidad ?? 0,
                    total_costo = pp.total_costo ?? 0
                }).ToList()
        };

        // Listado completo de recetas para mostrar en la tabla
        var lista = db.tabla_costos_recetas.Select(rec => new Receta
        {
            id = rec.id,
            nombre = rec.nombre,
            porcion = rec.porcion ?? 0,
            costo_total_receta = rec.costo_total_receta ?? 0,
            costo_por_porcion = rec.costo_por_porcion ?? 0,

            MateriasPrimasUtilizadas = db.costos_receta_materias_primas_utilizadas
                .Where(mp => mp.id_receta == rec.id)
                .Select(mp => new MateriaPrimaUtilizada
                {
                    id = mp.id,
                    id_materia_prima_utilizada = mp.id_materia_prima_utilizada ?? 0,
                    nombre = mp.tabla_materias_primas.nombre,
                    cantidad = mp.cantidad ?? 0,
                    unidad_de_medida = mp.unidad_de_medida,
                    costo_por_cantidad = mp.costo_por_cantidad ?? 0,
                    total_costo = mp.total_costo ?? 0
                }).ToList(),

            ProductosPreparadosUtilizados = db.costos_receta_productos_preparados_utilizados
                .Where(pp => pp.id_receta == rec.id)
                .Select(pp => new ProductoPreparadoUtilizado
                {
                    id = pp.id,
                    id_producto_preparado_utilizado = pp.id_producto_preparado_utilizado ?? 0,
                    nombre = pp.tabla_productos_preparados.nombre,
                    cantidad = pp.cantidad ?? 0,
                    unidad_de_medida = pp.unidad_de_medida,
                    costo_por_cantidad = pp.costo_por_cantidad ?? 0,
                    total_costo = pp.total_costo ?? 0
                }).ToList()
        }).ToList();

        ViewBag.Editando = true;
        ViewBag.MateriasPrimas = new SelectList(db.tabla_materias_primas.ToList(), "nombre", "nombre");
        ViewBag.ProductosPreparados = new SelectList(db.tabla_productos_preparados.ToList(), "nombre", "nombre");
        return View("costos_recetas", new InsumosModel
        {
            RecetaEditada = receta,
            CostosRecetas = lista
        });
    }

    // Editar receta existente (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditarReceta(Receta receta)
    {
        var errores = new List<string>();

        if (db.tabla_costos_recetas.Any(rec => rec.nombre.ToLower() == receta.nombre.ToLower() && rec.id != receta.id))
            errores.Add("Ya existe una receta con ese nombre.");

        if (string.IsNullOrWhiteSpace(receta.nombre))
            errores.Add("El nombre de la receta es obligatorio.");

        if (receta.porcion <= 0)
            errores.Add("La porción debe ser mayor a cero.");

        decimal costoTotalReceta = 0;

        // Validar filas de Materias Primas
        if (receta.MateriasPrimasUtilizadas != null)
        {
            var nombresMP = new HashSet<string>();
            for (int i = 0; i < receta.MateriasPrimasUtilizadas.Count; i++)
            {
                var mp = receta.MateriasPrimasUtilizadas[i];
                string nombre = mp?.nombre?.Trim().ToLower();

                if (string.IsNullOrWhiteSpace(mp.nombre) && mp.cantidad == 0 && string.IsNullOrWhiteSpace(mp.unidad_de_medida))
                {
                    errores.Add($"Fila {i + 1} de Materias Primas: No puede dejar filas vacías.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(mp.nombre))
                    errores.Add($"Fila {i + 1} de Materias Primas: Debe seleccionar una materia prima.");

                if (mp.cantidad <= 0)
                    errores.Add($"Fila {i + 1} de Materias Primas: La cantidad debe ser mayor a cero.");

                if (string.IsNullOrWhiteSpace(mp.unidad_de_medida) ||
                    (mp.unidad_de_medida.ToLower() != "unidad" && mp.unidad_de_medida.ToLower() != "unidades"))
                    errores.Add($"Fila {i + 1} de Materias Primas: La unidad de medida debe ser 'unidad' o 'unidades'.");

                if (mp.cantidad == 1 && mp.unidad_de_medida.ToLower() != "unidad")
                    errores.Add($"Fila {i + 1} de Materias Primas: Si la cantidad es 1, debe ser 'unidad'.");

                if (mp.cantidad > 1 && mp.unidad_de_medida.ToLower() != "unidades")
                    errores.Add($"Fila {i + 1} de Materias Primas: Si la cantidad es mayor a 1, debe ser 'unidades'.");

                if (!string.IsNullOrWhiteSpace(nombre))
                {
                    if (!nombresMP.Add(nombre))
                        errores.Add($"Fila {i + 1} de Materias Primas: Materia prima repetida: {mp.nombre}");
                    
                    // Validar existencia en BD
                    var materia_prima = db.tabla_materias_primas.FirstOrDefault(m => m.nombre.ToLower() == nombre);
                    if (materia_prima == null)
                    {
                        errores.Add($"Fila {i + 1} de Materias Primas: La materia prima '{mp.nombre}' no existe en el sistema.");
                        continue;
                    }

                    // Si existe, asignar valores para cálculos
                    mp.id_materia_prima_utilizada = materia_prima.id;
                    mp.costo_por_cantidad = (decimal)(materia_prima.costo_por_gramo_con_merma ?? 0);
                    mp.total_costo = mp.cantidad * mp.costo_por_cantidad;
                    costoTotalReceta += mp.total_costo;
                }

            }
        }

        // Validar filas de Productos Preparados
        if (receta.ProductosPreparadosUtilizados != null)
        {
            var nombresPP = new HashSet<string>();
            for (int i = 0; i < receta.ProductosPreparadosUtilizados.Count; i++)
            {
                var pp = receta.ProductosPreparadosUtilizados[i];
                string nombre = pp?.nombre?.Trim().ToLower();

                if (string.IsNullOrWhiteSpace(pp.nombre) && pp.cantidad == 0 && string.IsNullOrWhiteSpace(pp.unidad_de_medida))
                {
                    errores.Add($"Fila {i + 1} de Productos Preparados: No puede dejar filas vacías.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(pp.nombre))
                    errores.Add($"Fila {i + 1} de Productos Preparados: Debe seleccionar un producto preparado.");

                if (pp.cantidad <= 0)
                    errores.Add($"Fila {i + 1} de Productos Preparados: La cantidad debe ser mayor a cero.");

                if (string.IsNullOrWhiteSpace(pp.unidad_de_medida) ||
                    (pp.unidad_de_medida.ToLower() != "unidad" && pp.unidad_de_medida.ToLower() != "unidades"))
                    errores.Add($"Fila {i + 1} de Productos Preparados: La unidad debe ser 'unidad' o 'unidades'.");

                if (pp.cantidad == 1 && pp.unidad_de_medida.ToLower() != "unidad")
                    errores.Add($"Fila {i + 1} de Productos Preparados: Si la cantidad es 1, debe ser 'unidad'.");

                if (pp.cantidad > 1 && pp.unidad_de_medida.ToLower() != "unidades")
                    errores.Add($"Fila {i + 1} de Productos Preparados: Si la cantidad es mayor a 1, debe ser 'unidades'.");

                if (!string.IsNullOrWhiteSpace(nombre))
                {
                    if (!nombresPP.Add(nombre))
                        errores.Add($"Fila {i + 1} de Productos Preparados: Producto preparado repetido: {pp.nombre}");
                    
                    // Validar existencia en BD
                    var producto_preparado = db.tabla_productos_preparados.FirstOrDefault(p => p.nombre.ToLower() == nombre);
                    if (producto_preparado == null)
                    {
                        errores.Add($"Fila {i + 1} de Productos Preparados: El producto preparado '{pp.nombre}' no existe en el sistema.");
                        continue;
                    }

                    // Si existe, asignar valores para cálculos
                    pp.id_producto_preparado_utilizado = producto_preparado.id;
                    pp.costo_por_cantidad = (decimal)(producto_preparado.costo_por_peso ?? 0);
                    pp.total_costo = pp.cantidad * pp.costo_por_cantidad;
                    costoTotalReceta += pp.total_costo;

                }
            }
        }

        if (errores.Any())
        {
            ViewBag.Errores = errores;
            ViewBag.Editando = true;
            ViewBag.MateriasPrimas = new SelectList(db.tabla_materias_primas.ToList(), "nombre", "nombre");
            ViewBag.ProductosPreparados = new SelectList(db.tabla_productos_preparados.ToList(), "nombre", "nombre");

            var lista = db.tabla_costos_recetas.Select(rec => new Receta
            {
                id = rec.id,
                nombre = rec.nombre,
                porcion = rec.porcion ?? 0,
                costo_total_receta = rec.costo_total_receta ?? 0,
                costo_por_porcion = rec.costo_por_porcion ?? 0,

                MateriasPrimasUtilizadas = db.costos_receta_materias_primas_utilizadas
                .Where(mp => mp.id_receta == rec.id)
                .Select(mp => new MateriaPrimaUtilizada
                {
                    id = mp.id,
                    id_materia_prima_utilizada = mp.id_materia_prima_utilizada ?? 0,
                    nombre = mp.tabla_materias_primas.nombre,
                    cantidad = mp.cantidad ?? 0,
                    unidad_de_medida = mp.unidad_de_medida,
                    costo_por_cantidad = mp.costo_por_cantidad ?? 0,
                    total_costo = mp.total_costo ?? 0
                }).ToList(),

                ProductosPreparadosUtilizados = db.costos_receta_productos_preparados_utilizados
                .Where(pp => pp.id_receta == rec.id)
                .Select(pp => new ProductoPreparadoUtilizado
                {
                    id = pp.id,
                    id_producto_preparado_utilizado = pp.id_producto_preparado_utilizado ?? 0,
                    nombre = pp.tabla_productos_preparados.nombre,
                    cantidad = pp.cantidad ?? 0,
                    unidad_de_medida = pp.unidad_de_medida,
                    costo_por_cantidad = pp.costo_por_cantidad ?? 0,
                    total_costo = pp.total_costo ?? 0
                }).ToList()
            }).ToList();

            return View("costos_recetas", new InsumosModel
            {
                RecetaEditada = receta,
                CostosRecetas = lista
            });
        }

        if (receta.MateriasPrimasUtilizadas != null)
        {
            foreach (var mp in receta.MateriasPrimasUtilizadas)
            {
                var materiaPrima = db.tabla_materias_primas.FirstOrDefault(m => m.nombre == mp.nombre);
                if (materiaPrima != null)
                {
                    mp.id_materia_prima_utilizada = materiaPrima.id;
                    mp.costo_por_cantidad = (decimal)(materiaPrima.costo_por_gramo_con_merma ?? 0);
                    mp.total_costo = mp.cantidad * mp.costo_por_cantidad;
                    costoTotalReceta += mp.total_costo;
                }
            }
        }

        if (receta.ProductosPreparadosUtilizados != null)
        {
            foreach (var pp in receta.ProductosPreparadosUtilizados)
            {
                var productoPreparado = db.tabla_productos_preparados.FirstOrDefault(p => p.nombre == pp.nombre);
                if (productoPreparado != null)
                {
                    pp.id_producto_preparado_utilizado = productoPreparado.id;
                    pp.costo_por_cantidad = (decimal)(productoPreparado.costo_por_peso ?? 0);
                    pp.total_costo = pp.cantidad * pp.costo_por_cantidad;
                    costoTotalReceta += pp.total_costo;
                }
            }
        }

        decimal costoPorPorcion = (receta.porcion > 0) ? (costoTotalReceta / receta.porcion) : 0;

        var r = db.tabla_costos_recetas.Find(receta.id);
        if (r == null) return HttpNotFound();

        r.nombre = receta.nombre;
        r.porcion = receta.porcion;
        r.costo_total_receta = costoTotalReceta;
        r.costo_por_porcion = costoPorPorcion;

        db.costos_receta_materias_primas_utilizadas.RemoveRange(
            db.costos_receta_materias_primas_utilizadas.Where(x => x.id_receta == receta.id));
        db.costos_receta_productos_preparados_utilizados.RemoveRange(
            db.costos_receta_productos_preparados_utilizados.Where(x => x.id_receta == receta.id));

        if (receta.MateriasPrimasUtilizadas != null)
        {
            foreach (var mp in receta.MateriasPrimasUtilizadas)
            {
                db.costos_receta_materias_primas_utilizadas.Add(new costos_receta_materias_primas_utilizadas
                {
                    id_receta = r.id,
                    id_materia_prima_utilizada = mp.id_materia_prima_utilizada,
                    cantidad = mp.cantidad,
                    unidad_de_medida = mp.unidad_de_medida,
                    costo_por_cantidad = mp.costo_por_cantidad,
                    total_costo = mp.total_costo
                });
            }
        }

        if (receta.ProductosPreparadosUtilizados != null)
        {
            foreach (var pp in receta.ProductosPreparadosUtilizados)
            {
                db.costos_receta_productos_preparados_utilizados.Add(new costos_receta_productos_preparados_utilizados
                {
                    id_receta = r.id,
                    id_producto_preparado_utilizado = pp.id_producto_preparado_utilizado,
                    cantidad = pp.cantidad,
                    unidad_de_medida = pp.unidad_de_medida,
                    costo_por_cantidad = pp.costo_por_cantidad,
                    total_costo = pp.total_costo
                });
            }
        }

        db.SaveChanges();
        TempData["SuccessMessage"] = "¡Receta actualizada con éxito!";
        return RedirectToAction("costos_recetas");
    }

    // Eliminar una receta existente
    public ActionResult EliminarReceta(int id)
    {
        var r = db.tabla_costos_recetas.Find(id);
        if (r != null)
        {
            var mp = db.costos_receta_materias_primas_utilizadas.Where(x => x.id_receta == id).ToList();
            foreach (var item in mp) db.costos_receta_materias_primas_utilizadas.Remove(item);

            var pp = db.costos_receta_productos_preparados_utilizados.Where(x => x.id_receta == id).ToList();
            foreach (var item in pp) db.costos_receta_productos_preparados_utilizados.Remove(item);

            db.tabla_costos_recetas.Remove(r);
        }
        db.SaveChanges();
        TempData["SuccessMessage"] = "¡Receta eliminada con éxito!";
        return RedirectToAction("costos_recetas");
    }

    // ----------- Precios Finales Sugeridos de Productos Finales -----------

    // Listar y buscar productos finales
    public ActionResult precio_final(string search)
    {
        // Verificar si el usuario es administrador
        if (Session["Rol"] == null || (int)Session["Rol"] != 1)
        {
            return RedirectToAction("registro_usuarios", "Registro_Usuarios");
        }

        var query = db.tabla_precios_finales_sugeridos.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(pf =>
                pf.nombre_receta.Contains(search) ||
                pf.costo_total_receta.ToString().Contains(search) ||
                pf.margen_de_utilidad.ToString().Contains(search) ||
                pf.costo_sin_margen_de_utilidad.ToString().Contains(search) ||
                pf.costo_con_margen_de_utilidad.ToString().Contains(search) ||
                pf.costo_empaque_decoracion_utilizado.ToString().Contains(search) ||
                pf.costo_implemento_utilizado.ToString().Contains(search) ||
                pf.costo_suministro_utilizado.ToString().Contains(search) ||
                pf.factura.ToString().Contains(search) ||
                pf.factura_por_insumo.ToString().Contains(search) ||
                pf.costo_total_de_impresion_de_factura.ToString().Contains(search) ||
                pf.costo_total_empaque_decoracion_implemento_suministro.ToString().Contains(search) ||
                pf.costo_suministro_por_porcentaje_de_ganancia.ToString().Contains(search) ||
                pf.costo_implemento_por_porcentaje_de_ganancia.ToString().Contains(search) ||
                pf.costo_suministro_por_porcentaje_de_ganancia.ToString().Contains(search) ||
                pf.iva.ToString().Contains(search) ||
                pf.impuesto_de_servicio.ToString().Contains(search) ||
                pf.envio.ToString().Contains(search) ||
                pf.plataforma_de_envio.Contains(search) ||
                pf.precio_final_sugerido.ToString().Contains(search) ||

                db.precios_empaques_decoraciones_utilizados.Any(ed =>
                    ed.id_precio_final_sugerido == pf.id &&
                    (
                        ed.tabla_empaques_decoraciones.nombre.Contains(search) ||
                        ed.cantidad.ToString().Contains(search) ||
                        ed.unidad_de_medida.Contains(search) ||
                        ed.costo_por_cantidad.ToString().Contains(search) ||
                        ed.total_costo.ToString().Contains(search)
                    )
                ) ||

                db.precios_implementos_utilizados.Any(i =>
                    i.id_precio_final_sugerido == pf.id &&
                    (
                        i.tabla_implementos.nombre.Contains(search) ||
                        i.cantidad.ToString().Contains(search) ||
                        i.unidad_de_medida.Contains(search) ||
                        i.costo_por_cantidad.ToString().Contains(search) ||
                        i.total_costo.ToString().Contains(search)
                    )
                ) ||

                db.precios_suministros_utilizados.Any(s =>
                    s.id_precio_final_sugerido == pf.id &&
                    (
                        s.tabla_suministros.nombre.Contains(search) ||
                        s.cantidad.ToString().Contains(search) ||
                        s.unidad_de_medida.Contains(search) ||
                        s.costo_por_cantidad.ToString().Contains(search) ||
                        s.total_costo.ToString().Contains(search)
                    )
                )
            );
        }

        var producto_final = new InsumosModel
        {
            ProductosFinales = query.Select(pf => new ProductoFinal
            {
                id = pf.id,
                id_receta = pf.id_receta ?? 0,
                nombre_receta = pf.nombre_receta,
                costo_total_receta = pf.costo_total_receta ?? 0,
                margen_de_utilidad = pf.margen_de_utilidad ?? 0,
                costo_sin_margen_de_utilidad = pf.costo_sin_margen_de_utilidad ?? 0,
                costo_con_margen_de_utilidad = pf.costo_con_margen_de_utilidad ?? 0,
                costo_empaque_decoracion_utilizado = pf.costo_empaque_decoracion_utilizado ?? 0,
                costo_implemento_utilizado = pf.costo_implemento_utilizado ?? 0,
                costo_suministro_utilizado = pf.costo_suministro_utilizado ?? 0,
                costo_de_impresion_de_factura_por_insumo = pf.costo_de_impresion_de_factura_por_insumo ?? 0,
                costo_total_de_impresion_de_factura = pf.costo_total_de_impresion_de_factura ?? 0,
                factura = pf.factura ?? 0,
                costo_empaque_decoracion_por_porcentaje_de_ganancia = pf.costo_empaque_decoracion_por_porcentaje_de_ganancia ?? 0,
                costo_implemento_por_porcentaje_de_ganancia = pf.costo_implemento_por_porcentaje_de_ganancia ?? 0,
                costo_suministro_por_porcentaje_de_ganancia = pf.costo_suministro_por_porcentaje_de_ganancia ?? 0,
                costo_total_empaque_decoracion_implemento_suministro = pf.costo_total_empaque_decoracion_implemento_suministro ?? 0,
                factura_por_insumo = pf.factura_por_insumo ?? 0,
                iva = pf.iva ?? 0,
                impuesto_de_servicio = pf.impuesto_de_servicio ?? 0,
                envio = pf.envio ?? 0,
                plataforma_de_envio = pf.plataforma_de_envio,
                precio_final_sugerido = pf.precio_final_sugerido ?? 0,

                EmpaquesDecoracionesUtilizados = db.precios_empaques_decoraciones_utilizados
                    .Where(ed => ed.id_precio_final_sugerido == pf.id)
                    .Select(ed => new EmpaqueDecoracionUtilizado
                    {
                        id = ed.id,
                        id_empaque_decoracion_utilizado = ed.id_empaque_decoracion_utilizado ?? 0,
                        nombre = ed.tabla_empaques_decoraciones.nombre,
                        cantidad = ed.cantidad ?? 0,
                        unidad_de_medida = ed.unidad_de_medida,
                        costo_por_cantidad = ed.costo_por_cantidad ?? 0,
                        total_costo = ed.total_costo ?? 0
                    }).ToList(),

                ImplementosUtilizados = db.precios_implementos_utilizados
                    .Where(i => i.id_precio_final_sugerido == pf.id)
                    .Select(i => new ImplementoUtilizado
                    {
                        id = i.id,
                        id_implemento_utilizado = i.id_implemento_utilizado ?? 0,
                        nombre = i.tabla_implementos.nombre,
                        cantidad = i.cantidad ?? 0,
                        unidad_de_medida = i.unidad_de_medida,
                        costo_por_cantidad = i.costo_por_cantidad ?? 0,
                        total_costo = i.total_costo ?? 0
                    }).ToList(),

                SuministrosUtilizados = db.precios_suministros_utilizados
                    .Where(s => s.id_precio_final_sugerido == pf.id)
                    .Select(s => new SuministroUtilizado
                    {
                        id = s.id,
                        id_suministro_utilizado = s.id_suministro_utilizado ?? 0,
                        nombre = s.tabla_suministros.nombre,
                        cantidad = s.cantidad ?? 0,
                        unidad_de_medida = s.unidad_de_medida,
                        costo_por_cantidad = s.costo_por_cantidad ?? 0,
                        total_costo = s.total_costo ?? 0
                    }).ToList()
            }).ToList()
        };
        ViewBag.Search = search;
        ViewBag.Recetas = new SelectList(db.tabla_costos_recetas.ToList(), "nombre", "nombre");
        ViewBag.EmpaquesDecoraciones = new SelectList(db.tabla_empaques_decoraciones.ToList(), "nombre", "nombre");
        ViewBag.Implementos = new SelectList(db.tabla_implementos.ToList(), "nombre", "nombre");
        ViewBag.Suministros = new SelectList(db.tabla_suministros.ToList(), "nombre", "nombre"); 
        return View(producto_final);
    }

    // Crear un nuevo producto final
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CrearProductoFinal(ProductoFinal producto_final)
    {
        var errores = new List<string>();

        // Validar que no exista un producto final con el mismo nombre de receta
        if (db.tabla_precios_finales_sugeridos.Any(p => p.nombre_receta.ToLower() == producto_final.nombre_receta.ToLower() && p.id != producto_final.id))
        {
            errores.Add("Ya existe un producto final para esa receta.");
        }

        // Validar que la receta seleccionada exista en la base de datos
        tabla_costos_recetas receta = null;
        if (!string.IsNullOrWhiteSpace(producto_final.nombre_receta))
        {
            receta = db.tabla_costos_recetas.FirstOrDefault(r => r.nombre.ToLower() == producto_final.nombre_receta.ToLower());
            if (receta == null)
            {
                errores.Add($"La receta seleccionada '{producto_final.nombre_receta}' no existe en el sistema.");
            }
        }

        // Validar campos obligatorios
        if (string.IsNullOrWhiteSpace(producto_final.nombre_receta))
        {
            errores.Add("El nombre de la receta es obligatorio.");
        }

        if (producto_final.margen_de_utilidad < 0 || producto_final.margen_de_utilidad > 100)
        { 
            errores.Add("El margen de utilidad debe estar entre 0 y 100.");
        }

        // Validar detalles (Empaques, Implementos, Suministros)
        if (producto_final.EmpaquesDecoracionesUtilizados != null)
        {
            var nombres = new HashSet<string>();
            for (int i = 0; i < producto_final.EmpaquesDecoracionesUtilizados.Count; i++)
            {
                var ed = producto_final.EmpaquesDecoracionesUtilizados[i];
                string nombre = ed?.nombre?.Trim().ToLower();

                if (string.IsNullOrWhiteSpace(ed.nombre) && ed.cantidad == 0 && string.IsNullOrWhiteSpace(ed.unidad_de_medida))
                {
                    errores.Add($"Fila {i + 1} de Empaques/Decoraciones: No puede dejar filas vacías.");
                    continue;
                }
                if (string.IsNullOrWhiteSpace(ed.nombre))
                    errores.Add($"Fila {i + 1} de Empaques/Decoraciones: Debe seleccionar un empaque/decoración.");
                if (ed.cantidad <= 0)
                    errores.Add($"Fila {i + 1} de Empaques/Decoraciones: La cantidad debe ser mayor a cero.");
                if (string.IsNullOrWhiteSpace(ed.unidad_de_medida))
                    errores.Add($"Fila {i + 1} de Empaques/Decoraciones: La unidad de medida es obligatoria.");

                if (!string.IsNullOrWhiteSpace(nombre))
                {
                    if (!nombres.Add(nombre))
                        errores.Add($"Fila {i + 1} de Empaques/Decoraciones: Empaque/decoración repetido: {ed.nombre}");

                    var empaque = db.tabla_empaques_decoraciones.FirstOrDefault(x => x.nombre.ToLower() == nombre);
                    if (empaque == null)
                    {
                        errores.Add($"Fila {i + 1} de Empaques/Decoraciones: El empaque/decoración '{ed.nombre}' no existe.");
                        continue;
                    }
                    ed.id_empaque_decoracion_utilizado = empaque.id;
                    ed.costo_por_cantidad = empaque.costo_por_cantidad ?? 0;
                    ed.total_costo = ed.cantidad * ed.costo_por_cantidad;
                }
            }
        }

        if (producto_final.ImplementosUtilizados != null)
        {
            var nombres = new HashSet<string>();
            for (int i = 0; i < producto_final.ImplementosUtilizados.Count; i++)
            {
                var impl = producto_final.ImplementosUtilizados[i];
                string nombre = impl?.nombre?.Trim().ToLower();

                if (string.IsNullOrWhiteSpace(impl.nombre) && impl.cantidad == 0 && string.IsNullOrWhiteSpace(impl.unidad_de_medida))
                {
                    errores.Add($"Fila {i + 1} de Implementos: No puede dejar filas vacías.");
                    continue;
                }
                if (string.IsNullOrWhiteSpace(impl.nombre))
                    errores.Add($"Fila {i + 1} de Implementos: Debe seleccionar un implemento.");
                if (impl.cantidad <= 0)
                    errores.Add($"Fila {i + 1} de Implementos: La cantidad debe ser mayor a cero.");
                if (string.IsNullOrWhiteSpace(impl.unidad_de_medida))
                    errores.Add($"Fila {i + 1} de Implementos: La unidad de medida es obligatoria.");

                if (!string.IsNullOrWhiteSpace(nombre))
                {
                    if (!nombres.Add(nombre))
                        errores.Add($"Fila {i + 1} de Implementos: Implemento repetido: {impl.nombre}");

                    var implemento = db.tabla_implementos.FirstOrDefault(x => x.nombre.ToLower() == nombre);
                    if (implemento == null)
                    {
                        errores.Add($"Fila {i + 1} de Implementos: El implemento '{impl.nombre}' no existe.");
                        continue;
                    }
                    impl.id_implemento_utilizado = implemento.id;
                    impl.costo_por_cantidad = implemento.costo_por_cantidad ?? 0;
                    impl.total_costo = impl.cantidad * impl.costo_por_cantidad;
                }
            }
        }

        if (producto_final.SuministrosUtilizados != null)
        {
            var nombres = new HashSet<string>();
            for (int i = 0; i < producto_final.SuministrosUtilizados.Count; i++)
            {
                var sum = producto_final.SuministrosUtilizados[i];
                string nombre = sum?.nombre?.Trim().ToLower();

                if (string.IsNullOrWhiteSpace(sum.nombre) && sum.cantidad == 0 && string.IsNullOrWhiteSpace(sum.unidad_de_medida))
                {
                    errores.Add($"Fila {i + 1} de Suministros: No puede dejar filas vacías.");
                    continue;
                }
                if (string.IsNullOrWhiteSpace(sum.nombre))
                    errores.Add($"Fila {i + 1} de Suministros: Debe seleccionar un suministro.");
                if (sum.cantidad <= 0)
                    errores.Add($"Fila {i + 1} de Suministros: La cantidad debe ser mayor a cero.");
                if (string.IsNullOrWhiteSpace(sum.unidad_de_medida))
                    errores.Add($"Fila {i + 1} de Suministros: La unidad de medida es obligatoria.");

                if (!string.IsNullOrWhiteSpace(nombre))
                {
                    if (!nombres.Add(nombre))
                        errores.Add($"Fila {i + 1} de Suministros: Suministro repetido: {sum.nombre}");

                    var suministro = db.tabla_suministros.FirstOrDefault(x => x.nombre.ToLower() == nombre);
                    if (suministro == null)
                    {
                        errores.Add($"Fila {i + 1} de Suministros: El suministro '{sum.nombre}' no existe.");
                        continue;
                    }
                    sum.id_suministro_utilizado = suministro.id;
                    sum.costo_por_cantidad = suministro.costo_por_cantidad ?? 0;
                    sum.total_costo = sum.cantidad * sum.costo_por_cantidad;
                }
            }
        }


        if (errores.Any())
        {
            ViewBag.Errores = errores;
            ViewBag.Recetas = new SelectList(db.tabla_costos_recetas.ToList(), "nombre", "nombre");
            ViewBag.EmpaquesDecoraciones = new SelectList(db.tabla_empaques_decoraciones.ToList(), "nombre", "nombre");
            ViewBag.Implementos = new SelectList(db.tabla_implementos.ToList(), "nombre", "nombre");
            ViewBag.Suministros = new SelectList(db.tabla_suministros.ToList(), "nombre", "nombre");
            var productosFinales = db.tabla_precios_finales_sugeridos.ToList().Select(pf => new ProductoFinal
            {
                id = pf.id,
                nombre_receta = pf.nombre_receta,
                costo_total_receta = pf.costo_total_receta ?? 0,
                margen_de_utilidad = pf.margen_de_utilidad ?? 0,
                costo_sin_margen_de_utilidad = pf.costo_sin_margen_de_utilidad ?? 0,
                costo_con_margen_de_utilidad = pf.costo_con_margen_de_utilidad ?? 0,
                costo_empaque_decoracion_utilizado = pf.costo_empaque_decoracion_utilizado ?? 0,
                costo_implemento_utilizado = pf.costo_implemento_utilizado ?? 0,
                costo_suministro_utilizado = pf.costo_suministro_utilizado ?? 0,
                factura = pf.factura ?? 0,
                factura_por_insumo = pf.factura_por_insumo ?? 0,
                costo_total_de_impresion_de_factura = pf.costo_total_de_impresion_de_factura  ?? 0,
                costo_total_empaque_decoracion_implemento_suministro = pf.costo_total_empaque_decoracion_implemento_suministro ?? 0,
                costo_suministro_por_porcentaje_de_ganancia = pf.costo_suministro_por_porcentaje_de_ganancia ?? 0,
                costo_implemento_por_porcentaje_de_ganancia = pf.costo_implemento_por_porcentaje_de_ganancia ?? 0,
                costo_empaque_decoracion_por_porcentaje_de_ganancia = pf.costo_empaque_decoracion_por_porcentaje_de_ganancia ?? 0,
                iva = pf.iva ?? 0,
                impuesto_de_servicio = pf.impuesto_de_servicio ?? 0,
                envio = pf.envio ?? 0,
                plataforma_de_envio = pf.plataforma_de_envio,
                precio_final_sugerido = pf.precio_final_sugerido ?? 0,
            }).ToList();
            return View("precio_final", new InsumosModel
            {
                ProductoFinalEditado = producto_final,
                ProductosFinales = productosFinales
            });
        }


        // Calcula los totales de insumos
        decimal totalEmpaques = producto_final.EmpaquesDecoracionesUtilizados?.Sum(e => e.total_costo) ?? 0;
        decimal totalImplementos = producto_final.ImplementosUtilizados?.Sum(i => i.total_costo) ?? 0;
        decimal totalSuministros = producto_final.SuministrosUtilizados?.Sum(s => s.total_costo) ?? 0;

        // Calcula el costo de la receta desde la base de datos
        decimal costoReceta = receta?.costo_total_receta ?? 0;
        decimal margenUtilidad = producto_final.margen_de_utilidad;
        decimal costoSinUtilidad = 100 - margenUtilidad;
        decimal costoConUtilidad = costoReceta / costoSinUtilidad;

        // 1. Costo de impresión de factura por insumo (primer suministro)
        decimal costoImpresionFacturaPorInsumo = 0;
        var primerSuministro = producto_final.SuministrosUtilizados?.FirstOrDefault();
        if (primerSuministro != null)
        {
            costoImpresionFacturaPorInsumo = primerSuministro.costo_por_cantidad / 20;
        }

        // 2. Costo total de impresión de factura
        decimal porcion = receta?.porcion ?? 0;
        decimal costoTotalImpresionFactura = porcion * costoImpresionFacturaPorInsumo;

        // 3. Factura (suma de totales + impresión)
        decimal factura = totalImplementos + totalEmpaques + totalSuministros + costoTotalImpresionFactura;

        // 4. Costos por porcentaje de ganancia
        decimal costoEmpaqueDecoracionPorPorcentajeDeGanancia = factura * 0.10m;
        decimal costoImplementoPorPorcentajeDeGanancia = factura * 0.10m;
        decimal costoSuministroPorPorcentajeDeGanancia = factura * 0.10m;

        // 5. Costo total empaque, implemento, suministro
        decimal costoTotalEmpaqueDecoracionImplementoSuministro =
            costoEmpaqueDecoracionPorPorcentajeDeGanancia +
            costoImplementoPorPorcentajeDeGanancia +
            costoSuministroPorPorcentajeDeGanancia;

        // 6. Factura por insumo (suma de costos por unidad de todos los insumos + impresión)
        decimal facturaPorInsumo = 0;
        facturaPorInsumo += producto_final.ImplementosUtilizados?.Sum(i => i.costo_por_cantidad) ?? 0;
        facturaPorInsumo += producto_final.EmpaquesDecoracionesUtilizados?.Sum(e => e.costo_por_cantidad) ?? 0;
        facturaPorInsumo += producto_final.SuministrosUtilizados?.Sum(s => s.costo_por_cantidad) ?? 0;
        facturaPorInsumo += costoImpresionFacturaPorInsumo;

        // 7. IVA y Servicio
        decimal baseImpuestos = costoConUtilidad + costoEmpaqueDecoracionPorPorcentajeDeGanancia + costoImplementoPorPorcentajeDeGanancia + costoSuministroPorPorcentajeDeGanancia;
        decimal iva = baseImpuestos * 0.13m;
        decimal servicio = baseImpuestos * 0.10m;

        // 8. Envío (igual que antes)
        decimal envio = 0;
        switch (producto_final.plataforma_de_envio)
        {
            case "PedidosYa (25%)":
            case "Rappi (25%)":
                envio = baseImpuestos * 0.25m;
                break;
            case "DidiFood (30%)":
                envio = baseImpuestos * 0.30m;
                break;
            case "UberEats (40%)":
                envio = baseImpuestos * 0.40m;
                break;
            default: // Propio (0%)
                envio = 0;
                break;
        }

        // 9. Precio final sugerido
        decimal precioFinal = baseImpuestos + iva + servicio + envio;

        var precio = new tabla_precios_finales_sugeridos
        {
            id_receta = receta?.id,
            nombre_receta = producto_final.nombre_receta,
            costo_total_receta = costoReceta,
            margen_de_utilidad = margenUtilidad,
            costo_sin_margen_de_utilidad = costoReceta,
            costo_con_margen_de_utilidad = costoConUtilidad,
            costo_empaque_decoracion_utilizado = totalEmpaques,
            costo_implemento_utilizado = totalImplementos,
            costo_suministro_utilizado = totalSuministros,
            costo_de_impresion_de_factura_por_insumo = costoImpresionFacturaPorInsumo,
            costo_total_de_impresion_de_factura = costoTotalImpresionFactura,
            factura = factura,
            costo_empaque_decoracion_por_porcentaje_de_ganancia = costoEmpaqueDecoracionPorPorcentajeDeGanancia,
            costo_implemento_por_porcentaje_de_ganancia = costoImplementoPorPorcentajeDeGanancia,
            costo_suministro_por_porcentaje_de_ganancia = costoSuministroPorPorcentajeDeGanancia,
            costo_total_empaque_decoracion_implemento_suministro = costoTotalEmpaqueDecoracionImplementoSuministro,
            factura_por_insumo = facturaPorInsumo,
            iva = iva,
            impuesto_de_servicio = servicio,
            envio = envio,
            plataforma_de_envio = producto_final.plataforma_de_envio,
            precio_final_sugerido = precioFinal
        };
        db.tabla_precios_finales_sugeridos.Add(precio);
        db.SaveChanges();


        if (producto_final.EmpaquesDecoracionesUtilizados != null)
        {
            foreach (var e in producto_final.EmpaquesDecoracionesUtilizados)
            {
                db.precios_empaques_decoraciones_utilizados.Add(new precios_empaques_decoraciones_utilizados
                {
                    id_precio_final_sugerido = precio.id,
                    id_empaque_decoracion_utilizado = e.id_empaque_decoracion_utilizado,
                    cantidad = e.cantidad,
                    unidad_de_medida = e.unidad_de_medida,
                    costo_por_cantidad = e.costo_por_cantidad,
                    total_costo = e.total_costo
                });
            }
        }

        if (producto_final.ImplementosUtilizados != null)
        {
            foreach (var i in producto_final.ImplementosUtilizados)
            {
                db.precios_implementos_utilizados.Add(new precios_implementos_utilizados
                {
                    id_precio_final_sugerido = precio.id,
                    id_implemento_utilizado = i.id_implemento_utilizado,
                    cantidad = i.cantidad,
                    unidad_de_medida = i.unidad_de_medida,
                    costo_por_cantidad = i.costo_por_cantidad,
                    total_costo = i.total_costo
                });
            }
        }

        if (producto_final.SuministrosUtilizados != null)
        {
            foreach (var s in producto_final.SuministrosUtilizados)
            {
                db.precios_suministros_utilizados.Add(new precios_suministros_utilizados
                {
                    id_precio_final_sugerido = precio.id,
                    id_suministro_utilizado = s.id_suministro_utilizado,
                    cantidad = s.cantidad,
                    unidad_de_medida = s.unidad_de_medida,
                    costo_por_cantidad = s.costo_por_cantidad,
                    total_costo = s.total_costo
                });
            }
        }
        db.SaveChanges();
        TempData["SuccessMessage"] = "¡Producto final registrado con éxito!";
        return RedirectToAction("precio_final");
    }

    //Editar un producto final existente (GET id)
    public ActionResult EditarProductoFinal(int id)
    {
        var pf = db.tabla_precios_finales_sugeridos.Find(id);
        if (pf == null) return HttpNotFound();

        var producto_final = new ProductoFinal
        {
            id = pf.id,
            nombre_receta = pf.nombre_receta,
            costo_total_receta = pf.costo_total_receta ?? 0,
            margen_de_utilidad = pf.margen_de_utilidad ?? 0,
            costo_sin_margen_de_utilidad = pf.costo_sin_margen_de_utilidad ?? 0,
            costo_con_margen_de_utilidad = pf.costo_con_margen_de_utilidad ?? 0,
            costo_empaque_decoracion_utilizado = pf.costo_empaque_decoracion_utilizado ?? 0,
            costo_implemento_utilizado = pf.costo_implemento_utilizado ?? 0,
            costo_suministro_utilizado = pf.costo_suministro_utilizado ?? 0,
            costo_de_impresion_de_factura_por_insumo = pf.costo_de_impresion_de_factura_por_insumo ?? 0,
            costo_total_de_impresion_de_factura = pf.costo_total_de_impresion_de_factura ?? 0,
            factura = pf.factura ?? 0,
            costo_empaque_decoracion_por_porcentaje_de_ganancia = pf.costo_empaque_decoracion_por_porcentaje_de_ganancia ?? 0,
            costo_implemento_por_porcentaje_de_ganancia = pf.costo_implemento_por_porcentaje_de_ganancia ?? 0,
            costo_suministro_por_porcentaje_de_ganancia = pf.costo_suministro_por_porcentaje_de_ganancia ?? 0,
            costo_total_empaque_decoracion_implemento_suministro = pf.costo_total_empaque_decoracion_implemento_suministro ?? 0,
            factura_por_insumo = pf.factura_por_insumo ?? 0,
            iva = pf.iva ?? 0,
            impuesto_de_servicio = pf.impuesto_de_servicio ?? 0,
            envio = pf.envio ?? 0,
            plataforma_de_envio = pf.plataforma_de_envio,
            precio_final_sugerido = pf.precio_final_sugerido ?? 0,

            EmpaquesDecoracionesUtilizados = db.precios_empaques_decoraciones_utilizados
                .Where(ed => ed.id_precio_final_sugerido == pf.id)
                .Select(ed => new EmpaqueDecoracionUtilizado
                {
                    id = ed.id,
                    id_empaque_decoracion_utilizado = ed.id_empaque_decoracion_utilizado ?? 0,
                    nombre = ed.tabla_empaques_decoraciones.nombre,
                    cantidad = ed.cantidad ?? 0,
                    unidad_de_medida = ed.unidad_de_medida,
                    costo_por_cantidad = ed.costo_por_cantidad ?? 0,
                    total_costo = ed.total_costo ?? 0
                }).ToList(),

            ImplementosUtilizados = db.precios_implementos_utilizados
                .Where(i => i.id_precio_final_sugerido == pf.id)
                .Select(i => new ImplementoUtilizado
                {
                    id = i.id,
                    id_implemento_utilizado = i.id_implemento_utilizado ?? 0,
                    nombre = i.tabla_implementos.nombre,
                    cantidad = i.cantidad ?? 0,
                    unidad_de_medida = i.unidad_de_medida,
                    costo_por_cantidad = i.costo_por_cantidad ?? 0,
                    total_costo = i.total_costo ?? 0
                }).ToList(),

            SuministrosUtilizados = db.precios_suministros_utilizados
                .Where(s => s.id_precio_final_sugerido == pf.id)
                .Select(s => new SuministroUtilizado
                {
                    id = s.id,
                    id_suministro_utilizado = s.id_suministro_utilizado ?? 0,
                    nombre = s.tabla_suministros.nombre,
                    cantidad = s.cantidad ?? 0,
                    unidad_de_medida = s.unidad_de_medida,
                    costo_por_cantidad = s.costo_por_cantidad ?? 0,
                    total_costo = s.total_costo ?? 0
                }).ToList()
        };

        var productosFinales = db.tabla_precios_finales_sugeridos.ToList().Select(prodfinal => new ProductoFinal
        {
            id = prodfinal.id,
            nombre_receta = prodfinal.nombre_receta,
            costo_total_receta = prodfinal.costo_total_receta ?? 0,
            margen_de_utilidad = prodfinal.margen_de_utilidad ?? 0,
            costo_sin_margen_de_utilidad = prodfinal.costo_sin_margen_de_utilidad ?? 0,
            costo_con_margen_de_utilidad = prodfinal.costo_con_margen_de_utilidad ?? 0,
            costo_empaque_decoracion_utilizado = prodfinal.costo_empaque_decoracion_utilizado ?? 0,
            costo_implemento_utilizado = prodfinal.costo_implemento_utilizado ?? 0,
            costo_suministro_utilizado = prodfinal.costo_suministro_utilizado ?? 0,
            costo_de_impresion_de_factura_por_insumo = prodfinal.costo_de_impresion_de_factura_por_insumo ?? 0,
            costo_total_de_impresion_de_factura = prodfinal.costo_total_de_impresion_de_factura ?? 0,
            factura = prodfinal.factura ?? 0,
            costo_empaque_decoracion_por_porcentaje_de_ganancia = prodfinal.costo_empaque_decoracion_por_porcentaje_de_ganancia ?? 0,
            costo_implemento_por_porcentaje_de_ganancia = prodfinal.costo_implemento_por_porcentaje_de_ganancia ?? 0,
            costo_suministro_por_porcentaje_de_ganancia = prodfinal.costo_suministro_por_porcentaje_de_ganancia ?? 0,
            costo_total_empaque_decoracion_implemento_suministro = prodfinal.costo_total_empaque_decoracion_implemento_suministro ?? 0,
            factura_por_insumo = prodfinal.factura_por_insumo ?? 0,
            iva = prodfinal.iva ?? 0,
            impuesto_de_servicio = prodfinal.impuesto_de_servicio ?? 0,
            envio = prodfinal.envio ?? 0,
            plataforma_de_envio = prodfinal.plataforma_de_envio,
            precio_final_sugerido = prodfinal.precio_final_sugerido ?? 0,

            EmpaquesDecoracionesUtilizados = db.precios_empaques_decoraciones_utilizados
                .Where(ed => ed.id_precio_final_sugerido == prodfinal.id)
                .Select(ed => new EmpaqueDecoracionUtilizado
                {
                    id = ed.id,
                    id_empaque_decoracion_utilizado = ed.id_empaque_decoracion_utilizado ?? 0,
                    nombre = ed.tabla_empaques_decoraciones.nombre,
                    cantidad = ed.cantidad ?? 0,
                    unidad_de_medida = ed.unidad_de_medida,
                    costo_por_cantidad = ed.costo_por_cantidad ?? 0,
                    total_costo = ed.total_costo ?? 0
                }).ToList(),
            
            ImplementosUtilizados = db.precios_implementos_utilizados
                .Where(i => i.id_precio_final_sugerido == prodfinal.id)
                .Select(i => new ImplementoUtilizado
                {
                    id = i.id,
                    id_implemento_utilizado = i.id_implemento_utilizado ?? 0,
                    nombre = i.tabla_implementos.nombre,
                    cantidad = i.cantidad ?? 0,
                    unidad_de_medida = i.unidad_de_medida,
                    costo_por_cantidad = i.costo_por_cantidad ?? 0,
                    total_costo = i.total_costo ?? 0
                }).ToList(),
            
            SuministrosUtilizados = db.precios_suministros_utilizados
                .Where(s => s.id_precio_final_sugerido == prodfinal.id)
                .Select(s => new SuministroUtilizado
                {
                    id = s.id,
                    id_suministro_utilizado = s.id_suministro_utilizado ?? 0,
                    nombre = s.tabla_suministros.nombre,
                    cantidad = s.cantidad ?? 0,
                    unidad_de_medida = s.unidad_de_medida,
                    costo_por_cantidad = s.costo_por_cantidad ?? 0,
                    total_costo = s.total_costo ?? 0
                }).ToList()
        }).ToList();

        ViewBag.Editando = true;
        ViewBag.Recetas = new SelectList(db.tabla_costos_recetas.ToList(), "nombre", "nombre", producto_final.nombre_receta);
        ViewBag.EmpaquesDecoraciones = new SelectList(db.tabla_empaques_decoraciones.ToList(), "nombre", "nombre");
        ViewBag.Implementos = new SelectList(db.tabla_implementos.ToList(), "nombre", "nombre");
        ViewBag.Suministros = new SelectList(db.tabla_suministros.ToList(), "nombre", "nombre"); 
        return View("precio_final", new InsumosModel
        {
            ProductoFinalEditado = producto_final,
            ProductosFinales = productosFinales
        });
    }

    // Editar un producto final existente (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditarProductoFinal(ProductoFinal producto_final)
    {
        var errores = new List<string>();

        if (db.tabla_precios_finales_sugeridos.Any(pf => pf.nombre_receta.ToLower() == producto_final.nombre_receta.ToLower() && pf.id != producto_final.id))
            errores.Add("Ya existe un producto final para esa receta.");

        if (string.IsNullOrWhiteSpace(producto_final.nombre_receta))
            errores.Add("El nombre de la receta es obligatorio.");

        if (producto_final.margen_de_utilidad < 0 || producto_final.margen_de_utilidad > 100)
            errores.Add("El margen de utilidad debe estar entre 0 y 100.");

        // Validar detalles (Empaques, Implementos, Suministros)
        if (producto_final.EmpaquesDecoracionesUtilizados != null)
        {
            var nombres = new HashSet<string>();
            for (int i = 0; i < producto_final.EmpaquesDecoracionesUtilizados.Count; i++)
            {
                var ed = producto_final.EmpaquesDecoracionesUtilizados[i];
                string nombre = ed?.nombre?.Trim().ToLower();

                if (string.IsNullOrWhiteSpace(ed.nombre) && ed.cantidad == 0 && string.IsNullOrWhiteSpace(ed.unidad_de_medida))
                {
                    errores.Add($"Fila {i + 1} de Empaques/Decoraciones: No puede dejar filas vacías.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(ed.nombre))
                {
                    errores.Add($"Fila {i + 1} de Empaques/Decoraciones: Debe seleccionar un empaque/decoración.");
                }

                if (ed.cantidad <= 0)
                {
                    errores.Add($"Fila {i + 1} de Empaques/Decoraciones: La cantidad debe ser mayor a cero.");
                }

                if (string.IsNullOrWhiteSpace(ed.unidad_de_medida))
                {
                    errores.Add($"Fila {i + 1} de Empaques/Decoraciones: La unidad de medida es obligatoria.");
                }
                
                if (!string.IsNullOrWhiteSpace(nombre))
                {
                    if (!nombres.Add(nombre))
                    {
                        errores.Add($"Fila {i + 1} de Empaques/Decoraciones: Empaque/decoración repetido: {ed.nombre}");
                    }

                    var empaque = db.tabla_empaques_decoraciones.FirstOrDefault(x => x.nombre.ToLower() == nombre);
                    
                    if (empaque == null)
                    {
                        errores.Add($"Fila {i + 1} de Empaques/Decoraciones: El empaque/decoración '{ed.nombre}' no existe.");
                        continue;
                    }
                    ed.id_empaque_decoracion_utilizado = empaque.id;
                    ed.costo_por_cantidad = empaque.costo_por_cantidad ?? 0;
                    ed.total_costo = ed.cantidad * ed.costo_por_cantidad;
                }
            }
        }

        if (producto_final.ImplementosUtilizados != null)
        {
            var nombres = new HashSet<string>();
            for (int i = 0; i < producto_final.ImplementosUtilizados.Count; i++)
            {
                var impl = producto_final.ImplementosUtilizados[i];
                string nombre = impl?.nombre?.Trim().ToLower();

                if (string.IsNullOrWhiteSpace(impl.nombre) && impl.cantidad == 0 && string.IsNullOrWhiteSpace(impl.unidad_de_medida))
                {
                    errores.Add($"Fila {i + 1} de Implementos: No puede dejar filas vacías.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(impl.nombre))
                {
                    errores.Add($"Fila {i + 1} de Implementos: Debe seleccionar un implemento.");
                }

                if (impl.cantidad <= 0)
                {
                    errores.Add($"Fila {i + 1} de Implementos: La cantidad debe ser mayor a cero.");
                }

                if (string.IsNullOrWhiteSpace(impl.unidad_de_medida))
                {
                    errores.Add($"Fila {i + 1} de Implementos: La unidad de medida es obligatoria.");
                }

                if (!string.IsNullOrWhiteSpace(nombre))
                {
                    if (!nombres.Add(nombre))
                    {
                        errores.Add($"Fila {i + 1} de Implementos: Implemento repetido: {impl.nombre}");
                    }

                    var implemento = db.tabla_implementos.FirstOrDefault(x => x.nombre.ToLower() == nombre);
                    
                    if (implemento == null)
                    {
                        errores.Add($"Fila {i + 1} de Implementos: El implemento '{impl.nombre}' no existe.");
                        continue;
                    }
                    impl.id_implemento_utilizado = implemento.id;
                    impl.costo_por_cantidad = implemento.costo_por_cantidad ?? 0;
                    impl.total_costo = impl.cantidad * impl.costo_por_cantidad;
                }
            }
        }

        if (producto_final.SuministrosUtilizados != null)
        {
            var nombres = new HashSet<string>();
            for (int i = 0; i < producto_final.SuministrosUtilizados.Count; i++)
            {
                var sum = producto_final.SuministrosUtilizados[i];
                string nombre = sum?.nombre?.Trim().ToLower();

                if (string.IsNullOrWhiteSpace(sum.nombre) && sum.cantidad == 0 && string.IsNullOrWhiteSpace(sum.unidad_de_medida))
                {
                    errores.Add($"Fila {i + 1} de Suministros: No puede dejar filas vacías.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(sum.nombre))
                {
                    errores.Add($"Fila {i + 1} de Suministros: Debe seleccionar un suministro.");
                }

                if (sum.cantidad <= 0)
                {
                    errores.Add($"Fila {i + 1} de Suministros: La cantidad debe ser mayor a cero.");
                }

                if (string.IsNullOrWhiteSpace(sum.unidad_de_medida))
                {
                    errores.Add($"Fila {i + 1} de Suministros: La unidad de medida es obligatoria.");
                }

                if (!string.IsNullOrWhiteSpace(nombre))
                {
                    if (!nombres.Add(nombre))
                    {
                        errores.Add($"Fila {i + 1} de Suministros: Suministro repetido: {sum.nombre}");
                    }

                    var suministro = db.tabla_suministros.FirstOrDefault(x => x.nombre.ToLower() == nombre);
                    if (suministro == null)
                    {
                        errores.Add($"Fila {i + 1} de Suministros: El suministro '{sum.nombre}' no existe.");
                        continue;
                    }
                    sum.id_suministro_utilizado = suministro.id;
                    sum.costo_por_cantidad = suministro.costo_por_cantidad ?? 0;
                    sum.total_costo = sum.cantidad * sum.costo_por_cantidad;
                }
            }
        }

        var p = db.tabla_precios_finales_sugeridos.Find(producto_final.id);
        if (p == null) return HttpNotFound();

        // Obtener el costo total de la receta seleccionada
        var receta = db.tabla_costos_recetas.FirstOrDefault(r => r.nombre == producto_final.nombre_receta);
        if (receta == null)
            errores.Add("La receta seleccionada no existe.");
        decimal costoReceta = receta?.costo_total_receta ?? 0;

        if (errores.Any())
        {
            ViewBag.Errores = errores;
            ViewBag.Recetas = new SelectList(db.tabla_costos_recetas.ToList(), "nombre", "nombre");
            ViewBag.EmpaquesDecoraciones = new SelectList(db.tabla_empaques_decoraciones.ToList(), "nombre", "nombre");
            ViewBag.Implementos = new SelectList(db.tabla_implementos.ToList(), "nombre", "nombre");
            ViewBag.Suministros = new SelectList(db.tabla_suministros.ToList(), "nombre", "nombre");
            var productosFinales = db.tabla_precios_finales_sugeridos.ToList().Select(pf => new ProductoFinal
            {
                id = pf.id,
                nombre_receta = pf.nombre_receta,
                costo_total_receta = pf.costo_total_receta ?? 0,
                margen_de_utilidad = pf.margen_de_utilidad ?? 0,
                costo_sin_margen_de_utilidad = pf.costo_sin_margen_de_utilidad ?? 0,
                costo_con_margen_de_utilidad = pf.costo_con_margen_de_utilidad ?? 0,
                costo_empaque_decoracion_utilizado = pf.costo_empaque_decoracion_utilizado ?? 0,
                costo_implemento_utilizado = pf.costo_implemento_utilizado ?? 0,
                costo_suministro_utilizado = pf.costo_suministro_utilizado ?? 0,
                costo_de_impresion_de_factura_por_insumo = pf.costo_de_impresion_de_factura_por_insumo ?? 0,
                costo_total_de_impresion_de_factura = pf.costo_total_de_impresion_de_factura ?? 0,
                factura = pf.factura ?? 0,
                costo_empaque_decoracion_por_porcentaje_de_ganancia = pf.costo_empaque_decoracion_por_porcentaje_de_ganancia ?? 0,
                costo_implemento_por_porcentaje_de_ganancia = pf.costo_implemento_por_porcentaje_de_ganancia ?? 0,
                costo_suministro_por_porcentaje_de_ganancia = pf.costo_suministro_por_porcentaje_de_ganancia ?? 0,
                costo_total_empaque_decoracion_implemento_suministro = pf.costo_total_empaque_decoracion_implemento_suministro ?? 0,
                factura_por_insumo = pf.factura_por_insumo ?? 0,
                iva = pf.iva ?? 0,
                impuesto_de_servicio = pf.impuesto_de_servicio ?? 0,
                envio = pf.envio ?? 0,
                plataforma_de_envio = pf.plataforma_de_envio,
            }).ToList();
            return View("precio_final", new InsumosModel
            {
                ProductoFinalEditado = producto_final,
                ProductosFinales = productosFinales
            });
        }

        // Calcular totales de empaques, implementos y suministros
        decimal totalEmpaques = 0, totalImplementos = 0, totalSuministros = 0;

        if (producto_final.EmpaquesDecoracionesUtilizados != null)
            totalEmpaques = producto_final.EmpaquesDecoracionesUtilizados.Sum(e => e.total_costo);

        if (producto_final.ImplementosUtilizados != null)
            totalImplementos = producto_final.ImplementosUtilizados.Sum(i => i.total_costo);

        if (producto_final.SuministrosUtilizados != null)
            totalSuministros = producto_final.SuministrosUtilizados.Sum(s => s.total_costo);

        decimal margen = producto_final.margen_de_utilidad;
        decimal costoConUtilidad = costoReceta * (1 + margen / 100);
        // 1. Costo de impresión de factura por insumo (primer suministro)
        decimal costoImpresionFacturaPorInsumo = 0;
        var primerSuministro = producto_final.SuministrosUtilizados?.FirstOrDefault();
        if (primerSuministro != null)
        {
            costoImpresionFacturaPorInsumo = primerSuministro.costo_por_cantidad / 20;
        }

        // 2. Costo total de impresión de factura
        decimal porcion = receta?.porcion ?? 0;
        decimal costoTotalImpresionFactura = porcion * costoImpresionFacturaPorInsumo;

        // 3. Factura (suma de totales + impresión)
        decimal factura = totalImplementos + totalEmpaques + totalSuministros + costoTotalImpresionFactura;

        // 4. Costos por porcentaje de ganancia
        decimal costoEmpaqueDecoracionPorPorcentajeDeGanancia = factura * 0.10m;
        decimal costoImplementoPorPorcentajeDeGanancia = factura * 0.10m;
        decimal costoSuministroPorPorcentajeDeGanancia = factura * 0.10m;

        // 5. Costo total empaque, implemento, suministro
        decimal costoTotalEmpaqueDecoracionImplementoSuministro =
            costoEmpaqueDecoracionPorPorcentajeDeGanancia +
            costoImplementoPorPorcentajeDeGanancia +
            costoSuministroPorPorcentajeDeGanancia;

        // 6. Factura por insumo (suma de costos por unidad de todos los insumos + impresión)
        decimal facturaPorInsumo = 0;
        facturaPorInsumo += producto_final.ImplementosUtilizados?.Sum(i => i.costo_por_cantidad) ?? 0;
        facturaPorInsumo += producto_final.EmpaquesDecoracionesUtilizados?.Sum(e => e.costo_por_cantidad) ?? 0;
        facturaPorInsumo += producto_final.SuministrosUtilizados?.Sum(s => s.costo_por_cantidad) ?? 0;
        facturaPorInsumo += costoImpresionFacturaPorInsumo;

        // 7. IVA y Servicio
        decimal baseImpuestos = costoConUtilidad + costoEmpaqueDecoracionPorPorcentajeDeGanancia + costoImplementoPorPorcentajeDeGanancia + costoSuministroPorPorcentajeDeGanancia;
        decimal iva = baseImpuestos * 0.13m;
        decimal servicio = baseImpuestos * 0.10m;

        // 8. Envío (igual que antes)
        decimal envio = 0;
        switch (producto_final.plataforma_de_envio)
        {
            case "PedidosYa (25%)":
            case "Rappi (25%)":
                envio = baseImpuestos * 0.25m;
                break;
            case "DidiFood (30%)":
                envio = baseImpuestos * 0.30m;
                break;
            case "UberEats (40%)":
                envio = baseImpuestos * 0.40m;
                break;
            default: // Propio (0%)
                envio = 0;
                break;
        }

        // 9. Precio final sugerido
        decimal precioFinal = baseImpuestos + iva + servicio + envio;

        // Actualizar campos principales
        p.id_receta = receta?.id;
        p.nombre_receta = producto_final.nombre_receta;
        p.costo_total_receta = costoReceta;
        p.margen_de_utilidad = margen;
        p.costo_sin_margen_de_utilidad = costoReceta;
        p.costo_con_margen_de_utilidad = costoConUtilidad;
        p.factura = factura;
        p.factura_por_insumo = facturaPorInsumo;
        p.costo_empaque_decoracion_utilizado = totalEmpaques;
        p.costo_implemento_utilizado = totalImplementos;
        p.costo_suministro_utilizado = totalSuministros;
        p.costo_de_impresion_de_factura_por_insumo = costoImpresionFacturaPorInsumo;
        p.costo_total_de_impresion_de_factura = costoTotalImpresionFactura;
        p.factura = factura;
        p.costo_empaque_decoracion_por_porcentaje_de_ganancia = costoEmpaqueDecoracionPorPorcentajeDeGanancia;
        p.costo_implemento_por_porcentaje_de_ganancia = costoImplementoPorPorcentajeDeGanancia;
        p.costo_suministro_por_porcentaje_de_ganancia = costoSuministroPorPorcentajeDeGanancia;
        p.costo_total_empaque_decoracion_implemento_suministro = costoTotalEmpaqueDecoracionImplementoSuministro;
        p.factura_por_insumo = facturaPorInsumo;
        p.iva = iva;
        p.impuesto_de_servicio = servicio;
        p.envio = envio;
        p.plataforma_de_envio = producto_final.plataforma_de_envio;
        p.precio_final_sugerido = precioFinal;

        // Eliminar detalles existentes
        var empaques = db.precios_empaques_decoraciones_utilizados.Where(x => x.id_precio_final_sugerido == p.id).ToList();
        foreach (var item in empaques) db.precios_empaques_decoraciones_utilizados.Remove(item);

        var implementos = db.precios_implementos_utilizados.Where(x => x.id_precio_final_sugerido == p.id).ToList();
        foreach (var item in implementos) db.precios_implementos_utilizados.Remove(item);

        var suministros = db.precios_suministros_utilizados.Where(x => x.id_precio_final_sugerido == p.id).ToList();
        foreach (var item in suministros) db.precios_suministros_utilizados.Remove(item);

        // Agregar nuevos detalles
        if (producto_final.EmpaquesDecoracionesUtilizados != null)
        {
            foreach (var ed in producto_final.EmpaquesDecoracionesUtilizados)
            {
                db.precios_empaques_decoraciones_utilizados.Add(new precios_empaques_decoraciones_utilizados
                {
                    id_precio_final_sugerido = p.id,
                    id_empaque_decoracion_utilizado = ed.id_empaque_decoracion_utilizado,
                    cantidad = ed.cantidad,
                    unidad_de_medida = ed.unidad_de_medida,
                    costo_por_cantidad = ed.costo_por_cantidad,
                    total_costo = ed.total_costo
                });
            }
        }
        if (producto_final.ImplementosUtilizados != null)
        {
            foreach (var i in producto_final.ImplementosUtilizados)
            {
                db.precios_implementos_utilizados.Add(new precios_implementos_utilizados
                {
                    id_precio_final_sugerido = p.id,
                    id_implemento_utilizado = i.id_implemento_utilizado,
                    cantidad = i.cantidad,
                    unidad_de_medida = i.unidad_de_medida,
                    costo_por_cantidad = i.costo_por_cantidad,
                    total_costo = i.total_costo
                });
            }
        }
        if (producto_final.SuministrosUtilizados != null)
        {
            foreach (var s in producto_final.SuministrosUtilizados)
            {
                db.precios_suministros_utilizados.Add(new precios_suministros_utilizados
                {
                    id_precio_final_sugerido = p.id,
                    id_suministro_utilizado = s.id_suministro_utilizado,
                    cantidad = s.cantidad,
                    unidad_de_medida = s.unidad_de_medida,
                    costo_por_cantidad = s.costo_por_cantidad,
                    total_costo = s.total_costo
                });
            }
        }
        db.SaveChanges();
        TempData["SuccessMessage"] = "¡Producto final actualizado con éxito!";
        return RedirectToAction("precio_final");
    }

    // Eliminar un producto final existente
    public ActionResult EliminarProductoFinal(int id)
    {
        var p = db.tabla_precios_finales_sugeridos.Find(id);
        if (p != null)
        {
            var empaques = db.precios_empaques_decoraciones_utilizados.Where(x => x.id_precio_final_sugerido == id).ToList();
            foreach (var item in empaques) db.precios_empaques_decoraciones_utilizados.Remove(item);

            var implementos = db.precios_implementos_utilizados.Where(x => x.id_precio_final_sugerido == id).ToList();
            foreach (var item in implementos) db.precios_implementos_utilizados.Remove(item);

            var suministros = db.precios_suministros_utilizados.Where(x => x.id_precio_final_sugerido == id).ToList();
            foreach (var item in suministros) db.precios_suministros_utilizados.Remove(item);

            db.tabla_precios_finales_sugeridos.Remove(p);
        }
        db.SaveChanges();
        TempData["SuccessMessage"] = "¡Producto final eliminado con éxito!";
        return RedirectToAction("precio_final");
    }
}
