using Microsoft.Ajax.Utilities;
using Proyecto_CreandoRecuerdos.base_de_datos;
using Proyecto_CreandoRecuerdos.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

public class InsumosController : Controller
{
    private BD_CREANDO_RECUERDOSEntities db = new BD_CREANDO_RECUERDOSEntities();

    /* Materias Primas */

    // Listar y buscar materias primas
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
                m.cantidad.ToString().Contains(search) ||
                m.volumen_de_porcion_de_presentacion.ToString().Contains(search) ||
                m.unidad_de_medida_de_presentacion.Contains(search) ||
                m.volumen_de_porcion_convertido.ToString().Contains(search) ||
                m.unidad_de_medida_convertida.Contains(search) ||
                m.proveedor.Contains(search) ||
                m.costo.ToString().Contains(search) ||
                m.peso.ToString().Contains(search) ||
                m.unidad_de_medida_del_peso.Contains(search) ||
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
                cantidad = (int)m.cantidad,
                volumen_de_porcion_de_presentacion = m.volumen_de_porcion_de_presentacion ?? 0m,
                unidad_de_medida_de_presentacion = m.unidad_de_medida_de_presentacion,
                volumen_de_porcion_convertido = m.volumen_de_porcion_convertido,
                unidad_de_medida_convertida = m.unidad_de_medida_convertida,
                proveedor = m.proveedor,
                costo = m.costo ?? 0m,
                peso = m.peso ?? 0m,
                unidad_de_medida_del_peso = m.unidad_de_medida_del_peso,
                costo_por_gramo = m.costo_por_gramo,
                merma_total_en_gramos = m.merma_total_en_gramos,
                porcentaje_de_merma = m.porcentaje_de_merma,
                costo_de_merma_total = m.costo_de_merma_total,
                costo_total_mas_merma_total = m.costo_total_mas_merma_total,
                costo_por_gramo_con_merma = m.costo_por_gramo_con_merma
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
        // --- PARSE CORRECTO DEL COSTO ---
        string costoStr = Request.Form["costo"];
        if (!string.IsNullOrWhiteSpace(costoStr))
        {
            costoStr = costoStr.Replace(',', '.');
            if (decimal.TryParse(costoStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal costoDecimal))
            {
                materia_prima.costo = costoDecimal;
            }
        }

        // PARSE CORRECTO DEL VOLUMEN DE PORCION DE PRESENTACION
        string volumenDePorcionStr = Request.Form["volumen_de_porcion_de_presentacion"];
        if (!string.IsNullOrWhiteSpace(volumenDePorcionStr))
        {
            volumenDePorcionStr = volumenDePorcionStr.Replace(',', '.');
            if (decimal.TryParse(volumenDePorcionStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal volumenDePorcionDecimal))
            {
                materia_prima.volumen_de_porcion_de_presentacion = volumenDePorcionDecimal;
            }
        }

        // PARSE CORRECTO DE LA MERMA TOTAL EN GRAMOS
        string mermaTotalStr = Request.Form["merma_total_en_gramos"];
        if (!string.IsNullOrWhiteSpace(mermaTotalStr))
        {
            mermaTotalStr = mermaTotalStr.Replace(',', '.');
            if (decimal.TryParse(mermaTotalStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal mermaTotalDecimal))
            {
                materia_prima.merma_total_en_gramos = mermaTotalDecimal;
            }
        }

        // Unidades permitidas
        var unidadesPresentacion = new[] { "kg", "kilo", "kilos", "kilogramo", "kilogramos", "g", "gr", "grs", "gramo", "gramos", "l", "litro", "litros", "ml", "mililitro", "mililitros" };
        var unidadesPeso = new[] { "g", "gr", "grs", "gramo", "gramos", "ml", "mililitro", "mililitros" };
        var unidadesPresentacionMayorA0 = new[] { "g", "grs", "gramos", "kilos", "kilogramos", "l", "litros", "ml", "mililitros" };
        var unidadesPresentacionIgualA1 = new[] { "g", "gr", "gramo", "kilo", "kilogramo", "l", "litro", "ml", "mililitro" };

        string unidadPresentacion = materia_prima.unidad_de_medida_de_presentacion?.Trim().ToLower() ?? "";
        string unidadPeso = materia_prima.unidad_de_medida_del_peso?.Trim().ToLower() ?? "";
        decimal volumen = materia_prima.volumen_de_porcion_de_presentacion ?? 0m;

        var errores = new List<string>();

        // Normalizar valores para comparación
        string nombre = materia_prima.nombre?.Trim().ToLower() ?? "";
        string marca = materia_prima.marca?.Trim().ToLower() ?? "";
        string presentacion = materia_prima.presentacion?.Trim().ToLower() ?? "";
        int cantidad = materia_prima.cantidad;
        decimal? volumenDePorciondePresentacion = materia_prima.volumen_de_porcion_de_presentacion;
        string unidadDeMedidaDePresentacion = materia_prima.unidad_de_medida_de_presentacion?.Trim().ToLower() ?? "";
        string proveedor = materia_prima.proveedor?.Trim().ToLower() ?? "";
        decimal? costo = materia_prima.costo;
        string unidadDeMedidaDelPeso = materia_prima.unidad_de_medida_del_peso?.Trim().ToLower() ?? "";

        // Duplicado exacto (todos los campos)
        bool existeExacto = db.tabla_materias_primas.Any(m =>
            m.nombre.ToLower() == nombre &&
            m.marca.ToLower() == marca &&
            m.presentacion.ToLower() == presentacion &&
            m.cantidad == cantidad &&
            m.volumen_de_porcion_de_presentacion == volumenDePorciondePresentacion &&
            m.unidad_de_medida_de_presentacion.ToLower() == unidadDeMedidaDePresentacion.ToLower() &&
            m.proveedor.ToLower() == proveedor &&
            m.costo == costo &&
            m.unidad_de_medida_del_peso.ToLower() == unidadDeMedidaDelPeso
        );
        if (existeExacto)
        {
            errores.Add("Ya existe una materia prima con los mismos datos");
        }

        // Validar campos obligatorios y valores numéricos
        if (string.IsNullOrWhiteSpace(materia_prima.nombre) ||
            string.IsNullOrWhiteSpace(materia_prima.marca) ||
            string.IsNullOrWhiteSpace(materia_prima.presentacion) ||
            materia_prima.cantidad <= 0 ||
            string.IsNullOrWhiteSpace(materia_prima.volumen_de_porcion_de_presentacion.ToString()) ||
            string.IsNullOrWhiteSpace(materia_prima.unidad_de_medida_de_presentacion) ||
            string.IsNullOrWhiteSpace(materia_prima.proveedor) ||
            string.IsNullOrWhiteSpace(materia_prima.unidad_de_medida_del_peso))
        {
            errores.Add("Todos los campos son obligatorios.");
        }

        if (materia_prima.costo <= 0.99m)
            errores.Add("El costo debe ser mayor a ₡0.99.");

        if (materia_prima.cantidad <= 0)
            errores.Add("La cantidad debe ser mayor a 0.");

        if (materia_prima.volumen_de_porcion_de_presentacion <= 0m)
            errores.Add("El volumen de porción de presentación debe ser mayor a 0.00");

        if (materia_prima.merma_total_en_gramos < 0m)
            errores.Add("La merma total en gramos no puede ser negativa.");

        if (!unidadesPresentacion.Contains(unidadPresentacion))
            errores.Add("Unidad de medida de presentación no permitida.");

        if (!unidadesPeso.Contains(unidadPeso))
            errores.Add("Unidad de medida del peso no permitida.");

        if (volumen > 0m && volumen != 1m)
        {
            if (!unidadesPresentacionMayorA0.Contains(unidadPresentacion))
                errores.Add("Si el volumen de porción de presentación mayor a 0.00 y distinto de 1.00, solo se permiten palabras plurales (g, grs, gramos, kilos, kilogramos, l, litros, ml, mililitros.).");
        }
        else if (volumen == 1m)
        {
            if (!unidadesPresentacionIgualA1.Contains(unidadPresentacion))
                errores.Add("Si el volumen de porción de presentación igual a 1.00, solo se permiten palabras singurales (g, gr, gramo, kilo, kilogramo, l, litro, ml, mililitro.).");
        }

        if (errores.Any())
        {
            ViewBag.Errores = errores;
            var lista = db.tabla_materias_primas.Select(mp => new MateriaPrima
            {
                id = mp.id,
                nombre = mp.nombre,
                marca = mp.marca,
                presentacion = mp.presentacion,
                cantidad = mp.cantidad ?? 0,
                volumen_de_porcion_de_presentacion = mp.volumen_de_porcion_de_presentacion ?? 0m,
                unidad_de_medida_de_presentacion = mp.unidad_de_medida_de_presentacion,
                volumen_de_porcion_convertido = mp.volumen_de_porcion_convertido,
                unidad_de_medida_convertida = mp.unidad_de_medida_convertida,
                proveedor = mp.proveedor,
                costo = mp.costo ?? 0m,
                peso = mp.peso ?? 0m,
                unidad_de_medida_del_peso = mp.unidad_de_medida_del_peso,
                costo_por_gramo = mp.costo_por_gramo,
                merma_total_en_gramos = mp.merma_total_en_gramos,
                porcentaje_de_merma = mp.porcentaje_de_merma,
                costo_de_merma_total = mp.costo_de_merma_total,
                costo_total_mas_merma_total = mp.costo_total_mas_merma_total,
                costo_por_gramo_con_merma = mp.costo_por_gramo_con_merma
            }).ToList();
            return View("materias_primas", new InsumosModel
            {
                MateriaPrimaEditado = materia_prima,
                MateriasPrimas = lista
            });
        }

