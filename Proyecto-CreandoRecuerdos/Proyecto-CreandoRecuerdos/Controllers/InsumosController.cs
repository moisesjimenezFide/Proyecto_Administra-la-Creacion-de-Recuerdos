using Proyecto_CreandoRecuerdos.base_de_datos;
using Proyecto_CreandoRecuerdos.Filters;
using Proyecto_CreandoRecuerdos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Proyecto_CreandoRecuerdos.Controllers
{
    // Evitar el almacenamiento en caché de las vistas
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]

    [RolAuthorize("1")]

    public class InsumosController : Controller
    {
        private BD_CREANDO_RECUERDOSEntities db = new BD_CREANDO_RECUERDOSEntities();
        /* Materias Primas */

        // Listar y buscar materias primas
        public ActionResult materias_primas(string search)
        {

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
            ViewBag.Editando = false;
            return View(materia_prima);
        }

        [HttpGet]
        public JsonResult VerificarDuplicadoMateriaPrima(
        string nombre, string marca, string presentacion, int? cantidad,
        decimal? volumen_de_porcion_de_presentacion, string unidad_de_medida_de_presentacion,
        string proveedor, decimal? costo, string unidad_de_medida_del_peso, int id = 0)
        {
            nombre = nombre?.Trim().ToLower() ?? "";
            marca = marca?.Trim().ToLower() ?? "";
            presentacion = presentacion?.Trim().ToLower() ?? "";
            unidad_de_medida_de_presentacion = unidad_de_medida_de_presentacion?.Trim().ToLower() ?? "";
            proveedor = proveedor?.Trim().ToLower() ?? "";
            unidad_de_medida_del_peso = unidad_de_medida_del_peso?.Trim().ToLower() ?? "";

            int cantidadVal = cantidad ?? 0;
            decimal volumenVal = volumen_de_porcion_de_presentacion ?? 0m;
            decimal costoVal = costo ?? 0m;

            bool isUnique = !db.tabla_materias_primas.Any(mp =>
                mp.id != id &&
                (mp.nombre ?? "").Trim().ToLower() == nombre &&
                (mp.marca ?? "").Trim().ToLower() == marca &&
                (mp.presentacion ?? "").Trim().ToLower() == presentacion &&
                (mp.cantidad ?? 0) == cantidadVal &&
                (mp.volumen_de_porcion_de_presentacion ?? 0m) == volumenVal &&
                (mp.unidad_de_medida_de_presentacion ?? "").Trim().ToLower() == unidad_de_medida_de_presentacion &&
                (mp.proveedor ?? "").Trim().ToLower() == proveedor &&
                (mp.costo ?? 0m) == costoVal &&
                (mp.unidad_de_medida_del_peso ?? "").Trim().ToLower() == unidad_de_medida_del_peso
            );

            return Json(new { isUnique }, JsonRequestBehavior.AllowGet);
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

            var erroresPorCampo = new Dictionary<string, string>();

            // Validaciones por campo
            if (string.IsNullOrWhiteSpace(materia_prima.nombre))
                erroresPorCampo["nombre"] = "El nombre es obligatorio.";
            if (string.IsNullOrWhiteSpace(materia_prima.marca))
                erroresPorCampo["marca"] = "La marca es obligatoria.";
            if (string.IsNullOrWhiteSpace(materia_prima.presentacion))
                erroresPorCampo["presentacion"] = "La presentación es obligatoria.";
            if (materia_prima.cantidad <= 0)
                erroresPorCampo["cantidad"] = "La cantidad debe ser mayor a 0.";
            if (materia_prima.volumen_de_porcion_de_presentacion == null || materia_prima.volumen_de_porcion_de_presentacion <= 0m)
                erroresPorCampo["volumen_de_porcion_de_presentacion"] = "El volumen debe ser mayor a 0.";
            if (string.IsNullOrWhiteSpace(materia_prima.unidad_de_medida_de_presentacion))
                erroresPorCampo["unidad_de_medida_de_presentacion"] = "La unidad es obligatoria.";
            if (string.IsNullOrWhiteSpace(materia_prima.proveedor))
                erroresPorCampo["proveedor"] = "El proveedor es obligatorio.";
            if (materia_prima.costo <= 0.99m)
                erroresPorCampo["costo"] = "El costo debe ser mayor a ₡0.99.";
            if (string.IsNullOrWhiteSpace(materia_prima.unidad_de_medida_del_peso))
                erroresPorCampo["unidad_de_medida_del_peso"] = "La unidad es obligatoria.";
            if (materia_prima.merma_total_en_gramos != null && materia_prima.merma_total_en_gramos < 0m)
                erroresPorCampo["merma_total_en_gramos"] = "No puede ser negativa.";

            // Validaciones de unidades
            var unidadesPresentacion = new[] { "kg", "kilo", "kilos", "kilogramo", "kilogramos", "g", "gr", "grs", "gramo", "gramos", "l", "litro", "litros", "ml", "mililitro", "mililitros" };
            var unidadesPeso = new[] { "g", "gr", "grs", "gramo", "gramos", "ml", "mililitro", "mililitros" };
            var unidadesPresentacionMayorA0 = new[] { "g", "grs", "gramos", "kilos", "kilogramos", "l", "litros", "ml", "mililitros" };
            var unidadPresentacionIgualA1 = new[] { "g", "gr", "gramo", "kilo", "kilogramo", "l", "litro", "ml", "mililitro" };

            string unidadPresentacion = materia_prima.unidad_de_medida_de_presentacion?.Trim().ToLower() ?? "";
            string unidadPeso = materia_prima.unidad_de_medida_del_peso?.Trim().ToLower() ?? "";
            decimal volumen = materia_prima.volumen_de_porcion_de_presentacion ?? 0m;

            if (!string.IsNullOrWhiteSpace(unidadPresentacion) && !unidadesPresentacion.Contains(unidadPresentacion))
                erroresPorCampo["unidad_de_medida_de_presentacion"] = "Unidad de medida de presentación no permitida.";
            if (!string.IsNullOrWhiteSpace(unidadPeso) && !unidadesPeso.Contains(unidadPeso))
                erroresPorCampo["unidad_de_medida_del_peso"] = "Unidad de medida del peso no permitida.";

            if (volumen > 0m && volumen != 1m && !unidadesPresentacionMayorA0.Contains(unidadPresentacion))
                erroresPorCampo["unidad_de_medida_de_presentacion"] = "Si el volumen es mayor a 0 y distinto de 1, solo se permiten palabras plurales.";
            if (volumen == 1m && !unidadPresentacionIgualA1.Contains(unidadPresentacion))
                erroresPorCampo["unidad_de_medida_de_presentacion"] = "Si el volumen es igual a 1, solo se permiten palabras singulares.";

            if (erroresPorCampo.Any())
            {
                ViewBag.ErroresPorCampo = erroresPorCampo;
                ViewBag.Editando = false;
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
                var modelo = new InsumosModel
                {
                    MateriaPrimaEditado = materia_prima,
                    MateriasPrimas = lista
                };

                if (Request.IsAjaxRequest())
                    return PartialView("_FormularioMateriaPrima", modelo);

                return View("materias_primas", modelo);
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
            if (Request.IsAjaxRequest())
                return Json(new { success = true, message = "¡Materia prima agregada con éxito!" });
            if (Request.IsAjaxRequest())
                return Json(new { success = true });

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

            var erroresPorCampo = new Dictionary<string, string>();

            // Validaciones por campo
            if (string.IsNullOrWhiteSpace(materia_prima.nombre))
                erroresPorCampo["nombre"] = "El nombre es obligatorio.";
            if (string.IsNullOrWhiteSpace(materia_prima.marca))
                erroresPorCampo["marca"] = "La marca es obligatoria.";
            if (string.IsNullOrWhiteSpace(materia_prima.presentacion))
                erroresPorCampo["presentacion"] = "La presentación es obligatoria.";
            if (materia_prima.cantidad <= 0)
                erroresPorCampo["cantidad"] = "La cantidad debe ser mayor a 0.";
            if (materia_prima.volumen_de_porcion_de_presentacion == null || materia_prima.volumen_de_porcion_de_presentacion <= 0m)
                erroresPorCampo["volumen_de_porcion_de_presentacion"] = "El volumen debe ser mayor a 0.";
            if (string.IsNullOrWhiteSpace(materia_prima.unidad_de_medida_de_presentacion))
                erroresPorCampo["unidad_de_medida_de_presentacion"] = "La unidad es obligatoria.";
            if (string.IsNullOrWhiteSpace(materia_prima.proveedor))
                erroresPorCampo["proveedor"] = "El proveedor es obligatorio.";
            if (materia_prima.costo <= 0.99m)
                erroresPorCampo["costo"] = "El costo debe ser mayor a ₡0.99.";
            if (string.IsNullOrWhiteSpace(materia_prima.unidad_de_medida_del_peso))
                erroresPorCampo["unidad_de_medida_del_peso"] = "La unidad es obligatoria.";
            if (materia_prima.merma_total_en_gramos != null && materia_prima.merma_total_en_gramos < 0m)
                erroresPorCampo["merma_total_en_gramos"] = "No puede ser negativa.";

            // Validaciones de unidades
            var unidadesPresentacion = new[] { "kg", "kilo", "kilos", "kilogramo", "kilogramos", "g", "gr", "grs", "gramo", "gramos", "l", "litro", "litros", "ml", "mililitro", "mililitros" };
            var unidadesPeso = new[] { "g", "gr", "grs", "gramo", "gramos", "ml", "mililitro", "mililitros" };
            var unidadesPresentacionMayorA0 = new[] { "g", "grs", "gramos", "kilos", "kilogramos", "l", "litros", "ml", "mililitros" };
            var unidadPresentacionIgualA1 = new[] { "g", "gr", "gramo", "kilo", "kilogramo", "l", "litro", "ml", "mililitro" };

            string unidadPresentacion = materia_prima.unidad_de_medida_de_presentacion?.Trim().ToLower() ?? "";
            string unidadPeso = materia_prima.unidad_de_medida_del_peso?.Trim().ToLower() ?? "";
            decimal volumen = materia_prima.volumen_de_porcion_de_presentacion ?? 0m;

            if (!string.IsNullOrWhiteSpace(unidadPresentacion) && !unidadesPresentacion.Contains(unidadPresentacion))
                erroresPorCampo["unidad_de_medida_de_presentacion"] = "Unidad de medida de presentación no permitida.";
            if (!string.IsNullOrWhiteSpace(unidadPeso) && !unidadesPeso.Contains(unidadPeso))
                erroresPorCampo["unidad_de_medida_del_peso"] = "Unidad de medida del peso no permitida.";

            if (volumen > 0m && volumen != 1m && !unidadesPresentacionMayorA0.Contains(unidadPresentacion))
                erroresPorCampo["unidad_de_medida_de_presentacion"] = "Si el volumen es mayor a 0 y distinto de 1, solo se permiten palabras plurales.";
            if (volumen == 1m && !unidadPresentacionIgualA1.Contains(unidadPresentacion))
                erroresPorCampo["unidad_de_medida_de_presentacion"] = "Si el volumen es igual a 1, solo se permiten palabras singulares.";

            // Duplicado exacto
            string nombre = materia_prima.nombre?.Trim().ToLower() ?? "";
            string marca = materia_prima.marca?.Trim().ToLower() ?? "";
            string presentacion = materia_prima.presentacion?.Trim().ToLower() ?? "";
            int cantidad = materia_prima.cantidad;
            decimal? volumenDePorciondePresentacion = materia_prima.volumen_de_porcion_de_presentacion;
            string unidadDeMedidaDePresentacion = materia_prima.unidad_de_medida_de_presentacion?.Trim().ToLower() ?? "";
            string proveedor = materia_prima.proveedor?.Trim().ToLower() ?? "";
            decimal? costo = materia_prima.costo;
            string unidadDeMedidaDelPeso = materia_prima.unidad_de_medida_del_peso?.Trim().ToLower() ?? "";

            bool existeExacto = db.tabla_materias_primas.Any(mp =>
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
                erroresPorCampo["duplicado"] = "Ya existe una materia prima con los mismos datos.";

            if (erroresPorCampo.Any())
            {
                ViewBag.ErroresPorCampo = erroresPorCampo;
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

                var modelo = new InsumosModel
                {
                    MateriaPrimaEditado = materia_prima,
                    MateriasPrimas = lista
                };

                if (Request.IsAjaxRequest())
                    return PartialView("_FormularioMateriaPrima", modelo);

                return View("materias_primas", modelo);
            }

            var m = db.tabla_materias_primas.Find(materia_prima.id);
            if (m != null)
            {
                m.nombre = materia_prima.nombre;
                m.marca = materia_prima.marca;
                m.presentacion = materia_prima.presentacion;
                m.cantidad = materia_prima.cantidad;
                m.volumen_de_porcion_de_presentacion = materia_prima.volumen_de_porcion_de_presentacion;
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
            if (Request.IsAjaxRequest())
                return Json(new { success = true, message = "¡Materia prima actualizada con éxito!" });
            if (Request.IsAjaxRequest())
                return Json(new { success = true });

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

        [HttpGet]
        public JsonResult VerificarDuplicadoProductoPreparado(
            string tipo, string nombre, string marca, string presentacion, int? cantidad,
            decimal? volumen_de_porcion_de_presentacion, string unidad_de_medida_de_presentacion,
            string proveedor, decimal? costo, string unidad_de_medida_del_peso, int id = 0)
        {
            tipo = tipo?.Trim().ToLower() ?? "";
            nombre = nombre?.Trim().ToLower() ?? "";
            marca = marca?.Trim().ToLower() ?? "";
            presentacion = presentacion?.Trim().ToLower() ?? "";
            unidad_de_medida_de_presentacion = unidad_de_medida_de_presentacion?.Trim().ToLower() ?? "";
            proveedor = proveedor?.Trim().ToLower() ?? "";
            unidad_de_medida_del_peso = unidad_de_medida_del_peso?.Trim().ToLower() ?? "";

            // Evita errores por nulos en la consulta
            bool isUnique = !db.tabla_productos_preparados.Any(pp =>
                pp.id != id &&
                (pp.tipo ?? "").Trim().ToLower() == tipo &&
                (pp.nombre ?? "").Trim().ToLower() == nombre &&
                (pp.marca ?? "").Trim().ToLower() == marca &&
                (pp.presentacion ?? "").Trim().ToLower() == presentacion &&
                (pp.cantidad ?? 0) == cantidad &&
                (pp.volumen_de_porcion_de_presentacion ?? 0m) == volumen_de_porcion_de_presentacion &&
                (pp.unidad_de_medida_de_presentacion ?? "").Trim().ToLower() == unidad_de_medida_de_presentacion &&
                (pp.proveedor ?? "").Trim().ToLower() == proveedor &&
                (pp.costo ?? 0m) == costo &&
                (pp.unidad_de_medida_del_peso ?? "").Trim().ToLower() == unidad_de_medida_del_peso
            );

            return Json(new { isUnique }, JsonRequestBehavior.AllowGet);
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

            var erroresPorCampo = new Dictionary<string, string>();

            // Validaciones por campo
            if (string.IsNullOrWhiteSpace(producto_preparado.tipo))
                erroresPorCampo["tipo"] = "El tipo es obligatorio.";
            if (string.IsNullOrWhiteSpace(producto_preparado.nombre))
                erroresPorCampo["nombre"] = "El nombre es obligatorio.";
            if (string.IsNullOrWhiteSpace(producto_preparado.marca))
                erroresPorCampo["marca"] = "La marca es obligatoria.";
            if (string.IsNullOrWhiteSpace(producto_preparado.presentacion))
                erroresPorCampo["presentacion"] = "La presentación es obligatoria.";
            if (producto_preparado.cantidad <= 0)
                erroresPorCampo["cantidad"] = "La cantidad debe ser mayor a 0.";
            if (producto_preparado.volumen_de_porcion_de_presentacion == null || producto_preparado.volumen_de_porcion_de_presentacion <= 0m)
                erroresPorCampo["volumen_de_porcion_de_presentacion"] = "El volumen debe ser mayor a 0.";
            if (string.IsNullOrWhiteSpace(producto_preparado.unidad_de_medida_de_presentacion))
                erroresPorCampo["unidad_de_medida_de_presentacion"] = "La unidad es obligatoria.";
            if (string.IsNullOrWhiteSpace(producto_preparado.proveedor))
                erroresPorCampo["proveedor"] = "El proveedor es obligatorio.";
            if (producto_preparado.costo <= 0.99m)
                erroresPorCampo["costo"] = "El costo debe ser mayor a ₡0.99.";
            if (string.IsNullOrWhiteSpace(producto_preparado.unidad_de_medida_del_peso))
                erroresPorCampo["unidad_de_medida_del_peso"] = "La unidad es obligatoria.";

            // Validaciones de unidades
            var unidadesPresentacion = new[] { "kg", "kilo", "kilos", "kilogramo", "kilogramos", "g", "gr", "grs", "gramo", "gramos", "l", "litro", "litros", "ml", "mililitro", "mililitros" };
            var unidadesPeso = new[] { "g", "gr", "grs", "gramo", "gramos", "ml", "mililitro", "mililitros" };
            var unidadesPresentacionMayorA0 = new[] { "g", "grs", "gramos", "kilos", "kilogramos", "l", "litros", "ml", "mililitros" };
            var unidadPresentacionIgualA1 = new[] { "g", "gr", "gramo", "kilo", "kilogramo", "l", "litro", "ml", "mililitro" };

            string unidadPresentacion = producto_preparado.unidad_de_medida_de_presentacion?.Trim().ToLower() ?? "";
            string unidadPeso = producto_preparado.unidad_de_medida_del_peso?.Trim().ToLower() ?? "";
            decimal volumen = producto_preparado.volumen_de_porcion_de_presentacion ?? 0m;

            if (!string.IsNullOrWhiteSpace(unidadPresentacion) && !unidadesPresentacion.Contains(unidadPresentacion))
                erroresPorCampo["unidad_de_medida_de_presentacion"] = "Unidad de medida de presentación no permitida.";
            if (!string.IsNullOrWhiteSpace(unidadPeso) && !unidadesPeso.Contains(unidadPeso))
                erroresPorCampo["unidad_de_medida_del_peso"] = "Unidad de medida del peso no permitida.";

            if (volumen > 0m && volumen != 1m && !unidadesPresentacionMayorA0.Contains(unidadPresentacion))
                erroresPorCampo["unidad_de_medida_de_presentacion"] = "Si el volumen es mayor a 0 y distinto de 1, solo se permiten palabras plurales.";
            if (volumen == 1m && !unidadPresentacionIgualA1.Contains(unidadPresentacion))
                erroresPorCampo["unidad_de_medida_de_presentacion"] = "Si el volumen es igual a 1, solo se permiten palabras singulares.";

            if (erroresPorCampo.Any())
            {
                ViewBag.ErroresPorCampo = erroresPorCampo;
                ViewBag.Editando = false;
                var lista = db.tabla_productos_preparados.Select(pp => new ProductoPreparado
                {
                    id = pp.id,
                    tipo = pp.tipo,
                    nombre = pp.nombre,
                    marca = pp.marca,
                    presentacion = pp.presentacion,
                    cantidad = pp.cantidad ?? 0,
                    volumen_de_porcion_de_presentacion = pp.volumen_de_porcion_de_presentacion ?? 0m,
                    unidad_de_medida_de_presentacion = pp.unidad_de_medida_de_presentacion,
                    volumen_de_porcion_convertido = pp.volumen_de_porcion_convertido,
                    unidad_de_medida_convertida = pp.unidad_de_medida_convertida,
                    proveedor = pp.proveedor,
                    costo = pp.costo ?? 0m,
                    peso = pp.peso ?? 0m,
                    unidad_de_medida_del_peso = pp.unidad_de_medida_del_peso,
                    costo_por_peso = pp.costo_por_peso,
                    costo_por_porcion_con_merma = pp.costo_por_porcion_con_merma
                }).ToList();
                var modelo = new InsumosModel
                {
                    ProductoPreparadoEditado = producto_preparado,
                    ProductosPreparados = lista
                };

                if (Request.IsAjaxRequest())
                    return PartialView("_FormularioProductoPreparado", modelo);

                return View("productos_preparados", modelo);
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
            if (Request.IsAjaxRequest())
                return Json(new { success = true, message = "¡Producto preparado agregado con éxito!" });
            if (Request.IsAjaxRequest())
                return Json(new { success = true });

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

            var erroresPorCampo = new Dictionary<string, string>();

            // Validaciones por campo
            if (string.IsNullOrWhiteSpace(producto_preparado.tipo))
                erroresPorCampo["tipo"] = "El tipo es obligatorio.";
            if (string.IsNullOrWhiteSpace(producto_preparado.nombre))
                erroresPorCampo["nombre"] = "El nombre es obligatorio.";
            if (string.IsNullOrWhiteSpace(producto_preparado.marca))
                erroresPorCampo["marca"] = "La marca es obligatoria.";
            if (string.IsNullOrWhiteSpace(producto_preparado.presentacion))
                erroresPorCampo["presentacion"] = "La presentación es obligatoria.";
            if (producto_preparado.cantidad <= 0)
                erroresPorCampo["cantidad"] = "La cantidad debe ser mayor a 0.";
            if (producto_preparado.volumen_de_porcion_de_presentacion == null || producto_preparado.volumen_de_porcion_de_presentacion <= 0m)
                erroresPorCampo["volumen_de_porcion_de_presentacion"] = "El volumen debe ser mayor a 0.";
            if (string.IsNullOrWhiteSpace(producto_preparado.unidad_de_medida_de_presentacion))
                erroresPorCampo["unidad_de_medida_de_presentacion"] = "La unidad es obligatoria.";
            if (string.IsNullOrWhiteSpace(producto_preparado.proveedor))
                erroresPorCampo["proveedor"] = "El proveedor es obligatorio.";
            if (producto_preparado.costo <= 0.99m)
                erroresPorCampo["costo"] = "El costo debe ser mayor a ₡0.99.";
            if (string.IsNullOrWhiteSpace(producto_preparado.unidad_de_medida_del_peso))
                erroresPorCampo["unidad_de_medida_del_peso"] = "La unidad es obligatoria.";

            // Validaciones de unidades
            var unidadesPresentacion = new[] { "kg", "kilo", "kilos", "kilogramo", "kilogramos", "g", "gr", "grs", "gramo", "gramos", "l", "litro", "litros", "ml", "mililitro", "mililitros" };
            var unidadesPeso = new[] { "g", "gr", "grs", "gramo", "gramos", "ml", "mililitro", "mililitros" };
            var unidadesPresentacionMayorA0 = new[] { "g", "grs", "gramos", "kilos", "kilogramos", "l", "litros", "ml", "mililitros" };
            var unidadPresentacionIgualA1 = new[] { "g", "gr", "gramo", "kilo", "kilogramo", "l", "litro", "ml", "mililitro" };

            string unidadPresentacion = producto_preparado.unidad_de_medida_de_presentacion?.Trim().ToLower() ?? "";
            string unidadPeso = producto_preparado.unidad_de_medida_del_peso?.Trim().ToLower() ?? "";
            decimal volumen = producto_preparado.volumen_de_porcion_de_presentacion ?? 0m;

            if (!string.IsNullOrWhiteSpace(unidadPresentacion) && !unidadesPresentacion.Contains(unidadPresentacion))
                erroresPorCampo["unidad_de_medida_de_presentacion"] = "Unidad de medida de presentación no permitida.";
            if (!string.IsNullOrWhiteSpace(unidadPeso) && !unidadesPeso.Contains(unidadPeso))
                erroresPorCampo["unidad_de_medida_del_peso"] = "Unidad de medida del peso no permitida.";

            if (volumen > 0m && volumen != 1m && !unidadesPresentacionMayorA0.Contains(unidadPresentacion))
                erroresPorCampo["unidad_de_medida_de_presentacion"] = "Si el volumen es mayor a 0 y distinto de 1, solo se permiten palabras plurales.";
            if (volumen == 1m && !unidadPresentacionIgualA1.Contains(unidadPresentacion))
                erroresPorCampo["unidad_de_medida_de_presentacion"] = "Si el volumen es igual a 1, solo se permiten palabras singulares.";

            // Duplicado exacto
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

            bool existeExacto = db.tabla_productos_preparados.Any(p =>
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
                erroresPorCampo["duplicado"] = "Ya existe un producto preparado con los mismos datos.";

            if (erroresPorCampo.Any())
            {
                ViewBag.ErroresPorCampo = erroresPorCampo;
                ViewBag.Editando = true;
                var lista = db.tabla_productos_preparados.Select(p => new ProductoPreparado
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
                    costo_por_porcion_con_merma = p.costo_por_porcion_con_merma
                }).ToList();

                var modelo = new InsumosModel
                {
                    ProductoPreparadoEditado = producto_preparado,
                    ProductosPreparados = lista
                };

                if (Request.IsAjaxRequest())
                    return PartialView("_FormularioProductoPreparado", modelo);

                return View("productos_preparados", modelo);
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
            if (Request.IsAjaxRequest())
                return Json(new { success = true, message = "¡Producto preparado actualizado con éxito!" });
            if (Request.IsAjaxRequest())
                return Json(new { success = true });

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

        [HttpGet]
        public JsonResult VerificarDuplicadoEmpaqueDecoracion(
        string nombre, string marca, string presentacion, string proveedor,
        decimal costo, int cantidad, string unidad_de_medida, int id = 0)
        {
            nombre = nombre?.Trim().ToLower() ?? "";
            marca = marca?.Trim().ToLower() ?? "";
            presentacion = presentacion?.Trim().ToLower() ?? "";
            proveedor = proveedor?.Trim().ToLower() ?? "";
            unidad_de_medida = unidad_de_medida?.Trim().ToLower() ?? "";

            decimal costoVal = costo;
            int cantidadVal = cantidad;

            bool isUnique = !db.tabla_empaques_decoraciones.Any(ed =>
                ed.id != id &&
                (ed.nombre ?? "").Trim().ToLower() == nombre &&
                (ed.marca ?? "").Trim().ToLower() == marca &&
                (ed.presentacion ?? "").Trim().ToLower() == presentacion &&
                (ed.proveedor ?? "").Trim().ToLower() == proveedor &&
                (ed.costo ?? 0m) == costoVal &&
                (ed.cantidad ?? 0) == cantidadVal &&
                (ed.unidad_de_medida ?? "").Trim().ToLower() == unidad_de_medida
            );

            return Json(new { isUnique }, JsonRequestBehavior.AllowGet);
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

            var erroresPorCampo = new Dictionary<string, string>();

            // Validaciones por campo
            if (string.IsNullOrWhiteSpace(empaque_decoracion.nombre))
                erroresPorCampo["nombre"] = "El nombre es obligatorio.";
            if (string.IsNullOrWhiteSpace(empaque_decoracion.marca))
                erroresPorCampo["marca"] = "La marca es obligatoria.";
            if (string.IsNullOrWhiteSpace(empaque_decoracion.presentacion))
                erroresPorCampo["presentacion"] = "La presentación es obligatoria.";
            if (string.IsNullOrWhiteSpace(empaque_decoracion.proveedor))
                erroresPorCampo["proveedor"] = "El proveedor es obligatorio.";
            if (empaque_decoracion.costo <= 0.99m)
                erroresPorCampo["costo"] = "El costo debe ser mayor a ₡0.99.";
            if (empaque_decoracion.cantidad <= 0)
                erroresPorCampo["cantidad"] = "La cantidad debe ser mayor a cero.";
            if (string.IsNullOrWhiteSpace(empaque_decoracion.unidad_de_medida))
                erroresPorCampo["unidad_de_medida"] = "La unidad de medida es obligatoria.";

            // Validaciones de unidades
            var unidadesValidas = new[] { "unidad", "unidades" };
            var unidadesMayorA0 = new[] { "unidades" };
            var unidadIgualA1 = new[] { "unidad" };

            string unidadDeMedida = empaque_decoracion.unidad_de_medida?.Trim().ToLower() ?? "";
            int cantidad = empaque_decoracion.cantidad;

            if (!string.IsNullOrWhiteSpace(unidadDeMedida) && !unidadesValidas.Contains(unidadDeMedida))
                erroresPorCampo["unidad_de_medida"] = "Unidad de medida no permitida.";
            if (cantidad > 0 && cantidad != 1 && !unidadesMayorA0.Contains(unidadDeMedida))
                erroresPorCampo["unidad_de_medida"] = "Si la cantidad es mayor a 0 y distinta de 1, solo se permiten palabras plurales.";
            if (cantidad == 1 && !unidadIgualA1.Contains(unidadDeMedida))
                erroresPorCampo["unidad_de_medida"] = "Si la cantidad es igual a 1, solo se permiten palabras singulares.";

            if (erroresPorCampo.Any())
            {
                ViewBag.Errores = erroresPorCampo;
                var lista = db.tabla_empaques_decoraciones
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

                var modelo = new InsumosModel
                {
                    EmpaqueDecoracionEditado = empaque_decoracion,
                    EmpaquesDecoraciones = lista
                };

                if (Request.IsAjaxRequest())
                    return PartialView("_FormularioEmpaqueDecoracion", modelo);

                return View("empaques_decoraciones", modelo);
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
            if (Request.IsAjaxRequest())
                return Json(new { success = true, message = "¡Empaque o Decoración agregada(o) con éxito!" });
            if (Request.IsAjaxRequest())
                return Json(new { success = true });

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

            var erroresPorCampo = new Dictionary<string, string>();

            // Validaciones por campo
            if (string.IsNullOrWhiteSpace(empaque_decoracion.nombre))
                erroresPorCampo["nombre"] = "El nombre es obligatorio.";
            if (string.IsNullOrWhiteSpace(empaque_decoracion.marca))
                erroresPorCampo["marca"] = "La marca es obligatoria.";
            if (string.IsNullOrWhiteSpace(empaque_decoracion.presentacion))
                erroresPorCampo["presentacion"] = "La presentación es obligatoria.";
            if (string.IsNullOrWhiteSpace(empaque_decoracion.proveedor))
                erroresPorCampo["proveedor"] = "El proveedor es obligatorio.";
            if (empaque_decoracion.costo <= 0.99m)
                erroresPorCampo["costo"] = "El costo debe ser mayor a ₡0.99.";
            if (empaque_decoracion.cantidad <= 0)
                erroresPorCampo["cantidad"] = "La cantidad debe ser mayor a cero.";
            if (string.IsNullOrWhiteSpace(empaque_decoracion.unidad_de_medida))
                erroresPorCampo["unidad_de_medida"] = "La unidad de medida es obligatoria.";

            // Validaciones de unidades
            var unidadesValidas = new[] { "unidad", "unidades" };
            var unidadesMayorA0 = new[] { "unidades" };
            var unidadIgualA1 = new[] { "unidad" };

            string unidadDeMedida = empaque_decoracion.unidad_de_medida?.Trim().ToLower() ?? "";
            int cantidad = empaque_decoracion.cantidad;

            if (!string.IsNullOrWhiteSpace(unidadDeMedida) && !unidadesValidas.Contains(unidadDeMedida))
                erroresPorCampo["unidad_de_medida"] = "Unidad de medida no permitida.";
            if (cantidad > 0 && cantidad != 1 && !unidadesMayorA0.Contains(unidadDeMedida))
                erroresPorCampo["unidad_de_medida"] = "Si la cantidad es mayor a 0 y distinta de 1, solo se permiten palabras plurales.";
            if (cantidad == 1 && !unidadIgualA1.Contains(unidadDeMedida))
                erroresPorCampo["unidad_de_medida"] = "Si la cantidad es igual a 1, solo se permiten palabras singulares.";

            // Duplicado exacto
            string nombre = empaque_decoracion.nombre?.Trim().ToLower() ?? "";
            string marca = empaque_decoracion.marca?.Trim().ToLower() ?? "";
            string presentacion = empaque_decoracion.presentacion?.Trim().ToLower() ?? "";
            string proveedor = empaque_decoracion.proveedor?.Trim().ToLower() ?? "";
            decimal? costo = empaque_decoracion.costo;
            int cantidadEd = empaque_decoracion.cantidad;
            string unidadDeMedidaEd = empaque_decoracion.unidad_de_medida?.Trim().ToLower() ?? "";
            bool existeExacto = db.tabla_empaques_decoraciones.Any(empdec =>
                empdec.id != empaque_decoracion.id &&
                empdec.nombre.ToLower() == nombre &&
                empdec.marca.ToLower() == marca &&
                empdec.presentacion.ToLower() == presentacion &&
                empdec.proveedor.ToLower() == proveedor &&
                empdec.costo == costo &&
                empdec.cantidad == cantidadEd &&
                empdec.unidad_de_medida.ToLower() == unidadDeMedidaEd
            );
            if (existeExacto)
                erroresPorCampo["duplicado"] = "Ya existe un(a) empaque o decoración con los mismos datos.";

            if (erroresPorCampo.Any())
            {
                ViewBag.Errores = erroresPorCampo;
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

                var modelo = new InsumosModel
                {
                    EmpaqueDecoracionEditado = empaque_decoracion,
                    EmpaquesDecoraciones = lista
                };

                if (Request.IsAjaxRequest())
                    return PartialView("_FormularioEmpaqueDecoracion", modelo);

                return View("empaques_decoraciones", modelo);
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
            if (Request.IsAjaxRequest())
                return Json(new { success = true, message = "¡Empaque o Decoración actualizado(a) con éxito!" });
            if (Request.IsAjaxRequest())
                return Json(new { success = true });

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

        [HttpGet]
        public JsonResult VerificarDuplicadoImplemento(
            string nombre, string marca, string presentacion, string proveedor,
            decimal costo, int cantidad, string unidad_de_medida, int id = 0)
        {
            nombre = nombre?.Trim().ToLower() ?? "";
            marca = marca?.Trim().ToLower() ?? "";
            presentacion = presentacion?.Trim().ToLower() ?? "";
            proveedor = proveedor?.Trim().ToLower() ?? "";
            unidad_de_medida = unidad_de_medida?.Trim().ToLower() ?? "";
            decimal costoVal = costo;
            int cantidadVal = cantidad;
            bool isUnique = !db.tabla_implementos.Any(i =>
                i.id != id &&
                (i.nombre ?? "").Trim().ToLower() == nombre &&
                (i.marca ?? "").Trim().ToLower() == marca &&
                (i.presentacion ?? "").Trim().ToLower() == presentacion &&
                (i.proveedor ?? "").Trim().ToLower() == proveedor &&
                (i.costo ?? 0m) == costoVal &&
                (i.cantidad ?? 0) == cantidadVal &&
                (i.unidad_de_medida ?? "").Trim().ToLower() == unidad_de_medida
            );
            return Json(new { isUnique }, JsonRequestBehavior.AllowGet);
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

            var erroresPorCampo = new Dictionary<string, string>();

            // Validaciones por campo
            if (string.IsNullOrWhiteSpace(implemento.nombre))
                erroresPorCampo["nombre"] = "El nombre es obligatorio.";
            if (string.IsNullOrWhiteSpace(implemento.marca))
                erroresPorCampo["marca"] = "La marca es obligatoria.";
            if (string.IsNullOrWhiteSpace(implemento.presentacion))
                erroresPorCampo["presentacion"] = "La presentación es obligatoria.";
            if (string.IsNullOrWhiteSpace(implemento.proveedor))
                erroresPorCampo["proveedor"] = "El proveedor es obligatorio.";
            if (implemento.costo <= 0.99m)
                erroresPorCampo["costo"] = "El costo debe ser mayor a ₡0.99.";
            if (implemento.cantidad <= 0)
                erroresPorCampo["cantidad"] = "La cantidad debe ser mayor a cero.";
            if (string.IsNullOrWhiteSpace(implemento.unidad_de_medida))
                erroresPorCampo["unidad_de_medida"] = "La unidad de medida es obligatoria.";

            // Validaciones de unidades
            var unidadesValidas = new[] { "unidad", "unidades" };
            var unidadesMayorA0 = new[] { "unidades" };
            var unidadIgualA1 = new[] { "unidad" };

            string unidadDeMedida = implemento.unidad_de_medida?.Trim().ToLower() ?? "";
            int cantidad = implemento.cantidad;

            if (!string.IsNullOrWhiteSpace(unidadDeMedida) && !unidadesValidas.Contains(unidadDeMedida))
                erroresPorCampo["unidad_de_medida"] = "Unidad de medida no permitida.";
            if (cantidad > 0 && cantidad != 1 && !unidadesMayorA0.Contains(unidadDeMedida))
                erroresPorCampo["unidad_de_medida"] = "Si la cantidad es mayor a 0 y distinta de 1, solo se permiten palabras plurales.";
            if (cantidad == 1 && !unidadIgualA1.Contains(unidadDeMedida))
                erroresPorCampo["unidad_de_medida"] = "Si la cantidad es igual a 1, solo se permiten palabras singulares.";

            if (erroresPorCampo.Any())
            {
                ViewBag.Errores = erroresPorCampo;
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
                var modelo = new InsumosModel
                {
                    ImplementoEditado = implemento,
                    Implementos = lista
                };

                if (Request.IsAjaxRequest())
                    return PartialView("_FormularioImplemento", modelo);

                return View("implementos", modelo);
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
            if (Request.IsAjaxRequest())
                return Json(new { success = true, message = "¡Implemento agregado con éxito!" });

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

            var erroresPorCampo = new Dictionary<string, string>();

            // Validaciones por campo
            if (string.IsNullOrWhiteSpace(implemento.nombre))
                erroresPorCampo["nombre"] = "El nombre es obligatorio.";
            if (string.IsNullOrWhiteSpace(implemento.marca))
                erroresPorCampo["marca"] = "La marca es obligatoria.";
            if (string.IsNullOrWhiteSpace(implemento.presentacion))
                erroresPorCampo["presentacion"] = "La presentación es obligatoria.";
            if (string.IsNullOrWhiteSpace(implemento.proveedor))
                erroresPorCampo["proveedor"] = "El proveedor es obligatorio.";
            if (implemento.costo <= 0.99m)
                erroresPorCampo["costo"] = "El costo debe ser mayor a ₡0.99.";
            if (implemento.cantidad <= 0)
                erroresPorCampo["cantidad"] = "La cantidad debe ser mayor a cero.";
            if (string.IsNullOrWhiteSpace(implemento.unidad_de_medida))
                erroresPorCampo["unidad_de_medida"] = "La unidad de medida es obligatoria.";

            // Duplicado exacto
            string nombre = implemento.nombre?.Trim().ToLower() ?? "";
            string marca = implemento.marca?.Trim().ToLower() ?? "";
            string presentacion = implemento.presentacion?.Trim().ToLower() ?? "";
            string proveedor = implemento.proveedor?.Trim().ToLower() ?? "";
            decimal? costo = implemento.costo;
            int cantidadEd = implemento.cantidad;
            string unidadDeMedidaEd = implemento.unidad_de_medida?.Trim().ToLower() ?? "";
            bool existeExacto = db.tabla_implementos.Any(impl =>
                impl.id != implemento.id &&
                impl.nombre.ToLower() == nombre &&
                impl.marca.ToLower() == marca &&
                impl.presentacion.ToLower() == presentacion &&
                impl.proveedor.ToLower() == proveedor &&
                impl.costo == costo &&
                impl.cantidad == cantidadEd &&
                impl.unidad_de_medida.ToLower() == unidadDeMedidaEd
            );
            if (existeExacto)
                erroresPorCampo["duplicado"] = "Ya existe un implemento con los mismos datos.";

            // Validaciones de unidades
            var unidadesValidas = new[] { "unidad", "unidades" };
            var unidadesMayorA0 = new[] { "unidades" };
            var unidadIgualA1 = new[] { "unidad" };

            string unidadDeMedida = implemento.unidad_de_medida?.Trim().ToLower() ?? "";
            int cantidad = implemento.cantidad;

            if (!string.IsNullOrWhiteSpace(unidadDeMedida) && !unidadesValidas.Contains(unidadDeMedida))
                erroresPorCampo["unidad_de_medida"] = "Unidad de medida no permitida.";
            if (cantidad > 0 && cantidad != 1 && !unidadesMayorA0.Contains(unidadDeMedida))
                erroresPorCampo["unidad_de_medida"] = "Si la cantidad es mayor a 0 y distinta de 1, solo se permiten palabras plurales.";
            if (cantidad == 1 && !unidadIgualA1.Contains(unidadDeMedida))
                erroresPorCampo["unidad_de_medida"] = "Si la cantidad es igual a 1, solo se permiten palabras singulares.";

            if (erroresPorCampo.Any())
            {
                ViewBag.Errores = erroresPorCampo;
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
                var modelo = new InsumosModel
                {
                    ImplementoEditado = implemento,
                    Implementos = lista
                };

                if (Request.IsAjaxRequest())
                    return PartialView("_FormularioImplemento", modelo);

                return View("implementos", modelo);
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
            if (Request.IsAjaxRequest())
                return Json(new { success = true, message = "¡Implemento actualizado con éxito!" });

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

        [HttpGet]
        public JsonResult VerificarDuplicadoSuministro(
            string nombre, string marca, string presentacion, string proveedor,
            decimal costo, int cantidad, string unidad_de_medida, int id = 0)
        {
            nombre = nombre?.Trim().ToLower() ?? "";
            marca = marca?.Trim().ToLower() ?? "";
            presentacion = presentacion?.Trim().ToLower() ?? "";
            proveedor = proveedor?.Trim().ToLower() ?? "";
            unidad_de_medida = unidad_de_medida?.Trim().ToLower() ?? "";
            decimal costoVal = costo;
            int cantidadVal = cantidad;
            bool isUnique = !db.tabla_suministros.Any(s =>
                s.id != id &&
                (s.nombre ?? "").Trim().ToLower() == nombre &&
                (s.marca ?? "").Trim().ToLower() == marca &&
                (s.presentacion ?? "").Trim().ToLower() == presentacion &&
                (s.proveedor ?? "").Trim().ToLower() == proveedor &&
                (s.costo ?? 0m) == costoVal &&
                (s.cantidad ?? 0) == cantidadVal &&
                (s.unidad_de_medida ?? "").Trim().ToLower() == unidad_de_medida
            );
            return Json(new { isUnique }, JsonRequestBehavior.AllowGet);
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

            var erroresPorCampo = new Dictionary<string, string>();

            // Validaciones por campo
            if (string.IsNullOrWhiteSpace(suministro.nombre))
                erroresPorCampo["nombre"] = "El nombre es obligatorio.";
            if (string.IsNullOrWhiteSpace(suministro.marca))
                erroresPorCampo["marca"] = "La marca es obligatoria.";
            if (string.IsNullOrWhiteSpace(suministro.presentacion))
                erroresPorCampo["presentacion"] = "La presentación es obligatoria.";
            if (string.IsNullOrWhiteSpace(suministro.proveedor))
                erroresPorCampo["proveedor"] = "El proveedor es obligatorio.";
            if (suministro.costo <= 0.99m)
                erroresPorCampo["costo"] = "El costo debe ser mayor a ₡0.99.";
            if (suministro.cantidad <= 0)
                erroresPorCampo["cantidad"] = "La cantidad debe ser mayor a cero.";
            if (string.IsNullOrWhiteSpace(suministro.unidad_de_medida))
                erroresPorCampo["unidad_de_medida"] = "La unidad de medida es obligatoria.";

            // Validaciones de unidades
            var unidadesValidas = new[] { "unidad", "unidades" };
            var unidadesMayorA0 = new[] { "unidades" };
            var unidadIgualA1 = new[] { "unidad" };
            string unidadDeMedida = suministro.unidad_de_medida?.Trim().ToLower() ?? "";
            int cantidad = suministro.cantidad;

            if (!string.IsNullOrWhiteSpace(unidadDeMedida) && !unidadesValidas.Contains(unidadDeMedida))
                erroresPorCampo["unidad_de_medida"] = "Unidad de medida no permitida.";
            if (cantidad > 0 && cantidad != 1 && !unidadesMayorA0.Contains(unidadDeMedida))
                erroresPorCampo["unidad_de_medida"] = "Si la cantidad es mayor a 0 y distinta de 1, solo se permiten palabras plurales.";
            if (cantidad == 1 && !unidadIgualA1.Contains(unidadDeMedida))
                erroresPorCampo["unidad_de_medida"] = "Si la cantidad es igual a 1, solo se permiten palabras singulares.";

            if (erroresPorCampo.Any())
            {
                ViewBag.Errores = erroresPorCampo;
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
                var modelo = new InsumosModel
                {
                    SuministroEditado = suministro,
                    Suministros = lista
                };

                if (Request.IsAjaxRequest())
                    return PartialView("_FormularioSuministro", modelo);

                return View("suministros", modelo);
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
            if (Request.IsAjaxRequest())
                return Json(new { success = true, message = "¡Suministro agregado con éxito!" });

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

            var erroresPorCampo = new Dictionary<string, string>();

            // Validaciones por campo
            if (string.IsNullOrWhiteSpace(suministro.nombre))
                erroresPorCampo["nombre"] = "El nombre es obligatorio.";
            if (string.IsNullOrWhiteSpace(suministro.marca))
                erroresPorCampo["marca"] = "La marca es obligatoria.";
            if (string.IsNullOrWhiteSpace(suministro.presentacion))
                erroresPorCampo["presentacion"] = "La presentación es obligatoria.";
            if (string.IsNullOrWhiteSpace(suministro.proveedor))
                erroresPorCampo["proveedor"] = "El proveedor es obligatorio.";
            if (suministro.costo <= 0.99m)
                erroresPorCampo["costo"] = "El costo debe ser mayor a ₡0.99.";
            if (suministro.cantidad <= 0)
                erroresPorCampo["cantidad"] = "La cantidad debe ser mayor a cero.";
            if (string.IsNullOrWhiteSpace(suministro.unidad_de_medida))
                erroresPorCampo["unidad_de_medida"] = "La unidad de medida es obligatoria.";

            // Duplicado exacto
            string nombre = suministro.nombre?.Trim().ToLower() ?? "";
            string marca = suministro.marca?.Trim().ToLower() ?? "";
            string presentacion = suministro.presentacion?.Trim().ToLower() ?? "";
            string proveedor = suministro.proveedor?.Trim().ToLower() ?? "";
            decimal? costo = suministro.costo;
            int cantidadEd = suministro.cantidad;
            string unidadDeMedidaEd = suministro.unidad_de_medida?.Trim().ToLower() ?? "";

            bool existeExacto = db.tabla_suministros.Any(sumn =>
                sumn.id != suministro.id &&
                sumn.nombre.ToLower() == nombre &&
                sumn.marca.ToLower() == marca &&
                sumn.presentacion.ToLower() == presentacion &&
                sumn.proveedor.ToLower() == proveedor &&
                sumn.costo == costo &&
                sumn.cantidad == cantidadEd &&
                sumn.unidad_de_medida.ToLower() == unidadDeMedidaEd
            );
            if (existeExacto)
                erroresPorCampo["duplicado"] = "Ya existe un suministro con los mismos datos.";

            // Validaciones de unidades
            var unidadesValidas = new[] { "unidad", "unidades" };
            var unidadesMayorA0 = new[] { "unidades" };
            var unidadIgualA1 = new[] { "unidad" };
            string unidadDeMedida = suministro.unidad_de_medida?.Trim().ToLower() ?? "";
            int cantidad = suministro.cantidad;
            if (!string.IsNullOrWhiteSpace(unidadDeMedida) && !unidadesValidas.Contains(unidadDeMedida))
                erroresPorCampo["unidad_de_medida"] = "Unidad de medida no permitida.";
            if (cantidad > 0 && cantidad != 1 && !unidadesMayorA0.Contains(unidadDeMedida))
                erroresPorCampo["unidad_de_medida"] = "Si la cantidad es mayor a 0 y distinta de 1, solo se permiten palabras plurales.";
            if (cantidad == 1 && !unidadIgualA1.Contains(unidadDeMedida))
                erroresPorCampo["unidad_de_medida"] = "Si la cantidad es igual a 1, solo se permiten palabras singulares.";

            if (erroresPorCampo.Any())
            {
                ViewBag.Errores = erroresPorCampo;
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
                var modelo = new InsumosModel
                {
                    SuministroEditado = suministro,
                    Suministros = lista
                };

                if (Request.IsAjaxRequest())
                    return PartialView("_FormularioSuministro", modelo);

                return View("suministros", modelo);
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
            if (Request.IsAjaxRequest())
                return Json(new { success = true, message = "¡Suministro actualizado con éxito!" });
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

        private void CargarListasParaReceta()
        {
            ViewBag.MateriasPrimas = new SelectList(
                db.tabla_materias_primas.ToList()
                    .Select(mp => new
                    {
                        Value = mp.id,
                        Text = $"ID: {mp.id} | {mp.nombre} | Costo por gramo con merma: ₡{mp.costo_por_gramo_con_merma:N2}"
                    }),
                "Value", "Text"
            );

            ViewBag.ProductosPreparados = new SelectList(
                db.tabla_productos_preparados.ToList()
                    .Select(pp => new
                    {
                        Value = pp.id,
                        Text = $"ID: {pp.id} | {pp.nombre} | Costo por peso: ₡{pp.costo_por_peso:N2}"
                    }),
                "Value", "Text"
            );
        }

        // Listar y buscar recetas
        public ActionResult costos_recetas(string search)
        {

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
                    .Select(mp => new
                    {
                        Value = mp.id,
                        Text = $"ID: {mp.id} | {mp.nombre} | Costo por gramo con merma: ₡{mp.costo_por_gramo_con_merma:N2}"
                    }),
                    "Value", "Text"
                );

            ViewBag.ProductosPreparados = new SelectList(
                db.tabla_productos_preparados.ToList()
                    .Select(pp => new
                    {
                        Value = pp.id,
                        Text = $"ID: {pp.id} | {pp.nombre} | Costo por peso: ₡{pp.costo_por_peso:N2}"
                    }),
                "Value", "Text"
            );

            return View(receta);
        }

        [HttpGet]
        public JsonResult VerificarDuplicadoReceta(string nombre, int id = 0)
        {
            nombre = nombre?.Trim().ToLower() ?? "";
            bool isUnique = !db.tabla_costos_recetas.Any(r =>
                r.id != id &&
                (r.nombre ?? "").Trim().ToLower() == nombre
            );
            return Json(new { isUnique }, JsonRequestBehavior.AllowGet);
        }

        // Crear una nueva receta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearReceta(Receta receta)
        {
            var erroresPorCampo = new Dictionary<string, string>();

            // Validaciones por campos
            if (string.IsNullOrWhiteSpace(receta.nombre))
                erroresPorCampo["nombre"] = "El nombre de la receta es obligatorio.";
            if (receta.porcion <= 0)
                erroresPorCampo["porcion"] = "La porción debe ser mayor a cero.";

            decimal costoTotalReceta = 0;

            // Validar filas de Materias Primas
            if (receta.MateriasPrimasUtilizadas != null)
            {
                var idsMP = new HashSet<int>();
                for (int i = 0; i < receta.MateriasPrimasUtilizadas.Count; i++)
                {
                    var mp = receta.MateriasPrimasUtilizadas[i];
                    string prefix = $"MateriaPrima_{i}_";

                    if (mp.id_materia_prima_utilizada == 0 && mp.cantidad == 0 && string.IsNullOrWhiteSpace(mp.unidad_de_medida))
                    {
                        erroresPorCampo[$"{prefix}fila_vacia"] = $"Fila {i + 1}: No puede dejar filas vacías.";
                        continue;
                    }
                    if (mp.id_materia_prima_utilizada == 0)
                        erroresPorCampo[$"{prefix}id"] = $"Fila {i + 1}: Debe seleccionar una materia prima.";
                    if (mp.cantidad <= 0)
                        erroresPorCampo[$"{prefix}cantidad"] = $"Fila {i + 1}: La cantidad debe ser mayor a cero.";
                    if (string.IsNullOrWhiteSpace(mp.unidad_de_medida))
                        erroresPorCampo[$"{prefix}unidad"] = $"Fila {i + 1}: La unidad de medida es obligatoria.";

                    if (mp.id_materia_prima_utilizada != 0)
                    {
                        if (!idsMP.Add(mp.id_materia_prima_utilizada))
                            erroresPorCampo[$"{prefix}repetida"] = $"Fila {i + 1}: Materia prima repetida.";
                        var materia_prima = db.tabla_materias_primas.FirstOrDefault(m => m.id == mp.id_materia_prima_utilizada);
                        if (materia_prima == null)
                        {
                            erroresPorCampo[$"{prefix}no_existe"] = $"Fila {i + 1}: La materia prima seleccionada no existe en el sistema.";
                            continue;
                        }
                        // Validación de unidad de medida
                        var unidadesValidas = new[] { "g", "gr", "grs", "gramo", "gramos", "kg", "kilo", "kilos", "kilogramo", "kilogramos", "ml", "mililitro", "mililitros", "l", "litro", "litros" };
                        string unidad = mp.unidad_de_medida?.Trim().ToLower() ?? "";
                        if (!string.IsNullOrWhiteSpace(unidad) && !unidadesValidas.Contains(unidad))
                            erroresPorCampo[$"{prefix}unidad"] = $"Fila {i + 1}: Unidad de medida no permitida.";

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
                    string prefix = $"ProductoPreparado_{i}_";

                    if (pp.id_producto_preparado_utilizado == 0 && pp.cantidad == 0 && string.IsNullOrWhiteSpace(pp.unidad_de_medida))
                    {
                        erroresPorCampo[$"{prefix}fila_vacia"] = $"Fila {i + 1}: No puede dejar filas vacías.";
                        continue;
                    }
                    if (pp.id_producto_preparado_utilizado == 0)
                        erroresPorCampo[$"{prefix}id"] = $"Fila {i + 1}: Debe seleccionar un producto preparado.";
                    if (pp.cantidad <= 0)
                        erroresPorCampo[$"{prefix}cantidad"] = $"Fila {i + 1}: La cantidad debe ser mayor a cero.";
                    if (string.IsNullOrWhiteSpace(pp.unidad_de_medida))
                        erroresPorCampo[$"{prefix}unidad"] = $"Fila {i + 1}: La unidad de medida es obligatoria.";

                    if (pp.id_producto_preparado_utilizado != 0)
                    {
                        if (!idsPP.Add(pp.id_producto_preparado_utilizado))
                            erroresPorCampo[$"{prefix}repetido"] = $"Fila {i + 1}: Producto preparado repetido.";
                        var producto_preparado = db.tabla_productos_preparados.FirstOrDefault(p => p.id == pp.id_producto_preparado_utilizado);
                        if (producto_preparado == null)
                        {
                            erroresPorCampo[$"{prefix}no_existe"] = $"Fila {i + 1}: El producto preparado seleccionado no existe en el sistema.";
                            continue;
                        }
                        // Validación de unidad de medida
                        var unidadesValidas = new[] { "g", "gr", "grs", "gramo", "gramos", "kg", "kilo", "kilos", "kilogramo", "kilogramos", "ml", "mililitro", "mililitros", "l", "litro", "litros" };
                        string unidad = pp.unidad_de_medida?.Trim().ToLower() ?? "";
                        if (!string.IsNullOrWhiteSpace(unidad) && !unidadesValidas.Contains(unidad))
                            erroresPorCampo[$"{prefix}unidad"] = $"Fila {i + 1}: Unidad de medida no permitida.";

                        // Si existe, asignar valores para cálculos
                        pp.nombre = producto_preparado.nombre;
                        pp.costo_por_cantidad = producto_preparado.costo_por_peso ?? 0m;
                        pp.total_costo = pp.cantidad * pp.costo_por_cantidad;
                    }
                }
            }

            if (erroresPorCampo.Any())
            {
                ViewBag.ErroresPorCampo = erroresPorCampo;
                CargarListasParaReceta();
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

                var modelo = new InsumosModel
                {
                    RecetaEditada = receta,
                    CostosRecetas = lista
                };

                if (Request.IsAjaxRequest())
                    return PartialView("_FormularioCostoReceta", modelo);

                return View("costos_recetas", modelo);
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
            if (Request.IsAjaxRequest())
                return Json(new { success = true, message = "¡Receta agregada con éxito!" });

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
                    .Select(mp => new
                    {
                        Value = mp.id,
                        Text = $"ID: {mp.id} | {mp.nombre} | Costo por gramo con merma: ₡{mp.costo_por_gramo_con_merma:N2}"
                    }),
                    "Value", "Text"
                );

            ViewBag.ProductosPreparados = new SelectList(
                db.tabla_productos_preparados.ToList()
                    .Select(pp => new
                    {
                        Value = pp.id,
                        Text = $"ID: {pp.id} | {pp.nombre} | Costo por peso: ₡{pp.costo_por_peso:N2}"
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
            var erroresPorCampo = new Dictionary<string, string>();

            // Validaciones por campos
            if (string.IsNullOrWhiteSpace(receta.nombre))
                erroresPorCampo["nombre"] = "El nombre de la receta es obligatorio.";
            if (receta.porcion <= 0)
                erroresPorCampo["porcion"] = "La porción debe ser mayor a cero.";

            decimal costoTotalReceta = 0;

            // Validar filas de Materias Primas
            if (receta.MateriasPrimasUtilizadas != null)
            {
                var idsMP = new HashSet<int>();
                for (int i = 0; i < receta.MateriasPrimasUtilizadas.Count; i++)
                {
                    var mp = receta.MateriasPrimasUtilizadas[i];
                    string prefix = $"MateriaPrima_{i}_";

                    if (mp.id_materia_prima_utilizada == 0 && mp.cantidad == 0 && string.IsNullOrWhiteSpace(mp.unidad_de_medida))
                    {
                        erroresPorCampo[$"{prefix}fila_vacia"] = $"Fila {i + 1}: No puede dejar filas vacías.";
                        continue;
                    }
                    if (mp.id_materia_prima_utilizada == 0)
                        erroresPorCampo[$"{prefix}id"] = $"Fila {i + 1}: Debe seleccionar una materia prima.";
                    if (mp.cantidad <= 0)
                        erroresPorCampo[$"{prefix}cantidad"] = $"Fila {i + 1}: La cantidad debe ser mayor a cero.";
                    if (string.IsNullOrWhiteSpace(mp.unidad_de_medida))
                        erroresPorCampo[$"{prefix}unidad"] = $"Fila {i + 1}: La unidad de medida es obligatoria.";

                    if (mp.id_materia_prima_utilizada != 0)
                    {
                        if (!idsMP.Add(mp.id_materia_prima_utilizada))
                            erroresPorCampo[$"{prefix}repetida"] = $"Fila {i + 1}: Materia prima repetida.";
                        var materia_prima = db.tabla_materias_primas.FirstOrDefault(m => m.id == mp.id_materia_prima_utilizada);
                        if (materia_prima == null)
                        {
                            erroresPorCampo[$"{prefix}no_existe"] = $"Fila {i + 1}: La materia prima seleccionada no existe en el sistema.";
                            continue;
                        }
                        // Validación de unidad de medida
                        var unidadesValidas = new[] { "g", "gr", "grs", "gramo", "gramos", "kg", "kilo", "kilos", "kilogramo", "kilogramos", "ml", "mililitro", "mililitros", "l", "litro", "litros" };
                        string unidad = mp.unidad_de_medida?.Trim().ToLower() ?? "";
                        if (!string.IsNullOrWhiteSpace(unidad) && !unidadesValidas.Contains(unidad))
                            erroresPorCampo[$"{prefix}unidad"] = $"Fila {i + 1}: Unidad de medida no permitida.";

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
                    string prefix = $"ProductoPreparado_{i}_";

                    if (pp.id_producto_preparado_utilizado == 0 && pp.cantidad == 0 && string.IsNullOrWhiteSpace(pp.unidad_de_medida))
                    {
                        erroresPorCampo[$"{prefix}fila_vacia"] = $"Fila {i + 1}: No puede dejar filas vacías.";
                        continue;
                    }
                    if (pp.id_producto_preparado_utilizado == 0)
                        erroresPorCampo[$"{prefix}id"] = $"Fila {i + 1}: Debe seleccionar un producto preparado.";
                    if (pp.cantidad <= 0)
                        erroresPorCampo[$"{prefix}cantidad"] = $"Fila {i + 1}: La cantidad debe ser mayor a cero.";
                    if (string.IsNullOrWhiteSpace(pp.unidad_de_medida))
                        erroresPorCampo[$"{prefix}unidad"] = $"Fila {i + 1}: La unidad de medida es obligatoria.";

                    if (pp.id_producto_preparado_utilizado != 0)
                    {
                        if (!idsPP.Add(pp.id_producto_preparado_utilizado))
                            erroresPorCampo[$"{prefix}repetido"] = $"Fila {i + 1}: Producto preparado repetido.";
                        var producto_preparado = db.tabla_productos_preparados.FirstOrDefault(p => p.id == pp.id_producto_preparado_utilizado);
                        if (producto_preparado == null)
                        {
                            erroresPorCampo[$"{prefix}no_existe"] = $"Fila {i + 1}: El producto preparado seleccionado no existe en el sistema.";
                            continue;
                        }
                        // Validación de unidad de medida
                        var unidadesValidas = new[] { "g", "gr", "grs", "gramo", "gramos", "kg", "kilo", "kilos", "kilogramo", "kilogramos", "ml", "mililitro", "mililitros", "l", "litro", "litros" };
                        string unidad = pp.unidad_de_medida?.Trim().ToLower() ?? "";
                        if (!string.IsNullOrWhiteSpace(unidad) && !unidadesValidas.Contains(unidad))
                            erroresPorCampo[$"{prefix}unidad"] = $"Fila {i + 1}: Unidad de medida no permitida.";

                        // Si existe, asignar valores para cálculos
                        pp.nombre = producto_preparado.nombre;
                        pp.costo_por_cantidad = producto_preparado.costo_por_peso ?? 0m;
                        pp.total_costo = pp.cantidad * pp.costo_por_cantidad;
                    }
                }
            }

            // Validación de duplicado por nombre (excluyendo el actual)
            if (db.tabla_costos_recetas.Any(rec => rec.nombre.ToLower() == receta.nombre.ToLower() && rec.id != receta.id))
                erroresPorCampo["duplicado"] = "Ya existe una receta con ese nombre.";

            if (erroresPorCampo.Any())
            {
                ViewBag.Errores = erroresPorCampo;
                ViewBag.Editando = true;
                CargarListasParaReceta();

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

                var modelo = new InsumosModel
                {
                    RecetaEditada = receta,
                    CostosRecetas = lista
                };

                if (Request.IsAjaxRequest())
                    return PartialView("_FormularioCostoReceta", modelo);

                return View("costos_recetas", modelo);
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
            if (Request.IsAjaxRequest())
                return Json(new { success = true, message = "¡Receta actualizada con éxito!" });

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

        private void CargarListasParaProductosFinales()
        {
            ViewBag.Recetas = new SelectList(
                db.tabla_costos_recetas.ToList()
                .Select(r => new
                {
                    Value = r.id,
                    Text = $"ID: {r.id} | {r.nombre} | Costo Total: ₡{r.costo_total_receta:N2}"
                }),
                "Value", "Text"
            );

            ViewBag.EmpaquesDecoraciones = new SelectList(
                db.tabla_empaques_decoraciones.ToList()
                .Select(ed => new
                {
                    Value = ed.id,
                    Text = $"ID: {ed.id} | {ed.nombre} | Costo por cantidad: ₡{ed.costo_por_cantidad:N2}"
                }),
                "Value", "Text"
            );

            ViewBag.Implementos = new SelectList(
                db.tabla_implementos.ToList()
                .Select(i => new
                {
                    Value = i.id,
                    Text = $"ID: {i.id} | {i.nombre} | Costo por cantidad: ₡{i.costo_por_cantidad:N2}"
                }),
                "Value", "Text"
            );

            ViewBag.Suministros = new SelectList(
                db.tabla_suministros.ToList()
                .Select(s => new
                {
                    Value = s.id,
                    Text = $"ID: {s.id} | {s.nombre} | Costo por cantidad: ₡{s.costo_por_cantidad:N2}"
                }),
                "Value", "Text"
            );

        }

        // Listar y buscar productos finales
        public ActionResult precios_finales_sugeridos(string search)
        {

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
                    pf.costo_total_insumos.ToString().Contains(search) ||
                    pf.factura_total.ToString().Contains(search) ||
                    pf.factura_por_insumo.ToString().Contains(search) ||
                    pf.costo_total_de_impresion_de_factura.ToString().Contains(search) ||
                    pf.costo_total_empaque_decoracion_implemento_suministro_por_porcentaje_de_ganancia.ToString().Contains(search) ||
                    pf.porcentaje_de_iva.ToString().Contains(search) ||
                    pf.porcentaje_de_servicio.ToString().Contains(search) ||
                    pf.costo_con_iva.ToString().Contains(search) ||
                    pf.costo_con_servicio.ToString().Contains(search) ||
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
                    costo_total_insumos = pf.costo_total_insumos ?? 0m,
                    costo_de_impresion_de_factura_por_insumo = pf.costo_de_impresion_de_factura_por_insumo ?? 0m,
                    costo_total_de_impresion_de_factura = pf.costo_total_de_impresion_de_factura ?? 0m,
                    factura_total = pf.factura_total ?? 0m,
                    factura_por_insumo = pf.factura_por_insumo ?? 0m,
                    porcentaje_de_iva = pf.porcentaje_de_iva ?? 0m,
                    porcentaje_de_servicio = pf.porcentaje_de_servicio ?? 0m,
                    costo_con_iva = pf.costo_con_iva ?? 0m,
                    costo_con_servicio = pf.costo_con_servicio ?? 0m,
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

            ViewBag.Recetas = db.tabla_costos_recetas.ToList()
                    .Select(r => new SelectListItem
                    {
                        Value = r.id.ToString(),
                        Text = $"ID: {r.id} | {r.nombre} | Costo total: ₡{r.costo_total_receta:N2} | Porción: {r.porcion}"
                    })
                    .ToList();

            ViewBag.EmpaquesDecoraciones = new SelectList(
                db.tabla_empaques_decoraciones.ToList()
                .Select(ed => new
                {
                    Value = ed.id,
                    Text = $"ID: {ed.id} | {ed.nombre} | Costo por cantidad: ₡{ed.costo_por_cantidad:N2}"
                }),
                "Value", "Text"
            );

            ViewBag.Implementos = new SelectList(
                db.tabla_implementos.ToList()
                    .Select(i => new
                    {
                        Value = i.id,
                        Text = $"ID: {i.id} | {i.nombre} | Costo por cantidad: ₡{i.costo_por_cantidad:N2}"
                    }),
                "Value", "Text"
            );

            ViewBag.Suministros = new SelectList(
                db.tabla_suministros.ToList()
                    .Select(s => new
                    {
                        Value = s.id,
                        Text = $"ID: {s.id} | {s.nombre} | Costo por cantidad: ₡{s.costo_por_cantidad:N2}"
                    }),
                "Value", "Text"
            );
            return View(producto_final);
        }

        [HttpGet]
        public JsonResult VerificarDuplicadoProductoFinal(string nombre, int id = 0)
        {
            nombre = nombre?.Trim().ToLower() ?? "";
            bool isUnique = !db.tabla_precios_finales_sugeridos.Any(r =>
                r.id != id &&
                (r.nombre_receta ?? "").Trim().ToLower() == nombre
            );
            return Json(new { isUnique }, JsonRequestBehavior.AllowGet);
        }

        // Crear un nuevo producto final
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearProductoFinal(ProductoFinal producto_final)
        {
            int idReceta = 0;
            int.TryParse(Request.Form["id_receta"], out idReceta);
            var receta = db.tabla_costos_recetas.FirstOrDefault(r => r.id == idReceta);
            if (receta != null)
                producto_final.nombre_receta = receta.nombre;
            else
                producto_final.nombre_receta = "";

            if (producto_final.SuministrosUtilizados != null)
            {
                for (int i = 0; i < producto_final.SuministrosUtilizados.Count; i++)
                {
                    var key = $"SuministrosUtilizados[{i}].es_impresion_de_facturas";
                    producto_final.SuministrosUtilizados[i].es_impresion_de_facturas = Request.Form[key] == "on";
                }
            }

            var erroresPorCampo = new Dictionary<string, string>();

            // Validaciones por campo principal
            if (idReceta == 0 || string.IsNullOrWhiteSpace(producto_final.nombre_receta))
                erroresPorCampo["nombre_receta"] = "El nombre de la receta es obligatorio. Debe seleccionar una receta.";
            if (receta == null)
                erroresPorCampo["receta"] = "La receta seleccionada no existe en la base de datos.";
            if (producto_final.margen_de_utilidad < 0 || producto_final.margen_de_utilidad > 100)
                erroresPorCampo["margen_de_utilidad"] = "El margen de utilidad debe estar entre 0 y 100.";
            if (db.tabla_precios_finales_sugeridos.Any(p => p.nombre_receta.ToLower() == producto_final.nombre_receta.ToLower() && p.id != producto_final.id))
                erroresPorCampo["duplicado"] = "Ya existe un producto final para esa receta.";

            // Validaciones de detalles
            // Empaques/Decoraciones
            if (producto_final.EmpaquesDecoracionesUtilizados != null)
            {
                var idsEmpaques = new HashSet<int>();
                var unidadesValidas = new[] { "unidad", "unidades" };
                var unidadesMayorA0 = new[] { "unidades" };
                var unidadIgualA1 = new[] { "unidad" };

                for (int i = 0; i < producto_final.EmpaquesDecoracionesUtilizados.Count; i++)
                {
                    var ed = producto_final.EmpaquesDecoracionesUtilizados[i];
                    string prefix = $"Empaque_{i}_";

                    if ((ed.id_empaque_decoracion_utilizado == 0) && ed.cantidad == 0 && string.IsNullOrWhiteSpace(ed.unidad_de_medida))
                    {
                        erroresPorCampo[$"{prefix}fila_vacia"] = $"Fila {i + 1}: No puede dejar filas vacías.";
                        continue;
                    }
                    if (ed.id_empaque_decoracion_utilizado == 0)
                        erroresPorCampo[$"{prefix}id"] = $"Fila {i + 1}: Debe seleccionar un empaque/decoración.";
                    if (ed.cantidad <= 0)
                        erroresPorCampo[$"{prefix}cantidad"] = $"Fila {i + 1}: La cantidad debe ser mayor a cero.";
                    if (string.IsNullOrWhiteSpace(ed.unidad_de_medida))
                        erroresPorCampo[$"{prefix}unidad"] = $"Fila {i + 1}: La unidad de medida es obligatoria.";

                    string unidad = ed.unidad_de_medida?.Trim().ToLower() ?? "";
                    if (!string.IsNullOrWhiteSpace(unidad) && !unidadesValidas.Contains(unidad))
                        erroresPorCampo[$"{prefix}unidad_valida"] = $"Fila {i + 1}: Unidad de medida no permitida.";
                    if (ed.cantidad > 0 && ed.cantidad != 1 && !unidadesMayorA0.Contains(unidad))
                        erroresPorCampo[$"{prefix}unidad_plural"] = $"Fila {i + 1}: Si la cantidad es mayor a 0 y distinta de 1, solo se permiten palabras plurales.";
                    if (ed.cantidad == 1 && !unidadIgualA1.Contains(unidad))
                        erroresPorCampo[$"{prefix}unidad_singular"] = $"Fila {i + 1}: Si la cantidad es igual a 1, solo se permiten palabras singulares.";

                    if (ed.id_empaque_decoracion_utilizado != 0)
                    {
                        if (!idsEmpaques.Add(ed.id_empaque_decoracion_utilizado))
                            erroresPorCampo[$"{prefix}repetido"] = $"Fila {i + 1}: Empaque/decoración repetido.";
                        var empaque = db.tabla_empaques_decoraciones.FirstOrDefault(x => x.id == ed.id_empaque_decoracion_utilizado);
                        if (empaque == null)
                        {
                            erroresPorCampo[$"{prefix}no_existe"] = $"Fila {i + 1}: El empaque/decoración seleccionado no existe.";
                            continue;
                        }
                        ed.nombre = empaque.nombre;
                        ed.costo_por_cantidad = empaque.costo_por_cantidad ?? 0m;
                        ed.total_costo = ed.cantidad * ed.costo_por_cantidad;
                    }
                }
            }

            // Implementos
            if (producto_final.ImplementosUtilizados != null)
            {
                var idsImplementos = new HashSet<int>();
                var unidadesValidas = new[] { "unidad", "unidades" };
                var unidadesMayorA0 = new[] { "unidades" };
                var unidadIgualA1 = new[] { "unidad" };

                for (int i = 0; i < producto_final.ImplementosUtilizados.Count; i++)
                {
                    var impl = producto_final.ImplementosUtilizados[i];
                    string prefix = $"Implemento_{i}_";

                    if ((impl.id_implemento_utilizado == 0) && impl.cantidad == 0 && string.IsNullOrWhiteSpace(impl.unidad_de_medida))
                    {
                        erroresPorCampo[$"{prefix}fila_vacia"] = $"Fila {i + 1}: No puede dejar filas vacías.";
                        continue;
                    }
                    if (impl.id_implemento_utilizado == 0)
                        erroresPorCampo[$"{prefix}id"] = $"Fila {i + 1}: Debe seleccionar un implemento.";
                    if (impl.cantidad <= 0)
                        erroresPorCampo[$"{prefix}cantidad"] = $"Fila {i + 1}: La cantidad debe ser mayor a cero.";
                    if (string.IsNullOrWhiteSpace(impl.unidad_de_medida))
                        erroresPorCampo[$"{prefix}unidad"] = $"Fila {i + 1}: La unidad de medida es obligatoria.";

                    string unidad = impl.unidad_de_medida?.Trim().ToLower() ?? "";
                    if (!string.IsNullOrWhiteSpace(unidad) && !unidadesValidas.Contains(unidad))
                        erroresPorCampo[$"{prefix}unidad_valida"] = $"Fila {i + 1}: Unidad de medida no permitida.";
                    if (impl.cantidad > 0 && impl.cantidad != 1 && !unidadesMayorA0.Contains(unidad))
                        erroresPorCampo[$"{prefix}unidad_plural"] = $"Fila {i + 1}: Si la cantidad es mayor a 0 y distinta de 1, solo se permiten palabras plurales.";
                    if (impl.cantidad == 1 && !unidadIgualA1.Contains(unidad))
                        erroresPorCampo[$"{prefix}unidad_singular"] = $"Fila {i + 1}: Si la cantidad es igual a 1, solo se permiten palabras singulares.";

                    if (impl.id_implemento_utilizado != 0)
                    {
                        if (!idsImplementos.Add(impl.id_implemento_utilizado))
                            erroresPorCampo[$"{prefix}repetido"] = $"Fila {i + 1}: Implemento repetido.";
                        var implemento = db.tabla_implementos.FirstOrDefault(x => x.id == impl.id_implemento_utilizado);
                        if (implemento == null)
                        {
                            erroresPorCampo[$"{prefix}no_existe"] = $"Fila {i + 1}: El implemento seleccionado no existe.";
                            continue;
                        }
                        impl.nombre = implemento.nombre;
                        impl.costo_por_cantidad = implemento.costo_por_cantidad ?? 0m;
                        impl.total_costo = impl.cantidad * impl.costo_por_cantidad;
                    }
                }
            }

            // Suministros
            if (producto_final.SuministrosUtilizados != null)
            {
                var idsSuministros = new HashSet<int>();
                var unidadesValidas = new[] { "unidad", "unidades" };
                var unidadesMayorA0 = new[] { "unidades" };
                var unidadIgualA1 = new[] { "unidad" };

                for (int i = 0; i < producto_final.SuministrosUtilizados.Count; i++)
                {
                    var sumn = producto_final.SuministrosUtilizados[i];
                    string prefix = $"Suministro_{i}_";

                    if ((sumn.id_suministro_utilizado == 0) && sumn.cantidad == 0 && string.IsNullOrWhiteSpace(sumn.unidad_de_medida))
                    {
                        erroresPorCampo[$"{prefix}fila_vacia"] = $"Fila {i + 1}: No puede dejar filas vacías.";
                        continue;
                    }
                    if (sumn.id_suministro_utilizado == 0)
                        erroresPorCampo[$"{prefix}id"] = $"Fila {i + 1}: Debe seleccionar un suministro.";
                    if (sumn.cantidad <= 0)
                        erroresPorCampo[$"{prefix}cantidad"] = $"Fila {i + 1}: La cantidad debe ser mayor a cero.";
                    if (string.IsNullOrWhiteSpace(sumn.unidad_de_medida))
                        erroresPorCampo[$"{prefix}unidad"] = $"Fila {i + 1}: La unidad de medida es obligatoria.";

                    string unidad = sumn.unidad_de_medida?.Trim().ToLower() ?? "";
                    if (!string.IsNullOrWhiteSpace(unidad) && !unidadesValidas.Contains(unidad))
                        erroresPorCampo[$"{prefix}unidad_valida"] = $"Fila {i + 1}: Unidad de medida no permitida.";
                    if (sumn.cantidad > 0 && sumn.cantidad != 1 && !unidadesMayorA0.Contains(unidad))
                        erroresPorCampo[$"{prefix}unidad_plural"] = $"Fila {i + 1}: Si la cantidad es mayor a 0 y distinta de 1, solo se permiten palabras plurales.";
                    if (sumn.cantidad == 1 && !unidadIgualA1.Contains(unidad))
                        erroresPorCampo[$"{prefix}unidad_singular"] = $"Fila {i + 1}: Si la cantidad es igual a 1, solo se permiten palabras singulares.";

                    if (sumn.id_suministro_utilizado != 0)
                    {
                        if (!idsSuministros.Add(sumn.id_suministro_utilizado))
                            erroresPorCampo[$"{prefix}repetido"] = $"Fila {i + 1}: Suministro repetido.";
                        var suministro = db.tabla_suministros.FirstOrDefault(x => x.id == sumn.id_suministro_utilizado);
                        if (suministro == null)
                        {
                            erroresPorCampo[$"{prefix}no_existe"] = $"Fila {i + 1}: El suministro seleccionado no existe.";
                            continue;
                        }
                        sumn.nombre = suministro.nombre;
                        sumn.costo_por_cantidad = suministro.costo_por_cantidad ?? 0m;
                        sumn.total_costo = sumn.cantidad * sumn.costo_por_cantidad;
                    }
                }
            }

            if (erroresPorCampo.Any())
            {
                ViewBag.Errores = erroresPorCampo;
                ViewBag.Editando = false;
                ViewBag.Recetas = db.tabla_costos_recetas.ToList()
                    .Select(r => new SelectListItem
                    {
                        Value = r.id.ToString(),
                        Text = $"ID: {r.id} | {r.nombre} | Costo total: ₡{r.costo_total_receta:N2} | Porción: {r.porcion}"
                    })
                    .ToList();

                ViewBag.EmpaquesDecoraciones = new SelectList(
                    db.tabla_empaques_decoraciones.ToList()
                    .Select(ed => new
                    {
                        Value = ed.id,
                        Text = $"ID: {ed.id} | {ed.nombre} | Costo por cantidad: ₡{ed.costo_por_cantidad:N2}"
                    }),
                    "Value", "Text"
                );

                ViewBag.Implementos = new SelectList(
                    db.tabla_implementos.ToList()
                        .Select(i => new
                        {
                            Value = i.id,
                            Text = $"ID: {i.id} | {i.nombre} | Costo por cantidad: ₡{i.costo_por_cantidad:N2}"
                        }),
                    "Value", "Text"
                );

                ViewBag.Suministros = new SelectList(
                    db.tabla_suministros.ToList()
                        .Select(s => new
                        {
                            Value = s.id,
                            Text = $"ID: {s.id} | {s.nombre} | Costo por cantidad: ₡{s.costo_por_cantidad:N2}"
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
                    costo_total_insumos = pf.costo_total_insumos ?? 0m,
                    factura_total = pf.factura_total ?? 0m,
                    factura_por_insumo = pf.factura_por_insumo ?? 0m,
                    costo_total_de_impresion_de_factura = pf.costo_total_de_impresion_de_factura ?? 0m,
                    porcentaje_de_iva = pf.porcentaje_de_iva ?? 0m,
                    porcentaje_de_servicio = pf.porcentaje_de_servicio ?? 0m,
                    costo_con_iva = pf.costo_con_iva ?? 0m,
                    costo_con_servicio = pf.costo_con_servicio ?? 0m,
                    envio = pf.envio ?? 0m,
                    plataforma_de_envio = pf.plataforma_de_envio,
                    precio_final_sugerido = pf.precio_final_sugerido ?? 0m,
                }).ToList();

                var modelo = new InsumosModel
                {
                    ProductoFinalEditado = producto_final,
                    ProductosFinales = productosFinales
                };

                if (Request.IsAjaxRequest())
                    return PartialView("_FormularioPrecioFinalSugerido", modelo);

                return View("precios_finales_sugeridos", modelo);
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

            decimal costoTotalInsumos = sumaEmpaquesPorCantidad + sumaImplementosPorCantidad + sumaSuministrosPorCantidad;

            // Factura por insumo: suma de todos los costos individuales + impresión por insumo
            decimal facturaPorInsumo = costoTotalInsumos + costoImpresionFacturaPorInsumo;

            // Factura total: suma de todos los totales + impresión total
            decimal facturaTotal = costoTotalInsumos + costoTotalImpresionFactura;

            // Total insumos con porcentaje de ganancia
            decimal totalInsumosConGanancia = facturaTotal * 1.10m;

            // IVA y Servicio
            decimal porcentajeIva = producto_final.porcentaje_de_iva;
            decimal porcentajeServicio = producto_final.porcentaje_de_servicio;
            decimal baseImpuestos = costoConUtilidad + totalInsumosConGanancia;
            decimal costoConIva = baseImpuestos * (porcentajeIva / 100m);
            decimal costoConServicio = baseImpuestos * (porcentajeServicio / 100m);

            // Envío según plataforma
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
            decimal precioFinalSugerido = baseImpuestos + costoConIva + costoConServicio + envio;

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
                costo_total_insumos = costoTotalInsumos,
                costo_de_impresion_de_factura_por_insumo = costoImpresionFacturaPorInsumo,
                costo_total_de_impresion_de_factura = costoTotalImpresionFactura,
                costo_total_empaque_decoracion_implemento_suministro_por_porcentaje_de_ganancia = totalInsumosConGanancia,
                factura_por_insumo = facturaPorInsumo,
                factura_total = facturaTotal,
                porcentaje_de_iva = porcentajeIva,
                porcentaje_de_servicio = porcentajeServicio,
                costo_con_iva = costoConIva,
                costo_con_servicio = costoConServicio,
                envio = envio,
                plataforma_de_envio = producto_final.plataforma_de_envio,
                precio_final_sugerido = precioFinalSugerido
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
            if (Request.IsAjaxRequest())
                return Json(new { success = true, message = "¡Producto Final agregado con éxito!" });

            return RedirectToAction("precios_finales_sugeridos");
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
                id_receta = pf.id_receta ?? 0,
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
                porcentaje_de_iva = pf.porcentaje_de_iva ?? 0m,
                porcentaje_de_servicio = pf.porcentaje_de_servicio ?? 0m,
                costo_con_iva = pf.costo_con_iva ?? 0m,
                costo_con_servicio = pf.costo_con_servicio ?? 0m,
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
                porcentaje_de_iva = prodfinal.porcentaje_de_iva ?? 0m,
                porcentaje_de_servicio = prodfinal.porcentaje_de_servicio ?? 0m,
                costo_con_iva = prodfinal.costo_con_iva ?? 0m,
                costo_con_servicio = prodfinal.costo_con_servicio ?? 0m,
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

            ViewBag.Recetas = db.tabla_costos_recetas.ToList()
                    .Select(r => new SelectListItem
                    {
                        Value = r.id.ToString(),
                        Text = $"ID: {r.id} | {r.nombre} | Costo total: ₡{r.costo_total_receta:N2} | Porción: {r.porcion}",
                        Selected = r.id == producto_final.id_receta
                    }).ToList();

            ViewBag.EmpaquesDecoraciones = new SelectList(
                db.tabla_empaques_decoraciones.ToList()
                .Select(ed => new
                {
                    Value = ed.id,
                    Text = $"ID: {ed.id} | {ed.nombre} | Costo por cantidad: ₡{ed.costo_por_cantidad:N2}"
                }),
                "Value", "Text"
            );

            ViewBag.Implementos = new SelectList(
                db.tabla_implementos.ToList()
                    .Select(i => new
                    {
                        Value = i.id,
                        Text = $"ID: {i.id} | {i.nombre} | Costo por cantidad: ₡{i.costo_por_cantidad:N2}"
                    }),
                "Value", "Text"
            );

            ViewBag.Suministros = new SelectList(
                db.tabla_suministros.ToList()
                    .Select(s => new
                    {
                        Value = s.id,
                        Text = $"ID: {s.id} | {s.nombre} | Costo por cantidad: ₡{s.costo_por_cantidad:N2}"
                    }),
                "Value", "Text"
            );

            ViewBag.Editando = true;
            return View("precios_finales_sugeridos", new InsumosModel
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
            int idReceta = 0;
            int.TryParse(Request.Form["id_receta"], out idReceta);
            var receta = db.tabla_costos_recetas.FirstOrDefault(r => r.id == idReceta);
            if (receta != null)
                producto_final.nombre_receta = receta.nombre;
            else
                producto_final.nombre_receta = "";

            if (producto_final.SuministrosUtilizados != null)
            {
                for (int i = 0; i < producto_final.SuministrosUtilizados.Count; i++)
                {
                    var key = $"SuministrosUtilizados[{i}].es_impresion_de_facturas";
                    producto_final.SuministrosUtilizados[i].es_impresion_de_facturas = Request.Form[key] == "on";
                }
            }

            var erroresPorCampo = new Dictionary<string, string>();

            // Validaciones por campo principal
            if (idReceta == 0 || string.IsNullOrWhiteSpace(producto_final.nombre_receta))
                erroresPorCampo["nombre_receta"] = "El nombre de la receta es obligatorio. Debe seleccionar una receta.";
            if (receta == null)
                erroresPorCampo["receta"] = "La receta seleccionada no existe en la base de datos.";
            if (producto_final.margen_de_utilidad < 0 || producto_final.margen_de_utilidad > 100)
                erroresPorCampo["margen_de_utilidad"] = "El margen de utilidad debe estar entre 0 y 100.";
            if (db.tabla_precios_finales_sugeridos.Any(p => p.nombre_receta.ToLower() == producto_final.nombre_receta.ToLower() && p.id != producto_final.id))
                erroresPorCampo["duplicado"] = "Ya existe un producto final para esa receta.";

            // Validaciones de detalles
            // Empaques/Decoraciones
            if (producto_final.EmpaquesDecoracionesUtilizados != null)
            {
                var idsEmpaques = new HashSet<int>();
                var unidadesValidas = new[] { "unidad", "unidades" };
                var unidadesMayorA0 = new[] { "unidades" };
                var unidadIgualA1 = new[] { "unidad" };

                for (int i = 0; i < producto_final.EmpaquesDecoracionesUtilizados.Count; i++)
                {
                    var ed = producto_final.EmpaquesDecoracionesUtilizados[i];
                    string prefix = $"Empaque_{i}_";

                    if ((ed.id_empaque_decoracion_utilizado == 0) && ed.cantidad == 0 && string.IsNullOrWhiteSpace(ed.unidad_de_medida))
                    {
                        erroresPorCampo[$"{prefix}fila_vacia"] = $"Fila {i + 1}: No puede dejar filas vacías.";
                        continue;
                    }
                    if (ed.id_empaque_decoracion_utilizado == 0)
                        erroresPorCampo[$"{prefix}id"] = $"Fila {i + 1}: Debe seleccionar un empaque/decoración.";
                    if (ed.cantidad <= 0)
                        erroresPorCampo[$"{prefix}cantidad"] = $"Fila {i + 1}: La cantidad debe ser mayor a cero.";
                    if (string.IsNullOrWhiteSpace(ed.unidad_de_medida))
                        erroresPorCampo[$"{prefix}unidad"] = $"Fila {i + 1}: La unidad de medida es obligatoria.";

                    string unidad = ed.unidad_de_medida?.Trim().ToLower() ?? "";
                    if (!string.IsNullOrWhiteSpace(unidad) && !unidadesValidas.Contains(unidad))
                        erroresPorCampo[$"{prefix}unidad_valida"] = $"Fila {i + 1}: Unidad de medida no permitida.";
                    if (ed.cantidad > 0 && ed.cantidad != 1 && !unidadesMayorA0.Contains(unidad))
                        erroresPorCampo[$"{prefix}unidad_plural"] = $"Fila {i + 1}: Si la cantidad es mayor a 0 y distinta de 1, solo se permiten palabras plurales.";
                    if (ed.cantidad == 1 && !unidadIgualA1.Contains(unidad))
                        erroresPorCampo[$"{prefix}unidad_singular"] = $"Fila {i + 1}: Si la cantidad es igual a 1, solo se permiten palabras singulares.";

                    if (ed.id_empaque_decoracion_utilizado != 0)
                    {
                        if (!idsEmpaques.Add(ed.id_empaque_decoracion_utilizado))
                            erroresPorCampo[$"{prefix}repetido"] = $"Fila {i + 1}: Empaque/decoración repetido.";
                        var empaque = db.tabla_empaques_decoraciones.FirstOrDefault(x => x.id == ed.id_empaque_decoracion_utilizado);
                        if (empaque == null)
                        {
                            erroresPorCampo[$"{prefix}no_existe"] = $"Fila {i + 1}: El empaque/decoración seleccionado no existe.";
                            continue;
                        }
                        ed.nombre = empaque.nombre;
                        ed.costo_por_cantidad = empaque.costo_por_cantidad ?? 0m;
                        ed.total_costo = ed.cantidad * ed.costo_por_cantidad;
                    }
                }
            }

            // Implementos
            if (producto_final.ImplementosUtilizados != null)
            {
                var idsImplementos = new HashSet<int>();
                var unidadesValidas = new[] { "unidad", "unidades" };
                var unidadesMayorA0 = new[] { "unidades" };
                var unidadIgualA1 = new[] { "unidad" };

                for (int i = 0; i < producto_final.ImplementosUtilizados.Count; i++)
                {
                    var impl = producto_final.ImplementosUtilizados[i];
                    string prefix = $"Implemento_{i}_";

                    if ((impl.id_implemento_utilizado == 0) && impl.cantidad == 0 && string.IsNullOrWhiteSpace(impl.unidad_de_medida))
                    {
                        erroresPorCampo[$"{prefix}fila_vacia"] = $"Fila {i + 1}: No puede dejar filas vacías.";
                        continue;
                    }
                    if (impl.id_implemento_utilizado == 0)
                        erroresPorCampo[$"{prefix}id"] = $"Fila {i + 1}: Debe seleccionar un implemento.";
                    if (impl.cantidad <= 0)
                        erroresPorCampo[$"{prefix}cantidad"] = $"Fila {i + 1}: La cantidad debe ser mayor a cero.";
                    if (string.IsNullOrWhiteSpace(impl.unidad_de_medida))
                        erroresPorCampo[$"{prefix}unidad"] = $"Fila {i + 1}: La unidad de medida es obligatoria.";

                    string unidad = impl.unidad_de_medida?.Trim().ToLower() ?? "";
                    if (!string.IsNullOrWhiteSpace(unidad) && !unidadesValidas.Contains(unidad))
                        erroresPorCampo[$"{prefix}unidad_valida"] = $"Fila {i + 1}: Unidad de medida no permitida.";
                    if (impl.cantidad > 0 && impl.cantidad != 1 && !unidadesMayorA0.Contains(unidad))
                        erroresPorCampo[$"{prefix}unidad_plural"] = $"Fila {i + 1}: Si la cantidad es mayor a 0 y distinta de 1, solo se permiten palabras plurales.";
                    if (impl.cantidad == 1 && !unidadIgualA1.Contains(unidad))
                        erroresPorCampo[$"{prefix}unidad_singular"] = $"Fila {i + 1}: Si la cantidad es igual a 1, solo se permiten palabras singulares.";

                    if (impl.id_implemento_utilizado != 0)
                    {
                        if (!idsImplementos.Add(impl.id_implemento_utilizado))
                            erroresPorCampo[$"{prefix}repetido"] = $"Fila {i + 1}: Implemento repetido.";
                        var implemento = db.tabla_implementos.FirstOrDefault(x => x.id == impl.id_implemento_utilizado);
                        if (implemento == null)
                        {
                            erroresPorCampo[$"{prefix}no_existe"] = $"Fila {i + 1}: El implemento seleccionado no existe.";
                            continue;
                        }
                        impl.nombre = implemento.nombre;
                        impl.costo_por_cantidad = implemento.costo_por_cantidad ?? 0m;
                        impl.total_costo = impl.cantidad * impl.costo_por_cantidad;
                    }
                }
            }

            // Suministros
            if (producto_final.SuministrosUtilizados != null)
            {
                var idsSuministros = new HashSet<int>();
                var unidadesValidas = new[] { "unidad", "unidades" };
                var unidadesMayorA0 = new[] { "unidades" };
                var unidadIgualA1 = new[] { "unidad" };

                for (int i = 0; i < producto_final.SuministrosUtilizados.Count; i++)
                {
                    var sumn = producto_final.SuministrosUtilizados[i];
                    string prefix = $"Suministro_{i}_";

                    if ((sumn.id_suministro_utilizado == 0) && sumn.cantidad == 0 && string.IsNullOrWhiteSpace(sumn.unidad_de_medida))
                    {
                        erroresPorCampo[$"{prefix}fila_vacia"] = $"Fila {i + 1}: No puede dejar filas vacías.";
                        continue;
                    }
                    if (sumn.id_suministro_utilizado == 0)
                        erroresPorCampo[$"{prefix}id"] = $"Fila {i + 1}: Debe seleccionar un suministro.";
                    if (sumn.cantidad <= 0)
                        erroresPorCampo[$"{prefix}cantidad"] = $"Fila {i + 1}: La cantidad debe ser mayor a cero.";
                    if (string.IsNullOrWhiteSpace(sumn.unidad_de_medida))
                        erroresPorCampo[$"{prefix}unidad"] = $"Fila {i + 1}: La unidad de medida es obligatoria.";

                    string unidad = sumn.unidad_de_medida?.Trim().ToLower() ?? "";
                    if (!string.IsNullOrWhiteSpace(unidad) && !unidadesValidas.Contains(unidad))
                        erroresPorCampo[$"{prefix}unidad_valida"] = $"Fila {i + 1}: Unidad de medida no permitida.";
                    if (sumn.cantidad > 0 && sumn.cantidad != 1 && !unidadesMayorA0.Contains(unidad))
                        erroresPorCampo[$"{prefix}unidad_plural"] = $"Fila {i + 1}: Si la cantidad es mayor a 0 y distinta de 1, solo se permiten palabras plurales.";
                    if (sumn.cantidad == 1 && !unidadIgualA1.Contains(unidad))
                        erroresPorCampo[$"{prefix}unidad_singular"] = $"Fila {i + 1}: Si la cantidad es igual a 1, solo se permiten palabras singulares.";

                    if (sumn.id_suministro_utilizado != 0)
                    {
                        if (!idsSuministros.Add(sumn.id_suministro_utilizado))
                            erroresPorCampo[$"{prefix}repetido"] = $"Fila {i + 1}: Suministro repetido.";
                        var suministro = db.tabla_suministros.FirstOrDefault(x => x.id == sumn.id_suministro_utilizado);
                        if (suministro == null)
                        {
                            erroresPorCampo[$"{prefix}no_existe"] = $"Fila {i + 1}: El suministro seleccionado no existe.";
                            continue;
                        }
                        sumn.nombre = suministro.nombre;
                        sumn.costo_por_cantidad = suministro.costo_por_cantidad ?? 0m;
                        sumn.total_costo = sumn.cantidad * sumn.costo_por_cantidad;
                    }
                }
            }

            if (erroresPorCampo.Any())
            {
                ViewBag.Errores = erroresPorCampo;
                ViewBag.Editando = true;
                CargarListasParaProductosFinales();

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
                    porcentaje_de_iva = pf.porcentaje_de_iva ?? 0m,
                    porcentaje_de_servicio = pf.porcentaje_de_servicio ?? 0m,
                    costo_con_iva = pf.costo_con_iva ?? 0m,
                    costo_con_servicio = pf.costo_con_servicio ?? 0m,
                    envio = pf.envio ?? 0m,
                    plataforma_de_envio = pf.plataforma_de_envio,
                }).ToList();

                var modelo = new InsumosModel
                {
                    ProductoFinalEditado = producto_final,
                    ProductosFinales = productosFinales
                };

                if (Request.IsAjaxRequest())
                    return PartialView("_FormularioPrecioFinalSugerido", modelo);

                return View("precios_finales_sugeridos", modelo);
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

            decimal costoTotalInsumos = sumaEmpaquesPorCantidad + sumaImplementosPorCantidad + sumaSuministrosPorCantidad;

            // Factura por insumo: suma de todos los costos individuales + impresión por insumo
            decimal facturaPorInsumo = costoTotalInsumos + costoImpresionFacturaPorInsumo;

            // Factura total: suma de todos los totales + impresión total
            decimal facturaTotal = costoTotalInsumos + costoTotalImpresionFactura;

            // Total insumos con porcentaje de ganancia
            decimal totalInsumosConGanancia = facturaTotal * 1.10m;

            // IVA y Servicio
            decimal porcentajeIva = producto_final.porcentaje_de_iva;
            decimal porcentajeServicio = producto_final.porcentaje_de_servicio;
            decimal baseImpuestos = costoConUtilidad + totalInsumosConGanancia;
            decimal costoConIva = baseImpuestos * (porcentajeIva / 100m);
            decimal costoConServicio = baseImpuestos * (porcentajeServicio / 100m);

            // Envío según plataforma
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
            decimal precioFinalSugerido = baseImpuestos + costoConIva + costoConServicio + envio;

            // Actualizar campos principales
            producto_final.id_receta = receta.id;
            producto_final.nombre_receta = producto_final.nombre_receta;
            producto_final.costo_total_receta = costoReceta;
            producto_final.margen_de_utilidad = margenUtilidad;
            producto_final.costo_sin_margen_de_utilidad = costoReceta;
            producto_final.costo_con_margen_de_utilidad = costoConUtilidad;
            producto_final.costo_empaque_decoracion_utilizado = sumaEmpaquesPorCantidad;
            producto_final.costo_implemento_utilizado = sumaImplementosPorCantidad;
            producto_final.costo_suministro_utilizado = totalSuministros;
            producto_final.costo_total_insumos = costoTotalInsumos;
            producto_final.costo_de_impresion_de_factura_por_insumo = costoImpresionFacturaPorInsumo;
            producto_final.costo_total_de_impresion_de_factura = costoTotalImpresionFactura;
            producto_final.costo_total_empaque_decoracion_implemento_suministro_por_porcentaje_de_ganancia = totalInsumosConGanancia;
            producto_final.factura_por_insumo = facturaPorInsumo;
            producto_final.factura_total = facturaTotal;
            producto_final.porcentaje_de_iva = porcentajeIva;
            producto_final.porcentaje_de_servicio = porcentajeServicio;
            producto_final.costo_con_iva = costoConIva;
            producto_final.costo_con_servicio = costoConServicio;
            producto_final.envio = envio;
            producto_final.plataforma_de_envio = producto_final.plataforma_de_envio;
            producto_final.precio_final_sugerido = precioFinalSugerido;

            // Eliminar detalles existentes
            var empaques = db.precios_empaques_decoraciones_utilizados.Where(x => x.id_precio_final_sugerido == producto_final.id).ToList();
            foreach (var item in empaques) db.precios_empaques_decoraciones_utilizados.Remove(item);

            var implementos = db.precios_implementos_utilizados.Where(x => x.id_precio_final_sugerido == producto_final.id).ToList();
            foreach (var item in implementos) db.precios_implementos_utilizados.Remove(item);

            var suministros = db.precios_suministros_utilizados.Where(x => x.id_precio_final_sugerido == producto_final.id).ToList();
            foreach (var item in suministros) db.precios_suministros_utilizados.Remove(item);

            // Agregar nuevos detalles
            if (producto_final.EmpaquesDecoracionesUtilizados != null)
            {
                foreach (var ed in producto_final.EmpaquesDecoracionesUtilizados)
                {
                    db.precios_empaques_decoraciones_utilizados.Add(new precios_empaques_decoraciones_utilizados
                    {
                        id_precio_final_sugerido = producto_final.id,
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
                        id_precio_final_sugerido = producto_final.id,
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
                        id_precio_final_sugerido = producto_final.id,
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
            if (Request.IsAjaxRequest())
                return Json(new { success = true, message = "¡Producto Final actualizado con éxito!" });

            return RedirectToAction("precios_finales_sugeridos");
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
            return RedirectToAction("precios_finales_sugeridos");
        }
    }
}

