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
                costo = (decimal)m.costo,
                peso = (int)m.peso,
                unidad_de_medida = m.unidad_de_medida,
                costo_por_gramo = (decimal)m.costo_por_gramo,
                merma_total_en_gramos = (int)m.merma_total_en_gramos,
                porcentaje_de_merma = (decimal)m.porcentaje_de_merma,
                costo_de_merma_total = (decimal)m.costo_de_merma_total,
                costo_total_mas_merma_total = (decimal)m.costo_total_mas_merma_total,
                costo_por_gramo_con_merma = (decimal)m.costo_por_gramo_con_merma
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
        // Validar que no exista otra materia prima con el mismo nombre
        if (db.tabla_materias_primas.Any(m => m.nombre.ToLower() == materia_prima.nombre.ToLower()))
        {
            ModelState.AddModelError("nombre", "Ya existe una materia prima con ese nombre.");
            if (!ModelState.IsValid)
            {
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
        }

        // Validar campos obligatorios y valores numéricos
        if (string.IsNullOrWhiteSpace(materia_prima.nombre) ||
                                      string.IsNullOrWhiteSpace(materia_prima.marca) ||
                                      string.IsNullOrWhiteSpace(materia_prima.presentacion) ||
                                      string.IsNullOrWhiteSpace(materia_prima.proveedor) ||
                                      string.IsNullOrWhiteSpace(materia_prima.unidad_de_medida))
        {
            ModelState.AddModelError("", "Todos los campos son obligatorios");
            if (!ModelState.IsValid)
            {
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
        }

        if (materia_prima.costo <= 0 || materia_prima.peso <= 0 || materia_prima.merma_total_en_gramos < 0)
        {
            ModelState.AddModelError("", "Los valores numéricos deben ser mayores a cero.");
            if (!ModelState.IsValid)
            {
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
            costo = (decimal)m.costo,
            peso = (int)m.peso,
            unidad_de_medida = m.unidad_de_medida,
            costo_por_gramo = (decimal)m.costo_por_gramo,
            merma_total_en_gramos = (int)m.merma_total_en_gramos,
            porcentaje_de_merma = (decimal)m.porcentaje_de_merma,
            costo_de_merma_total = (decimal)m.costo_de_merma_total,
            costo_total_mas_merma_total = (decimal)m.costo_total_mas_merma_total,
            costo_por_gramo_con_merma = (decimal)m.costo_por_gramo_con_merma
        };

        //Obtén el listado de materias primas
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
        // Validar que no exista otra materia prima con el mismo nombre
        if (db.tabla_materias_primas.Any(mp => mp.nombre.ToLower() == materia_prima.nombre.ToLower() && mp.id != materia_prima.id))
        {
            ModelState.AddModelError("nombre", "Ya existe una materia prima con ese nombre.");
            if (!ModelState.IsValid)
            {
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
        }

        // Validar campos obligatorios y valores numéricos
        if (string.IsNullOrWhiteSpace(materia_prima.nombre) ||
                                      string.IsNullOrWhiteSpace(materia_prima.marca) ||
                                      string.IsNullOrWhiteSpace(materia_prima.presentacion) ||
                                      string.IsNullOrWhiteSpace(materia_prima.proveedor) ||
                                      string.IsNullOrWhiteSpace(materia_prima.unidad_de_medida))
        {
            ModelState.AddModelError("", "Todos los campos son obligatorios");
            if (!ModelState.IsValid)
            {
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
        }

        if (materia_prima.costo <= 0 || materia_prima.peso <= 0 || materia_prima.merma_total_en_gramos < 0)
        {
            ModelState.AddModelError("", "Los valores numéricos deben ser mayores a cero.");
            if (!ModelState.IsValid)
            {
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
                costo_por_peso = (decimal)p.costo_por_peso,
                costo_por_porcion_con_merma = (decimal)p.costo_por_porcion_con_merma
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
        // Validar que no exista otro producto preparado con el mismo nombre
        if (db.tabla_productos_preparados.Any(p => p.nombre.ToLower() == producto_preparado.nombre.ToLower()))
        {
            ModelState.AddModelError("nombre", "Ya existe un producto preparado con ese nombre.");
            if (!ModelState.IsValid)
            {
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
        }

        // Validar campos obligatorios y valores numéricos
        if (string.IsNullOrWhiteSpace(producto_preparado.tipo) ||
                                      string.IsNullOrWhiteSpace(producto_preparado.nombre) ||
                                      string.IsNullOrWhiteSpace(producto_preparado.marca) ||
                                      string.IsNullOrWhiteSpace(producto_preparado.presentacion) ||
                                      string.IsNullOrWhiteSpace(producto_preparado.proveedor) ||
                                      string.IsNullOrWhiteSpace(producto_preparado.unidad_de_medida))
        {
            ModelState.AddModelError("", "Todos los campos son obligatorios");
            if (!ModelState.IsValid)
            {
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
        }

        if (producto_preparado.costo <= 0 || producto_preparado.peso <= 0 || producto_preparado.volumen_de_porcion <= 0)
        {
            ModelState.AddModelError("", "Los valores numéricos deben ser mayores a cero.");
            if (!ModelState.IsValid)
            {
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
        // Validar que no exista otro producto preparado con el mismo nombre
        if (db.tabla_productos_preparados.Any(prodprep => prodprep.nombre.ToLower() == producto_preparado.nombre.ToLower() && prodprep.id != producto_preparado.id))
        {
            ModelState.AddModelError("nombre", "Ya existe un producto preparado con ese nombre.");
            if (!ModelState.IsValid)
            {
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
        }

        // Validar campos obligatorios y valores numéricos
        if (string.IsNullOrWhiteSpace(producto_preparado.tipo) ||
                                      string.IsNullOrWhiteSpace(producto_preparado.nombre) ||
                                      string.IsNullOrWhiteSpace(producto_preparado.marca) ||
                                      string.IsNullOrWhiteSpace(producto_preparado.presentacion) ||
                                      string.IsNullOrWhiteSpace(producto_preparado.proveedor) ||
                                      string.IsNullOrWhiteSpace(producto_preparado.unidad_de_medida))
        {
            ModelState.AddModelError("", "Todos los campos son obligatorios");
            if (!ModelState.IsValid)
            {
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
        }

        if (producto_preparado.costo <= 0 || producto_preparado.peso <= 0 || producto_preparado.volumen_de_porcion <= 0)
        {
            ModelState.AddModelError("", "Los valores numéricos deben ser mayores a cero.");
            if (!ModelState.IsValid)
            {
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
                costo_por_cantidad = (decimal)ed.costo_por_cantidad
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
        // Validar que no exista otro empaque/decoración con el mismo nombre
        if (db.tabla_empaques_decoraciones.Any(ed => ed.nombre.ToLower() == empaque_decoracion.nombre.ToLower()))
        {
            ModelState.AddModelError("nombre", "Ya existe un empaque o decoración con ese nombre.");
            if (!ModelState.IsValid)
            {
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
        }

        // Validar campos obligatorios y valores numéricos
        if (string.IsNullOrWhiteSpace(empaque_decoracion.nombre) ||
                                      string.IsNullOrWhiteSpace(empaque_decoracion.marca) ||
                                      string.IsNullOrWhiteSpace(empaque_decoracion.presentacion) ||
                                      string.IsNullOrWhiteSpace(empaque_decoracion.proveedor) ||
                                      string.IsNullOrWhiteSpace(empaque_decoracion.unidad_de_medida))
        {
            ModelState.AddModelError("", "Todos los campos son obligatorios");
            if (!ModelState.IsValid)
            {
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
        }

        if (empaque_decoracion.costo <= 0 || empaque_decoracion.cantidad <= 0)
        {
            ModelState.AddModelError("", "El costo y la cantidad deben ser mayores a cero.");
            if (!ModelState.IsValid)
            {
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
        return View("empaque_decoracion", new InsumosModel
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
        // Validar que no exista otro empaque/decoración con el mismo nombre
        if (db.tabla_empaques_decoraciones.Any(empdec => empdec.nombre.ToLower() == empaque_decoracion.nombre.ToLower() && empdec.id != empaque_decoracion.id))
        {
            ModelState.AddModelError("nombre", "Ya existe un empaque/decoración con ese nombre.");
            if (!ModelState.IsValid)
            {
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
        }

        // Validar campos obligatorios y valores numéricos
        if (string.IsNullOrWhiteSpace(empaque_decoracion.nombre) ||
                                      string.IsNullOrWhiteSpace(empaque_decoracion.marca) ||
                                      string.IsNullOrWhiteSpace(empaque_decoracion.presentacion) ||
                                      string.IsNullOrWhiteSpace(empaque_decoracion.proveedor) ||
                                      string.IsNullOrWhiteSpace(empaque_decoracion.unidad_de_medida))
        {
            ModelState.AddModelError("", "Todos los campos son obligatorios");
            if (!ModelState.IsValid)
            {
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
        }

        if (empaque_decoracion.costo <= 0 || empaque_decoracion.cantidad <= 0)
        {
            ModelState.AddModelError("", "Los valores numéricos deben ser mayores a cero.");
            if (!ModelState.IsValid)
            {
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
                costo_por_cantidad = (decimal)i.costo_por_cantidad
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
        // Validar que no exista otro implemento con el mismo nombre
        if (db.tabla_implementos.Any(i => i.nombre.ToLower() == implemento.nombre.ToLower()))
        {
            ModelState.AddModelError("nombre", "Ya existe un implemento con ese nombre.");
            if (!ModelState.IsValid)
            {
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
        }

        // Validar campos obligatorios y valores numéricos
        if (string.IsNullOrWhiteSpace(implemento.nombre) ||
                                      string.IsNullOrWhiteSpace(implemento.marca) ||
                                      string.IsNullOrWhiteSpace(implemento.presentacion) ||
                                      string.IsNullOrWhiteSpace(implemento.proveedor) ||
                                      string.IsNullOrWhiteSpace(implemento.unidad_de_medida))
        {
            ModelState.AddModelError("", "Todos los campos son obligatorios");
            if (!ModelState.IsValid)
            {
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
        }

        if (implemento.costo <= 0 || implemento.cantidad <= 0)
        {
            ModelState.AddModelError("", "Los valores numéricos deben ser mayores a cero.");
            if (!ModelState.IsValid)
            {
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
        // Validar que no exista otro implemento con el mismo nombre
        if (db.tabla_implementos.Any(impl => impl.nombre.ToLower() == implemento.nombre.ToLower() && impl.id != implemento.id))
        {
            ModelState.AddModelError("nombre", "Ya existe un implemento con ese nombre.");
            if (!ModelState.IsValid)
            {
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
        }

        // Validar campos obligatorios y valores numéricos
        if (string.IsNullOrWhiteSpace(implemento.nombre) ||
                                      string.IsNullOrWhiteSpace(implemento.marca) ||
                                      string.IsNullOrWhiteSpace(implemento.presentacion) ||
                                      string.IsNullOrWhiteSpace(implemento.proveedor) ||
                                      string.IsNullOrWhiteSpace(implemento.unidad_de_medida))
        {
            ModelState.AddModelError("", "Todos los campos son obligatorios");
            if (!ModelState.IsValid)
            {
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
        }

        if (implemento.costo <= 0 || implemento.cantidad <= 0)
        {
            ModelState.AddModelError("", "Los valores numéricos deben ser mayores a cero.");
            if (!ModelState.IsValid)
            {
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
                costo_por_cantidad = (decimal)s.costo_por_cantidad
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
        // Validar que no exista otro suministro con el mismo nombre
        if (db.tabla_suministros.Any(s => s.nombre.ToLower() == suministro.nombre.ToLower()))
        {
            ModelState.AddModelError("nombre", "Ya existe un suministro con ese nombre.");
            if (!ModelState.IsValid)
            {
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
        }

        // Validar campos obligatorios y valores numéricos
        if (string.IsNullOrWhiteSpace(suministro.nombre) ||
                                      string.IsNullOrWhiteSpace(suministro.marca) ||
                                      string.IsNullOrWhiteSpace(suministro.presentacion) ||
                                      string.IsNullOrWhiteSpace(suministro.proveedor) ||
                                      string.IsNullOrWhiteSpace(suministro.unidad_de_medida))
        {
            ModelState.AddModelError("", "Todos los campos son obligatorios");
            if (!ModelState.IsValid)
            {
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
        }

        if (suministro.costo <= 0 || suministro.cantidad <= 0)
        {
            ModelState.AddModelError("", "Los valores numéricos deben ser mayores a cero.");
            if (!ModelState.IsValid)
            {
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
        // Validar que no exista otro suministro con el mismo nombre
        if (db.tabla_suministros.Any(sumn => sumn.nombre.ToLower() == suministro.nombre.ToLower() && sumn.id != suministro.id))
        {
            ModelState.AddModelError("nombre", "Ya existe un suministro con ese nombre.");
            if (!ModelState.IsValid)
            {
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
        }

        // Validar campos obligatorios y valores numéricos
        if (string.IsNullOrWhiteSpace(suministro.nombre) ||
                                      string.IsNullOrWhiteSpace(suministro.marca) ||
                                      string.IsNullOrWhiteSpace(suministro.presentacion) ||
                                      string.IsNullOrWhiteSpace(suministro.proveedor) ||
                                      string.IsNullOrWhiteSpace(suministro.unidad_de_medida))
        {
            ModelState.AddModelError("", "Todos los campos son obligatorios");
            if (!ModelState.IsValid)
            {
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
        }

        if (suministro.costo <= 0 || suministro.cantidad <= 0)
        {
            ModelState.AddModelError("", "Los valores numéricos deben ser mayores a cero.");
            if (!ModelState.IsValid)
            {
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
        // Validar que no exista una receta con el mismo nombre
        if (db.tabla_costos_recetas.Any(rec => rec.nombre.ToLower() == receta.nombre.ToLower() && rec.id != receta.id))
            ModelState.AddModelError("nombre", "Ya existe una receta con ese nombre.");

        // Validar campos obligatorios
        if (string.IsNullOrWhiteSpace(receta.nombre))
            ModelState.AddModelError("nombre", "El nombre de la receta es obligatorio.");

        if (receta.porcion <= 0)
            ModelState.AddModelError("porcion", "La porción debe ser mayor a cero.");

        // Validar que no haya materias primas o productos preparados repetidos
        if (receta.MateriasPrimasUtilizadas != null)
        {
            var nombres = receta.MateriasPrimasUtilizadas
                .Where(mp => mp != null && !string.IsNullOrWhiteSpace(mp.nombre))
                .Select(mp => mp.nombre.ToLower())
                .ToList(); 
            if (nombres.Count != nombres.Distinct().Count())
                ModelState.AddModelError("", "No se permiten materias primas repetidas en la receta.");
        }

        if (receta.ProductosPreparadosUtilizados != null)
        {
            var nombres = receta.ProductosPreparadosUtilizados
                .Where(pp => pp != null && !string.IsNullOrWhiteSpace(pp.nombre))
                .Select(pp => pp.nombre.ToLower())
                .ToList(); 
            if (nombres.Count != nombres.Distinct().Count())
                ModelState.AddModelError("", "No se permiten productos preparados repetidos en la receta.");
        }

        if (!ModelState.IsValid)
        {
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

        // Calcular el costo total de la receta
        decimal costoTotalReceta = 0;

        // Calcular costos de materias primas
        if (receta.MateriasPrimasUtilizadas != null)
        {
            foreach (var mp in receta.MateriasPrimasUtilizadas)
            {
                var materia_prima = db.tabla_materias_primas.FirstOrDefault(m => m.nombre == mp.nombre);
                if (materia_prima == null)
                {
                    ModelState.AddModelError("", $"La materia prima '{mp.nombre}' no existe.");
                    return View("costos_recetas", new InsumosModel
                    {
                        RecetaEditada = receta,
                        CostosRecetas = db.tabla_costos_recetas.Select(rec => new Receta
                        {
                            id = rec.id,
                            nombre = rec.nombre,
                            porcion = rec.porcion ?? 0,
                            costo_total_receta = rec.costo_total_receta ?? 0,
                            costo_por_porcion = rec.costo_por_porcion ?? 0
                        }).ToList()
                    });
                }
                mp.id_materia_prima_utilizada = materia_prima.id;
                mp.costo_por_cantidad = (decimal)(materia_prima.costo_por_gramo_con_merma ?? 0);
                mp.total_costo = mp.cantidad * mp.costo_por_cantidad;
                costoTotalReceta += mp.total_costo;
            }
        }

        if (receta.ProductosPreparadosUtilizados != null)
        {
            foreach (var pp in receta.ProductosPreparadosUtilizados)
            {
                var producto_preparado = db.tabla_productos_preparados.FirstOrDefault(p => p.nombre == pp.nombre);
                if (producto_preparado == null)
                {
                    ModelState.AddModelError("", $"El producto preparado '{pp.nombre}' no existe.");
                    return View("costos_recetas", new InsumosModel
                    {
                        RecetaEditada = receta,
                        CostosRecetas = db.tabla_costos_recetas.Select(rec => new Receta
                        {
                            id = rec.id,
                            nombre = rec.nombre,
                            porcion = rec.porcion ?? 0,
                            costo_total_receta = rec.costo_total_receta ?? 0,
                            costo_por_porcion = rec.costo_por_porcion ?? 0
                        }).ToList()
                    });
                }
                pp.id_producto_preparado_utilizado = producto_preparado.id;
                pp.costo_por_cantidad = (decimal)(producto_preparado.costo_por_peso ?? 0);
                pp.total_costo = pp.cantidad * pp.costo_por_cantidad;
                costoTotalReceta += pp.total_costo;
            }
        }

        // Calcular costo por porción
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

        // Obtén el listado de recetas
        var lista = db.tabla_costos_recetas.Select(matprim => new Receta
        {
            id = matprim.id,
            nombre = matprim.nombre,
            porcion = matprim.porcion ?? 0,
            costo_total_receta = matprim.costo_total_receta ?? 0,
            costo_por_porcion = matprim.costo_por_porcion ?? 0,
            MateriasPrimasUtilizadas = db.costos_receta_materias_primas_utilizadas
                .Where(mp => mp.id_receta == receta.id)
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
                .Where(pp => pp.id_receta == receta.id)
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
        // Validar que no exista otra receta con el mismo nombre
        if (db.tabla_costos_recetas.Any(rec => rec.nombre.ToLower() == receta.nombre.ToLower() && rec.id != receta.id))
            ModelState.AddModelError("nombre", "Ya existe una receta con ese nombre.");

        // Validar campos obligatorios
        if (string.IsNullOrWhiteSpace(receta.nombre))
            ModelState.AddModelError("nombre", "El nombre de la receta es obligatorio.");

        if (receta.porcion <= 0)
            ModelState.AddModelError("porcion", "La porción debe ser mayor a cero.");

        // Validar materias primas y productos preparados utilizados
        if (receta.MateriasPrimasUtilizadas != null)
        {
            var nombres = receta.MateriasPrimasUtilizadas
                .Where(mp => mp != null && !string.IsNullOrWhiteSpace(mp.nombre))
                .Select(mp => mp.nombre.ToLower())
                .ToList();
            if (nombres.Count != nombres.Distinct().Count())
                ModelState.AddModelError("", "No se permiten materias primas repetidas en la receta.");
        }

        if (receta.ProductosPreparadosUtilizados != null)
        {
            var nombres = receta.ProductosPreparadosUtilizados
                .Where(pp => pp != null && !string.IsNullOrWhiteSpace(pp.nombre))
                .Select(pp => pp.nombre.ToLower())
                .ToList();
            if (nombres.Count != nombres.Distinct().Count())
                ModelState.AddModelError("", "No se permiten productos preparados repetidos en la receta.");
        }

        // Calcular el costo total de la receta
        decimal costoTotalReceta = 0;
        if (receta.MateriasPrimasUtilizadas != null)
        {
            foreach (var mp in receta.MateriasPrimasUtilizadas.Where(mp => mp != null && !string.IsNullOrWhiteSpace(mp.nombre)))
            {
                // Obtener el costo actual y el id de la materia prima desde la base de datos
                var materia_prima = db.tabla_materias_primas.FirstOrDefault(m => m.nombre == mp.nombre);
                if (materia_prima == null)
                    ModelState.AddModelError("", $"La materia prima '{mp.nombre}' no existe.");

                mp.id_materia_prima_utilizada = materia_prima?.id ?? 0; // Asignar el id correcto
                mp.costo_por_cantidad = (decimal)(materia_prima?.costo_por_gramo_con_merma ?? 0);
                mp.total_costo = mp.cantidad * mp.costo_por_cantidad;
                costoTotalReceta += mp.total_costo;
            }
        }

        if (receta.ProductosPreparadosUtilizados != null)
        {
            foreach (var pp in receta.ProductosPreparadosUtilizados.Where(pp => pp != null && !string.IsNullOrWhiteSpace(pp.nombre)))
            {
                // Obtener el costo actual y el id del producto preparado desde la base de datos
                var producto_preparado = db.tabla_productos_preparados.FirstOrDefault(p => p.nombre == pp.nombre);
                if (producto_preparado == null)
                    ModelState.AddModelError("", $"El producto preparado '{pp.nombre}' no existe.");

                pp.id_producto_preparado_utilizado = producto_preparado?.id ?? 0; // Asignar el id correcto
                pp.costo_por_cantidad = (decimal)(producto_preparado?.costo_por_peso ?? 0);
                pp.total_costo = pp.cantidad * pp.costo_por_cantidad;
                costoTotalReceta += pp.total_costo;
            }
        }

        if (!ModelState.IsValid)
        {
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

        // Calcular costo por porción
        decimal costoPorPorcion = (receta.porcion > 0) ? (costoTotalReceta / receta.porcion) : 0;

        var r = db.tabla_costos_recetas.Find(receta.id);
        if (r == null) return HttpNotFound();

        r.nombre = receta.nombre;
        r.porcion = receta.porcion;
        r.costo_total_receta = costoTotalReceta;
        r.costo_por_porcion = costoPorPorcion;

        // Actualizar detalles: eliminar y volver a agregar (más simple para arrays)
        var existentesMP = db.costos_receta_materias_primas_utilizadas.Where(x => x.id_receta == r.id).ToList();
        foreach (var item in existentesMP) db.costos_receta_materias_primas_utilizadas.Remove(item);

        var existentesPP = db.costos_receta_productos_preparados_utilizados.Where(x => x.id_receta == r.id).ToList();
        foreach (var item in existentesPP) db.costos_receta_productos_preparados_utilizados.Remove(item);

        if (receta.MateriasPrimasUtilizadas != null)
        {
            foreach (var mp in receta.MateriasPrimasUtilizadas.Where(mp => mp != null && !string.IsNullOrWhiteSpace(mp.nombre)))
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
            foreach (var pp in receta.ProductosPreparadosUtilizados.Where(pp => pp != null && !string.IsNullOrWhiteSpace(pp.nombre)))
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
        var query = db.tabla_precios_finales_sugeridos.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p =>
                p.nombre_receta.Contains(search) ||
                p.costo_total_receta.ToString().Contains(search) ||
                p.margen_de_utilidad.ToString().Contains(search) ||
                p.costo_sin_margen_de_utilidad.ToString().Contains(search) ||
                p.costo_con_margen_de_utilidad.ToString().Contains(search) ||
                p.costo_empaque_decoracion_utilizado.ToString().Contains(search) ||
                p.costo_implemento_utilizado.ToString().Contains(search) ||
                p.costo_suministro_utilizado.ToString().Contains(search) ||
                p.iva.ToString().Contains(search) ||
                p.impuesto_de_servicio.ToString().Contains(search) ||
                p.envio.ToString().Contains(search) ||
                p.plataforma_de_envio.Contains(search) ||
                p.precio_final_sugerido.ToString().Contains(search) ||
                db.precios_empaques_decoraciones_utilizados.Any(e =>
                    e.id_precio_final_sugerido == p.id &&
                    (
                        e.tabla_empaques_decoraciones.nombre.Contains(search) ||
                        e.cantidad.ToString().Contains(search) ||
                        e.unidad_de_medida.Contains(search) ||
                        e.costo_por_cantidad.ToString().Contains(search) ||
                        e.total_costo.ToString().Contains(search)
                    )
                ) ||
                db.precios_implementos_utilizados.Any(i =>
                    i.id_precio_final_sugerido == p.id &&
                    (
                        i.tabla_implementos.nombre.Contains(search) ||
                        i.cantidad.ToString().Contains(search) ||
                        i.unidad_de_medida.Contains(search) ||
                        i.costo_por_cantidad.ToString().Contains(search) ||
                        i.total_costo.ToString().Contains(search)
                    )
                ) ||
                db.precios_suministros_utilizados.Any(s =>
                    s.id_precio_final_sugerido == p.id &&
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
        ViewBag.Recetas = new SelectList(db.tabla_costos_recetas.ToList(), "nombre");
        ViewBag.EmpaquesDecoraciones = new SelectList(db.tabla_empaques_decoraciones.ToList(),"nombre");
        ViewBag.Implementos = new SelectList(db.tabla_implementos.ToList(), "nombre");
        ViewBag.Suministros = new SelectList(db.tabla_suministros.ToList(), "nombre");
        return View(producto_final);
    }

    // Crear un nuevo producto final
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CrearProductoFinal(ProductoFinal producto_final)
    {
        // Validaciones
        if (db.tabla_precios_finales_sugeridos.Any(p => p.nombre_receta.ToLower() == producto_final.nombre_receta.ToLower() && p.id != producto_final.id))
            ModelState.AddModelError("nombre_receta", "Ya existe un producto final para esa receta.");

        if (string.IsNullOrWhiteSpace(producto_final.nombre_receta))
            ModelState.AddModelError("nombre_receta", "El nombre de la receta es obligatorio.");

        if (producto_final.margen_de_utilidad < 0 || producto_final.margen_de_utilidad > 100)
            ModelState.AddModelError("margen_de_utilidad", "El margen de utilidad debe estar entre 0 y 100.");

        // Validar insumos repetidos y campos obligatorios
        if (producto_final.EmpaquesDecoracionesUtilizados != null)
        {
            var nombres = producto_final.EmpaquesDecoracionesUtilizados.Select(ed => ed.nombre.ToLower()).ToList();
            if (nombres.Count != nombres.Distinct().Count())
                ModelState.AddModelError("", "No se permiten empaques/decoraciones repetidos.");
            foreach (var ed in producto_final.EmpaquesDecoracionesUtilizados)
            {
                if (string.IsNullOrWhiteSpace(ed.nombre))
                    ModelState.AddModelError("EmpaquesDecoracionesUtilizados", "El nombre del empaque/decoración es obligatorio.");
                if (ed.cantidad <= 0)
                    ModelState.AddModelError("EmpaquesDecoracionesUtilizados", "La cantidad del empaque/decoración debe ser mayor a cero.");
                if (string.IsNullOrWhiteSpace(ed.unidad_de_medida))
                    ModelState.AddModelError("EmpaquesDecoracionesUtilizados", "La unidad de medida del empaque/decoración es obligatoria.");
            }
        }
        if (producto_final.ImplementosUtilizados != null)
        {
            var nombres = producto_final.ImplementosUtilizados.Select(i => i.nombre.ToLower()).ToList();
            if (nombres.Count != nombres.Distinct().Count())
                ModelState.AddModelError("", "No se permiten implementos repetidos.");
            foreach (var i in producto_final.ImplementosUtilizados)
            {
                if (string.IsNullOrWhiteSpace(i.nombre))
                    ModelState.AddModelError("ImplementosUtilizados", "El nombre del implemento es obligatorio.");
                if (i.cantidad <= 0)
                    ModelState.AddModelError("ImplementosUtilizados", "La cantidad del implemento debe ser mayor a cero.");
                if (string.IsNullOrWhiteSpace(i.unidad_de_medida))
                    ModelState.AddModelError("ImplementosUtilizados", "La unidad de medida del implemento es obligatoria.");
            }
        }
        if (producto_final.SuministrosUtilizados != null)
        {
            var nombres = producto_final.SuministrosUtilizados.Select(s => s.nombre.ToLower()).ToList();
            if (nombres.Count != nombres.Distinct().Count())
                ModelState.AddModelError("", "No se permiten suministros repetidos.");
            foreach (var s in producto_final.SuministrosUtilizados)
            {
                if (string.IsNullOrWhiteSpace(s.nombre))
                    ModelState.AddModelError("SuministrosUtilizados", "El nombre del suministro es obligatorio.");
                if (s.cantidad <= 0)
                    ModelState.AddModelError("SuministrosUtilizados", "La cantidad del suministro debe ser mayor a cero.");
                if (string.IsNullOrWhiteSpace(s.unidad_de_medida))
                    ModelState.AddModelError("SuministrosUtilizados", "La unidad de medida del suministro es obligatoria.");
            }
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Recetas = new SelectList(db.tabla_costos_recetas.ToList(), "nombre");
            ViewBag.EmpaquesDecoraciones = new SelectList(db.tabla_empaques_decoraciones.ToList(), "nombre");
            ViewBag.Implementos = new SelectList(db.tabla_implementos.ToList(), "nombre");
            ViewBag.Suministros = new SelectList(db.tabla_suministros.ToList(), "nombre");
            var lista = db.tabla_precios_finales_sugeridos.Select(pf => new ProductoFinal
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
                iva = pf.iva ?? 0,
                impuesto_de_servicio = pf.impuesto_de_servicio ?? 0,
                envio = pf.envio ?? 0,
                plataforma_de_envio = pf.plataforma_de_envio,
                precio_final_sugerido = pf.precio_final_sugerido ?? 0
            }).ToList();
            return View("precio_final", new InsumosModel
            {
                ProductoFinalEditado = producto_final,
                ProductosFinales = lista
            });
        }

        // Obtener el costo total de la receta seleccionada
        var receta = db.tabla_costos_recetas.FirstOrDefault(r => r.nombre == producto_final.nombre_receta);
        if (receta == null)
        {
            ModelState.AddModelError("nombre_receta", "La receta seleccionada no existe.");
            return View("precio_final", new InsumosModel
            {
                ProductoFinalEditado = producto_final,
                ProductosFinales = db.tabla_precios_finales_sugeridos.Select(pf => new ProductoFinal
                {
                    id = pf.id,
                    nombre_receta = pf.nombre_receta
                }).ToList()
            });
        }
        decimal costoReceta = receta?.costo_total_receta ?? 0;

        // Calcular totales de empaques, implementos y suministros
        decimal totalEmpaques = 0, totalImplementos = 0, totalSuministros = 0;

        if (producto_final.EmpaquesDecoracionesUtilizados != null)
        {
            foreach (var e in producto_final.EmpaquesDecoracionesUtilizados)
            {
                var empaque = db.tabla_empaques_decoraciones.FirstOrDefault(x => x.nombre == e.nombre);
                decimal costoUnitario = empaque?.costo_por_cantidad ?? 0;
                e.costo_por_cantidad = costoUnitario;
                e.total_costo = e.cantidad * costoUnitario;
                totalEmpaques += e.total_costo;
            }
        }
        if (producto_final.ImplementosUtilizados != null)
        {
            foreach (var i in producto_final.ImplementosUtilizados)
            {
                var implemento = db.tabla_implementos.FirstOrDefault(x => x.nombre == i.nombre);
                decimal costoUnitario = implemento?.costo_por_cantidad ?? 0;
                i.costo_por_cantidad = costoUnitario;
                i.total_costo = i.cantidad * costoUnitario;
                totalImplementos += i.total_costo;
            }
        }
        if (producto_final.SuministrosUtilizados != null)
        {
            foreach (var s in producto_final.SuministrosUtilizados)
            {
                var suministro = db.tabla_suministros.FirstOrDefault(x => x.nombre == s.nombre);
                decimal costoUnitario = suministro?.costo_por_cantidad ?? 0;
                s.costo_por_cantidad = costoUnitario;
                s.total_costo = s.cantidad * costoUnitario;
                totalSuministros += s.total_costo;
            }
        }

        decimal margen = producto_final.margen_de_utilidad;
        decimal costoConUtilidad = costoReceta * (1 + margen / 100);
        decimal iva = producto_final.iva;
        decimal servicio = producto_final.impuesto_de_servicio;
        decimal envio = producto_final.envio;
        decimal precioFinal = costoConUtilidad + totalEmpaques + totalImplementos + totalSuministros + iva + servicio + envio;

        var precio = new tabla_precios_finales_sugeridos
        {
            id_receta = receta?.id,
            nombre_receta = producto_final.nombre_receta,
            costo_total_receta = costoReceta,
            margen_de_utilidad = margen,
            costo_sin_margen_de_utilidad = costoReceta,
            costo_con_margen_de_utilidad = costoConUtilidad,
            costo_empaque_decoracion_utilizado = totalEmpaques,
            costo_implemento_utilizado = totalImplementos,
            costo_suministro_utilizado = totalSuministros,
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
            id_receta = pf.id_receta ?? 0,
            nombre_receta = pf.nombre_receta,
            costo_total_receta = pf.costo_total_receta ?? 0,
            margen_de_utilidad = pf.margen_de_utilidad ?? 0,
            costo_sin_margen_de_utilidad = pf.costo_sin_margen_de_utilidad ?? 0,
            costo_con_margen_de_utilidad = pf.costo_con_margen_de_utilidad ?? 0,
            costo_empaque_decoracion_utilizado = pf.costo_empaque_decoracion_utilizado ?? 0,
            costo_implemento_utilizado = pf.costo_implemento_utilizado ?? 0,
            costo_suministro_utilizado = pf.costo_suministro_utilizado ?? 0,
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

        // Obtén el listado de productos finales
        var lista = db.tabla_precios_finales_sugeridos.Select(prodfinal => new ProductoFinal
        {
            id = prodfinal.id,
            id_receta = prodfinal.id_receta ?? 0,
            nombre_receta = prodfinal.nombre_receta,
            costo_total_receta = prodfinal.costo_total_receta ?? 0,
            margen_de_utilidad = prodfinal.margen_de_utilidad ?? 0,
            costo_sin_margen_de_utilidad = prodfinal.costo_sin_margen_de_utilidad ?? 0,
            costo_con_margen_de_utilidad = prodfinal.costo_con_margen_de_utilidad ?? 0,
            costo_empaque_decoracion_utilizado = prodfinal.costo_empaque_decoracion_utilizado ?? 0,
            costo_implemento_utilizado = prodfinal.costo_implemento_utilizado ?? 0,
            costo_suministro_utilizado = prodfinal.costo_suministro_utilizado ?? 0,
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
        }).ToList();

        ViewBag.Editando = true;
        ViewBag.Recetas = new SelectList(db.tabla_costos_recetas.ToList(), "nombre");
        ViewBag.EmpaquesDecoraciones = new SelectList(db.tabla_empaques_decoraciones.ToList(), "nombre");
        ViewBag.Implementos = new SelectList(db.tabla_implementos.ToList(), "nombre");
        ViewBag.Suministros = new SelectList(db.tabla_suministros.ToList(), "nombre");
        return View("precio_final", new InsumosModel
        {
            ProductoFinalEditado = producto_final,
            ProductosFinales = lista
        });
    }

    // Editar un producto final existente (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditarProductoFinal(ProductoFinal producto_final)
    {
        // Validar que no exista otro producto final con el mismo nombre de receta
        if (db.tabla_precios_finales_sugeridos.Any(pf => pf.nombre_receta.ToLower() == producto_final.nombre_receta.ToLower() && pf.id != producto_final.id))
            ModelState.AddModelError("nombre_receta", "Ya existe un producto final para esa receta.");

        // Validar campos obligatorios
        if (string.IsNullOrWhiteSpace(producto_final.nombre_receta))
            ModelState.AddModelError("nombre_receta", "El nombre de la receta es obligatorio.");

        if (producto_final.margen_de_utilidad < 0 || producto_final.margen_de_utilidad > 100)
            ModelState.AddModelError("margen_de_utilidad", "El margen de utilidad debe estar entre 0 y 100.");

        // Validar que no haya empaques, implementos o suministros repetidos
        if (producto_final.EmpaquesDecoracionesUtilizados != null)
        {
            var nombres = producto_final.EmpaquesDecoracionesUtilizados.Select(ed => ed.nombre.ToLower()).ToList();
            if (nombres.Count != nombres.Distinct().Count())
                ModelState.AddModelError("", "No se permiten empaques/decoraciones repetidos.");
        }

        if (producto_final.ImplementosUtilizados != null)
        {
            var nombres = producto_final.ImplementosUtilizados.Select(i => i.nombre.ToLower()).ToList();
            if (nombres.Count != nombres.Distinct().Count())
                ModelState.AddModelError("", "No se permiten implementos repetidos.");
        }

        if (producto_final.SuministrosUtilizados != null)
        {
            var nombres = producto_final.SuministrosUtilizados.Select(s => s.nombre.ToLower()).ToList();
            if (nombres.Count != nombres.Distinct().Count())
                ModelState.AddModelError("", "No se permiten suministros repetidos.");
        }

        // Validar campos de empaques, implementos y suministros
        if (producto_final.EmpaquesDecoracionesUtilizados != null)
        {
            foreach (var e in producto_final.EmpaquesDecoracionesUtilizados)
            {
                if (string.IsNullOrWhiteSpace(e.nombre))
                    ModelState.AddModelError("EmpaquesDecoracionesUtilizados", "El nombre del empaque/decoración es obligatorio.");

                if (e.cantidad <= 0)
                    ModelState.AddModelError("EmpaquesDecoracionesUtilizados", "La cantidad del empaque/decoración debe ser mayor a cero.");
                
                if (string.IsNullOrWhiteSpace(e.unidad_de_medida))
                    ModelState.AddModelError("EmpaquesDecoracionesUtilizados", "La unidad de medida del empaque/decoración es obligatoria.");
            }
        }

        if (producto_final.ImplementosUtilizados != null)
        {
            foreach (var i in producto_final.ImplementosUtilizados)
            {
                if (string.IsNullOrWhiteSpace(i.nombre))
                    ModelState.AddModelError("ImplementosUtilizados", "El nombre del implemento es obligatorio.");
                
                if (i.cantidad <= 0)
                    ModelState.AddModelError("ImplementosUtilizados", "La cantidad del implemento debe ser mayor a cero.");
                
                if (string.IsNullOrWhiteSpace(i.unidad_de_medida))
                    ModelState.AddModelError("ImplementosUtilizados", "La unidad de medida del implemento es obligatoria.");
            }
        }

        if (producto_final.SuministrosUtilizados != null)
        {
            foreach (var s in producto_final.SuministrosUtilizados)
            {
                if (string.IsNullOrWhiteSpace(s.nombre))
                    ModelState.AddModelError("SuministrosUtilizados", "El nombre del suministro es obligatorio.");
                
                if (s.cantidad <= 0)
                    ModelState.AddModelError("SuministrosUtilizados", "La cantidad del suministro debe ser mayor a cero.");
                
                if (string.IsNullOrWhiteSpace(s.unidad_de_medida))
                    ModelState.AddModelError("SuministrosUtilizados", "La unidad de medida del suministro es obligatoria.");
            }
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Recetas = new SelectList(db.tabla_costos_recetas.ToList(), "nombre");
            ViewBag.EmpaquesDecoraciones = new SelectList(db.tabla_empaques_decoraciones.ToList(), "nombre");
            ViewBag.Implementos = new SelectList(db.tabla_implementos.ToList(), "nombre");
            ViewBag.Suministros = new SelectList(db.tabla_suministros.ToList(), "nombre");
            var lista = db.tabla_precios_finales_sugeridos.Select(pf => new ProductoFinal
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
            }).ToList();
            return View("precio_final", new InsumosModel
            {
                ProductoFinalEditado = producto_final,
                ProductosFinales = lista
            });
        }

        var p = db.tabla_precios_finales_sugeridos.Find(producto_final.id);
        if (p == null) return HttpNotFound();

        // Obtener el costo total de la receta seleccionada
        var receta = db.tabla_costos_recetas.FirstOrDefault(r => r.nombre == producto_final.nombre_receta);
        if (receta == null)
        {
            ModelState.AddModelError("nombre_receta", "La receta seleccionada no existe.");
            return View(producto_final);
        }
        decimal costoReceta = receta?.costo_total_receta ?? 0;

        // Calcular totales de empaques, implementos y suministros
        decimal totalEmpaques = 0, totalImplementos = 0, totalSuministros = 0;

        if (producto_final.EmpaquesDecoracionesUtilizados != null)
        {
            foreach (var ed in producto_final.EmpaquesDecoracionesUtilizados)
            {
                var empaque = db.tabla_empaques_decoraciones.FirstOrDefault(x => x.nombre == ed.nombre);
                decimal costoUnitario = empaque?.costo_por_cantidad ?? 0;
                ed.costo_por_cantidad = costoUnitario;
                ed.total_costo = ed.cantidad * costoUnitario;
                totalEmpaques += ed.total_costo;
            }
        }
        if (producto_final.ImplementosUtilizados != null)
        {
            foreach (var i in producto_final.ImplementosUtilizados)
            {
                var implemento = db.tabla_implementos.FirstOrDefault(x => x.nombre == i.nombre);
                decimal costoUnitario = implemento?.costo_por_cantidad ?? 0;
                i.costo_por_cantidad = costoUnitario;
                i.total_costo = i.cantidad * costoUnitario;
                totalImplementos += i.total_costo;
            }
        }
        if (producto_final.SuministrosUtilizados != null)
        {
            foreach (var s in producto_final.SuministrosUtilizados)
            {
                var suministro = db.tabla_suministros.FirstOrDefault(x => x.nombre == s.nombre);
                decimal costoUnitario = suministro?.costo_por_cantidad ?? 0;
                s.costo_por_cantidad = costoUnitario;
                s.total_costo = s.cantidad * costoUnitario;
                totalSuministros += s.total_costo;
            }
        }

        // Calcular campos derivados
        decimal margen = producto_final.margen_de_utilidad;
        decimal costoConUtilidad = costoReceta * (1 + margen / 100);
        decimal iva = producto_final.iva;
        decimal servicio = producto_final.impuesto_de_servicio;
        decimal envio = producto_final.envio;
        decimal precioFinal = costoConUtilidad + totalEmpaques + totalImplementos + totalSuministros + iva + servicio + envio;

        // Actualizar campos principales
        p.id_receta = receta?.id;
        p.nombre_receta = producto_final.nombre_receta;
        p.costo_total_receta = costoReceta;
        p.margen_de_utilidad = margen;
        p.costo_sin_margen_de_utilidad = costoReceta;
        p.costo_con_margen_de_utilidad = costoConUtilidad;
        p.costo_empaque_decoracion_utilizado = totalEmpaques;
        p.costo_implemento_utilizado = totalImplementos;
        p.costo_suministro_utilizado = totalSuministros;
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
