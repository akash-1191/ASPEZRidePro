using System.Diagnostics;
using System.Text;
using EZRide_Project.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EZRide_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WhatsAppController : ControllerBase
    {
        [HttpPost("sendWhatsAppMessage")]
        public async Task<IActionResult> SendMessage([FromBody] WhatsAppSendDTO dto)
        {
            try
            {
                var tcs = new TaskCompletionSource<bool>();

                var psi = new ProcessStartInfo
                {
                    FileName = @"C:\Program Files\nodejs\node.exe",
                    Arguments = $"sendMessage.js \"{dto.Message}\" {dto.Phone}",
                    WorkingDirectory = @"D:\Akash\MSC(ICT)\Sem-2\Project_sem2\ASP\EZRide_Project\WhatsAppScripts",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                };

                var process = new Process { StartInfo = psi };

                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(e.Data))
                    {
                        Console.WriteLine("Node Output: " + e.Data);
                        if (e.Data.Contains(" Message Sent Successfully!"))
                            tcs.TrySetResult(true);
                    }
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(e.Data))
                    {
                        Console.WriteLine("Node Error: " + e.Data);
                        tcs.TrySetException(new Exception(e.Data));
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // wait max 60 seconds
                var completed = await Task.WhenAny(tcs.Task, Task.Delay(60000));

                if (completed == tcs.Task && tcs.Task.Result)
                {
                    return Ok(new { status = "success", message = "Message sent via WhatsApp." });
                }
                else if (completed == tcs.Task && tcs.Task.IsFaulted)
                {
                    return StatusCode(500, new { status = "failed", error = tcs.Task.Exception?.Message });
                }
                else
                {
                    return StatusCode(504, new { status = "timeout", error = "WhatsApp message script took too long to respond." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", error = ex.Message });
            }
        }

    }
}
