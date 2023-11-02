using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Cryptography.Xml;
using WebAPI_Azapfy.Models;
using WebAPI_Azapfy.Services;


namespace WebAPI_Azapfy.Controllers
{
    [Route("api/")]
    [ApiController]
    public class NotaController : ControllerBase
    {
        private readonly NotaService _notaService;

        public NotaController(NotaService notaService)
        {
            _notaService = notaService;
        }

        [HttpGet("notas")]
        public async Task<IActionResult> GetAllNotas()
        {
            try
            {
                var notas = await _notaService.GetAllNotasAsync();
                return Ok(notas);

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("notas/agrupadas/remetente")]
        public async Task<IActionResult> GetNotasAgrupadas()
        {
            try
            {
                var notas = await _notaService.GetNotasAgrupadasAsync();
                return Ok(notas);

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("notas/entregue")]
        public async Task<IActionResult> GetAllNotasEntregueSemAtraso()
        {
            try
            {
                var notas = await _notaService.GetNotasEntregueSemAtrasoAsync();
                return Ok(notas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("notas/entrega/pendente")]
        public async Task<IActionResult> GetNotasPendentes()
        {
            try
            {
                var notas = await _notaService.GetAllNotasEntregaPendenteAsync();
                return Ok(notas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        [HttpGet("notas/entrega/atrasada")]
        public async Task<IActionResult> GetNotasEntregaAtrasada()
        {
            try
            {
                var notas = await _notaService.GetNotasEntregaAtrasada();
                return Ok(notas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        [HttpGet("notas/vltotal/rementente")]
        public async Task<IActionResult> GetVlTotalNotasRemetente()
        {
            try
            {
                var notas = await _notaService.GetVlTotalNotasRemetente();
                return Ok(notas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        [HttpGet("notas/vltotal/entregue")]
        public async Task<IActionResult> GetNotasVlTotalEntregue()
        {
            try
            {
                //Obs.:Para o remetente receber por um produto, é necessário que o documento esteja entregue(status comprovado)
                //e que a entrega tenha sido feita em no máximo dois dias após a sua data de emissão
                var notas = await _notaService.GetVlTotalEntregue();
                return Ok(notas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        [HttpGet("notas/vltotal/pendentes")]
        public async Task<IActionResult> GetNotasVlTotalPendentes()
        {
            try
            {
                var notas = await _notaService.GetVlTotalEntregaPendente();
                return Ok(notas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("notas/vltotal/atrasada")]
        public async Task<IActionResult> GetNotasVlTotalAtrasada()
        {
            try
            {
                var notas = await _notaService.GetVlTotalEntregaEmAtraso();
                return Ok(notas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

    }
}
