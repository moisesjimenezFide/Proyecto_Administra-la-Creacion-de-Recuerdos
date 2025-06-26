using Proyecto_CreandoRecuerdos.base_de_datos;
using Proyecto_CreandoRecuerdos.Models;
using System.Linq;
using System.Web.Mvc;

public class InsumosController : Controller
{
    private BD_CREANDO_RECUERDOSEntities4 db = new BD_CREANDO_RECUERDOSEntities4();

    // ----------- Materias Primas -----------
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
        var productofinal = new InsumosModel
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
        return View(productofinal);
    }

    [HttpPost]
    public ActionResult CrearMateriaPrima(MateriaPrima materia)
    {
        db.tabla_materias_primas.Add(new tabla_materias_primas
        {
            nombre = materia.nombre,
            marca = materia.marca,
            presentacion = materia.presentacion,
            proveedor = materia.proveedor,
            costo = materia.costo,
            peso = materia.peso,
            unidad_de_medida = materia.unidad_de_medida,
            costo_por_gramo = materia.costo_por_gramo,
            merma_total_en_gramos = materia.merma_total_en_gramos,
            porcentaje_de_merma = materia.porcentaje_de_merma,
            costo_de_merma_total = materia.costo_de_merma_total,
            costo_total_mas_merma_total = materia.costo_total_mas_merma_total,
            costo_por_gramo_con_merma = materia.costo_por_gramo_con_merma
        });
        db.SaveChanges();
        return RedirectToAction("materias_primas");
    }

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
    public ActionResult EditarMateriaPrima(MateriaPrima materia)
    {
        var m = db.tabla_materias_primas.Find(materia.id);
        if (m != null)
        {
            m.nombre = materia.nombre;
            m.marca = materia.marca;
            m.presentacion = materia.presentacion;
            m.proveedor = materia.proveedor;
            m.costo = materia.costo;
            m.peso = materia.peso;
            m.unidad_de_medida = materia.unidad_de_medida;
            m.costo_por_gramo = materia.costo_por_gramo;
            m.merma_total_en_gramos = materia.merma_total_en_gramos;
            m.porcentaje_de_merma = materia.porcentaje_de_merma;
            m.costo_de_merma_total = materia.costo_de_merma_total;
            m.costo_total_mas_merma_total = materia.costo_total_mas_merma_total;
            m.costo_por_gramo_con_merma = materia.costo_por_gramo_con_merma;
            db.SaveChanges();
        }
        return RedirectToAction("materias_primas");
    }

    public ActionResult EliminarMateriaPrima(int id)
    {
        var m = db.tabla_materias_primas.Find(id);
        if (m != null)
        {
            db.tabla_materias_primas.Remove(m);
            db.SaveChanges();
        }
        return RedirectToAction("materias_primas");
    }

    // ----------- Productos Preparados -----------
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
        var productofinal = new InsumosModel
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
        return View(productofinal);
    }

    [HttpPost]
    public ActionResult CrearProductoPreparado(ProductoPreparado producto)
    {
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
            costo_por_peso = producto.costo_por_peso,
            costo_por_porcion_con_merma = producto.costo_por_porcion_con_merma
        });
        db.SaveChanges();
        return RedirectToAction("productos_preparados");
    }

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
    public ActionResult EditarProductoPreparado(ProductoPreparado producto)
    {
        var p = db.tabla_productos_preparados.Find(producto.id);
        if (p != null)
        {
            p.tipo = producto.tipo;
            p.nombre = producto.nombre;
            p.marca = producto.marca;
            p.presentacion = producto.presentacion;
            p.proveedor = producto.proveedor;
            p.volumen_de_porcion = producto.volumen_de_porcion;
            p.costo = producto.costo;
            p.peso = producto.peso;
            p.unidad_de_medida = producto.unidad_de_medida;
            p.costo_por_peso = producto.costo_por_peso;
            p.costo_por_porcion_con_merma = producto.costo_por_porcion_con_merma;
            db.SaveChanges();
        }
        return RedirectToAction("productos_preparados");
    }

    public ActionResult EliminarProductoPreparado(int id)
    {
        var p = db.tabla_productos_preparados.Find(id);
        if (p != null)
        {
            db.tabla_productos_preparados.Remove(p);
            db.SaveChanges();
        }
        return RedirectToAction("productos_preparados");
    }

    // ----------- Empaques y Decoraciones -----------
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
        var productofinal = new InsumosModel
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
        return View(productofinal);
    }

    [HttpPost]
    public ActionResult CrearEmpaqueDecoracion(EmpaqueDecoracion empaque)
    {
        db.tabla_empaques_decoraciones.Add(new tabla_empaques_decoraciones
        {
            nombre = empaque.nombre,
            marca = empaque.marca,
            presentacion = empaque.presentacion,
            proveedor = empaque.proveedor,
            costo = empaque.costo,
            cantidad = empaque.cantidad,
            unidad_de_medida = empaque.unidad_de_medida,
            costo_por_cantidad = empaque.costo_por_cantidad
        });
        db.SaveChanges();
        return RedirectToAction("empaques_decoraciones");
    }

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

    [HttpPost]
    public ActionResult EditarEmpaqueDecoracion(EmpaqueDecoracion empaque)
    {
        var e = db.tabla_empaques_decoraciones.Find(empaque.id);
        if (e != null)
        {
            e.nombre = empaque.nombre;
            e.marca = empaque.marca;
            e.presentacion = empaque.presentacion;
            e.proveedor = empaque.proveedor;
            e.costo = empaque.costo;
            e.cantidad = empaque.cantidad;
            e.unidad_de_medida = empaque.unidad_de_medida;
            e.costo_por_cantidad = empaque.costo_por_cantidad;
            db.SaveChanges();
        }
        return RedirectToAction("empaques_decoraciones");
    }

    public ActionResult EliminarEmpaqueDecoracion(int id)
    {
        var e = db.tabla_empaques_decoraciones.Find(id);
        if (e != null)
        {
            db.tabla_empaques_decoraciones.Remove(e);
            db.SaveChanges();
        }
        return RedirectToAction("empaques_decoraciones");
    }

    // ----------- Implementos -----------
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
        var productofinal = new InsumosModel
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
        return View(productofinal);
    }

    [HttpPost]
    public ActionResult CrearImplemento(Implemento implemento)
    {
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
        return RedirectToAction("implementos");
    }

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

    [HttpPost]
    public ActionResult EditarImplemento(Implemento implemento)
    {
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
            i.costo_por_cantidad = implemento.costo_por_cantidad;
            db.SaveChanges();
        }
        return RedirectToAction("implementos");
    }

    public ActionResult EliminarImplemento(int id)
    {
        var i = db.tabla_implementos.Find(id);
        if (i != null)
        {
            db.tabla_implementos.Remove(i);
            db.SaveChanges();
        }
        return RedirectToAction("implementos");
    }

    // ----------- Suministros -----------
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
        var productofinal = new InsumosModel
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
        return View(productofinal);
    }

    [HttpPost]
    public ActionResult CrearSuministro(Suministro suministro)
    {
        db.tabla_suministros.Add(new tabla_suministros
        {
            nombre = suministro.nombre,
            marca = suministro.marca,
            presentacion = suministro.presentacion,
            proveedor = suministro.proveedor,
            costo = suministro.costo,
            cantidad = suministro.cantidad,
            unidad_de_medida = suministro.unidad_de_medida,
            costo_por_cantidad = suministro.costo_por_cantidad
        });
        db.SaveChanges();
        return RedirectToAction("suministros");
    }

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

    [HttpPost]
    public ActionResult EditarSuministro(Suministro suministro)
    {
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
            s.costo_por_cantidad = suministro.costo_por_cantidad;
            db.SaveChanges();
        }
        return RedirectToAction("suministros");
    }

    public ActionResult EliminarSuministro(int id)
    {
        var s = db.tabla_suministros.Find(id);
        if (s != null)
        {
            db.tabla_suministros.Remove(s);
            db.SaveChanges();
        }
        return RedirectToAction("suministros");
    }

    // ----------- Costos de Recetas -----------
    public ActionResult costos_recetas(string search)
    {
        var query = db.tabla_costos_recetas.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(r =>
                r.nombre.Contains(search) ||
                r.costo_total_receta.ToString().Contains(search) ||
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
                costo_total_receta = r.costo_total_receta ?? 0,
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
        return View(productofinal);
    }

    // Crear
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CrearCostoReceta(RecetaCosto receta)
    {
        var r = new tabla_costos_recetas
        {
            nombre = receta.nombre,
            costo_total_receta = receta.costo_total_receta
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
                    id_receta = receta.id,
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
                    id_receta = receta.id,
                    id_producto_preparado_utilizado = pp.id_producto_preparado_utilizado,
                    cantidad = pp.cantidad,
                    unidad_de_medida = pp.unidad_de_medida,
                    costo_por_cantidad = pp.costo_por_cantidad,
                    total_costo = pp.total_costo
                });
            }
        }
        db.SaveChanges();
        return RedirectToAction("costos_recetas");
    }

    // Editar (GET)
    public ActionResult EditarCostoReceta(int id)
    {
        var r = db.tabla_costos_recetas.Find(id);
        if (r == null) return HttpNotFound();

        var productofinal = new RecetaCosto
        {
            id = r.id,
            nombre = r.nombre,
            costo_total_receta = r.costo_total_receta ?? 0,
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

    // Editar (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditarCostoReceta(RecetaCosto receta)
    {
        var r = db.tabla_costos_recetas.Find(receta.id);
        if (r == null) return HttpNotFound();

        r.nombre = receta.nombre;
        r.costo_total_receta = receta.costo_total_receta;

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
        return RedirectToAction("costos_recetas");
    }

    // Eliminar
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
            db.SaveChanges();
        }
        return RedirectToAction("costos_recetas");
    }

    // ----------- Precios Finales Sugeridos de Productos Finales -----------

    // Listar y buscar
    public ActionResult precio_final(string search)
    {
        var query = db.tabla_precios_finales_sugeridos.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p =>
                p.nombre_receta.Contains(search) ||
                p.costo_total_receta.ToString().Contains(search) ||
                p.utilidad.ToString().Contains(search) ||
                p.costo_sin_utilidad.ToString().Contains(search) ||
                p.costo_con_margen_de_utilidad.ToString().Contains(search) ||
                p.costo_empaque_decoracion_utilizado.ToString().Contains(search) ||
                p.costo_implemento_utilizado.ToString().Contains(search) ||
                p.costo_suministro_utilizado.ToString().Contains(search) ||
                p.iva.ToString().Contains(search) ||
                p.impuesto_de_servicio.ToString().Contains(search) ||
                p.envio.ToString().Contains(search) ||
                p.plataforma_envio.Contains(search) ||
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
                utilidad = p.utilidad ?? 0,
                costo_sin_utilidad = p.costo_sin_utilidad ?? 0,
                costo_con_margen_de_utilidad = p.costo_con_margen_de_utilidad ?? 0,
                costo_empaque_decoracion_utilizado = p.costo_empaque_decoracion_utilizado ?? 0,
                costo_implemento_utilizado = p.costo_implemento_utilizado ?? 0,
                costo_suministro_utilizado = p.costo_suministro_utilizado ?? 0,
                iva = p.iva ?? 0,
                impuesto_de_servicio = p.impuesto_de_servicio ?? 0,
                envio = p.envio ?? 0,
                plataforma_envio = p.plataforma_envio,
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
        return View(productofinal);
    }

    // Crear
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CrearPrecioFinal(ProductoFinal productofinal)
    {
        var precio = new tabla_precios_finales_sugeridos
        {
            id_receta = productofinal.id_receta,
            nombre_receta = productofinal.nombre_receta,
            costo_total_receta = productofinal.costo_total_receta,
            utilidad = productofinal.utilidad,
            costo_sin_utilidad = productofinal.costo_sin_utilidad,
            costo_con_margen_de_utilidad = productofinal.costo_con_margen_de_utilidad,
            costo_empaque_decoracion_utilizado = productofinal.costo_empaque_decoracion_utilizado,
            costo_implemento_utilizado = productofinal.costo_implemento_utilizado,
            costo_suministro_utilizado = productofinal.costo_suministro_utilizado,
            iva = productofinal.iva,
            impuesto_de_servicio = productofinal.impuesto_de_servicio,
            envio = productofinal.envio,
            plataforma_envio = productofinal.plataforma_envio,
            precio_final_sugerido = productofinal.precio_final_sugerido
        };
        db.tabla_precios_finales_sugeridos.Add(precio);
        db.SaveChanges();

        // Empaques utilizados
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
        // Implementos utilizados
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
        // Suministros utilizados
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
        return RedirectToAction("precio_final");
    }

    // Editar (GET)
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
            utilidad = p.utilidad ?? 0,
            costo_sin_utilidad = p.costo_sin_utilidad ?? 0,
            costo_con_margen_de_utilidad = p.costo_con_margen_de_utilidad ?? 0,
            costo_empaque_decoracion_utilizado = p.costo_empaque_decoracion_utilizado ?? 0,
            costo_implemento_utilizado = p.costo_implemento_utilizado ?? 0,
            costo_suministro_utilizado = p.costo_suministro_utilizado ?? 0,
            iva = p.iva ?? 0,
            impuesto_de_servicio = p.impuesto_de_servicio ?? 0,
            envio = p.envio ?? 0,
            plataforma_envio = p.plataforma_envio,
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

    // Editar (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditarPrecioFinal(ProductoFinal productofinal)
    {
        var p = db.tabla_precios_finales_sugeridos.Find(productofinal.id);
        if (p == null) return HttpNotFound();

        p.id_receta = productofinal.id_receta;
        p.nombre_receta = productofinal.nombre_receta;
        p.costo_total_receta = productofinal.costo_total_receta;
        p.utilidad = productofinal.utilidad;
        p.costo_sin_utilidad = productofinal.costo_sin_utilidad;
        p.costo_con_margen_de_utilidad = productofinal.costo_con_margen_de_utilidad;
        p.costo_empaque_decoracion_utilizado = productofinal.costo_empaque_decoracion_utilizado;
        p.costo_implemento_utilizado = productofinal.costo_implemento_utilizado;
        p.costo_suministro_utilizado = productofinal.costo_suministro_utilizado;
        p.iva = productofinal.iva;
        p.impuesto_de_servicio = productofinal.impuesto_de_servicio;
        p.envio = productofinal.envio;
        p.plataforma_envio = productofinal.plataforma_envio;
        p.precio_final_sugerido = productofinal.precio_final_sugerido;

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
        return RedirectToAction("precio_final");
    }

    // Eliminar
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
            db.SaveChanges();
        }
        return RedirectToAction("precio_final");
    }
}