        db.tabla_materias_primas.Add(new tabla_materias_primas
        {
            nombre = materia_prima.nombre,
            marca = materia_prima.marca,
            presentacion = materia_prima.presentacion,
            cantidad = materia_prima.cantidad,
            volumen_de_porcion_de_presentacion = materia_prima.volumen_de_porcion_de_presentacion,
            unidad_de_medida_de_presentacion = materia_prima.unidad_de_medida_de_presentacion,
            volumen_de_porcion_convertido = materia_prima.volumen_de_porcion_convertido,
            unidad_de_medida_convertida = materia_prima.unidad_de_medida_convertida,
            proveedor = materia_prima.proveedor,
            costo = materia_prima.costo,
            peso = materia_prima.peso,
            unidad_de_medida_del_peso = materia_prima.unidad_de_medida_del_peso,
            merma_total_en_gramos = materia_prima.merma_total_en_gramos ?? 0m,
        });
        db.SaveChanges();
        db.Database.ExecuteSqlCommand("EXEC sp_calculos_materiaprima");
        TempData["SuccessMessage"] = "¡Materia prima agregada con éxito!";
        return RedirectToAction("materias_primas");
    }

    // Editar una materia prima existente (GET id)
    [HttpGet]
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
            cantidad = m.cantidad ?? 0,
            volumen_de_porcion_de_presentacion = m.volumen_de_porcion_de_presentacion ?? 0m,
            unidad_de_medida_de_presentacion = m.unidad_de_medida_de_presentacion,
            volumen_de_porcion_convertido = m.volumen_de_porcion_convertido,
            unidad_de_medida_convertida = m.unidad_de_medida_convertida,
            proveedor = m.proveedor,
            costo = m.costo ?? 0m,
            peso = m.peso ?? 0m,
            unidad_de_medida_del_peso = m.unidad_de_medida_del_peso,
            costo_por_gramo = m.costo_por_gramo,
            merma_total_en_gramos = m.merma_total_en_gramos,
            porcentaje_de_merma = m.porcentaje_de_merma,
            costo_de_merma_total = m.costo_de_merma_total,
            costo_total_mas_merma_total = m.costo_total_mas_merma_total,
            costo_por_gramo_con_merma = m.costo_por_gramo_con_merma
        };

        //Obtener el listado de materias primas
        var lista = db.tabla_materias_primas.Select(mp => new MateriaPrima
        {
            id = mp.id,
            nombre = mp.nombre,
            marca = mp.marca,
            presentacion = mp.presentacion,
            cantidad = mp.cantidad ?? 0,
            volumen_de_porcion_de_presentacion = mp.volumen_de_porcion_de_presentacion ?? 0m,
            unidad_de_medida_de_presentacion = mp.unidad_de_medida_de_presentacion,
            volumen_de_porcion_convertido = mp.volumen_de_porcion_convertido,
            unidad_de_medida_convertida = mp.unidad_de_medida_convertida,
            proveedor = mp.proveedor,
            costo = mp.costo ?? 0m,
            peso = mp.peso ?? 0m,
            unidad_de_medida_del_peso = mp.unidad_de_medida_del_peso,
            costo_por_gramo = mp.costo_por_gramo,
            merma_total_en_gramos = mp.merma_total_en_gramos,
            porcentaje_de_merma = mp.porcentaje_de_merma,
            costo_de_merma_total = mp.costo_de_merma_total,
            costo_total_mas_merma_total = mp.costo_total_mas_merma_total,
            costo_por_gramo_con_merma = mp.costo_por_gramo_con_merma
        }).ToList();

        ViewBag.Editando = true;
        return View("materias_primas", new InsumosModel
        {
            MateriaPrimaEditado = materia_prima,
            MateriasPrimas = lista
        });
    }

    // Editar una materia prima existente (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditarMateriaPrima(MateriaPrima materia_prima)
    {
        // --- PARSE CORRECTO DEL COSTO ---
        string costoStr = Request.Form["costo"];
        if (!string.IsNullOrWhiteSpace(costoStr))
        {
            // Reemplaza la coma por punto para la conversión
            costoStr = costoStr.Replace(',', '.');
            if (decimal.TryParse(costoStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal costoDecimal))
            {
                materia_prima.costo = costoDecimal;
            }
        }

        // PARSE CORRECTO DEL VOLUMEN DE PORCION DE PRESENTACION
        string volumenDePorcionStr = Request.Form["volumen_de_porcion_de_presentacion"];
        if (!string.IsNullOrWhiteSpace(volumenDePorcionStr))
        {
            volumenDePorcionStr = volumenDePorcionStr.Replace(',', '.');
            if (decimal.TryParse(volumenDePorcionStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal volumenDePorcionDecimal))
            {
                materia_prima.volumen_de_porcion_de_presentacion = volumenDePorcionDecimal;
            }
        }

        // PASE CORRECTO DE LA MERMA TOTAL EN GRAMOS
        string mermaTotalStr = Request.Form["merma_total_en_gramos"];
        if (!string.IsNullOrWhiteSpace(mermaTotalStr))
        {
            mermaTotalStr = mermaTotalStr.Replace(',', '.');
            if (decimal.TryParse(mermaTotalStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal mermaTotalDecimal))
            {
                materia_prima.merma_total_en_gramos = mermaTotalDecimal;
            }
        }

        // Unidades permitidas
        var unidadesPresentacion = new[] { "kg", "kilo", "kilos", "kilogramo", "kilogramos", "g", "gr", "grs", "gramo", "gramos", "l", "litro", "litros", "ml", "mililitro", "mililitros" };
        var unidadesPeso = new[] { "g", "gr", "grs", "gramo", "gramos", "ml", "mililitro", "mililitros" };
        var unidadesPresentacionMayorA0 = new[] { "g", "grs", "gramos", "kilos", "kilogramos", "l", "litros", "ml", "mililitros" };
        var unidadesPresentacionIgualA1 = new[] { "g", "gr", "gramo", "kilo", "kilogramo", "l", "litro", "ml", "mililitro" };

        string unidadPresentacion = materia_prima.unidad_de_medida_de_presentacion?.Trim().ToLower() ?? "";
        string unidadPeso = materia_prima.unidad_de_medida_del_peso?.Trim().ToLower() ?? "";
        decimal volumen = materia_prima.volumen_de_porcion_de_presentacion ?? 0m;

        var errores = new List<string>();

        // Normalizar valores para comparación
        string nombre = materia_prima.nombre?.Trim().ToLower() ?? "";
        string marca = materia_prima.marca?.Trim().ToLower() ?? "";
        string presentacion = materia_prima.presentacion?.Trim().ToLower() ?? "";
        int cantidad = materia_prima.cantidad;
        decimal? volumenDePorciondePresentacion = materia_prima.volumen_de_porcion_de_presentacion;
        string unidadDeMedidaDePresentacion = materia_prima.unidad_de_medida_de_presentacion?.Trim().ToLower() ?? "";
        string proveedor = materia_prima.proveedor?.Trim().ToLower() ?? "";
        decimal? costo = materia_prima.costo;
        string unidadDeMedidaDelPeso = materia_prima.unidad_de_medida_del_peso?.Trim().ToLower() ?? "";

        // Duplicado exacto (todos los campos)
        bool existeExacto = db.tabla_materias_primas.Any(mp =>
            mp.id != materia_prima.id &&
            mp.nombre.ToLower() == nombre &&
            mp.marca.ToLower() == marca &&
            mp.presentacion.ToLower() == presentacion &&
            mp.cantidad == cantidad &&
            mp.volumen_de_porcion_de_presentacion == volumenDePorciondePresentacion &&
            mp.unidad_de_medida_de_presentacion.ToLower() == unidadDeMedidaDePresentacion &&
            mp.proveedor.ToLower() == proveedor &&
            mp.costo == costo &&
            mp.unidad_de_medida_del_peso.ToLower() == unidadDeMedidaDelPeso
        );
        if (existeExacto)
        {
            errores.Add("Ya existe una materia prima con los mismos datos");
        }

        // Validar campos obligatorios y valores numéricos
        if (string.IsNullOrWhiteSpace(materia_prima.nombre) ||
            string.IsNullOrWhiteSpace(materia_prima.marca) ||
            string.IsNullOrWhiteSpace(materia_prima.presentacion) ||
            materia_prima.cantidad <= 0 ||
            string.IsNullOrWhiteSpace(materia_prima.volumen_de_porcion_de_presentacion.ToString()) ||
            string.IsNullOrWhiteSpace(materia_prima.unidad_de_medida_de_presentacion) ||
            string.IsNullOrWhiteSpace(materia_prima.proveedor) ||
            string.IsNullOrWhiteSpace(materia_prima.unidad_de_medida_del_peso))
        {
            errores.Add("Todos los campos son obligatorios.");
        }

        if (materia_prima.costo <= 0.99m)
            errores.Add("El costo debe ser mayor a ₡0.99.");

        if (materia_prima.cantidad <= 0)
            errores.Add("La cantidad debe ser mayor a 0.");

        if (materia_prima.volumen_de_porcion_de_presentacion <= 0m)
            errores.Add("El volumen de porción de presentación debe ser mayor a 0.00");

        if (materia_prima.merma_total_en_gramos < 0)
            errores.Add("La merma total en gramos no puede ser negativa.");

        if (!unidadesPresentacion.Contains(unidadPresentacion))
            errores.Add("Unidad de medida de presentación no permitida.");

        if (!unidadesPeso.Contains(unidadPeso))
            errores.Add("Unidad de medida del peso no permitida.");

        if (volumen > 0m && volumen != 1m)
        {
            if (!unidadesPresentacionMayorA0.Contains(unidadPresentacion))
                errores.Add("Si el volumen de porción de presentación mayor a 0.00 y distinto de 1.00, solo se permiten palabras plurales (g, grs, gramos, kilos, kilogramos, l, litros, ml, mililitros.).");
        }
        else if (volumen == 1m)
        {
            if (!unidadesPresentacionIgualA1.Contains(unidadPresentacion))
                errores.Add("Si el volumen de porción de presentación igual a 1.00, solo se permiten palabras singurales (g, gr, gramo, kilo, kilogramo, l, litro, ml, mililitro.).");
        }

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
                cantidad = mp.cantidad ?? 0,
                volumen_de_porcion_de_presentacion = mp.volumen_de_porcion_de_presentacion ?? 0m,
                unidad_de_medida_de_presentacion = mp.unidad_de_medida_de_presentacion,
                volumen_de_porcion_convertido = mp.volumen_de_porcion_convertido ?? 0m,
                unidad_de_medida_convertida = mp.unidad_de_medida_convertida,
                proveedor = mp.proveedor,
                costo = mp.costo ?? 0m,
                peso = mp.peso ?? 0m,
                unidad_de_medida_del_peso = mp.unidad_de_medida_del_peso,
                costo_por_gramo = mp.costo_por_gramo,
                merma_total_en_gramos = mp.merma_total_en_gramos,
                porcentaje_de_merma = mp.porcentaje_de_merma,
                costo_de_merma_total = mp.costo_de_merma_total,
                costo_total_mas_merma_total = mp.costo_total_mas_merma_total,
                costo_por_gramo_con_merma = mp.costo_por_gramo_con_merma
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
            m.nombre = materia_prima.nombre;
            m.marca = materia_prima.marca;
            m.presentacion = materia_prima.presentacion;
            m.cantidad = materia_prima.cantidad;
            m.volumen_de_porcion_de_presentacion = materia_prima.volumen_de_porcion_de_presentacion ;
            m.unidad_de_medida_de_presentacion = materia_prima.unidad_de_medida_de_presentacion;
            m.volumen_de_porcion_convertido = materia_prima.volumen_de_porcion_convertido;
            m.unidad_de_medida_convertida = materia_prima.unidad_de_medida_convertida;
            m.proveedor = materia_prima.proveedor;
            m.costo = materia_prima.costo;
            m.peso = materia_prima.peso;
            m.unidad_de_medida_del_peso = materia_prima.unidad_de_medida_del_peso;
            m.merma_total_en_gramos = materia_prima.merma_total_en_gramos ?? 0m;
        }
        db.SaveChanges();
        db.Database.ExecuteSqlCommand("EXEC sp_calculos_materiaprima");
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

    /* Productos Preparados */

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
                p.cantidad.ToString().Contains(search) ||
                p.volumen_de_porcion_de_presentacion.ToString().Contains(search) ||
                p.unidad_de_medida_de_presentacion.Contains(search) ||
                p.volumen_de_porcion_convertido.ToString().Contains(search) ||
                p.unidad_de_medida_convertida.Contains(search) ||
                p.proveedor.Contains(search) ||
                p.costo.ToString().Contains(search) ||
                p.peso.ToString().Contains(search) ||
                p.unidad_de_medida_del_peso.Contains(search) ||
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
                cantidad = (int)p.cantidad,
                volumen_de_porcion_de_presentacion = p.volumen_de_porcion_de_presentacion ?? 0m,
                unidad_de_medida_de_presentacion = p.unidad_de_medida_de_presentacion,
                volumen_de_porcion_convertido = p.volumen_de_porcion_convertido,
                unidad_de_medida_convertida = p.unidad_de_medida_convertida,
                proveedor = p.proveedor,
                costo = p.costo ?? 0m,
                peso = p.peso ?? 0m,
                unidad_de_medida_del_peso = p.unidad_de_medida_del_peso,
                costo_por_peso = p.costo_por_peso,
                costo_por_porcion_con_merma = p.costo_por_porcion_con_merma
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
        // --- PARSE CORRECTO DEL COSTO ---
        string costoStr = Request.Form["costo"];
        if (!string.IsNullOrWhiteSpace(costoStr))
        {
            costoStr = costoStr.Replace(',', '.');
            if (decimal.TryParse(costoStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal costoDecimal))
            {
                producto_preparado.costo = costoDecimal;
            }
        }

        // PARSE CORRECTO DEL VOLUMEN DE PORCION DE PRESENTACION
        string volumenDePorcionStr = Request.Form["volumen_de_porcion_de_presentacion"];
        if (!string.IsNullOrWhiteSpace(volumenDePorcionStr))
        {
            volumenDePorcionStr = volumenDePorcionStr.Replace(',', '.');
            if (decimal.TryParse(volumenDePorcionStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal volumenDePorcionDecimal))
            {
                producto_preparado.volumen_de_porcion_de_presentacion = volumenDePorcionDecimal;
            }
        }

        // Unidades permitidas
        var unidadesPresentacion = new[] { "kg", "kilo", "kilos", "kilogramo", "kilogramos", "g", "gr", "grs", "gramo", "gramos", "l", "litro", "litros", "ml", "mililitro", "mililitros" };
        var unidadesPeso = new[] { "g", "gr", "grs", "gramo", "gramos", "ml", "mililitro", "mililitros" };
        var unidadesPresentacionMayorA0 = new[] { "g", "grs", "gramos", "kilos", "kilogramos", "l", "litros", "ml", "mililitros" };
        var unidadesPresentacionIgualA1 = new[] { "g", "gr", "gramo", "kilo", "kilogramo", "l", "litro", "ml", "mililitro" };

        string unidadPresentacion = producto_preparado.unidad_de_medida_de_presentacion?.Trim().ToLower() ?? "";
        string unidadPeso = producto_preparado.unidad_de_medida_del_peso?.Trim().ToLower() ?? "";
        decimal volumen = producto_preparado.volumen_de_porcion_de_presentacion ?? 0m;

        var errores = new List<string>();
        
        // Normalizar valores para comparación
        string tipo = producto_preparado.tipo?.Trim().ToLower() ?? "";
        string nombre = producto_preparado.nombre?.Trim().ToLower() ?? "";
        string marca = producto_preparado.marca?.Trim().ToLower() ?? "";
        string presentacion = producto_preparado.presentacion?.Trim().ToLower() ?? "";
        int cantidad = producto_preparado.cantidad;
        decimal? volumenDePorciondePresentacion = producto_preparado.volumen_de_porcion_de_presentacion;
        string unidadDeMedidaDePresentacion = producto_preparado.unidad_de_medida_de_presentacion?.Trim().ToLower() ?? "";
        string proveedor = producto_preparado.proveedor?.Trim().ToLower() ?? "";
        decimal? costo = producto_preparado.costo;
        string unidadDeMedidaDelPeso = producto_preparado.unidad_de_medida_del_peso?.Trim().ToLower() ?? "";

        // Duplicado exacto (todos los campos)
        if (db.tabla_productos_preparados.Any(p =>
            p.tipo.ToLower() == tipo &&
            p.nombre.ToLower() == nombre &&
            p.marca.ToLower() == marca &&
            p.presentacion.ToLower() == presentacion &&
            p.cantidad == cantidad &&
            p.volumen_de_porcion_de_presentacion == volumenDePorciondePresentacion &&
            p.unidad_de_medida_de_presentacion.ToLower() == unidadDeMedidaDePresentacion &&
            p.proveedor.ToLower() == proveedor &&
            p.costo == costo &&
            p.unidad_de_medida_del_peso.ToLower() == unidadDeMedidaDelPeso))
        {
            errores.Add("Ya existe un producto preparado con los mismos datos.");
        }

        // Validar campos obligatorios y valores numéricos
        if (string.IsNullOrWhiteSpace(producto_preparado.tipo) ||
            string.IsNullOrWhiteSpace(producto_preparado.nombre) ||
            string.IsNullOrWhiteSpace(producto_preparado.marca) ||
            string.IsNullOrWhiteSpace(producto_preparado.presentacion) ||
            string.IsNullOrWhiteSpace(producto_preparado.cantidad.ToString()) ||
            string.IsNullOrWhiteSpace(producto_preparado.volumen_de_porcion_de_presentacion.ToString()) ||
            string.IsNullOrWhiteSpace(producto_preparado.unidad_de_medida_de_presentacion) ||
            string.IsNullOrWhiteSpace(producto_preparado.proveedor) ||
            string.IsNullOrWhiteSpace(producto_preparado.unidad_de_medida_del_peso))
        {
            errores.Add("Todos los campos son obligatorios.");
        }

        if (producto_preparado.costo <= 0.99m)
            errores.Add("El costo debe ser mayor a ₡0.99.");

        if (producto_preparado.cantidad <= 0)
            errores.Add("La cantidad debe ser mayor a cero.");

        if (producto_preparado.volumen_de_porcion_de_presentacion <= 0m)
            errores.Add("El volumen de porción de presentación debe ser mayor a 0.00");

        if (!unidadesPresentacion.Contains(unidadPresentacion))
            errores.Add("Unidad de medida de presentación no permitida.");

        if (!unidadesPeso.Contains(unidadPeso))
            errores.Add("Unidad de medida del peso no permitida.");

        if (volumen > 0m && volumen != 1m)
        {
            if (!unidadesPresentacionMayorA0.Contains(unidadPresentacion))
                errores.Add("Si el volumen de porción de presentación mayor a 0.00 y distinto de 1.00, solo se permiten palabras plurales (g, grs, gramos, kilos, kilogramos, l, litros, ml, mililitros.).");
        }
        else if (volumen == 1m)
        {
            if (!unidadesPresentacionIgualA1.Contains(unidadPresentacion))
                errores.Add("Si el volumen de porción de presentación igual a 1.00, solo se permiten palabras singurales (g, gr, gramo, kilo, kilogramo, l, litro, ml, mililitro.).");
        }

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
                cantidad = p.cantidad ?? 0 ,
                volumen_de_porcion_de_presentacion = p.volumen_de_porcion_de_presentacion ?? 0m,
                unidad_de_medida_de_presentacion = p.unidad_de_medida_de_presentacion,
                volumen_de_porcion_convertido = p.volumen_de_porcion_convertido,
                unidad_de_medida_convertida = p.unidad_de_medida_convertida,
                proveedor = p.proveedor,
                costo = p.costo ?? 0m,
                peso = p.peso ?? 0m,
                unidad_de_medida_del_peso = p.unidad_de_medida_del_peso,
                costo_por_peso = p.costo_por_peso,
                costo_por_porcion_con_merma = p.costo_por_porcion_con_merma
            }).ToList();
            return View("productos_preparados", new InsumosModel
            {
                ProductoPreparadoEditado = producto_preparado,
                ProductosPreparados = lista
            });
        }

        db.tabla_productos_preparados.Add(new tabla_productos_preparados
        {
            tipo = producto_preparado.tipo,
            nombre = producto_preparado.nombre,
            marca = producto_preparado.marca,
            presentacion = producto_preparado.presentacion,
            cantidad = producto_preparado.cantidad,
            volumen_de_porcion_de_presentacion = producto_preparado.volumen_de_porcion_de_presentacion,
            unidad_de_medida_de_presentacion = producto_preparado.unidad_de_medida_de_presentacion,
            volumen_de_porcion_convertido = producto_preparado.volumen_de_porcion_convertido,
            unidad_de_medida_convertida = producto_preparado.unidad_de_medida_convertida,
            proveedor = producto_preparado.proveedor,
            costo = producto_preparado.costo,
            peso = producto_preparado.peso,
            unidad_de_medida_del_peso = producto_preparado.unidad_de_medida_del_peso,
        });
        db.SaveChanges();
        db.Database.ExecuteSqlCommand("EXEC sp_calculos_productopreparado");
        TempData["SuccessMessage"] = "¡Producto preparado agregado con éxito!";
        return RedirectToAction("productos_preparados");
    }

    // Editar un producto preparado existente (GET id)
    [HttpGet]
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
            cantidad = p.cantidad ?? 0,
            volumen_de_porcion_de_presentacion = p.volumen_de_porcion_de_presentacion ?? 0m,
            unidad_de_medida_de_presentacion = p.unidad_de_medida_de_presentacion,
            volumen_de_porcion_convertido = p.volumen_de_porcion_convertido,
            unidad_de_medida_convertida = p.unidad_de_medida_convertida,
            proveedor = p.proveedor,
            costo = p.costo ?? 0m,
            peso = p.peso ?? 0m,
            unidad_de_medida_del_peso = p.unidad_de_medida_del_peso,
            costo_por_peso = p.costo_por_peso,
            costo_por_porcion_con_merma = p.costo_por_porcion_con_merma,
        };

        // Obtener el listado de productos preparados
        var lista = db.tabla_productos_preparados.Select(prodprep => new ProductoPreparado
        {
            id = prodprep.id,
            tipo = prodprep.tipo,
            nombre = prodprep.nombre,
            marca = prodprep.marca,
            presentacion = prodprep.presentacion,
            cantidad = prodprep.cantidad ?? 0,
            volumen_de_porcion_de_presentacion = prodprep.volumen_de_porcion_de_presentacion ?? 0m,
            unidad_de_medida_de_presentacion = prodprep.unidad_de_medida_de_presentacion,
            volumen_de_porcion_convertido = prodprep.volumen_de_porcion_convertido,
            unidad_de_medida_convertida = prodprep.unidad_de_medida_convertida,
            proveedor = prodprep.proveedor,
            costo = prodprep.costo ?? 0m,
            peso = prodprep.peso ?? 0m,
            unidad_de_medida_del_peso = prodprep.unidad_de_medida_del_peso,
            costo_por_peso = prodprep.costo_por_peso,
            costo_por_porcion_con_merma = prodprep.costo_por_porcion_con_merma
        }).ToList();

        ViewBag.Editando = true;
        return View("productos_preparados", new InsumosModel
        {
            ProductoPreparadoEditado = producto_preparado,
            ProductosPreparados = lista
        });
    }

    // Editar un producto preparado existente (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditarProductoPreparado(ProductoPreparado producto_preparado)
    {
        // --- PARSE CORRECTO DEL COSTO ---
        string costoStr = Request.Form["costo"];
        if (!string.IsNullOrWhiteSpace(costoStr))
        {
            // Reemplaza la coma por punto para la conversión
            costoStr = costoStr.Replace(',', '.');
            if (decimal.TryParse(costoStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal costoDecimal))
            {
                producto_preparado.costo = costoDecimal;
            }
        }

        // PARSE CORRECTO DEL VOLUMEN DE PORCION DE PRESENTACION
        string volumenDePorcionStr = Request.Form["volumen_de_porcion_de_presentacion"];
        if (!string.IsNullOrWhiteSpace(volumenDePorcionStr))
        {
            volumenDePorcionStr = volumenDePorcionStr.Replace(',', '.');
            if (decimal.TryParse(volumenDePorcionStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal volumenDePorcionDecimal))
            {
                producto_preparado.volumen_de_porcion_de_presentacion = volumenDePorcionDecimal;
            }
        }

        // Unidades permitidas
        var unidadesPresentacion = new[] { "kg", "kilo", "kilos", "kilogramo", "kilogramos", "g", "gr", "grs", "gramo", "gramos", "l", "litro", "litros", "ml", "mililitro", "mililitros" };
        var unidadesPeso = new[] { "g", "gr", "grs", "gramo", "gramos", "ml", "mililitro", "mililitros" };
        var unidadesPresentacionMayorA0 = new[] { "g", "grs", "gramos", "kilos", "kilogramos", "l", "litros", "ml", "mililitros" };
        var unidadesPresentacionIgualA1 = new[] { "g", "gr", "gramo", "kilo", "kilogramo", "l", "litro", "ml", "mililitro" };

        string unidadPresentacion = producto_preparado.unidad_de_medida_de_presentacion?.Trim().ToLower() ?? "";
        string unidadPeso = producto_preparado.unidad_de_medida_del_peso?.Trim().ToLower() ?? "";
        decimal volumen = producto_preparado.volumen_de_porcion_de_presentacion ?? 0m;


        var errores = new List<string>();

        // Normalizar valores para comparación
        string tipo = producto_preparado.tipo?.Trim().ToLower() ?? "";
        string nombre = producto_preparado.nombre?.Trim().ToLower() ?? "";
        string marca = producto_preparado.marca?.Trim().ToLower() ?? "";
        string presentacion = producto_preparado.presentacion?.Trim().ToLower() ?? "";
        int cantidad = producto_preparado.cantidad;
        decimal? volumenDePorciondePresentacion = producto_preparado.volumen_de_porcion_de_presentacion;
        string unidadDeMedidaDePresentacion = producto_preparado.unidad_de_medida_de_presentacion?.Trim().ToLower() ?? "";
        string proveedor = producto_preparado.proveedor?.Trim().ToLower() ?? "";
        decimal? costo = producto_preparado.costo;
        string unidadDeMedidaDelPeso = producto_preparado.unidad_de_medida_del_peso?.Trim().ToLower() ?? "";

        // Duplicado exacto (todos los campos excepto ID)
        bool existeExacto = db.tabla_productos_preparados.Any(p =>
            p.id != producto_preparado.id &&
            p.tipo.ToLower() == tipo &&
            p.nombre.ToLower() == nombre &&
            p.marca.ToLower() == marca &&
            p.presentacion.ToLower() == presentacion &&
            p.cantidad == cantidad &&
            p.volumen_de_porcion_de_presentacion == volumenDePorciondePresentacion &&
            p.unidad_de_medida_de_presentacion.ToLower() == unidadDeMedidaDePresentacion &&
            p.proveedor.ToLower() == proveedor &&
            p.costo == costo && 
            p.unidad_de_medida_del_peso.ToLower() == unidadDeMedidaDelPeso
        );
        if (existeExacto)
        {
            errores.Add("Ya existe un producto preparado con los mismos datos.");
        }

        // Validar campos obligatorios y valores numéricos
        if (string.IsNullOrWhiteSpace(producto_preparado.tipo) ||
            string.IsNullOrWhiteSpace(producto_preparado.nombre) ||
            string.IsNullOrWhiteSpace(producto_preparado.marca) ||
            string.IsNullOrWhiteSpace(producto_preparado.presentacion) ||
            string.IsNullOrWhiteSpace(producto_preparado.cantidad.ToString()) ||
            string.IsNullOrWhiteSpace(producto_preparado.volumen_de_porcion_de_presentacion.ToString()) ||
            string.IsNullOrWhiteSpace(producto_preparado.unidad_de_medida_de_presentacion) ||
            string.IsNullOrWhiteSpace(producto_preparado.proveedor) ||
            string.IsNullOrWhiteSpace(producto_preparado.unidad_de_medida_del_peso))
        {
            errores.Add("Todos los campos son obligatorios.");
        }

        if (producto_preparado.costo <= 0.99m)
            errores.Add("El costo debe ser mayor a ₡0.99.");

        if (producto_preparado.cantidad <= 0)
            errores.Add("La cantidad debe ser mayor a cero.");

        if (producto_preparado.volumen_de_porcion_de_presentacion <= 0m)
            errores.Add("El volumen de porción de presentación debe ser mayor a 0.00");

        if (!unidadesPresentacion.Contains(unidadPresentacion))
            errores.Add("Unidad de medida de presentación no permitida.");

        if (!unidadesPeso.Contains(unidadPeso))
            errores.Add("Unidad de medida del peso no permitida.");

        if (volumen > 0m && volumen != 1m)
        {
            if (!unidadesPresentacionMayorA0.Contains(unidadPresentacion))
                errores.Add("Si el volumen de porción de presentación mayor a 0.00 y distinto de 1.00, solo se permiten palabras plurales (g, grs, gramos, kilos, kilogramos, l, litros, ml, mililitros.).");
        }
        else if (volumen == 1m)
        {
            if (!unidadesPresentacionIgualA1.Contains(unidadPresentacion))
                errores.Add("Si el volumen de porción de presentación igual a 1.00, solo se permiten palabras singurales (g, gr, gramo, kilo, kilogramo, l, litro, ml, mililitro.).");
        }

        if (errores.Any())
        {
            ViewBag.Errores = errores;
            ViewBag.Editando = true;
            var lista = db.tabla_productos_preparados.Select(prodprep => new ProductoPreparado
            {
                id = prodprep.id,
                tipo = prodprep.tipo,
                nombre = prodprep.nombre,
                marca = prodprep.marca,
                presentacion = prodprep.presentacion,
                cantidad = prodprep.cantidad ?? 0,
                volumen_de_porcion_de_presentacion = prodprep.volumen_de_porcion_de_presentacion ?? 0m,
                unidad_de_medida_de_presentacion = prodprep.unidad_de_medida_de_presentacion,
                volumen_de_porcion_convertido = prodprep.volumen_de_porcion_convertido ?? 0m,
                unidad_de_medida_convertida = prodprep.unidad_de_medida_convertida,
                proveedor = prodprep.proveedor,
                costo = prodprep.costo ?? 0m,
                peso = prodprep.peso,
                unidad_de_medida_del_peso = prodprep.unidad_de_medida_del_peso,
                costo_por_peso = prodprep.costo_por_peso,
                costo_por_porcion_con_merma = prodprep.costo_por_porcion_con_merma,
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
            pp.tipo = producto_preparado.tipo;
            pp.nombre = producto_preparado.nombre;
            pp.marca = producto_preparado.marca;
            pp.presentacion = producto_preparado.presentacion;
            pp.cantidad = producto_preparado.cantidad;
            pp.volumen_de_porcion_de_presentacion = producto_preparado.volumen_de_porcion_de_presentacion;
            pp.unidad_de_medida_de_presentacion = producto_preparado.unidad_de_medida_de_presentacion;
            pp.volumen_de_porcion_convertido = producto_preparado.volumen_de_porcion_convertido;
            pp.unidad_de_medida_convertida = producto_preparado.unidad_de_medida_convertida;
            pp.proveedor = producto_preparado.proveedor;
            pp.costo = producto_preparado.costo;
            pp.peso = producto_preparado.peso ?? 0m;
            pp.unidad_de_medida_del_peso = producto_preparado.unidad_de_medida_del_peso;
        }
        db.SaveChanges();
        db.Database.ExecuteSqlCommand("EXEC sp_calculos_productopreparado"); 
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

    /* Empaques y/o Decoraciones */

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
                costo = ed.costo ?? 0m,
                cantidad = (int)ed.cantidad,
                unidad_de_medida = ed.unidad_de_medida,
                costo_por_cantidad = ed.costo_por_cantidad
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
        // --- PARSE CORRECTO DEL COSTO ---
        string costoStr = Request.Form["costo"];
        if (!string.IsNullOrWhiteSpace(costoStr))
        {
            costoStr = costoStr.Replace(',', '.');
            if (decimal.TryParse(costoStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal costoDecimal))
            {
                empaque_decoracion.costo = costoDecimal;
            }
        }

        var errores = new List<string>();

        // Normalizar valores para comparación
        string nombre = empaque_decoracion.nombre?.Trim().ToLower() ?? "";
        string marca = empaque_decoracion.marca?.Trim().ToLower() ?? "";
        string presentacion = empaque_decoracion.presentacion?.Trim().ToLower() ?? "";
        string proveedor = empaque_decoracion.proveedor?.Trim().ToLower() ?? "";
        decimal? costo = empaque_decoracion.costo;
        int cantidad = empaque_decoracion.cantidad;
        
        // Duplicado exacto (todos los campos)
        if (db.tabla_empaques_decoraciones.Any(ed =>
            ed.nombre.ToLower() == nombre &&
            ed.marca.ToLower() == marca &&
            ed.presentacion.ToLower() == presentacion &&
            ed.proveedor.ToLower() == proveedor &&
            ed.costo == costo &&
            ed.cantidad == cantidad))
        {
            errores.Add("Ya existe un empaque o decoración con los mismos datos.");
        }

        // Validar campos obligatorios y valores numéricos
        if (string.IsNullOrWhiteSpace(empaque_decoracion.nombre) ||
            string.IsNullOrWhiteSpace(empaque_decoracion.marca) ||
            string.IsNullOrWhiteSpace(empaque_decoracion.presentacion) ||
            string.IsNullOrWhiteSpace(empaque_decoracion.proveedor) ||
            string.IsNullOrWhiteSpace(empaque_decoracion.cantidad.ToString()) ||
            string.IsNullOrWhiteSpace(empaque_decoracion.unidad_de_medida))
        {
            errores.Add("Todos los campos son obligatorios.");
        }

        if (empaque_decoracion.costo <= 0.99m)
            errores.Add("El costo debe ser mayor a ₡0.99.");

        if (empaque_decoracion.cantidad <= 0)
            errores.Add("La cantidad debe ser mayor a cero.");

        if (errores.Any())
        {
            ViewBag.Errores = errores;
            var empaques = db.tabla_empaques_decoraciones
                         .AsNoTracking()
                         .Select(ed => new EmpaqueDecoracion
                         {
                             id = ed.id,
                             nombre = ed.nombre,
                             marca = ed.marca,
                             presentacion = ed.presentacion,
                             proveedor = ed.proveedor,
                             costo = ed.costo ?? 0m,
                             cantidad = ed.cantidad ?? 0,
                             unidad_de_medida = ed.unidad_de_medida,
                             costo_por_cantidad = ed.costo_por_cantidad
                         }).ToList();

        return View("empaques_decoraciones", new InsumosModel
        {
            EmpaqueDecoracionEditado = empaque_decoracion,
            EmpaquesDecoraciones = empaques
        });
        }

        db.tabla_empaques_decoraciones.Add(new tabla_empaques_decoraciones
        {
            nombre = empaque_decoracion.nombre,
            marca = empaque_decoracion.marca,
            presentacion = empaque_decoracion.presentacion,
            proveedor = empaque_decoracion.proveedor,
            costo = empaque_decoracion.costo,
            cantidad = empaque_decoracion.cantidad,
            unidad_de_medida = empaque_decoracion.unidad_de_medida,
        });
        db.SaveChanges();
        db.Database.ExecuteSqlCommand("EXEC sp_calculos_empaque_decoracion");
        TempData["SuccessMessage"] = "¡Empaque o decoración agregado con éxito!";
        return RedirectToAction("empaques_decoraciones");
    }

    // Editar un empaque o decoración existente (GET id)
    [HttpGet]
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
            costo = ed.costo ?? 0m,
            cantidad = (int)ed.cantidad,
            unidad_de_medida = ed.unidad_de_medida,
            costo_por_cantidad = ed.costo_por_cantidad
        };

        //Obtén el listado de empaques y decoraciones
        var lista = db.tabla_empaques_decoraciones.Select(empdec => new EmpaqueDecoracion
        {
            id = empdec.id,
            nombre = empdec.nombre,
            marca = empdec.marca,
            presentacion = empdec.presentacion,
            proveedor = empdec.proveedor,
            costo = empdec.costo ?? 0m,
            cantidad = (int)empdec.cantidad,
            unidad_de_medida = empdec.unidad_de_medida,
            costo_por_cantidad = empdec.costo_por_cantidad
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
        // --- PARSE CORRECTO DEL COSTO ---
        string costoStr = Request.Form["costo"];
        if (!string.IsNullOrWhiteSpace(costoStr))
        {
            // Reemplaza la coma por punto para la conversión
            costoStr = costoStr.Replace(',', '.');
            if (decimal.TryParse(costoStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal costoDecimal))
            {
                empaque_decoracion.costo = costoDecimal;
            }
        }

        var errores = new List<string>();

        // Normalizar valores para comparación
        string nombre = empaque_decoracion.nombre?.Trim().ToLower() ?? "";
        string marca = empaque_decoracion.marca?.Trim().ToLower() ?? "";
        string presentacion = empaque_decoracion.presentacion?.Trim().ToLower() ?? "";
        string proveedor = empaque_decoracion.proveedor?.Trim().ToLower() ?? "";
        decimal? costo = empaque_decoracion.costo;
        int cantidad = empaque_decoracion.cantidad;

        // Duplicado exacto (todos los campos excepto ID)
        bool existeExacto = db.tabla_empaques_decoraciones.Any(empdec =>
            empdec.id != empaque_decoracion.id &&
            empdec.nombre.ToLower() == nombre &&
            empdec.marca.ToLower() == marca &&
            empdec.presentacion.ToLower() == presentacion &&
            empdec.proveedor.ToLower() == proveedor &&
            empdec.costo == costo &&
            empdec.cantidad == cantidad
        );
        if (existeExacto)
        {
            errores.Add("Ya existe un empaque o decoración con los mismos datos.");
        }

        // Validar campos obligatorios y valores numéricos
        if (string.IsNullOrWhiteSpace(empaque_decoracion.nombre) ||
            string.IsNullOrWhiteSpace(empaque_decoracion.marca) ||
            string.IsNullOrWhiteSpace(empaque_decoracion.presentacion) ||
            string.IsNullOrWhiteSpace(empaque_decoracion.proveedor) ||
            string.IsNullOrWhiteSpace(empaque_decoracion.cantidad.ToString()) ||
            string.IsNullOrWhiteSpace(empaque_decoracion.unidad_de_medida))
        {
            errores.Add("Todos los campos son obligatorios.");
        }

        if (empaque_decoracion.costo <= 0.99m)
            errores.Add("El costo debe ser mayor a ₡0.99.");

        if (empaque_decoracion.cantidad <= 0)
            errores.Add("La cantidad debe ser mayor a cero.");

        if (errores.Any())
        {
            ViewBag.Errores = errores;
            ViewBag.Editando = true;
            var lista = db.tabla_empaques_decoraciones.Select(empdec => new EmpaqueDecoracion
            {
                id = empdec.id,
                nombre = empdec.nombre,
                marca = empdec.marca,
                presentacion = empdec.presentacion,
                proveedor = empdec.proveedor,
                costo = empdec.costo ?? 0m,
                cantidad = (int)empdec.cantidad,
                unidad_de_medida = empdec.unidad_de_medida,
                costo_por_cantidad = empdec.costo_por_cantidad
            }).ToList();
            return View("empaques_decoraciones", new InsumosModel
            {
                EmpaqueDecoracionEditado = empaque_decoracion,
                EmpaquesDecoraciones = lista
            });
        }

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
        }
        db.SaveChanges();
        db.Database.ExecuteSqlCommand("EXEC sp_calculos_empaque_decoracion");
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

    /* Implementos */

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
                costo = i.costo ?? 0m,
                cantidad = (int)i.cantidad,
                unidad_de_medida = i.unidad_de_medida,
                costo_por_cantidad = i.costo_por_cantidad
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
        // --- PARSE CORRECTO DEL COSTO ---
        string costoStr = Request.Form["costo"];
        if (!string.IsNullOrWhiteSpace(costoStr))
        {
            costoStr = costoStr.Replace(',', '.');
            if (decimal.TryParse(costoStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal costoDecimal))
            {
                implemento.costo = costoDecimal;
            }
        }

        var errores = new List<string>();

        // Normalizar valores para comparación
        string nombre = implemento.nombre?.Trim().ToLower() ?? "";
        string marca = implemento.marca?.Trim().ToLower() ?? "";
        string presentacion = implemento.presentacion?.Trim().ToLower() ?? "";
        string proveedor = implemento.proveedor?.Trim().ToLower() ?? "";
        decimal? costo = implemento.costo;
        int cantidad = implemento.cantidad;
        
        // Duplicado exacto (todos los campos)
        if (db.tabla_implementos.Any(i =>
            i.nombre.ToLower() == nombre &&
            i.marca.ToLower() == marca &&
            i.presentacion.ToLower() == presentacion &&
            i.proveedor.ToLower() == proveedor &&
            i.costo == costo &&
            i.cantidad == cantidad))
        {
            errores.Add("Ya existe un implemento con los mismos datos.");
        }

        // Validar campos obligatorios y valores numéricos
        if (string.IsNullOrWhiteSpace(implemento.nombre) ||
            string.IsNullOrWhiteSpace(implemento.marca) ||
            string.IsNullOrWhiteSpace(implemento.presentacion) ||
            string.IsNullOrWhiteSpace(implemento.proveedor) ||
            string.IsNullOrWhiteSpace(implemento.cantidad.ToString()) ||
            string.IsNullOrWhiteSpace(implemento.unidad_de_medida))
        {
            errores.Add("Todos los campos son obligatorios.");
        }

        if (implemento.costo <= 0.99m)
            errores.Add("El costo debe ser mayor a ₡0.99.");

        if (implemento.cantidad <= 0)
            errores.Add("La cantidad debe ser mayor a cero.");

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
                costo = i.costo ?? 0m,
                cantidad = (int)i.cantidad,
                unidad_de_medida = i.unidad_de_medida,
                costo_por_cantidad = i.costo_por_cantidad
            }).ToList();
            return View("implementos", new InsumosModel
            {
                ImplementoEditado = implemento,
                Implementos = lista
            });
        }

        db.tabla_implementos.Add(new tabla_implementos
        {
            nombre = implemento.nombre,
            marca = implemento.marca,
            presentacion = implemento.presentacion,
            proveedor = implemento.proveedor,
            costo = implemento.costo,
            cantidad = implemento.cantidad,
            unidad_de_medida = implemento.unidad_de_medida,
        });
        db.SaveChanges();
        db.Database.ExecuteSqlCommand("EXEC sp_calculos_implemento");
        TempData["SuccessMessage"] = "¡Implemento agregado con éxito!";
        return RedirectToAction("implementos");
    }

    // Editar un implemento existente (GET id)
    [HttpGet]
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
            costo = i.costo ?? 0m,
            cantidad = (int)i.cantidad,
            unidad_de_medida = i.unidad_de_medida,
            costo_por_cantidad = i.costo_por_cantidad
        };

        //Obtén el listado de implementos
        var lista = db.tabla_implementos.Select(impl => new Implemento
        {
            id = impl.id,
            nombre = impl.nombre,
            marca = impl.marca,
            presentacion = impl.presentacion,
            proveedor = impl.proveedor,
            costo = impl.costo ?? 0m,
            cantidad = (int)impl.cantidad,
            unidad_de_medida = impl.unidad_de_medida,
            costo_por_cantidad = impl.costo_por_cantidad
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
        // --- PARSE CORRECTO DEL COSTO ---
        string costoStr = Request.Form["costo"];
        if (!string.IsNullOrWhiteSpace(costoStr))
        {
            // Reemplaza la coma por punto para la conversión
            costoStr = costoStr.Replace(',', '.');
            if (decimal.TryParse(costoStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal costoDecimal))
            {
                implemento.costo = costoDecimal;
            }
        }

        var errores = new List<string>();

        // Normalizar valores para comparación
        string nombre = implemento.nombre?.Trim().ToLower() ?? "";
        string marca = implemento.marca?.Trim().ToLower() ?? "";
        string presentacion = implemento.presentacion?.Trim().ToLower() ?? "";
        string proveedor = implemento.proveedor?.Trim().ToLower() ?? "";
        decimal? costo = implemento.costo;
        int cantidad = implemento.cantidad;
        // Duplicado exacto (todos los campos excepto ID)
        bool existeExacto = db.tabla_implementos.Any(impl =>
            impl.id != implemento.id &&
            impl.nombre.ToLower() == nombre &&
            impl.marca.ToLower() == marca &&
            impl.presentacion.ToLower() == presentacion &&
            impl.proveedor.ToLower() == proveedor &&
            impl.costo == costo &&
            impl.cantidad == cantidad
        );
        if (existeExacto)
        {
            errores.Add("Ya existe un implemento con los mismos datos.");
        }

        // Validar campos obligatorios y valores numéricos
        if (string.IsNullOrWhiteSpace(implemento.nombre) ||
            string.IsNullOrWhiteSpace(implemento.marca) ||
            string.IsNullOrWhiteSpace(implemento.presentacion) ||
            string.IsNullOrWhiteSpace(implemento.proveedor) ||
            string.IsNullOrWhiteSpace(implemento.cantidad.ToString()) ||
            string.IsNullOrWhiteSpace(implemento.unidad_de_medida))
        {
            errores.Add("Todos los campos son obligatorios.");
        }

        if (implemento.costo <= 0.99m)
            errores.Add("El costo debe ser mayor a ₡0.99.");

        if (implemento.cantidad <= 0)
            errores.Add("La cantidad debe ser mayor a cero.");

        if (errores.Any())
        {
            ViewBag.Errores = errores;
            ViewBag.Editando = true;
            var lista = db.tabla_implementos.Select(impl => new Implemento
            {
                id = impl.id,
                nombre = impl.nombre,
                marca = impl.marca,
                presentacion = impl.presentacion,
                proveedor = impl.proveedor,
                costo = impl.costo ?? 0m,
                cantidad = (int)impl.cantidad,
                unidad_de_medida = impl.unidad_de_medida,
                costo_por_cantidad = impl.costo_por_cantidad
            }).ToList();
            return View("implementos", new InsumosModel
            {
                ImplementoEditado = implemento,
                Implementos = lista
            });
        }

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
        }
        db.SaveChanges();
        db.Database.ExecuteSqlCommand("EXEC sp_calculos_implemento"); 
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

    /*  Suministros */

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
                costo = s.costo ?? 0m,
                cantidad = (int)s.cantidad,
                unidad_de_medida = s.unidad_de_medida,
                costo_por_cantidad = s.costo_por_cantidad
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
        // --- PARSE CORRECTO DEL COSTO ---
        string costoStr = Request.Form["costo"];
        if (!string.IsNullOrWhiteSpace(costoStr))
        {
            // Reemplaza la coma por punto para la conversión
            costoStr = costoStr.Replace(',', '.');
            if (decimal.TryParse(costoStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal costoDecimal))
            {
                suministro.costo = costoDecimal;
            }
        }

        var errores = new List<string>();

        // Normalizar valores para comparación
        string nombre = suministro.nombre?.Trim().ToLower() ?? "";
        string marca = suministro.marca?.Trim().ToLower() ?? "";
        string presentacion = suministro.presentacion?.Trim().ToLower() ?? "";
        string proveedor = suministro.proveedor?.Trim().ToLower() ?? "";
        decimal? costo = suministro.costo;
        int cantidad = suministro.cantidad;

        // Duplicado exacto (todos los campos)
        bool existeExacto = db.tabla_suministros.Any(s =>
            s.nombre.ToLower() == nombre &&
            s.marca.ToLower() == marca &&
            s.presentacion.ToLower() == presentacion &&
            s.proveedor.ToLower() == proveedor &&
            s.costo == costo &&
            s.cantidad == cantidad
        );
        if (existeExacto)
        {
            errores.Add("Ya existe un suministro con los mismos datos.");
        }

        // Validar campos obligatorios y valores numéricos
        if (string.IsNullOrWhiteSpace(suministro.nombre) ||
            string.IsNullOrWhiteSpace(suministro.marca) ||
            string.IsNullOrWhiteSpace(suministro.presentacion) ||
            string.IsNullOrWhiteSpace(suministro.proveedor) ||
            string.IsNullOrWhiteSpace(suministro.cantidad.ToString()) ||
            string.IsNullOrWhiteSpace(suministro.unidad_de_medida))
        {
            errores.Add("Todos los campos son obligatorios.");
        }

        if (suministro.costo <= 0.99m)
            errores.Add("El costo debe ser mayor a ₡0.99.");

        if (suministro.cantidad <= 0)
            errores.Add("La cantidad debe ser mayor a cero.");

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
                costo = s.costo ?? 0m,
                cantidad = (int)s.cantidad,
                unidad_de_medida = s.unidad_de_medida,
                costo_por_cantidad = s.costo_por_cantidad
            }).ToList();
            return View("suministros", new InsumosModel
            {
                SuministroEditado = suministro,
                Suministros = lista
            });
        }

        db.tabla_suministros.Add(new tabla_suministros
        {
            nombre = suministro.nombre,
            marca = suministro.marca,
            presentacion = suministro.presentacion,
            proveedor = suministro.proveedor,
            costo = suministro.costo,
            cantidad = suministro.cantidad,
            unidad_de_medida = suministro.unidad_de_medida,
        });
        db.SaveChanges();
        db.Database.ExecuteSqlCommand("EXEC sp_calculos_suministro");
        TempData["SuccessMessage"] = "¡Suministro agregado con éxito!";
        return RedirectToAction("suministros");
    }

    // Editar un suministro existente (GET id)
    [HttpGet]
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
            costo = s.costo ?? 0m,
            cantidad = (int)s.cantidad,
            unidad_de_medida = s.unidad_de_medida,
            costo_por_cantidad = s.costo_por_cantidad
        };

        // Obtén el listado de suministros
        var lista = db.tabla_suministros.Select(sumn => new Suministro
        {
            id = sumn.id,
            nombre = sumn.nombre,
            marca = sumn.marca,
            presentacion = sumn.presentacion,
            proveedor = sumn.proveedor,
            costo = sumn.costo ?? 0m,
            cantidad = (int)sumn.cantidad,
            unidad_de_medida = sumn.unidad_de_medida,
            costo_por_cantidad = sumn.costo_por_cantidad
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
        // --- PARSE CORRECTO DEL COSTO ---
        string costoStr = Request.Form["costo"];
        if (!string.IsNullOrWhiteSpace(costoStr))
        {
            // Reemplaza la coma por punto para la conversión
            costoStr = costoStr.Replace(',', '.');
            if (decimal.TryParse(costoStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal costoDecimal))
            {
                suministro.costo = costoDecimal;
            }
        }

        var errores = new List<string>();

        // Normalizar valores para comparación
        string nombre = suministro.nombre?.Trim().ToLower() ?? "";
        string marca = suministro.marca?.Trim().ToLower() ?? "";
        string presentacion = suministro.presentacion?.Trim().ToLower() ?? "";
        string proveedor = suministro.proveedor?.Trim().ToLower() ?? "";
        decimal? costo = suministro.costo;
        int cantidad = suministro.cantidad;
        
        // Duplicado exacto (todos los campos excepto ID)
        bool existeExacto = db.tabla_suministros.Any(sumn =>
            sumn.id != suministro.id &&
            sumn.nombre.ToLower() == nombre &&
            sumn.marca.ToLower() == marca &&
            sumn.presentacion.ToLower() == presentacion &&
            sumn.proveedor.ToLower() == proveedor &&
            sumn.costo == costo &&
            sumn.cantidad == cantidad
        );
        if (existeExacto)
        {
            errores.Add("Ya existe un suministro con los mismos datos.");
        }
        
        // Validar campos obligatorios y valores numéricos
        if (string.IsNullOrWhiteSpace(suministro.nombre) ||
            string.IsNullOrWhiteSpace(suministro.marca) ||
            string.IsNullOrWhiteSpace(suministro.presentacion) ||
            string.IsNullOrWhiteSpace(suministro.proveedor) ||
            string.IsNullOrWhiteSpace(suministro.cantidad.ToString()) ||
            string.IsNullOrWhiteSpace(suministro.unidad_de_medida))
        {
            errores.Add("Todos los campos son obligatorios.");
        }

        if (suministro.costo > 0.99m)
            errores.Add("El costo debe ser mayor a ₡0.99.");

        if (suministro.cantidad <= 0)
            errores.Add("La cantidad debe ser mayor a cero.");

        if (errores.Any())
        {
            ViewBag.Errores = errores;
            ViewBag.Editando = true;
            var lista = db.tabla_suministros.Select(sumn => new Suministro
            {
                id = sumn.id,
                nombre = sumn.nombre,
                marca = sumn.marca,
                presentacion = sumn.presentacion,
                proveedor = sumn.proveedor,
                costo = sumn.costo ?? 0m,
                cantidad = (int)sumn.cantidad,
                unidad_de_medida = sumn.unidad_de_medida,
                costo_por_cantidad = sumn.costo_por_cantidad
            }).ToList();
            return View("suministros", new InsumosModel
            {
                SuministroEditado = suministro,
                Suministros = lista
            });
        }

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
        }
        db.SaveChanges();
        db.Database.ExecuteSqlCommand("EXEC sp_calculos_suministro");
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

   /* Costos de Recetas */

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
                porcion = r.porcion,
                costo_total_receta = r.costo_total_receta,
                costo_por_porcion = r.costo_por_porcion,
                MateriasPrimasUtilizadas = db.costos_receta_materias_primas_utilizadas
                    .Where(mp => mp.id_receta == r.id)
                    .Select(mp => new MateriaPrimaUtilizada
                    {
                        id = mp.id,
                        id_materia_prima_utilizada = mp.id_materia_prima_utilizada ?? 0,
                        nombre = mp.tabla_materias_primas.nombre,
                        cantidad = mp.cantidad ?? 0,
                        unidad_de_medida = mp.unidad_de_medida,
                        costo_por_cantidad = mp.costo_por_cantidad ?? 0m,
                        total_costo = mp.total_costo ?? 0m
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
                        costo_por_cantidad = pp.costo_por_cantidad ?? 0m,
                        total_costo = pp.total_costo ?? 0m
                    }).ToList()
            }).ToList()
        };
        ViewBag.Search = search;
        ViewBag.MateriasPrimas = new SelectList(
                db.tabla_materias_primas.ToList()
                .Select(mp => new {
                    Value = mp.id,
                    Text = $"ID: {mp.id} | Nombre: {mp.nombre} | Costo por gramo con merma: ₡{mp.costo_por_gramo_con_merma}"
                }),
                "Value", "Text"
            );

        ViewBag.ProductosPreparados = new SelectList(
            db.tabla_productos_preparados.ToList()
                .Select(pp => new {
                    Value = pp.id,
                    Text = $"ID: {pp.id} | Nombre: {pp.nombre} | Costo por peso: ₡{pp.costo_por_peso}"
                }),
            "Value", "Text"
        );

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
            var idsMP = new HashSet<int>();
            for (int i = 0; i < receta.MateriasPrimasUtilizadas.Count; i++)
            {
                var mp = receta.MateriasPrimasUtilizadas[i];

                if (mp.id_materia_prima_utilizada == 0 && mp.cantidad == 0 && string.IsNullOrWhiteSpace(mp.unidad_de_medida))
                {
                    errores.Add($"Fila {i + 1} de Materias Primas: No puede dejar filas vacías.");
                    continue;
                }

                if (mp.id_materia_prima_utilizada == 0)
                    errores.Add($"Fila {i + 1} de Materias Primas: Debe seleccionar una materia prima.");

                if (mp.cantidad <= 0)
                    errores.Add($"Fila {i + 1} de Materias Primas: La cantidad debe ser mayor a cero.");

                if (mp.id_materia_prima_utilizada != 0)
                {
                    if (!idsMP.Add(mp.id_materia_prima_utilizada))
                        errores.Add($"Fila {i + 1} de Materias Primas: Materia prima repetida.");

                    // Validar existencia en BD
                    var materia_prima = db.tabla_materias_primas.FirstOrDefault(m => m.id == mp.id_materia_prima_utilizada);
                    if (materia_prima == null)
                    {
                        errores.Add($"Fila {i + 1} de Materias Primas: La materia prima seleccionada no existe en el sistema.");
                        continue;
                    }

                    // Si existe, asignar valores para cálculos
                    mp.nombre = materia_prima.nombre;
                    mp.costo_por_cantidad = materia_prima.costo_por_gramo_con_merma ?? 0m;
                    mp.total_costo = mp.cantidad * mp.costo_por_cantidad;
                }
            }
        }

        // Validar filas de Productos Preparados
        if (receta.ProductosPreparadosUtilizados != null)
        {
            var idsPP = new HashSet<int>();
            for (int i = 0; i < receta.ProductosPreparadosUtilizados.Count; i++)
            {
                var pp = receta.ProductosPreparadosUtilizados[i];

                if (pp.id_producto_preparado_utilizado == 0 && pp.cantidad == 0 && string.IsNullOrWhiteSpace(pp.unidad_de_medida))
                {
                    errores.Add($"Fila {i + 1} de Productos Preparados: No puede dejar filas vacías.");
                    continue;
                }

                if (pp.id_producto_preparado_utilizado == 0)
                    errores.Add($"Fila {i + 1} de Productos Preparados: Debe seleccionar un producto preparado.");

                if (pp.cantidad <= 0)
                    errores.Add($"Fila {i + 1} de Productos Preparados: La cantidad debe ser mayor a cero.");

                if (pp.id_producto_preparado_utilizado != 0)
                {
                    if (!idsPP.Add(pp.id_producto_preparado_utilizado))
                        errores.Add($"Fila {i + 1} de Productos Preparados: Producto preparado repetido.");

                    // Validar existencia en BD
                    var producto_preparado = db.tabla_productos_preparados.FirstOrDefault(p => p.id == pp.id_producto_preparado_utilizado);
                    if (producto_preparado == null)
                    {
                        errores.Add($"Fila {i + 1} de Productos Preparados: El producto preparado seleccionado no existe en el sistema.");
                        continue;
                    }

                    // Si existe, asignar valores para cálculos
                    pp.nombre = producto_preparado.nombre;
                    pp.costo_por_cantidad = producto_preparado.costo_por_peso ?? 0m;
                    pp.total_costo = pp.cantidad * pp.costo_por_cantidad;
                }
            }
        }

        if (errores.Any())
        {
            ViewBag.Errores = errores;
            ViewBag.MateriasPrimas = new SelectList(
                db.tabla_materias_primas.ToList()
                .Select(mp => new {
                    Value = mp.id,
                    Text = $"ID: {mp.id} | Nombre: {mp.nombre} | Costo por gramo con merma: ₡{mp.costo_por_gramo_con_merma}"
                }),
                "Value", "Text"
            );

            ViewBag.ProductosPreparados = new SelectList(
                db.tabla_productos_preparados.ToList()
                    .Select(pp => new {
                        Value = pp.id,
                        Text = $"ID: {pp.id} | Nombre: {pp.nombre} | Costo por peso: ₡{pp.costo_por_peso}"
                    }),
                "Value", "Text"
            );

            var lista = db.tabla_costos_recetas.Select(rec => new Receta
            {
                id = rec.id,
                nombre = rec.nombre,
                porcion = rec.porcion,
                costo_total_receta = rec.costo_total_receta,
                costo_por_porcion = rec.costo_por_porcion,

                MateriasPrimasUtilizadas = db.costos_receta_materias_primas_utilizadas
                .Where(mp => mp.id_receta == rec.id)
                .Select(mp => new MateriaPrimaUtilizada
                {
                    id = mp.id,
                    id_materia_prima_utilizada = mp.id_materia_prima_utilizada ?? 0,
                    nombre = mp.tabla_materias_primas.nombre,
                    cantidad = mp.cantidad ?? 0,
                    unidad_de_medida = mp.unidad_de_medida,
                    costo_por_cantidad = mp.costo_por_cantidad ?? 0m,
                    total_costo = mp.total_costo ?? 0m
                }).ToList(),

                ProductosPreparadosUtilizados = db.costos_receta_productos_preparados_utilizados
                .Where(pp => pp.id_receta == rec.id)
                .Select(pp => new ProductoPreparadoUtilizado
                {
                    id = pp.id,
                    id_producto_preparado_utilizado = pp.id_producto_preparado_utilizado ?? 0 ,
                    nombre = pp.tabla_productos_preparados.nombre,
                    cantidad = pp.cantidad ?? 0,
                    unidad_de_medida = pp.unidad_de_medida,
                    costo_por_cantidad = pp.costo_por_cantidad ?? 0m,
                    total_costo = pp.total_costo ?? 0m
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
                var materiaPrima = db.tabla_materias_primas.FirstOrDefault(m => m.id == mp.id_materia_prima_utilizada);
                if (materiaPrima != null)
                {
                    mp.id_materia_prima_utilizada = materiaPrima.id;
                    mp.costo_por_cantidad = materiaPrima.costo_por_gramo_con_merma ?? 0m;
                    mp.total_costo = mp.cantidad * mp.costo_por_cantidad;
                    costoTotalReceta += mp.total_costo;
                }
            }
        }

        if (receta.ProductosPreparadosUtilizados != null)
        {
            foreach (var pp in receta.ProductosPreparadosUtilizados)
            {
                var productoPreparado = db.tabla_productos_preparados.FirstOrDefault(p => p.id == pp.id_producto_preparado_utilizado);
                if (productoPreparado != null)
                {
                    pp.id_producto_preparado_utilizado = productoPreparado.id;
                    pp.costo_por_cantidad = productoPreparado.costo_por_peso ?? 0m;
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
        TempData["SuccessMessage"] = "¡Receta agregada con éxito!";
        return RedirectToAction("costos_recetas");
    }

    // Editar receta existente (GET id)
    [HttpGet]
    public ActionResult EditarReceta(int id)
    {
        var r = db.tabla_costos_recetas.Find(id);
        if (r == null) return HttpNotFound();

        // Receta a editar
        var receta = new Receta
        {
            id = r.id,
            nombre = r.nombre,
            porcion = r.porcion,
            costo_total_receta = r.costo_total_receta,
            costo_por_porcion = r.costo_por_porcion,

            MateriasPrimasUtilizadas = db.costos_receta_materias_primas_utilizadas
                .Where(mp => mp.id_receta == r.id)
                .Select(mp => new MateriaPrimaUtilizada
                {
                    id = mp.id,
                    id_materia_prima_utilizada = mp.id_materia_prima_utilizada ?? 0,
                    nombre = mp.tabla_materias_primas.nombre,
                    cantidad = mp.cantidad ?? 0,
                    unidad_de_medida = mp.unidad_de_medida,
                    costo_por_cantidad = mp.costo_por_cantidad ?? 0m,
                    total_costo = mp.total_costo ?? 0m
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
                    costo_por_cantidad = pp.costo_por_cantidad ?? 0m,
                    total_costo = pp.total_costo ?? 0m
                }).ToList()
        };

        // Listado completo de recetas para mostrar en la tabla
        var lista = db.tabla_costos_recetas.Select(rec => new Receta
        {
            id = rec.id,
            nombre = rec.nombre,
            porcion = rec.porcion,
            costo_total_receta = rec.costo_total_receta,
            costo_por_porcion = rec.costo_por_porcion,

            MateriasPrimasUtilizadas = db.costos_receta_materias_primas_utilizadas
                .Where(mp => mp.id_receta == rec.id)
                .Select(mp => new MateriaPrimaUtilizada
                {
                    id = mp.id,
                    id_materia_prima_utilizada = mp.id_materia_prima_utilizada ?? 0,
                    nombre = mp.tabla_materias_primas.nombre,
                    cantidad = mp.cantidad ?? 0,
                    unidad_de_medida = mp.unidad_de_medida,
                    costo_por_cantidad = mp.costo_por_cantidad ?? 0m,
                    total_costo = mp.total_costo ?? 0m
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
                    costo_por_cantidad = pp.costo_por_cantidad ?? 0m,
                    total_costo = pp.total_costo ?? 0m
                }).ToList()
        }).ToList();

        ViewBag.MateriasPrimas = new SelectList(
                db.tabla_materias_primas.ToList()
                .Select(mp => new {
                    Value = mp.id,
                    Text = $"ID: {mp.id} | Nombre: {mp.nombre} | Costo por gramo con merma: ₡{mp.costo_por_gramo_con_merma}"
                }),
                "Value", "Text"
            );

        ViewBag.ProductosPreparados = new SelectList(
            db.tabla_productos_preparados.ToList()
                .Select(pp => new {
                    Value = pp.id,
                    Text = $"ID: {pp.id} | Nombre: {pp.nombre} | Costo por peso: ₡{pp.costo_por_peso}"
                }),
            "Value", "Text"
        );

        ViewBag.Editando = true;
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
            var idsMP = new HashSet<int>();
            for (int i = 0; i < receta.MateriasPrimasUtilizadas.Count; i++)
            {
                var mp = receta.MateriasPrimasUtilizadas[i];

                if (mp.id_materia_prima_utilizada == 0 && mp.cantidad == 0 && string.IsNullOrWhiteSpace(mp.unidad_de_medida))
                {
                    errores.Add($"Fila {i + 1} de Materias Primas: No puede dejar filas vacías.");
                    continue;
                }

                if (mp.id_materia_prima_utilizada == 0)
                    errores.Add($"Fila {i + 1} de Materias Primas: Debe seleccionar una materia prima.");

                if (mp.cantidad <= 0)
                    errores.Add($"Fila {i + 1} de Materias Primas: La cantidad debe ser mayor a cero.");

                if (mp.id_materia_prima_utilizada != 0)
                {
                    if (!idsMP.Add(mp.id_materia_prima_utilizada))
                        errores.Add($"Fila {i + 1} de Materias Primas: Materia prima repetida.");

                    // Validar existencia en BD
                    var materia_prima = db.tabla_materias_primas.FirstOrDefault(m => m.id == mp.id_materia_prima_utilizada);
                    if (materia_prima == null)
                    {
                        errores.Add($"Fila {i + 1} de Materias Primas: La materia prima seleccionada no existe en el sistema.");
                        continue;
                    }

                    // Si existe, asignar valores para cálculos
                    mp.nombre = materia_prima.nombre;
                    mp.costo_por_cantidad = materia_prima.costo_por_gramo_con_merma ?? 0m;
                    mp.total_costo = mp.cantidad * mp.costo_por_cantidad;
                }
            }
        }

        // Validar filas de Productos Preparados
        if (receta.ProductosPreparadosUtilizados != null)
        {
            var idsPP = new HashSet<int>();
            for (int i = 0; i < receta.ProductosPreparadosUtilizados.Count; i++)
            {
                var pp = receta.ProductosPreparadosUtilizados[i];

                if (pp.id_producto_preparado_utilizado == 0 && pp.cantidad == 0 && string.IsNullOrWhiteSpace(pp.unidad_de_medida))
                {
                    errores.Add($"Fila {i + 1} de Productos Preparados: No puede dejar filas vacías.");
                    continue;
                }

                if (pp.id_producto_preparado_utilizado == 0)
                    errores.Add($"Fila {i + 1} de Productos Preparados: Debe seleccionar un producto preparado.");

                if (pp.cantidad <= 0)
                    errores.Add($"Fila {i + 1} de Productos Preparados: La cantidad debe ser mayor a cero.");

                if (pp.id_producto_preparado_utilizado != 0)
                {
                    if (!idsPP.Add(pp.id_producto_preparado_utilizado))
                        errores.Add($"Fila {i + 1} de Productos Preparados: Producto preparado repetido.");

                    // Validar existencia en BD
                    var producto_preparado = db.tabla_productos_preparados.FirstOrDefault(p => p.id == pp.id_producto_preparado_utilizado);
                    if (producto_preparado == null)
                    {
                        errores.Add($"Fila {i + 1} de Productos Preparados: El producto preparado seleccionado no existe en el sistema.");
                        continue;
                    }

                    // Si existe, asignar valores para cálculos
                    pp.nombre = producto_preparado.nombre;
                    pp.costo_por_cantidad = producto_preparado.costo_por_peso ?? 0m;
                    pp.total_costo = pp.cantidad * pp.costo_por_cantidad;
                }
            }
        }

        if (errores.Any())
        {
            ViewBag.Errores = errores;
            ViewBag.Editando = true;
            ViewBag.MateriasPrimas = new SelectList(
                db.tabla_materias_primas.ToList()
                .Select(mp => new {
                    Value = mp.id,
                    Text = $"ID: {mp.id} | Nombre: {mp.nombre} | Costo por gramo con merma: ₡{mp.costo_por_gramo_con_merma}"
                }),
                "Value", "Text"
            );

            ViewBag.ProductosPreparados = new SelectList(
                db.tabla_productos_preparados.ToList()
                    .Select(pp => new {
                        Value = pp.id,
                        Text = $"ID: {pp.id} | Nombre: {pp.nombre} | Costo por peso: ₡{pp.costo_por_peso}"
                    }),
                "Value", "Text"
            );

            var lista = db.tabla_costos_recetas.Select(rec => new Receta
            {
                id = rec.id,
                nombre = rec.nombre,
                porcion = rec.porcion,
                costo_total_receta = rec.costo_total_receta,
                costo_por_porcion = rec.costo_por_porcion,

                MateriasPrimasUtilizadas = db.costos_receta_materias_primas_utilizadas
                .Where(mp => mp.id_receta == rec.id)
                .Select(mp => new MateriaPrimaUtilizada
                {
                    id = mp.id,
                    id_materia_prima_utilizada = mp.id_materia_prima_utilizada ?? 0,
                    nombre = mp.tabla_materias_primas.nombre,
                    cantidad = mp.cantidad ?? 0,
                    unidad_de_medida = mp.unidad_de_medida,
                    costo_por_cantidad = mp.costo_por_cantidad ?? 0m,
                    total_costo = mp.total_costo ?? 0m
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
                    costo_por_cantidad = pp.costo_por_cantidad ?? 0m,
                    total_costo = pp.total_costo ?? 0m
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
                var materiaPrima = db.tabla_materias_primas.FirstOrDefault(m => m.id == mp.id_materia_prima_utilizada);
                if (materiaPrima != null)
                {
                    mp.id_materia_prima_utilizada = materiaPrima.id;
                    mp.costo_por_cantidad = materiaPrima.costo_por_gramo_con_merma ?? 0m;
                    mp.total_costo = mp.cantidad * mp.costo_por_cantidad;
                    costoTotalReceta += mp.total_costo;
                }
            }
        }

        if (receta.ProductosPreparadosUtilizados != null)
        {
            foreach (var pp in receta.ProductosPreparadosUtilizados)
            {
                var productoPreparado = db.tabla_productos_preparados.FirstOrDefault(p => p.id == pp.id_producto_preparado_utilizado);
                if (productoPreparado != null)
                {
                    pp.id_producto_preparado_utilizado = productoPreparado.id;
                    pp.costo_por_cantidad = productoPreparado.costo_por_peso ?? 0m;
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

    /* Precios Finales Sugeridos de Productos Finales */

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
                pf.factura_total.ToString().Contains(search) ||
                pf.factura_por_insumo.ToString().Contains(search) ||
                pf.costo_total_de_impresion_de_factura.ToString().Contains(search) ||
                pf.costo_total_empaque_decoracion_implemento_suministro_por_porcentaje_de_ganancia.ToString().Contains(search) ||
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
                costo_total_receta = pf.costo_total_receta ?? 0m,
                margen_de_utilidad = pf.margen_de_utilidad,
                costo_sin_margen_de_utilidad = pf.costo_sin_margen_de_utilidad ?? 0,
                costo_con_margen_de_utilidad = pf.costo_con_margen_de_utilidad ?? 0m,
                costo_empaque_decoracion_utilizado = pf.costo_empaque_decoracion_utilizado ?? 0m,
                costo_implemento_utilizado = pf.costo_implemento_utilizado ?? 0m,
                costo_suministro_utilizado = pf.costo_suministro_utilizado ?? 0m,
                costo_de_impresion_de_factura_por_insumo = pf.costo_de_impresion_de_factura_por_insumo ?? 0m,
                costo_total_de_impresion_de_factura = pf.costo_total_de_impresion_de_factura ?? 0m,
                factura_total = pf.factura_total ?? 0m,
                factura_por_insumo = pf.factura_por_insumo ?? 0m,
                iva = pf.iva ?? 0m,
                impuesto_de_servicio = pf.impuesto_de_servicio ?? 0m,
                envio = pf.envio ?? 0m,
                plataforma_de_envio = pf.plataforma_de_envio,
                precio_final_sugerido = pf.precio_final_sugerido ?? 0m,

                EmpaquesDecoracionesUtilizados = db.precios_empaques_decoraciones_utilizados
                    .Where(ed => ed.id_precio_final_sugerido == pf.id)
                    .Select(ed => new EmpaqueDecoracionUtilizado
                    {
                        id = ed.id,
                        id_empaque_decoracion_utilizado = ed.id_empaque_decoracion_utilizado ?? 0,
                        nombre = ed.tabla_empaques_decoraciones.nombre,
                        cantidad = ed.cantidad ?? 0,
                        unidad_de_medida = ed.unidad_de_medida,
                        costo_por_cantidad = ed.costo_por_cantidad ?? 0m,
                        total_costo = ed.total_costo ?? 0m
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
                        costo_por_cantidad = i.costo_por_cantidad ?? 0m,
                        total_costo = i.total_costo ?? 0m
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
                        costo_por_cantidad = s.costo_por_cantidad ?? 0m,
                        total_costo = s.total_costo ?? 0m,
                        es_impresion_de_facturas = s.es_impresion_de_facturas ?? false
                    }).ToList()
            }).ToList()
        };
        ViewBag.Search = search;
        ViewBag.Recetas = new SelectList(db.tabla_costos_recetas.ToList(), "nombre", "nombre");
        ViewBag.EmpaquesDecoraciones = new SelectList(
            db.tabla_empaques_decoraciones.ToList()
            .Select(ed => new {
            Value = ed.id,
            Text = $"ID: {ed.id} | Nombre: {ed.nombre} | Costo por cantidad: ₡{ed.costo_por_cantidad}"
            }),
            "Value", "Text"
        );

        ViewBag.Implementos = new SelectList(
            db.tabla_implementos.ToList()
                .Select(i => new {
                    Value = i.id,
                    Text = $"ID: {i.id} | Nombre: {i.nombre} | Costo por cantidad: ₡{i.costo_por_cantidad}"
                }),
            "Value", "Text"
        );

        ViewBag.Suministros = new SelectList(
            db.tabla_suministros.ToList()
                .Select(s => new {
                    Value = s.id,
                    Text = $"ID: {s.id} | Nombre: {s.nombre} | Costo por cantidad: ₡{s.costo_por_cantidad}"
                }),
            "Value", "Text"
        ); 
        return View(producto_final);
    }

    // Crear un nuevo producto final
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CrearProductoFinal(ProductoFinal producto_final)
    {
        // Asignar el valor del checkbox manualmente para cada suministro
        if (producto_final.SuministrosUtilizados != null)
        {
            for (int i = 0; i < producto_final.SuministrosUtilizados.Count; i++)
            {
                var key = $"SuministrosUtilizados[{i}].es_impresion_de_facturas";
                producto_final.SuministrosUtilizados[i].es_impresion_de_facturas = Request.Form[key] == "on";
            }
        }

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

        // Validar detalles (Empaques, Implementos, suministros)
        if (producto_final.EmpaquesDecoracionesUtilizados != null)
        {
            var idsEmpaques = new HashSet<int>();
            for (int i = 0; i < producto_final.EmpaquesDecoracionesUtilizados.Count; i++)
            {
                var ed = producto_final.EmpaquesDecoracionesUtilizados[i];

                if ((ed.id_empaque_decoracion_utilizado == 0) && ed.cantidad == 0 && string.IsNullOrWhiteSpace(ed.unidad_de_medida))
                {
                    errores.Add($"Fila {i + 1} de Empaques/Decoraciones: No puede dejar filas vacías.");
                    continue;
                }
                if (ed.id_empaque_decoracion_utilizado == 0)
                    errores.Add($"Fila {i + 1} de Empaques/Decoraciones: Debe seleccionar un empaque/decoración.");
                if (ed.cantidad <= 0)
                    errores.Add($"Fila {i + 1} de Empaques/Decoraciones: La cantidad debe ser mayor a cero.");
                if (string.IsNullOrWhiteSpace(ed.unidad_de_medida))
                    errores.Add($"Fila {i + 1} de Empaques/Decoraciones: La unidad de medida es obligatoria.");

                if (ed.id_empaque_decoracion_utilizado != 0)
                {
                    if (!idsEmpaques.Add(ed.id_empaque_decoracion_utilizado))
                        errores.Add($"Fila {i + 1} de Empaques/Decoraciones: Empaque/decoración repetido.");

                    var empaque = db.tabla_empaques_decoraciones.FirstOrDefault(x => x.id == ed.id_empaque_decoracion_utilizado);
                    if (empaque == null)
                    {
                        errores.Add($"Fila {i + 1} de Empaques/Decoraciones: El empaque/decoración seleccionado no existe.");
                        continue;
                    }
                    ed.nombre = empaque.nombre;
                    ed.costo_por_cantidad = empaque.costo_por_cantidad ?? 0m;
                    ed.total_costo = ed.cantidad * ed.costo_por_cantidad;
                }
            }
        }

        if (producto_final.ImplementosUtilizados != null)
        {
            var idsImplementos = new HashSet<int>();
            for (int i = 0; i < producto_final.ImplementosUtilizados.Count; i++)
            {
                var impl = producto_final.ImplementosUtilizados[i];

                if ((impl.id_implemento_utilizado == 0) && impl.cantidad == 0 && string.IsNullOrWhiteSpace(impl.unidad_de_medida))
                {
                    errores.Add($"Fila {i + 1} de Implementos: No puede dejar filas vacías.");
                    continue;
                }
                if (impl.id_implemento_utilizado == 0)
                    errores.Add($"Fila {i + 1} de Implementos: Debe seleccionar un implemento.");
                if (impl.cantidad <= 0)
                    errores.Add($"Fila {i + 1} de Implementos: La cantidad debe ser mayor a cero.");
                if (string.IsNullOrWhiteSpace(impl.unidad_de_medida))
                    errores.Add($"Fila {i + 1} de Implementos: La unidad de medida es obligatoria.");

                if (impl.id_implemento_utilizado != 0)
                {
                    if (!idsImplementos.Add(impl.id_implemento_utilizado))
                        errores.Add($"Fila {i + 1} de Implementos: Implemento repetido.");

                    var implemento = db.tabla_implementos.FirstOrDefault(x => x.id == impl.id_implemento_utilizado);
                    if (implemento == null)
                    {
                        errores.Add($"Fila {i + 1} de Implementos: El implemento seleccionado no existe.");
                        continue;
                    }
                    impl.nombre = implemento.nombre;
                    impl.costo_por_cantidad = implemento.costo_por_cantidad ?? 0m;
                    impl.total_costo = impl.cantidad * impl.costo_por_cantidad;
                }
            }
        }

        if (producto_final.SuministrosUtilizados != null)
        {
            var idsSuministros = new HashSet<int>();
            for (int i = 0; i < producto_final.SuministrosUtilizados.Count; i++)
            {
                var sumn = producto_final.SuministrosUtilizados[i];

                if ((sumn.id_suministro_utilizado == 0) && sumn.cantidad == 0 && string.IsNullOrWhiteSpace(sumn.unidad_de_medida))
                {
                    errores.Add($"Fila {i + 1} de Suministros: No puede dejar filas vacías.");
                    continue;
                }
                if (sumn.id_suministro_utilizado == 0)
                    errores.Add($"Fila {i + 1} de Suministros: Debe seleccionar un suministro.");
                if (sumn.cantidad <= 0)
                    errores.Add($"Fila {i + 1} de Suministros: La cantidad debe ser mayor a cero.");
                if (string.IsNullOrWhiteSpace(sumn.unidad_de_medida))
                    errores.Add($"Fila {i + 1} de Suministros: La unidad de medida es obligatoria.");

                if (sumn.id_suministro_utilizado != 0)
                {
                    if (!idsSuministros.Add(sumn.id_suministro_utilizado))
                        errores.Add($"Fila {i + 1} de Suministros: Suministro repetido.");

                    var suministro = db.tabla_suministros.FirstOrDefault(x => x.id == sumn.id_suministro_utilizado);
                    if (suministro == null)
                    {
                        errores.Add($"Fila {i + 1} de Suministros: El suministro seleccionado no existe.");
                        continue;
                    }
                    sumn.nombre = suministro.nombre;
                    sumn.costo_por_cantidad = suministro.costo_por_cantidad ?? 0m;
                    sumn.total_costo = sumn.cantidad * sumn.costo_por_cantidad;
                }
            }
        }

        if (errores.Any())
        {
            ViewBag.Errores = errores;
            ViewBag.Recetas = new SelectList(db.tabla_costos_recetas.ToList(), "nombre", "nombre");
            ViewBag.EmpaquesDecoraciones = new SelectList(
                db.tabla_empaques_decoraciones.ToList()
                .Select(ed => new {
                    Value = ed.id,
                    Text = $"ID: {ed.id} | Nombre: {ed.nombre} | Costo por cantidad: ₡{ed.costo_por_cantidad}"
                }),
                "Value", "Text"
            );

            ViewBag.Implementos = new SelectList(
                db.tabla_implementos.ToList()
                    .Select(i => new {
                        Value = i.id,
                        Text = $"ID: {i.id} | Nombre: {i.nombre} | Costo por cantidad: ₡{i.costo_por_cantidad}"
                    }),
                "Value", "Text"
            );

            ViewBag.Suministros = new SelectList(
                db.tabla_suministros.ToList()
                    .Select(s => new {
                        Value = s.id,
                        Text = $"ID: {s.id} | Nombre: {s.nombre} | Costo por cantidad: ₡{s.costo_por_cantidad}"
                    }),
                "Value", "Text"
            ); 
            
            var productosFinales = db.tabla_precios_finales_sugeridos.ToList().Select(pf => new ProductoFinal
            {
                id = pf.id,
                nombre_receta = pf.nombre_receta,
                costo_total_receta = pf.costo_total_receta ?? 0m,
                margen_de_utilidad = pf.margen_de_utilidad,
                costo_sin_margen_de_utilidad = pf.costo_sin_margen_de_utilidad ?? 0,
                costo_con_margen_de_utilidad = pf.costo_con_margen_de_utilidad ?? 0m,
                costo_empaque_decoracion_utilizado = pf.costo_empaque_decoracion_utilizado ?? 0m,
                costo_implemento_utilizado = pf.costo_implemento_utilizado ?? 0m,
                costo_suministro_utilizado = pf.costo_suministro_utilizado ?? 0m,
                factura_total = pf.factura_total ?? 0m,
                factura_por_insumo = pf.factura_por_insumo ?? 0m,
                costo_total_de_impresion_de_factura = pf.costo_total_de_impresion_de_factura ?? 0m,
                iva = pf.iva ?? 0m,
                impuesto_de_servicio = pf.impuesto_de_servicio ?? 0m,
                envio = pf.envio ?? 0m,
                plataforma_de_envio = pf.plataforma_de_envio,
                precio_final_sugerido = pf.precio_final_sugerido ?? 0m,
            }).ToList();
            return View("precio_final", new InsumosModel
            {
                ProductoFinalEditado = producto_final,
                ProductosFinales = productosFinales
            });
        }

        // Calcula el costo de la receta desde la base de datos
        decimal costoReceta = receta?.costo_total_receta ?? 0;
        decimal margenUtilidad = producto_final.margen_de_utilidad;
        decimal costoSinUtilidad = 100m - margenUtilidad;
        decimal costoConUtilidad = costoReceta / (costoSinUtilidad / 100m);

        // Suministros normales (excluyendo el de impresión)
        var suministrosNormales = producto_final.SuministrosUtilizados?
            .Where(s => !s.es_impresion_de_facturas)
            .ToList() ?? new List<SuministroUtilizado>();

        decimal totalSuministros = suministrosNormales.Sum(s => s.total_costo);

        // Buscar el suministro de impresión por el campo booleano
        var suministroImpresion = producto_final.SuministrosUtilizados?
            .FirstOrDefault(s => s.es_impresion_de_facturas);

        decimal costoImpresionFacturaPorInsumo = 0;
        decimal costoTotalImpresionFactura = 0;
        decimal porcion = receta?.porcion ?? 1;

        if (suministroImpresion != null)
        {
            costoImpresionFacturaPorInsumo = suministroImpresion.costo_por_cantidad / 20m;
            costoTotalImpresionFactura = porcion * costoImpresionFacturaPorInsumo;
        }

        // Suma de costos por cantidad (multiplicando por la cantidad)
        decimal sumaEmpaquesPorCantidad = producto_final.EmpaquesDecoracionesUtilizados?.Sum(e => e.costo_por_cantidad * e.cantidad) ?? 0;
        decimal sumaImplementosPorCantidad = producto_final.ImplementosUtilizados?.Sum(i => i.costo_por_cantidad * i.cantidad) ?? 0;
        decimal sumaSuministrosPorCantidad = suministrosNormales.Sum(s => s.costo_por_cantidad * s.cantidad);

        // Factura por insumo: suma de todos los costos individuales + impresión por insumo
        decimal facturaPorInsumo = sumaEmpaquesPorCantidad + sumaImplementosPorCantidad + sumaSuministrosPorCantidad + costoImpresionFacturaPorInsumo;

        // Factura total: suma de todos los totales + impresión total
        decimal facturaTotal = sumaEmpaquesPorCantidad + sumaImplementosPorCantidad + sumaSuministrosPorCantidad + costoTotalImpresionFactura;

        // Total insumos con porcentaje de ganancia
        decimal totalInsumosConGanancia = facturaTotal * 1.10m;

        // IVA y Servicio
        // Obtén los porcentajes digitados desde el formulario
        decimal ivaPorcentaje = 0;
        decimal servicioPorcentaje = 0;
        decimal.TryParse(Request.Form["iva_porcentaje"], out ivaPorcentaje);
        decimal.TryParse(Request.Form["servicio_porcentaje"], out servicioPorcentaje);

        decimal baseImpuestos = costoConUtilidad + totalInsumosConGanancia;
        decimal iva = baseImpuestos * (ivaPorcentaje / 100m);
        decimal servicio = baseImpuestos * (servicioPorcentaje / 100m);

        // Envío
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

        // Precio final sugerido
        decimal precioFinal = baseImpuestos + iva + servicio + envio;

        var precio = new tabla_precios_finales_sugeridos
        {
            id_receta = receta.id,
            nombre_receta = producto_final.nombre_receta,
            costo_total_receta = costoReceta,
            margen_de_utilidad = margenUtilidad,
            costo_sin_margen_de_utilidad = costoReceta,
            costo_con_margen_de_utilidad = costoConUtilidad,
            costo_empaque_decoracion_utilizado = sumaEmpaquesPorCantidad,
            costo_implemento_utilizado = sumaImplementosPorCantidad,
            costo_suministro_utilizado = totalSuministros,
            costo_de_impresion_de_factura_por_insumo = costoImpresionFacturaPorInsumo,
            costo_total_de_impresion_de_factura = costoTotalImpresionFactura,
            costo_total_empaque_decoracion_implemento_suministro_por_porcentaje_de_ganancia = totalInsumosConGanancia,
            factura_por_insumo = facturaPorInsumo,
            factura_total = facturaTotal,
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
                    total_costo = s.total_costo,
                    es_impresion_de_facturas = s.es_impresion_de_facturas
                });
            }
        }
        db.SaveChanges();
        TempData["SuccessMessage"] = "¡Producto final agregado con éxito!";
        return RedirectToAction("precio_final");
    }

    //Editar un producto final existente (GET id)
    [HttpGet]
    public ActionResult EditarProductoFinal(int id)
    {
        var pf = db.tabla_precios_finales_sugeridos.Find(id);
        if (pf == null) return HttpNotFound();

        var producto_final = new ProductoFinal
        {
            id = pf.id,
            nombre_receta = pf.nombre_receta,
            costo_total_receta = pf.costo_total_receta ?? 0m,
            margen_de_utilidad = pf.margen_de_utilidad,
            costo_sin_margen_de_utilidad = pf.costo_sin_margen_de_utilidad ?? 0,
            costo_con_margen_de_utilidad = pf.costo_con_margen_de_utilidad ?? 0m,
            costo_empaque_decoracion_utilizado = pf.costo_empaque_decoracion_utilizado ?? 0m,
            costo_implemento_utilizado = pf.costo_implemento_utilizado ?? 0m,
            costo_suministro_utilizado = pf.costo_suministro_utilizado ?? 0m,
            costo_de_impresion_de_factura_por_insumo = pf.costo_de_impresion_de_factura_por_insumo ?? 0m,
            costo_total_de_impresion_de_factura = pf.costo_total_de_impresion_de_factura ?? 0m,
            factura_total = pf.factura_total ?? 0m,
            factura_por_insumo = pf.factura_por_insumo ?? 0m,
            iva = pf.iva ?? 0m,
            impuesto_de_servicio = pf.impuesto_de_servicio ?? 0m,
            envio = pf.envio ?? 0m,
            plataforma_de_envio = pf.plataforma_de_envio,
            precio_final_sugerido = pf.precio_final_sugerido ?? 0m,

            EmpaquesDecoracionesUtilizados = db.precios_empaques_decoraciones_utilizados
                .Where(ed => ed.id_precio_final_sugerido == pf.id)
                .Select(ed => new EmpaqueDecoracionUtilizado
                {
                    id = ed.id,
                    id_empaque_decoracion_utilizado = ed.id_empaque_decoracion_utilizado ?? 0,
                    nombre = ed.tabla_empaques_decoraciones.nombre,
                    cantidad = ed.cantidad ?? 0,
                    unidad_de_medida = ed.unidad_de_medida,
                    costo_por_cantidad = ed.costo_por_cantidad ?? 0m,
                    total_costo = ed.total_costo ?? 0m
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
                    costo_por_cantidad = i.costo_por_cantidad ?? 0m,
                    total_costo = i.total_costo ?? 0m
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
                    costo_por_cantidad = s.costo_por_cantidad ?? 0m,
                    total_costo = s.total_costo ?? 0m,
                    es_impresion_de_facturas = s.es_impresion_de_facturas ?? false
                }).ToList()
        };

        var productosFinales = db.tabla_precios_finales_sugeridos.ToList().Select(prodfinal => new ProductoFinal
        {
            id = prodfinal.id,
            nombre_receta = prodfinal.nombre_receta,
            costo_total_receta = prodfinal.costo_total_receta ?? 0m,
            margen_de_utilidad = prodfinal.margen_de_utilidad,
            costo_sin_margen_de_utilidad = prodfinal.costo_sin_margen_de_utilidad ?? 0,
            costo_con_margen_de_utilidad = prodfinal.costo_con_margen_de_utilidad ?? 0m,
            costo_empaque_decoracion_utilizado = prodfinal.costo_empaque_decoracion_utilizado ?? 0m,
            costo_implemento_utilizado = prodfinal.costo_implemento_utilizado ?? 0m,
            costo_suministro_utilizado = prodfinal.costo_suministro_utilizado ?? 0m,
            costo_de_impresion_de_factura_por_insumo = prodfinal.costo_de_impresion_de_factura_por_insumo ?? 0m,
            costo_total_de_impresion_de_factura = prodfinal.costo_total_de_impresion_de_factura ?? 0m,
            factura_total = prodfinal.factura_total ?? 0m,
            factura_por_insumo = prodfinal.factura_por_insumo ?? 0m,
            iva = prodfinal.iva ?? 0m,
            impuesto_de_servicio = prodfinal.impuesto_de_servicio ?? 0m,
            envio = prodfinal.envio ?? 0m,
            plataforma_de_envio = prodfinal.plataforma_de_envio,
            precio_final_sugerido = prodfinal.precio_final_sugerido ?? 0m,

            EmpaquesDecoracionesUtilizados = db.precios_empaques_decoraciones_utilizados
                .Where(ed => ed.id_precio_final_sugerido == prodfinal.id)
                .Select(ed => new EmpaqueDecoracionUtilizado
                {
                    id = ed.id,
                    id_empaque_decoracion_utilizado = ed.id_empaque_decoracion_utilizado ?? 0,
                    nombre = ed.tabla_empaques_decoraciones.nombre,
                    cantidad = ed.cantidad ?? 0,
                    unidad_de_medida = ed.unidad_de_medida,
                    costo_por_cantidad = ed.costo_por_cantidad ?? 0m,
                    total_costo = ed.total_costo ?? 0m
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
                    costo_por_cantidad = i.costo_por_cantidad ?? 0m,
                    total_costo = i.total_costo ?? 0m
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
                    costo_por_cantidad = s.costo_por_cantidad ?? 0m,
                    total_costo = s.total_costo ?? 0m,
                    es_impresion_de_facturas = s.es_impresion_de_facturas ?? false
                }).ToList()
        }).ToList();

        ViewBag.Recetas = new SelectList(db.tabla_costos_recetas.ToList(), "nombre", "nombre", producto_final.nombre_receta);
        ViewBag.EmpaquesDecoraciones = new SelectList(
            db.tabla_empaques_decoraciones.ToList()
            .Select(ed => new {
                Value = ed.id,
                Text = $"ID: {ed.id} | Nombre: {ed.nombre} | Costo por cantidad: ₡{ed.costo_por_cantidad}"
            }),
            "Value", "Text"
        );

        ViewBag.Implementos = new SelectList(
            db.tabla_implementos.ToList()
                .Select(i => new {
                    Value = i.id,
                    Text = $"ID: {i.id} | Nombre: {i.nombre} | Costo por cantidad: ₡{i.costo_por_cantidad}"
                }),
            "Value", "Text"
        );

        ViewBag.Suministros = new SelectList(
            db.tabla_suministros.ToList()
                .Select(s => new {
                    Value = s.id,
                    Text = $"ID: {s.id} | Nombre: {s.nombre} | Costo por cantidad: ₡{s.costo_por_cantidad}"
                }),
            "Value", "Text"
        );

        ViewBag.Editando = true;
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
        // Asignar el valor del checkbox manualmente para cada suministro
        if (producto_final.SuministrosUtilizados != null)
        {
            for (int i = 0; i < producto_final.SuministrosUtilizados.Count; i++)
            {
                var key = $"SuministrosUtilizados[{i}].es_impresion_de_facturas";
                producto_final.SuministrosUtilizados[i].es_impresion_de_facturas = Request.Form[key] == "on";
            }
        }

        var errores = new List<string>();

        if (db.tabla_precios_finales_sugeridos.Any(pf => pf.nombre_receta.ToLower() == producto_final.nombre_receta.ToLower() && pf.id != producto_final.id))
            errores.Add("Ya existe un producto final para esa receta.");

        if (string.IsNullOrWhiteSpace(producto_final.nombre_receta))
            errores.Add("El nombre de la receta es obligatorio.");

        if (producto_final.margen_de_utilidad < 0 || producto_final.margen_de_utilidad > 100)
            errores.Add("El margen de utilidad debe estar entre 0 y 100.");

        // Validar detalles (Empaques, Implementos, suministros)
        if (producto_final.EmpaquesDecoracionesUtilizados != null)
        {
            var idsEmpaques = new HashSet<int>();
            for (int i = 0; i < producto_final.EmpaquesDecoracionesUtilizados.Count; i++)
            {
                var ed = producto_final.EmpaquesDecoracionesUtilizados[i];

                if ((ed.id_empaque_decoracion_utilizado == 0) && ed.cantidad == 0 && string.IsNullOrWhiteSpace(ed.unidad_de_medida))
                {
                    errores.Add($"Fila {i + 1} de Empaques/Decoraciones: No puede dejar filas vacías.");
                    continue;
                }
                if (ed.id_empaque_decoracion_utilizado == 0)
                    errores.Add($"Fila {i + 1} de Empaques/Decoraciones: Debe seleccionar un empaque/decoración.");
                if (ed.cantidad <= 0)
                    errores.Add($"Fila {i + 1} de Empaques/Decoraciones: La cantidad debe ser mayor a cero.");
                if (string.IsNullOrWhiteSpace(ed.unidad_de_medida))
                    errores.Add($"Fila {i + 1} de Empaques/Decoraciones: La unidad de medida es obligatoria.");

                if (ed.id_empaque_decoracion_utilizado != 0)
                {
                    if (!idsEmpaques.Add(ed.id_empaque_decoracion_utilizado))
                        errores.Add($"Fila {i + 1} de Empaques/Decoraciones: Empaque/decoración repetido.");

                    var empaque = db.tabla_empaques_decoraciones.FirstOrDefault(x => x.id == ed.id_empaque_decoracion_utilizado);
                    if (empaque == null)
                    {
                        errores.Add($"Fila {i + 1} de Empaques/Decoraciones: El empaque/decoración seleccionado no existe.");
                        continue;
                    }
                    ed.nombre = empaque.nombre;
                    ed.costo_por_cantidad = empaque.costo_por_cantidad ?? 0m;
                    ed.total_costo = ed.cantidad * ed.costo_por_cantidad;
                }
            }
        }

        if (producto_final.ImplementosUtilizados != null)
        {
            var idsImplementos = new HashSet<int>();
            for (int i = 0; i < producto_final.ImplementosUtilizados.Count; i++)
            {
                var impl = producto_final.ImplementosUtilizados[i];

                if ((impl.id_implemento_utilizado == 0) && impl.cantidad == 0 && string.IsNullOrWhiteSpace(impl.unidad_de_medida))
                {
                    errores.Add($"Fila {i + 1} de Implementos: No puede dejar filas vacías.");
                    continue;
                }
                if (impl.id_implemento_utilizado == 0)
                    errores.Add($"Fila {i + 1} de Implementos: Debe seleccionar un implemento.");
                if (impl.cantidad <= 0)
                    errores.Add($"Fila {i + 1} de Implementos: La cantidad debe ser mayor a cero.");
                if (string.IsNullOrWhiteSpace(impl.unidad_de_medida))
                    errores.Add($"Fila {i + 1} de Implementos: La unidad de medida es obligatoria.");

                if (impl.id_implemento_utilizado != 0)
                {
                    if (!idsImplementos.Add(impl.id_implemento_utilizado))
                        errores.Add($"Fila {i + 1} de Implementos: Implemento repetido.");

                    var implemento = db.tabla_implementos.FirstOrDefault(x => x.id == impl.id_implemento_utilizado);
                    if (implemento == null)
                    {
                        errores.Add($"Fila {i + 1} de Implementos: El implemento seleccionado no existe.");
                        continue;
                    }
                    impl.nombre = implemento.nombre;
                    impl.costo_por_cantidad = implemento.costo_por_cantidad ?? 0m;
                    impl.total_costo = impl.cantidad * impl.costo_por_cantidad;
                }
            }
        }

        if (producto_final.SuministrosUtilizados != null)
        {
            var idsSuministros = new HashSet<int>();
            for (int i = 0; i < producto_final.SuministrosUtilizados.Count; i++)
            {
                var sumn = producto_final.SuministrosUtilizados[i];

                if ((sumn.id_suministro_utilizado == 0) && sumn.cantidad == 0 && string.IsNullOrWhiteSpace(sumn.unidad_de_medida))
                {
                    errores.Add($"Fila {i + 1} de Suministros: No puede dejar filas vacías.");
                    continue;
                }
                if (sumn.id_suministro_utilizado == 0)
                    errores.Add($"Fila {i + 1} de Suministros: Debe seleccionar un suministro.");
                if (sumn.cantidad <= 0)
                    errores.Add($"Fila {i + 1} de Suministros: La cantidad debe ser mayor a cero.");
                if (string.IsNullOrWhiteSpace(sumn.unidad_de_medida))
                    errores.Add($"Fila {i + 1} de Suministros: La unidad de medida es obligatoria.");

                if (sumn.id_suministro_utilizado != 0)
                {
                    if (!idsSuministros.Add(sumn.id_suministro_utilizado))
                        errores.Add($"Fila {i + 1} de Suministros: Suministro repetido.");

                    var suministro = db.tabla_suministros.FirstOrDefault(x => x.id == sumn.id_suministro_utilizado);
                    if (suministro == null)
                    {
                        errores.Add($"Fila {i + 1} de Suministros: El suministro seleccionado no existe.");
                        continue;
                    }
                    sumn.nombre = suministro.nombre;
                    sumn.costo_por_cantidad = suministro.costo_por_cantidad ?? 0m;
                    sumn.total_costo = sumn.cantidad * sumn.costo_por_cantidad;
                }
            }
        }

        var p = db.tabla_precios_finales_sugeridos.Find(producto_final.id);
        if (p == null) return HttpNotFound();

        // Obtener el costo total de la receta seleccionada
        var receta = db.tabla_costos_recetas.FirstOrDefault(r => r.nombre == producto_final.nombre_receta);
        if (receta == null)
            errores.Add("La receta seleccionada no existe.");

        if (errores.Any())
        {
            ViewBag.Errores = errores;
            ViewBag.Editando = true;
            ViewBag.Recetas = new SelectList(db.tabla_costos_recetas.ToList(), "nombre", "nombre");
            ViewBag.EmpaquesDecoraciones = new SelectList(
                db.tabla_empaques_decoraciones.ToList()
                .Select(ed => new {
                    Value = ed.id,
                    Text = $"ID: {ed.id} | Nombre: {ed.nombre} | Costo por cantidad: ₡{ed.costo_por_cantidad}"
                }),
                "Value", "Text"
            );

            ViewBag.Implementos = new SelectList(
                db.tabla_implementos.ToList()
                    .Select(i => new {
                        Value = i.id,
                        Text = $"ID: {i.id} | Nombre: {i.nombre} | Costo por cantidad: ₡{i.costo_por_cantidad}"
                    }),
                "Value", "Text"
            );

            ViewBag.Suministros = new SelectList(
                db.tabla_suministros.ToList()
                    .Select(s => new {
                        Value = s.id,
                        Text = $"ID: {s.id} | Nombre: {s.nombre} | Costo por cantidad: ₡{s.costo_por_cantidad}"
                    }),
                "Value", "Text"
            ); 
            
            var productosFinales = db.tabla_precios_finales_sugeridos.ToList().Select(pf => new ProductoFinal
            {
                id = pf.id,
                nombre_receta = pf.nombre_receta,
                costo_total_receta = pf.costo_total_receta ?? 0m,
                margen_de_utilidad = pf.margen_de_utilidad,
                costo_sin_margen_de_utilidad = pf.costo_sin_margen_de_utilidad ?? 0,
                costo_con_margen_de_utilidad = pf.costo_con_margen_de_utilidad ?? 0m,
                costo_empaque_decoracion_utilizado = pf.costo_empaque_decoracion_utilizado ?? 0m,
                costo_implemento_utilizado = pf.costo_implemento_utilizado ?? 0m,
                costo_suministro_utilizado = pf.costo_suministro_utilizado ?? 0m,
                costo_de_impresion_de_factura_por_insumo = pf.costo_de_impresion_de_factura_por_insumo ?? 0m,
                costo_total_de_impresion_de_factura = pf.costo_total_de_impresion_de_factura ?? 0m,
                factura_total = pf.factura_total ?? 0m,
                factura_por_insumo = pf.factura_por_insumo ?? 0m,
                iva = pf.iva ?? 0m,
                impuesto_de_servicio = pf.impuesto_de_servicio ?? 0m,
                envio = pf.envio ?? 0m,
                plataforma_de_envio = pf.plataforma_de_envio,
            }).ToList();
            return View("precio_final", new InsumosModel
            {
                ProductoFinalEditado = producto_final,
                ProductosFinales = productosFinales
            });
        }

        // Calcula el costo de la receta desde la base de datos
        decimal costoReceta = receta?.costo_total_receta ?? 0;
        decimal margenUtilidad = producto_final.margen_de_utilidad;
        decimal costoSinUtilidad = 100m - margenUtilidad;
        decimal costoConUtilidad = costoReceta / (costoSinUtilidad / 100m);

        // Suministros normales (excluyendo el de impresión)
        var suministrosNormales = producto_final.SuministrosUtilizados?
            .Where(s => !s.es_impresion_de_facturas)
            .ToList() ?? new List<SuministroUtilizado>();

        decimal totalSuministros = suministrosNormales.Sum(s => s.total_costo);

        // Buscar el suministro de impresión por el campo booleano
        var suministroImpresion = producto_final.SuministrosUtilizados?
            .FirstOrDefault(s => s.es_impresion_de_facturas);

        decimal costoImpresionFacturaPorInsumo = 0;
        decimal costoTotalImpresionFactura = 0;
        decimal porcion = receta?.porcion ?? 0;

        if (suministroImpresion != null)
        {
            costoImpresionFacturaPorInsumo = suministroImpresion.costo_por_cantidad / 20m;
            costoTotalImpresionFactura = porcion * costoImpresionFacturaPorInsumo;
        }

        // Suma de costos por cantidad (multiplicando por la cantidad)
        decimal sumaEmpaquesPorCantidad = producto_final.EmpaquesDecoracionesUtilizados?.Sum(e => e.costo_por_cantidad * e.cantidad) ?? 0;
        decimal sumaImplementosPorCantidad = producto_final.ImplementosUtilizados?.Sum(i => i.costo_por_cantidad * i.cantidad) ?? 0;
        decimal sumaSuministrosPorCantidad = suministrosNormales.Sum(s => s.costo_por_cantidad * s.cantidad);

        // Factura por insumo: suma de todos los costos individuales + impresión por insumo
        decimal facturaPorInsumo = sumaEmpaquesPorCantidad + sumaImplementosPorCantidad + sumaSuministrosPorCantidad + costoImpresionFacturaPorInsumo;

        // Factura total: suma de todos los totales + impresión total
        decimal facturaTotal = sumaEmpaquesPorCantidad + sumaImplementosPorCantidad + sumaSuministrosPorCantidad + costoTotalImpresionFactura;

        // Total insumos con porcentaje de ganancia
        decimal totalInsumosConGanancia = facturaTotal * 1.10m;

        // IVA y Servicio
        // Obtén los porcentajes digitados desde el formulario
        decimal ivaPorcentaje = 0;
        decimal servicioPorcentaje = 0;
        decimal.TryParse(Request.Form["iva_porcentaje"], out ivaPorcentaje);
        decimal.TryParse(Request.Form["servicio_porcentaje"], out servicioPorcentaje);

        decimal baseImpuestos = costoConUtilidad + totalInsumosConGanancia;
        decimal iva = baseImpuestos * (ivaPorcentaje / 100m);
        decimal servicio = baseImpuestos * (servicioPorcentaje / 100m);

        // Envío
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

        // Precio final sugerido
        decimal precioFinal = baseImpuestos + iva + servicio + envio;

        // Actualizar campos principales
        p.id_receta = receta.id;
        p.nombre_receta = producto_final.nombre_receta;
        p.costo_total_receta = costoReceta;
        p.margen_de_utilidad = margenUtilidad;
        p.costo_sin_margen_de_utilidad = costoReceta;
        p.costo_con_margen_de_utilidad = costoConUtilidad;
        p.costo_empaque_decoracion_utilizado = sumaEmpaquesPorCantidad;
        p.costo_implemento_utilizado = sumaImplementosPorCantidad;
        p.costo_suministro_utilizado = totalSuministros;
        p.costo_de_impresion_de_factura_por_insumo = costoImpresionFacturaPorInsumo;
        p.costo_total_de_impresion_de_factura = costoTotalImpresionFactura;
        p.costo_total_empaque_decoracion_implemento_suministro_por_porcentaje_de_ganancia = totalInsumosConGanancia;
        p.factura_por_insumo = facturaPorInsumo;
        p.factura_total = facturaTotal;
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
                    total_costo = s.total_costo,
                    es_impresion_de_facturas = s.es_impresion_de_facturas
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
