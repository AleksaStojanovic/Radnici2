using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Radnici.Data;
using Radnici.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using OfficeOpenXml;
using System.Drawing.Printing;

using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Org.BouncyCastle.Asn1.Crmf;
using RestSharp.Authenticators;
using RestSharp;
using Humanizer;
using Radnici.Currency;
using Radnici.Logic;
using NuGet.Protocol;

namespace Radnici.Controllers
{
    public class RadniksController : Controller
    {
        private readonly RadniciContext _context;

        public RadniksController(RadniciContext context)
        {
            _context = context;
        }

        // GET: Radniks


        public async Task<IActionResult> Index(string searchString)
        {
            if (_context.Radnik == null)
            {
                return Problem("Entity set 'RadniciContext.Radnik' is null.");
            }

            var radnici = from r in _context.Radnik select r;

            if (!System.String.IsNullOrEmpty(searchString))
            {
                radnici = radnici.Where(s => s.ime!.Contains(searchString));
            }

            return View(await radnici.ToListAsync());
        }

        public async Task<IActionResult> ExportToCSV()
        {
            if (_context.Radnik == null)
            {
                return Problem("Entity set 'RadniciContext.Radnik' is null.");
            }

            var radnici = from r in _context.Radnik select r;


            string filePath = "C:\\Users\\aleks\\OneDrive\\Desktop\\Radnici-Current - Copy (2)\\Radnici\\Saves\\radnici.csv";

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var item in radnici)
                {
                    // Write the item to the file
                    var line = new { item.ime, item.prezime, item.iznosNetoPlate, item.radnaPozicija };


                    writer.WriteLine(line);
                }
            }

