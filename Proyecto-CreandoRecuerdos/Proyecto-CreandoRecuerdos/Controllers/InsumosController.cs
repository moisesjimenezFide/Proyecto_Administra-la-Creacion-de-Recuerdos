using Proyecto_CreandoRecuerdos.base_de_datos;
using Proyecto_CreandoRecuerdos.Models;
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
        var materia = new InsumosModel
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
        return View(materia);
    }

    // Crear una nueva materia prima
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CrearMateriaPrima(MateriaPrima materia)
    {
        if (db.tabla_materias_primas.Any(m => m.nombre.ToLower() == materia.nombre.ToLower()))
        {
            ModelState.AddModelError("nombre", "Ya existe una materia prima con ese nombre.");
            return View(materia);
        }

        if (string.IsNullOrWhiteSpace(materia.nombre) ||
                                      string.IsNullOrWhiteSpace(materia.marca) ||
                                      string.IsNullOrWhiteSpace(materia.presentacion) ||
                                      string.IsNullOrWhiteSpace(materia.proveedor) ||
                                      string.IsNullOrWhiteSpace(materia.unidad_de_medida))
        {
            ModelState.AddModelError("", "Todos los campos son obligatorios");
            return View(materia);
        }

        if (materia.costo <= 0 || materia.peso <= 0 || materia.merma_total_en_gramos < 0)
        {
            ModelState.AddModelError("", "Los valores numéricos deben ser mayores a cero.");
            return View(materia);
        }

        // Cálculos de campos derivados
        decimal costoPorGramo = (materia.peso > 0) ? (materia.costo / materia.peso) : 0;
        decimal porcentajeMerma = (materia.peso > 0) ? ((decimal)materia.merma_total_en_gramos / materia.peso) * 100 : 0;
        decimal costoMermaTotal = costoPorGramo * materia.merma_total_en_gramos;
        decimal costoTotalMasMerma = materia.costo + costoMermaTotal;
        decimal costoPorGramoConMerma = (materia.peso > 0) ? (costoTotalMasMerma / materia.peso) : 0;

        db.tabla_materias_primas.Add(new tabla_materias_primas
        {
            nombre = materia.nombre,
            marca = materia.marca,
            presentacion = materia.presentacion,
            proveedor = materia.proveedor,
            costo = materia.costo,
            peso = materia.peso,
            unidad_de_medida = materia.unidad_de_medida,
            costo_por_gramo = costoPorGramo,
            merma_total_en_gramos = materia.merma_total_en_gramos,
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
        var materia = new MateriaPrima
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
        return View(materia);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditarMateriaPrima(MateriaPrima materia)
    {
        if (db.tabla_materias_primas.Any(mp => mp.nombre.ToLower() == materia.nombre.ToLower() && mp.id != materia.id))
        {
            ModelState.AddModelError("nombre", "Ya existe una materia prima con ese nombre.");
            return View(materia);
        }

        if (string.IsNullOrWhiteSpace(materia.nombre) ||
                                      string.IsNullOrWhiteSpace(materia.marca) ||
                                      string.IsNullOrWhiteSpace(materia.presentacion) ||
                                      string.IsNullOrWhiteSpace(materia.proveedor) ||
                                      string.IsNullOrWhiteSpace(materia.unidad_de_medida))
        {
            ModelState.AddModelError("", "Todos los campos son obligatorios");
            return View(materia);
        }

        if (materia.costo <= 0 || materia.peso <= 0 || materia.merma_total_en_gramos < 0)
        {
            ModelState.AddModelError("", "Los valores numéricos deben ser mayores a cero.");
            return View(materia);
        }

        var m = db.tabla_materias_primas.Find(materia.id);
        if (m != null)
        {
            // Cálculos de campos derivados
            decimal costoPorGramo = (materia.peso > 0) ? (materia.costo / materia.peso) : 0;
            decimal porcentajeMerma = (materia.peso > 0) ? ((decimal)materia.merma_total_en_gramos / materia.peso) * 100 : 0;
            decimal costoMermaTotal = costoPorGramo * materia.merma_total_en_gramos;
            decimal costoTotalMasMerma = materia.costo + costoMermaTotal;
            decimal costoPorGramoConMerma = (materia.peso > 0) ? (costoTotalMasMerma / materia.peso) : 0;

            m.nombre = materia.nombre;
            m.marca = materia.marca;
            m.presentacion = materia.presentacion;
            m.proveedor = materia.proveedor;
            m.costo = materia.costo;
            m.peso = materia.peso;
            m.unidad_de_medida = materia.unidad_de_medida;
            m.merma_total_en_gramos = materia.merma_total_en_gramos;
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
        var productopreparado = new InsumosModel
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
        return View(productopreparado);
    }

    // Crear un nuevo producto preparado
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CrearProductoPreparado(ProductoPreparado producto)
    {
        if (db.tabla_productos_preparados.Any(p => p.nombre.ToLower() == producto.nombre.ToLower()))
        {
            ModelState.AddModelError("nombre", "Ya existe un producto preparado con ese nombre.");
            return View(producto);
        }

        if (string.IsNullOrWhiteSpace(producto.tipo) ||
                                      string.IsNullOrWhiteSpace(producto.nombre) ||
                                      string.IsNullOrWhiteSpace(producto.marca) ||
                                      string.IsNullOrWhiteSpace(producto.presentacion) ||
                                      string.IsNullOrWhiteSpace(producto.proveedor) ||
                                      string.IsNullOrWhiteSpace(producto.unidad_de_medida))
        {
            ModelState.AddModelError("", "Todos los campos son obligatorios");
            return View(producto);
        }

        if (producto.costo <= 0 || producto.peso <= 0 || producto.volumen_de_porcion <= 0)
        {
            ModelState.AddModelError("", "Los valores numéricos deben ser mayores a cero.");
            return View(producto);
        }

        decimal costoPorPeso = (producto.peso > 0) ? (producto.costo / producto.peso) : 0;
        decimal costoPorPorcionConMerma = (producto.volumen_de_porcion > 0) ? (producto.costo / producto.volumen_de_porcion) : 0;

        db.tabla_productos_preparados.Add(new tabla_productos_preparados
        {
            tipo = producto.tipo,
            nombre = producto.nombre,
            marca = producto.marca,
            presentacion = producto.presentacion,
            proveedor = producto.proveedor,
            volumen_de_porcion = producto.volumen_de_porcion,
            costo = producto.costo,
            peso = producto.peso,
            unidad_de_medida = producto.unidad_de_medida,
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
        var producto = new ProductoPreparado
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
        return View(producto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditarProductoPreparado(ProductoPreparado producto)
    {
        if (db.tabla_productos_preparados.Any(pp => pp.nombre.ToLower() == producto.nombre.ToLower() && pp.id != producto.id))
        {
            ModelState.AddModelError("nombre", "Ya existe un producto preparado con ese nombre.");
            return View(producto);
        }

        if (string.IsNullOrWhiteSpace(producto.tipo) ||
                                      string.IsNullOrWhiteSpace(producto.nombre) ||
                                      string.IsNullOrWhiteSpace(producto.marca) ||
                                      string.IsNullOrWhiteSpace(producto.presentacion) ||
                                      string.IsNullOrWhiteSpace(producto.proveedor) ||
                                      string.IsNullOrWhiteSpace(producto.unidad_de_medida))
        {
            ModelState.AddModelError("", "Todos los campos son obligatorios");
            return View(producto);
        }

        if (producto.costo <= 0 || producto.peso <= 0 || producto.volumen_de_porcion <= 0)
        {
            ModelState.AddModelError("", "Los valores numéricos deben ser mayores a cero.");
            return View(producto);
        }

        var p = db.tabla_productos_preparados.Find(producto.id);
        if (p != null)
        {
            decimal costoPorPeso = (producto.peso > 0) ? (producto.costo / producto.peso) : 0;
            decimal costoPorPorcionConMerma = (producto.volumen_de_porcion > 0) ? (producto.costo / producto.volumen_de_porcion) : 0;

            p.tipo = producto.tipo;
            p.nombre = producto.nombre;
            p.marca = producto.marca;
            p.presentacion = producto.presentacion;
            p.proveedor = producto.proveedor;
            p.volumen_de_porcion = producto.volumen_de_porcion;
            p.costo = producto.costo;
            p.peso = producto.peso;
            p.unidad_de_medida = producto.unidad_de_medida;
            p.costo_por_peso = costoPorPeso;
            p.costo_por_porcion_con_merma = costoPorPorcionConMerma;
        }
        db.SaveChanges();
        TempData["SuccessMessage"] = "¡Producto preparado actualizado con éxito!";
        return RedirectToAction("productos_preparados");
    }

    // Eliminar un producto preparado
    public ActionResult EliminarProductoPreparado(int id)
    {
        var p = db.tabla_productos_preparados.Find(id);
        if (p != null)
        {
            db.tabla_productos_preparados.Remove(p);
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
            query = query.Where(e =>
                e.nombre.Contains(search) ||
                e.marca.Contains(search) ||
                e.presentacion.Contains(search) ||
                e.proveedor.Contains(search) ||
                e.unidad_de_medida.Contains(search) ||
                e.costo.ToString().Contains(search) ||
                e.cantidad.ToString().Contains(search) ||
                e.costo_por_cantidad.ToString().Contains(search)
            );
        }
        var empaquedecoracion = new InsumosModel
        {
            EmpaquesDecoraciones = query.Select(e => new EmpaqueDecoracion
            {
                id = e.id,
                nombre = e.nombre,
                marca = e.marca,
                presentacion = e.presentacion,
                proveedor = e.proveedor,
                costo = (decimal)e.costo,
                cantidad = (int)e.cantidad,
                unidad_de_medida = e.unidad_de_medida,
                costo_por_cantidad = (decimal)e.costo_por_cantidad
            }).ToList()
        };
        ViewBag.Search = search;
        return View(empaquedecoracion);
    }

    // Crear un nuevo empaque o decoración
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CrearEmpaqueDecoracion(EmpaqueDecoracion empaque_decoracion)
    {
        if (db.tabla_empaques_decoraciones.Any(e => e.nombre.ToLower() == empaque_decoracion.nombre.ToLower()))
        {
            ModelState.AddModelError("nombre", "Ya existe un empaque o decoración con ese nombre.");
            return View(empaque_decoracion);
        }

        if (string.IsNullOrWhiteSpace(empaque_decoracion.nombre) ||
                                      string.IsNullOrWhiteSpace(empaque_decoracion.marca) ||
                                      string.IsNullOrWhiteSpace(empaque_decoracion.presentacion) ||
                                      string.IsNullOrWhiteSpace(empaque_decoracion.proveedor) ||
                                      string.IsNullOrWhiteSpace(empaque_decoracion.unidad_de_medida))
        {
            ModelState.AddModelError("", "Todos los campos son obligatorios");
            return View(empaque_decoracion);
        }

        if (empaque_decoracion.costo <= 0 || empaque_decoracion.cantidad <= 0)
        {
            ModelState.AddModelError("", "El costo y la cantidad deben ser mayores a cero.");
            return View(empaque_decoracion);
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
        var e = db.tabla_empaques_decoraciones.Find(id);
        if (e == null) return HttpNotFound();
        var empaque = new EmpaqueDecoracion
        {
            id = e.id,
            nombre = e.nombre,
            marca = e.marca,
            presentacion = e.presentacion,
            proveedor = e.proveedor,
            costo = (decimal)e.costo,
            cantidad = (int)e.cantidad,
            unidad_de_medida = e.unidad_de_medida,
            costo_por_cantidad = (decimal)e.costo_por_cantidad
        };
        return View(empaque);
    }

    // Editar un empaque o decoración existente (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditarEmpaqueDecoracion(EmpaqueDecoracion empaque_decoracion)
    {
        if (db.tabla_empaques_decoraciones.Any(ed => ed.nombre.ToLower() == empaque_decoracion.nombre.ToLower() && ed.id != empaque_decoracion.id))
        {
            ModelState.AddModelError("nombre", "Ya existe un empaque/decoración con ese nombre.");
            return View(empaque_decoracion);
        }

        if (string.IsNullOrWhiteSpace(empaque_decoracion.nombre) ||
                                      string.IsNullOrWhiteSpace(empaque_decoracion.marca) ||
                                      string.IsNullOrWhiteSpace(empaque_decoracion.presentacion) ||
                                      string.IsNullOrWhiteSpace(empaque_decoracion.proveedor) ||
                                      string.IsNullOrWhiteSpace(empaque_decoracion.unidad_de_medida))
        {
            ModelState.AddModelError("", "Todos los campos son obligatorios");
            return View(empaque_decoracion);
        }

        if (empaque_decoracion.costo <= 0 || empaque_decoracion.cantidad <= 0)
        {
            ModelState.AddModelError("", "Los valores numéricos deben ser mayores a cero.");
            return View(empaque_decoracion);
        }

        // Calcular el campo derivado
        decimal costoPorCantidad = (empaque_decoracion.cantidad > 0) ? (empaque_decoracion.costo / empaque_decoracion.cantidad) : 0;
        var e = db.tabla_empaques_decoraciones.Find(empaque_decoracion.id);
        if (e != null)
        {
            e.nombre = empaque_decoracion.nombre;
            e.marca = empaque_decoracion.marca;
            e.presentacion = empaque_decoracion.presentacion;
            e.proveedor = empaque_decoracion.proveedor;
            e.costo = empaque_decoracion.costo;
            e.cantidad = empaque_decoracion.cantidad;
            e.unidad_de_medida = empaque_decoracion.unidad_de_medida;
            e.costo_por_cantidad = costoPorCantidad;
        }
        db.SaveChanges();
        TempData["SuccessMessage"] = "¡Empaque o decoración actualizado con éxito!";
        return RedirectToAction("empaques_decoraciones");
    }

    // Eliminar un empaque o decoración existente
    public ActionResult EliminarEmpaqueDecoracion(int id)
    {
        var e = db.tabla_empaques_decoraciones.Find(id);
        if (e != null)
        {
            db.tabla_empaques_decoraciones.Remove(e);
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
        if (db.tabla_implementos.Any(i => i.nombre.ToLower() == implemento.nombre.ToLower()))
        {
            ModelState.AddModelError("nombre", "Ya existe un implemento con ese nombre.");
            return View(implemento);
        }


        if (string.IsNullOrWhiteSpace(implemento.nombre) ||
                                      string.IsNullOrWhiteSpace(implemento.marca) ||
                                      string.IsNullOrWhiteSpace(implemento.presentacion) ||
                                      string.IsNullOrWhiteSpace(implemento.proveedor) ||
                                      string.IsNullOrWhiteSpace(implemento.unidad_de_medida))
        {
            ModelState.AddModelError("", "Todos los campos son obligatorios");
            return View(implemento);
        }

        if (implemento.costo <= 0 || implemento.cantidad <= 0)
        {
            ModelState.AddModelError("", "Los valores numéricos deben ser mayores a cero.");
            return View(implemento);
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
        return View(implemento);
    }

    // Editar un implemento existente (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditarImplemento(Implemento implemento)
    {
        if (db.tabla_implementos.Any(impl => impl.nombre.ToLower() == implemento.nombre.ToLower() && impl.id != implemento.id))
        {
            ModelState.AddModelError("nombre", "Ya existe un implemento con ese nombre.");
            return View(implemento);
        }

        if (string.IsNullOrWhiteSpace(implemento.nombre) ||
                                      string.IsNullOrWhiteSpace(implemento.marca) ||
                                      string.IsNullOrWhiteSpace(implemento.presentacion) ||
                                      string.IsNullOrWhiteSpace(implemento.proveedor) ||
                                      string.IsNullOrWhiteSpace(implemento.unidad_de_medida))
        {
            ModelState.AddModelError("", "Todos los campos son obligatorios");
            return View(implemento);
        }

        if (implemento.costo <= 0 || implemento.cantidad <= 0)
        {
            ModelState.AddModelError("", "Los valores numéricos deben ser mayores a cero.");
            return View(implemento);
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
        if (db.tabla_suministros.Any(s => s.nombre.ToLower() == suministro.nombre.ToLower()))
        {
            ModelState.AddModelError("nombre", "Ya existe un suministro con ese nombre.");
            return View(suministro);
        }

        if (string.IsNullOrWhiteSpace(suministro.nombre) ||
                                      string.IsNullOrWhiteSpace(suministro.marca) ||
                                      string.IsNullOrWhiteSpace(suministro.presentacion) ||
                                      string.IsNullOrWhiteSpace(suministro.proveedor) ||
                                      string.IsNullOrWhiteSpace(suministro.unidad_de_medida))
        {
            ModelState.AddModelError("", "Todos los campos son obligatorios");
            return View(suministro);
        }

        if (suministro.costo <= 0 || suministro.cantidad <= 0)
        {
            ModelState.AddModelError("", "Los valores numéricos deben ser mayores a cero.");
            return View(suministro);
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
        return View(suministro);
    }

    // Editar un suministro existente (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditarSuministro(Suministro suministro)
    {
        if (db.tabla_suministros.Any(sumn => sumn.nombre.ToLower() == suministro.nombre.ToLower() && sumn.id != suministro.id))
        {
            ModelState.AddModelError("nombre", "Ya existe un suministro con ese nombre.");
            return View(suministro);
        }

        if (string.IsNullOrWhiteSpace(suministro.nombre) ||
                                      string.IsNullOrWhiteSpace(suministro.marca) ||
                                      string.IsNullOrWhiteSpace(suministro.presentacion) ||
                                      string.IsNullOrWhiteSpace(suministro.proveedor) ||
                                      string.IsNullOrWhiteSpace(suministro.unidad_de_medida))
        {
            ModelState.AddModelError("", "Todos los campos son obligatorios");
            return View(suministro);
        }

        if (suministro.costo <= 0 || suministro.cantidad <= 0)
        {
            ModelState.AddModelError("", "Los valores numéricos deben ser mayores a cero.");
            return View(suministro);
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

        var productofinal = new InsumosModel
        {
            CostosRecetas = query.Select(r => new RecetaCosto
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
        ViewBag.MateriasPrimas = new SelectList(db.tabla_materias_primas.ToList(), "nombre");
        ViewBag.ProductosPreparados = new SelectList(db.tabla_productos_preparados.ToList(), "nombre");

        return View(productofinal);
    }

    // Crear una nueva receta
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CrearCostoReceta(RecetaCosto receta)
    {
        if (db.tabla_costos_recetas.Any(rec => rec.nombre.ToLower() == receta.nombre.ToLower() && rec.id != receta.id))
        {
            ModelState.AddModelError("nombre", "Ya existe una receta con ese nombre.");
            return View(receta);
        }

        if (string.IsNullOrWhiteSpace(receta.nombre))
        {
            ModelState.AddModelError("nombre", "El nombre de la receta es obligatorio.");
            return View(receta);
        }

        if (receta.porcion <= 0)
        {
            ModelState.AddModelError("porcion", "La porción debe ser mayor a cero.");
            return View(receta);
        }

        if (receta.MateriasPrimasUtilizadas != null)
        {
            var nombres = receta.MateriasPrimasUtilizadas.Select(mp => mp.nombre.ToLower()).ToList();
            if (nombres.Count != nombres.Distinct().Count())
            {
                ModelState.AddModelError("", "No se permiten materias primas repetidas en la receta.");
                return View(receta);
            }
        }
        if (receta.ProductosPreparadosUtilizados != null)
        {
            var nombres = receta.ProductosPreparadosUtilizados.Select(pp => pp.nombre.ToLower()).ToList();
            if (nombres.Count != nombres.Distinct().Count())
            {
                ModelState.AddModelError("", "No se permiten productos preparados repetidos en la receta.");
                return View(receta);
            }
        }

        // Calcular el costo total de la receta
        decimal costoTotalReceta = 0;

        // Calcular costos de materias primas
        if (receta.MateriasPrimasUtilizadas != null)
        {
            foreach (var mp in receta.MateriasPrimasUtilizadas)
            {
                // Obtener el costo actual de la materia prima desde la base de datos
                var materia = db.tabla_materias_primas.FirstOrDefault(m => m.nombre == mp.nombre);
                mp.costo_por_cantidad = materia != null ? (decimal)materia.costo_por_gramo : 0;
                mp.total_costo = mp.cantidad * mp.costo_por_cantidad;
                costoTotalReceta += mp.total_costo;
            }
        }
        if (receta.ProductosPreparadosUtilizados != null)
        {
            foreach (var pp in receta.ProductosPreparadosUtilizados)
            {
                var producto = db.tabla_productos_preparados.FirstOrDefault(p => p.nombre == pp.nombre);
                pp.costo_por_cantidad = producto != null ? (decimal)producto.costo_por_peso : 0;
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

        // Materias primas utilizadas
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

        // Productos preparados utilizados
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
    public ActionResult EditarCostoReceta(int id)
    {
        var r = db.tabla_costos_recetas.Find(id);
        if (r == null) return HttpNotFound();

        var productofinal = new RecetaCosto
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
        return View(productofinal);
    }

    // Editar receta existente (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditarCostoReceta(RecetaCosto receta)
    {
        if (db.tabla_costos_recetas.Any(rec => rec.nombre.ToLower() == receta.nombre.ToLower() && rec.id != receta.id))
        {
            ModelState.AddModelError("nombre", "Ya existe una receta con ese nombre.");
            return View(receta);
        }

        if (string.IsNullOrWhiteSpace(receta.nombre))
        {
            ModelState.AddModelError("nombre", "El nombre de la receta es obligatorio.");
            return View(receta);
        }

        if (receta.porcion <= 0)
        {
            ModelState.AddModelError("porcion", "La porción debe ser mayor a cero.");
            return View(receta);
        }

        if (receta.MateriasPrimasUtilizadas != null)
        {
            var nombres = receta.MateriasPrimasUtilizadas.Select(mp => mp.nombre.ToLower()).ToList();
            if (nombres.Count != nombres.Distinct().Count())
            {
                ModelState.AddModelError("", "No se permiten materias primas repetidas en la receta.");
                return View(receta);
            }
        }

        if (receta.ProductosPreparadosUtilizados != null)
        {
            var nombres = receta.ProductosPreparadosUtilizados.Select(pp => pp.nombre.ToLower()).ToList();
            if (nombres.Count != nombres.Distinct().Count())
            {
                ModelState.AddModelError("", "No se permiten productos preparados repetidos en la receta.");
                return View(receta);
            }
        }

        // Calcular el costo total de la receta
        decimal costoTotalReceta = 0;
        if (receta.MateriasPrimasUtilizadas != null)
        {
            foreach (var mp in receta.MateriasPrimasUtilizadas)
            {
                mp.costo_por_cantidad = /* lógica para obtener el costo por cantidad de la materia prima */
                mp.total_costo = mp.cantidad * mp.costo_por_cantidad;
                costoTotalReceta += mp.total_costo;
            }
        }

        if (receta.ProductosPreparadosUtilizados != null)
        {
            foreach (var pp in receta.ProductosPreparadosUtilizados)
            {
                pp.costo_por_cantidad = /* lógica para obtener el costo por cantidad del producto preparado */
                pp.total_costo = pp.cantidad * pp.costo_por_cantidad;
                costoTotalReceta += pp.total_costo;
            }
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
    public ActionResult EliminarCostoReceta(int id)
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

        var productofinal = new InsumosModel
        {
            ProductosFinales = query.Select(p => new ProductoFinal
            {
                id = p.id,
                id_receta = p.id_receta ?? 0,
                nombre_receta = p.nombre_receta,
                costo_total_receta = p.costo_total_receta ?? 0,
                margen_de_utilidad = p.margen_de_utilidad ?? 0,
                costo_sin_margen_de_utilidad = p.costo_sin_margen_de_utilidad ?? 0,
                costo_con_margen_de_utilidad = p.costo_con_margen_de_utilidad ?? 0,
                costo_empaque_decoracion_utilizado = p.costo_empaque_decoracion_utilizado ?? 0,
                costo_implemento_utilizado = p.costo_implemento_utilizado ?? 0,
                costo_suministro_utilizado = p.costo_suministro_utilizado ?? 0,
                iva = p.iva ?? 0,
                impuesto_de_servicio = p.impuesto_de_servicio ?? 0,
                envio = p.envio ?? 0,
                plataforma_de_envio = p.plataforma_de_envio,
                precio_final_sugerido = p.precio_final_sugerido ?? 0,
                EmpaquesUtilizados = db.precios_empaques_decoraciones_utilizados
                    .Where(e => e.id_precio_final_sugerido == p.id)
                    .Select(e => new EmpaqueDecoracionUtilizado
                    {
                        id = e.id,
                        id_empaque_decoracion_utilizado = e.id_empaque_decoracion_utilizado ?? 0,
                        nombre = e.tabla_empaques_decoraciones.nombre,
                        cantidad = e.cantidad ?? 0,
                        unidad_de_medida = e.unidad_de_medida,
                        costo_por_cantidad = e.costo_por_cantidad ?? 0,
                        total_costo = e.total_costo ?? 0
                    }).ToList(),
                ImplementosUtilizados = db.precios_implementos_utilizados
                    .Where(i => i.id_precio_final_sugerido == p.id)
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
                    .Where(s => s.id_precio_final_sugerido == p.id)
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
        return View(productofinal);
    }

    // Crear un nuevo producto final
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CrearPrecioFinal(ProductoFinal productofinal)
    {
        if (db.tabla_precios_finales_sugeridos.Any(p => p.nombre_receta.ToLower() == productofinal.nombre_receta.ToLower() && p.id != productofinal.id))
        {
            ModelState.AddModelError("nombre_receta", "Ya existe un producto final para esa receta.");
            return View(productofinal);
        }

        if (string.IsNullOrWhiteSpace(productofinal.nombre_receta))
        {
            ModelState.AddModelError("nombre_receta", "El nombre de la receta es obligatorio.");
            return View(productofinal);
        }

        if (productofinal.margen_de_utilidad < 0 || productofinal.margen_de_utilidad > 100)
        {
            ModelState.AddModelError("margen_de_utilidad", "El margen de utilidad debe estar entre 0 y 100.");
            return View(productofinal);
        }


        if (productofinal.EmpaquesUtilizados != null)
        {
            var nombres = productofinal.EmpaquesUtilizados.Select(e => e.nombre.ToLower()).ToList();
            if (nombres.Count != nombres.Distinct().Count())
            {
                ModelState.AddModelError("", "No se permiten empaques/decoraciones repetidos.");
                return View(productofinal);
            }
        }

        if (productofinal.ImplementosUtilizados != null)
        {
            var nombres = productofinal.ImplementosUtilizados.Select(i => i.nombre.ToLower()).ToList();
            if (nombres.Count != nombres.Distinct().Count())
            {
                ModelState.AddModelError("", "No se permiten implementos repetidos.");
                return View(productofinal);
            }
        }

        if (productofinal.SuministrosUtilizados != null)
        {
            var nombres = productofinal.SuministrosUtilizados.Select(s => s.nombre.ToLower()).ToList();
            if (nombres.Count != nombres.Distinct().Count())
            {
                ModelState.AddModelError("", "No se permiten suministros repetidos.");
                return View(productofinal);
            }
        }
        // Obtener el costo total de la receta seleccionada
        var receta = db.tabla_costos_recetas.FirstOrDefault(r => r.nombre == productofinal.nombre_receta);
        decimal costoReceta = receta?.costo_total_receta ?? 0;

        // Calcular totales de empaques, implementos y suministros
        decimal totalEmpaques = 0, totalImplementos = 0, totalSuministros = 0;

        if (productofinal.EmpaquesUtilizados != null)
        {
            foreach (var e in productofinal.EmpaquesUtilizados)
            {
                var empaque = db.tabla_empaques_decoraciones.FirstOrDefault(x => x.nombre == e.nombre);
                decimal costoUnitario = empaque?.costo_por_cantidad ?? 0;
                e.costo_por_cantidad = costoUnitario;
                e.total_costo = e.cantidad * costoUnitario;
                totalEmpaques += e.total_costo;
            }
        }
        if (productofinal.ImplementosUtilizados != null)
        {
            foreach (var i in productofinal.ImplementosUtilizados)
            {
                var implemento = db.tabla_implementos.FirstOrDefault(x => x.nombre == i.nombre);
                decimal costoUnitario = implemento?.costo_por_cantidad ?? 0;
                i.costo_por_cantidad = costoUnitario;
                i.total_costo = i.cantidad * costoUnitario;
                totalImplementos += i.total_costo;
            }
        }
        if (productofinal.SuministrosUtilizados != null)
        {
            foreach (var s in productofinal.SuministrosUtilizados)
            {
                var suministro = db.tabla_suministros.FirstOrDefault(x => x.nombre == s.nombre);
                decimal costoUnitario = suministro?.costo_por_cantidad ?? 0;
                s.costo_por_cantidad = costoUnitario;
                s.total_costo = s.cantidad * costoUnitario;
                totalSuministros += s.total_costo;
            }
        }

        // Calcular campos derivados
        decimal margen = productofinal.margen_de_utilidad;
        decimal costoConUtilidad = costoReceta * (1 + margen / 100);
        decimal iva = productofinal.iva;
        decimal servicio = productofinal.impuesto_de_servicio;
        decimal envio = productofinal.envio;
        decimal precioFinal = costoConUtilidad + totalEmpaques + totalImplementos + totalSuministros + iva + servicio + envio;

        var precio = new tabla_precios_finales_sugeridos
        {
            id_receta = receta?.id,
            nombre_receta = productofinal.nombre_receta,
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
            plataforma_de_envio = productofinal.plataforma_de_envio,
            precio_final_sugerido = precioFinal
        };
        db.tabla_precios_finales_sugeridos.Add(precio);
        db.SaveChanges();

        // Guardar detalles de empaques, implementos y suministros utilizados
        if (productofinal.EmpaquesUtilizados != null)
        {
            foreach (var e in productofinal.EmpaquesUtilizados)
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
        if (productofinal.ImplementosUtilizados != null)
        {
            foreach (var i in productofinal.ImplementosUtilizados)
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
        if (productofinal.SuministrosUtilizados != null)
        {
            foreach (var s in productofinal.SuministrosUtilizados)
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
        TempData["SuccessMessage"] = "¡Producto final creado con éxito!";
        return RedirectToAction("precio_final");
    }

    //Editar un producto final existente (GET id)
    public ActionResult EditarPrecioFinal(int id)
    {
        var p = db.tabla_precios_finales_sugeridos.Find(id);
        if (p == null) return HttpNotFound();

        var productofinal = new ProductoFinal
        {
            id = p.id,
            id_receta = p.id_receta ?? 0,
            nombre_receta = p.nombre_receta,
            costo_total_receta = p.costo_total_receta ?? 0,
            margen_de_utilidad = p.margen_de_utilidad ?? 0,
            costo_sin_margen_de_utilidad = p.costo_sin_margen_de_utilidad ?? 0,
            costo_con_margen_de_utilidad = p.costo_con_margen_de_utilidad ?? 0,
            costo_empaque_decoracion_utilizado = p.costo_empaque_decoracion_utilizado ?? 0,
            costo_implemento_utilizado = p.costo_implemento_utilizado ?? 0,
            costo_suministro_utilizado = p.costo_suministro_utilizado ?? 0,
            iva = p.iva ?? 0,
            impuesto_de_servicio = p.impuesto_de_servicio ?? 0,
            envio = p.envio ?? 0,
            plataforma_de_envio = p.plataforma_de_envio,
            precio_final_sugerido = p.precio_final_sugerido ?? 0,
            EmpaquesUtilizados = db.precios_empaques_decoraciones_utilizados
                .Where(e => e.id_precio_final_sugerido == p.id)
                .Select(e => new EmpaqueDecoracionUtilizado
                {
                    id = e.id,
                    id_empaque_decoracion_utilizado = e.id_empaque_decoracion_utilizado ?? 0,
                    nombre = e.tabla_empaques_decoraciones.nombre,
                    cantidad = e.cantidad ?? 0,
                    unidad_de_medida = e.unidad_de_medida,
                    costo_por_cantidad = e.costo_por_cantidad ?? 0,
                    total_costo = e.total_costo ?? 0
                }).ToList(),
            ImplementosUtilizados = db.precios_implementos_utilizados
                .Where(i => i.id_precio_final_sugerido == p.id)
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
                .Where(s => s.id_precio_final_sugerido == p.id)
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
        return View(productofinal);
    }

    // Editar un producto final existente (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditarPrecioFinal(ProductoFinal productofinal)
    {
        if (db.tabla_precios_finales_sugeridos.Any(pf => pf.nombre_receta.ToLower() == productofinal.nombre_receta.ToLower() && pf.id != productofinal.id))
        {
            ModelState.AddModelError("nombre_receta", "Ya existe un producto final para esa receta.");
            return View(productofinal);
        }

        if (string.IsNullOrWhiteSpace(productofinal.nombre_receta))
        {
            ModelState.AddModelError("nombre_receta", "El nombre de la receta es obligatorio.");
            return View(productofinal);
        }

        if (productofinal.margen_de_utilidad < 0 || productofinal.margen_de_utilidad > 100)
        {
            ModelState.AddModelError("margen_de_utilidad", "El margen de utilidad debe estar entre 0 y 100.");
            return View(productofinal);
        }

        if (productofinal.EmpaquesUtilizados != null)
        {
            var nombres = productofinal.EmpaquesUtilizados.Select(e => e.nombre.ToLower()).ToList();
            if (nombres.Count != nombres.Distinct().Count())
            {
                ModelState.AddModelError("", "No se permiten empaques/decoraciones repetidos.");
                return View(productofinal);
            }
        }

        if (productofinal.ImplementosUtilizados != null)
        {
            var nombres = productofinal.ImplementosUtilizados.Select(i => i.nombre.ToLower()).ToList();
            if (nombres.Count != nombres.Distinct().Count())
            {
                ModelState.AddModelError("", "No se permiten implementos repetidos.");
                return View(productofinal);
            }
        }

        if (productofinal.SuministrosUtilizados != null)
        {
            var nombres = productofinal.SuministrosUtilizados.Select(s => s.nombre.ToLower()).ToList();
            if (nombres.Count != nombres.Distinct().Count())
            {
                ModelState.AddModelError("", "No se permiten suministros repetidos.");
                return View(productofinal);
            }
        }

        var p = db.tabla_precios_finales_sugeridos.Find(productofinal.id);
        if (p == null) return HttpNotFound();

        // Obtener el costo total de la receta seleccionada
        var receta = db.tabla_costos_recetas.FirstOrDefault(r => r.nombre == productofinal.nombre_receta);
        decimal costoReceta = receta?.costo_total_receta ?? 0;

        // Calcular totales de empaques, implementos y suministros
        decimal totalEmpaques = 0, totalImplementos = 0, totalSuministros = 0;

        if (productofinal.EmpaquesUtilizados != null)
        {
            foreach (var e in productofinal.EmpaquesUtilizados)
            {
                var empaque = db.tabla_empaques_decoraciones.FirstOrDefault(x => x.nombre == e.nombre);
                decimal costoUnitario = empaque?.costo_por_cantidad ?? 0;
                e.costo_por_cantidad = costoUnitario;
                e.total_costo = e.cantidad * costoUnitario;
                totalEmpaques += e.total_costo;
            }
        }
        if (productofinal.ImplementosUtilizados != null)
        {
            foreach (var i in productofinal.ImplementosUtilizados)
            {
                var implemento = db.tabla_implementos.FirstOrDefault(x => x.nombre == i.nombre);
                decimal costoUnitario = implemento?.costo_por_cantidad ?? 0;
                i.costo_por_cantidad = costoUnitario;
                i.total_costo = i.cantidad * costoUnitario;
                totalImplementos += i.total_costo;
            }
        }
        if (productofinal.SuministrosUtilizados != null)
        {
            foreach (var s in productofinal.SuministrosUtilizados)
            {
                var suministro = db.tabla_suministros.FirstOrDefault(x => x.nombre == s.nombre);
                decimal costoUnitario = suministro?.costo_por_cantidad ?? 0;
                s.costo_por_cantidad = costoUnitario;
                s.total_costo = s.cantidad * costoUnitario;
                totalSuministros += s.total_costo;
            }
        }

        // Calcular campos derivados
        decimal margen = productofinal.margen_de_utilidad;
        decimal costoConUtilidad = costoReceta * (1 + margen / 100);
        decimal iva = productofinal.iva;
        decimal servicio = productofinal.impuesto_de_servicio;
        decimal envio = productofinal.envio;
        decimal precioFinal = costoConUtilidad + totalEmpaques + totalImplementos + totalSuministros + iva + servicio + envio;

        // Actualizar campos principales
        p.id_receta = receta?.id;
        p.nombre_receta = productofinal.nombre_receta;
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
        p.plataforma_de_envio = productofinal.plataforma_de_envio;
        p.precio_final_sugerido = precioFinal;

        // Eliminar detalles existentes
        var empaques = db.precios_empaques_decoraciones_utilizados.Where(x => x.id_precio_final_sugerido == p.id).ToList();
        foreach (var item in empaques) db.precios_empaques_decoraciones_utilizados.Remove(item);

        var implementos = db.precios_implementos_utilizados.Where(x => x.id_precio_final_sugerido == p.id).ToList();
        foreach (var item in implementos) db.precios_implementos_utilizados.Remove(item);

        var suministros = db.precios_suministros_utilizados.Where(x => x.id_precio_final_sugerido == p.id).ToList();
        foreach (var item in suministros) db.precios_suministros_utilizados.Remove(item);

        // Agregar nuevos detalles
        if (productofinal.EmpaquesUtilizados != null)
        {
            foreach (var e in productofinal.EmpaquesUtilizados)
            {
                db.precios_empaques_decoraciones_utilizados.Add(new precios_empaques_decoraciones_utilizados
                {
                    id_precio_final_sugerido = p.id,
                    id_empaque_decoracion_utilizado = e.id_empaque_decoracion_utilizado,
                    cantidad = e.cantidad,
                    unidad_de_medida = e.unidad_de_medida,
                    costo_por_cantidad = e.costo_por_cantidad,
                    total_costo = e.total_costo
                });
            }
        }
        if (productofinal.ImplementosUtilizados != null)
        {
            foreach (var i in productofinal.ImplementosUtilizados)
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
        if (productofinal.SuministrosUtilizados != null)
        {
            foreach (var s in productofinal.SuministrosUtilizados)
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
    public ActionResult EliminarPrecioFinal(int id)
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