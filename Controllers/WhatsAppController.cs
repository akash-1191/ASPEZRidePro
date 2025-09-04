using EZRide_Project.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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

                // Properly escape the arguments
                var escapedMessage = dto.Message.Replace("\"", "\\\"");
                var escapedPhone = dto.Phone;

                var psi = new ProcessStartInfo
                {
                    FileName = "node",
                    Arguments = $"sendMessage.js \"{escapedMessage}\" {escapedPhone}",
                    WorkingDirectory = @"D:\Akash\MSC(ICT)\Sem-2\Project_sem2\ASP\EZRide_Project\EZRide_Project\WhatsAppScripts",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var process = new Process { StartInfo = psi };

                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(e.Data))
                    {
                        Console.WriteLine("Node Output: " + e.Data);

                        if (e.Data.Contains("Message sent successfully"))
                            tcs.TrySetResult(true);
                    }
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(e.Data))
                    {
                        Console.WriteLine("Node Error: " + e.Data);

                       
                        if (e.Data.Contains("Help Keep This Project Going") ||
                            e.Data.StartsWith("- ") ||
                            e.Data.Contains("Node.js version") ||
                            e.Data.Contains("Executable path browser") ||
                            e.Data.Contains("Platform: win32") ||
                            e.Data.Contains("You're up to date"))
                        {
                            return; 
                        }

                        tcs.TrySetException(new Exception(e.Data));
                    }
                };
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

               
                var completed = await Task.WhenAny(tcs.Task, Task.Delay(40000));

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