            return View();
        }

        public async Task<IActionResult> ExportToXLSX()
        {
            if (_context.Radnik == null)
            {
                return Problem("Entity set 'RadniciContext.Radnik' is null.");
            }

            var radnici = from r in _context.Radnik select r;


            string filePath = "C:\\Users\\aleks\\OneDrive\\Desktop\\Radnici-Current - Copy (2)\\Radnici\\Saves\\radnici.xlsx";

            List<Radnik> data = new List<Radnik>();
            foreach (var radnik in radnici)
            {
                data.Add(radnik);
            }
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                // Add a new worksheet to the empty workbook
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Load data into the worksheet, starting from cell A1
                for (int i = 0; i < data.Count; i++)
                {
                    worksheet.Cells[i + 1, 1].Value = data[i].ime;
                    worksheet.Cells[i + 1, 2].Value = data[i].prezime;
                    worksheet.Cells[i + 1, 3].Value = data[i].iznosNetoPlate;
                    worksheet.Cells[i + 1, 4].Value = data[i].radnaPozicija;
                }

                // Save to file
                FileInfo fileInfo = new FileInfo(filePath);
                package.SaveAs(fileInfo);
            }




            return View();
        }

        public async Task<IActionResult> exportToPDF(int? id)
        {
            if (id == null || _context.Radnik == null)
            {
                return NotFound();
            }

            var radnik = await _context.Radnik
                .FirstOrDefaultAsync(m => m.Id == id);
            if (radnik == null)
            {
                return NotFound();
            }


            var document = new Document(PageSize.A4, 50, 50, 25, 25);

            string filePath = "C:\\Users\\aleks\\OneDrive\\Desktop\\Radnici-Current - Copy (2)\\Radnici\\Saves\\radnici.pdf";



            // Create a writer that listens to the document
            PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));

            // Open the document
            document.Open();
            double bruto = (double)radnik.iznosNetoPlate * 1.38;
            var netString = radnik.iznosNetoPlate.ToString();

            // Add content to the document
            document.Add(new Paragraph("Podaci o konvertovanju bruto plate za radnika:"));
            document.Add(new Paragraph($"{radnik.ime}, {radnik.prezime}, {radnik.adresa}, {radnik.radnaPozicija}, {radnik.iznosNetoPlate}"));
            document.Add(new Paragraph("Bruto: " + bruto));
            document.Add(new Paragraph("Osnovica za doprinose: " + bruto));
            document.Add(new Paragraph("PIO 14%:\n " + (bruto * 0.14).ToString("#.##")));
            document.Add(new Paragraph("Zdravstveno osiguranje 5.15%:\n " + (bruto * 0.0515).ToString("#.##")));


            document.Add(new Paragraph("Doprinos za nezaposlenost 0.75%:\n " + (bruto * 0.0075).ToString("#.##")));


            document.Add(new Paragraph("Porez i doprinosi na teret zaposlenog: 29.90\n " + (bruto * 0.2990).ToString("#.##")));
            document.Add(new Paragraph("Neto:\n" + radnik.iznosNetoPlate));
            document.Add(new Paragraph("Doprinosi na teret poslodavca: "));
            document.Add(new Paragraph("PIO 10.00%:\n " + (bruto * 0.10).ToString("#.##")));
            document.Add(new Paragraph("Zdravstveno osiguranje 5.15%\n\t: " + (bruto * 0.0515).ToString("#.##")));

            // Close the document
            document.Close();




            return View(radnik);
        }


        public async Task<IActionResult> convertToUSD(int? id)
        {
            if (id == null || _context.Radnik == null)
            {
                return NotFound();
            }

            var radnik = await _context.Radnik
                .FirstOrDefaultAsync(m => m.Id == id);
            if (radnik == null)
            {
                return NotFound();
            }



            CurrencyConverter currencyConverter = new CurrencyConverter();

            Dictionary<string, string> symbolData = currencyConverter.GetSymbols();

            string fromCurrency = "rsd";

            string toCurrency = "usd";

            double currencyToConvert = (double)radnik.iznosNetoPlate;

            double finalValue = currencyConverter.Convert(fromCurrency, toCurrency, currencyToConvert);




            ViewBag.CurrencyName = "usd";
            ViewBag.FinalValue = finalValue.ToString();



            return View();
        }



        public async Task<IActionResult> convertToEUR(int? id)
        {
            if (id == null || _context.Radnik == null)
            {
                return NotFound();
            }

            var radnik = await _context.Radnik
                .FirstOrDefaultAsync(m => m.Id == id);
            if (radnik == null)
            {
                return NotFound();
            }



            CurrencyConverter currencyConverter = new CurrencyConverter();

            Dictionary<string, string> symbolData = currencyConverter.GetSymbols();

            string fromCurrency = "rsd";

            string toCurrency = "eur";

            double currencyToConvert = (double)radnik.iznosNetoPlate;

            double finalValue = currencyConverter.Convert(fromCurrency, toCurrency, currencyToConvert);




            ViewBag.CurrencyName = "eur";
            ViewBag.FinalValue = finalValue.ToString();



            return View();
        }

        public async Task<IActionResult> sendPDFEmail(int? id)
        {
            if (id == null || _context.Radnik == null)
            {
                return NotFound();
            }

            var radnik = await _context.Radnik
                .FirstOrDefaultAsync(m => m.Id == id);
            if (radnik == null)
            {
                return NotFound();
            }

            var document = new Document(PageSize.A4, 50, 50, 25, 25);

            string filePath = "C:\\Users\\aleks\\OneDrive\\Desktop\\Radnici-Current - Copy (2)\\Radnici\\Saves\\radnici.pdf";



            // Create a writer that listens to the document
            PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));

            // Open the document
            document.Open();
            double bruto = (double)radnik.iznosNetoPlate * 1.38;
            var netString = radnik.iznosNetoPlate.ToString();

            // Add content to the document
            document.Add(new Paragraph("Podaci o konvertovanju bruto plate za radnika:"));
            document.Add(new Paragraph($"{radnik.ime}, {radnik.prezime}, {radnik.adresa}, {radnik.radnaPozicija}, {radnik.iznosNetoPlate}"));
            document.Add(new Paragraph("Bruto: " + bruto));
            document.Add(new Paragraph("Osnovica za doprinose: " + bruto));
            document.Add(new Paragraph("PIO 14%:\n " + (bruto * 0.14).ToString("#.##")));
            document.Add(new Paragraph("Zdravstveno osiguranje 5.15%:\n " + (bruto * 0.0515).ToString("#.##")));


            document.Add(new Paragraph("Doprinos za nezaposlenost 0.75%:\n " + (bruto * 0.0075).ToString("#.##")));


            document.Add(new Paragraph("Porez i doprinosi na teret zaposlenog: 29.90\n " + (bruto * 0.2990).ToString("#.##")));
            document.Add(new Paragraph("Neto:\n" + radnik.iznosNetoPlate));
            document.Add(new Paragraph("Doprinosi na teret poslodavca: "));
            document.Add(new Paragraph("PIO 10.00%:\n " + (bruto * 0.10).ToString("#.##")));
            document.Add(new Paragraph("Zdravstveno osiguranje 5.15%\n\t: " + (bruto * 0.0515).ToString("#.##")));

            document.ToJson();
            // Close the document
            document.Close();

            Logic.Logic logic = new Logic.Logic();


            MemoryStream memoryStream;
            using (var fileStream = new FileStream("C:\\Users\\aleks\\OneDrive\\Desktop\\Radnici-Current - Copy (2)\\Radnici\\Saves\\radnici.pdf", FileMode.Open))
            {
                memoryStream = new MemoryStream();
                fileStream.CopyTo(memoryStream);
                memoryStream.Position = 0; // Reset the memory stream position to the beginning
            }

            IFormFile formFile = new CustomFormFile(memoryStream, "file.pdf", "application/pdf");


            logic.sendEmail("aleksastojanovicz@outlook.com", formFile);



            return View(radnik);
        }




        // GET: Radniks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Radnik == null)
            {
                return NotFound();
            }

            var radnik = await _context.Radnik
                .FirstOrDefaultAsync(m => m.Id == id);
            if (radnik == null)
            {
                return NotFound();
            }

            return View(radnik);
        }

        // GET: Radniks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Radniks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ime,prezime,adresa,iznosNetoPlate,radnaPozicija")] Radnik radnik)
        {
            if (ModelState.IsValid)
            {
                _context.Add(radnik);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(radnik);
        }

        // GET: Radniks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Radnik == null)
            {
                return NotFound();
            }

            var radnik = await _context.Radnik.FindAsync(id);
            if (radnik == null)
            {
                return NotFound();
            }
            return View(radnik);
        }

        // POST: Radniks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ime,prezime,adresa,iznosNetoPlate,radnaPozicija")] Radnik radnik)
        {
            if (id != radnik.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(radnik);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RadnikExists(radnik.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(radnik);
        }

        // GET: Radniks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Radnik == null)
            {
                return NotFound();
            }

            var radnik = await _context.Radnik
                .FirstOrDefaultAsync(m => m.Id == id);
            if (radnik == null)
            {
                return NotFound();
            }

            return View(radnik);
        }

        // POST: Radniks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Radnik == null)
            {
                return Problem("Entity set 'RadniciContext.Radnik'  is null.");
            }
            var radnik = await _context.Radnik.FindAsync(id);
            if (radnik != null)
            {
                _context.Radnik.Remove(radnik);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RadnikExists(int id)
        {
            return (_context.Radnik?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
