using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Neodynamic.SDK.ZPLPrinter;

namespace WebApplication9.Controllers
{
    [ApiController]
    [Route("[controller]")]
    
    public class ValuesController : ControllerBase
    {
        
        [HttpPost]
        public IActionResult Post(Values Weatherin)
        {
            string? PrinterDpi = Weatherin.lstPrinterDpi;
            string? LabelWidth = Weatherin.txtLabelWidth;
            string? LabelHeight = Weatherin.txtLabelHeight;
            string? RibbonColor = Weatherin.cpRibbonColor;
            string? BackColor = Weatherin.cpBackColor;
            string? OutputFormat = Weatherin.lstOutputFormat;
            string? OutputRotate = Weatherin.lstOutputRotate;
            string? Commands = Weatherin.zplCommands;


            var json = new StringBuilder();
            json.Append('{');

            //if (string.IsNullOrEmpty(hola)) throw new ArgumentException("Please specify some ZPL commands.");

            var zplPrinter = new ZPLPrinter("LICENSE OWNER", "LICENSE KEY");

            zplPrinter.Dpi = float.Parse(PrinterDpi.Substring(0, 3));
            //set label size
            zplPrinter.LabelWidth = float.Parse(LabelWidth) * zplPrinter.Dpi;
            if (zplPrinter.LabelWidth <= 0) zplPrinter.LabelWidth = 4;
            zplPrinter.LabelHeight = float.Parse(LabelHeight) * zplPrinter.Dpi;
            if (zplPrinter.LabelHeight <= 0) zplPrinter.LabelHeight = 6;
            //Set Label BackColor
            zplPrinter.LabelBackColor = BackColor;
            //Set Ribbon Color
            zplPrinter.RibbonColor = RibbonColor;
            //Set image or doc format for output rendering 
            zplPrinter.RenderOutputFormat = (RenderOutputFormat)Enum.Parse(typeof(RenderOutputFormat), OutputFormat);
            //Set rotation for output rendering
            zplPrinter.RenderOutputRotation = (RenderOutputRotation)Enum.Parse(typeof(RenderOutputRotation), OutputRotate);
            //Set text encoding
            Encoding enc = Encoding.UTF8;

            var buffer = zplPrinter.ProcessCommands(Commands, enc, false);

            // the buffer variable contains the binary output of the ZPL rendering result
            // The format of this buffer depends on the RenderOutputFormat property setting
            if (buffer != null && buffer.Count > 0)
            {

                if (zplPrinter.RenderOutputFormat == RenderOutputFormat.PDF)
                {
                    json.Append($"\"labelPDF\":\"data:application/pdf;base64,{Convert.ToBase64String(buffer[0])}\"");
                }
                json.Append($",\"renderedElements\":" + zplPrinter.RenderedElementsAsJson);

            }
            else
                throw new ArgumentException("No output available for the specified ZPL commands.");

            json.Append("}");

            return Ok(json);
        }
    }
}
