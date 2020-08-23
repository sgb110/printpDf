using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FastReport;

using FastReport.Utils;

using FastReport.Export.Html;
using System.IO;
using System.Drawing;
using FastReport.Export.PdfSimple;

namespace sample.Controllers
{
    [ApiController]
    [Route("print")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get(int i=0)
        {
            List<string> l = new System.Collections.Generic.List<string>();
            foreach (string s in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                l.Add(s);
            }
            //return this.Ok();
            string mime = "application/html";
            using (MemoryStream stream = new MemoryStream()) // Create a stream for the report
            {
                Report WebReport = new Report();
                WebReport.Load("SimpleList.frx");
                FastReport.Export.Image.ImageExport image=new FastReport.Export.Image.ImageExport();
                image.ImageFormat =FastReport.Export.Image.ImageExportFormat.Tiff;
                
                WebReport.Prepare();
                WebReport.Export(image, stream);
                System.Drawing.Printing.PrintDocument document=new System.Drawing.Printing.PrintDocument();
                // document.DocumentName ="SimpleList.frx" ;
                
                document.PrinterSettings =new System.Drawing.Printing.PrinterSettings(){PrinterName=l.Single(s=> s=="PDF")    };
              // document.DefaultPageSettings.PrinterResolution =document.PrinterSettings.PrinterResolutions[i];
            document.DefaultPageSettings.Margins =new System.Drawing.Printing.Margins(1,1,1,1);
            var paperSize = document.PrinterSettings.PaperSizes.Cast<System.Drawing.Printing.PaperSize>().FirstOrDefault(e => e.PaperName == "A5");
            document.DefaultPageSettings.PaperSize = paperSize;
                document.PrintPage+= new System.Drawing.Printing.PrintPageEventHandler((o,e)=>{
                    // string s = System.Text.Encoding.ASCII.GetString(stream.ToArray());
                     e.Graphics.DrawImage(  Image.FromStream(stream),new Point(0,0)      );
                 });
                document.Print();
                return this.Ok();
                
                //return File(stream.ToArray(), mime);
            }

        }
    }
}
