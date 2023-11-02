using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Immutable;
using System.Drawing;
using WebAPI_Azapfy.Models;

namespace WebAPI_Azapfy.Services
{
    public class NotaService
    {
        private readonly HttpClient _httpClient;

        public NotaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://homologacao3.azapfy.com.br/api/ps/");
        }

        public async Task<IEnumerable> GetAllNotasAsync()
        {
            var notas = await _httpClient.GetFromJsonAsync<List<Nota>>("notas");
            return notas;
        }

        //Agrupar as notas por remetente
        public async Task<IEnumerable> GetNotasAgrupadasAsync()
        {
            var notas = await _httpClient.GetFromJsonAsync<List<Nota>>("notas");

            var lstRemetentes = new List<Remetente>();
            foreach (var item in notas)
                lstRemetentes.Add(new Remetente { Cnpj = item.cnpj_remete, Nome = item.nome_remete });

            var notasAgrupadas = notas
                    .GroupBy(nota => nota.cnpj_remete)
                    .Select(group => new
                    {
                        Remetente = lstRemetentes.Find(remetente => remetente.Cnpj == group.Key),
                        Notas = group.ToList()
                    })
                    .ToList();

            return notasAgrupadas;
        }

        public async Task<IEnumerable<Nota>> GetNotasEntregueSemAtrasoAsync()
        {
            var notas = await _httpClient.GetFromJsonAsync<Nota[]>("notas");
            var notasEntSemAtraso = notas.
                Where
                (
                    x => x.dt_entrega != string.Empty 
                    && x.dt_entrega != null
                    && DateTime.Parse(x.dt_entrega) <= DateTime.Parse(x.dt_emis).AddDays(2)
                );

            return notasEntSemAtraso;
        }

        public async Task<IEnumerable<Nota>> GetAllNotasEntregaPendenteAsync()
        {
            var notas = await _httpClient.GetFromJsonAsync<Nota[]>("notas");
            var notasPendentes = notas.
                Where
                (
                    x => x.dt_entrega == string.Empty 
                    || x.dt_entrega == null 
                    && x.status != "COMPROVADO"
                );

            return notasPendentes;
        }

        public async Task<IEnumerable<Nota>> GetNotasEntregaAtrasada()
        {
            var notas = await _httpClient.GetFromJsonAsync<Nota[]>("notas");
            var notasEntEmAtraso = notas.
                Where
                (
                    x => x.dt_entrega != string.Empty 
                    && x.dt_entrega != null
                    && DateTime.Parse(x.dt_entrega) > DateTime.Parse(x.dt_emis).AddDays(2)
                );
            
            return notasEntEmAtraso;
        }

        //Calcular o valor que o remetente irá receber pelo que já foi entregue.
        //Obs.:Para o remetente receber por um produto, é necessário que o documento esteja entregue(status comprovado)
        //e que a entrega tenha sido feita em no máximo dois dias após a sua data de emissão
        public async Task<IEnumerable> GetVlTotalEntregue()
        {
            var notas = await _httpClient.GetFromJsonAsync<Nota[]>("notas");
            var notasEntSemAtraso = notas.
                Where
                (
                    x => x.dt_entrega != string.Empty
                    && x.dt_entrega != null
                    && DateTime.Parse(x.dt_entrega) <= DateTime.Parse(x.dt_emis).AddDays(2)
                );

            var notasAgrupadas = CalcularValorTotalPorRemetente(notasEntSemAtraso, notas);
            return notasAgrupadas;
        }

        //Calcular o valor que o remetente irá receber pelo que ainda não foi entregue
        public async Task<IEnumerable> GetVlTotalEntregaPendente()
        {
            var notas = await _httpClient.GetFromJsonAsync<Nota[]>("notas");
            var notasPendentes = notas.
                Where
                (
                    x => (x.dt_entrega == string.Empty || x.dt_entrega == null) 
                    && x.status != "COMPROVADO"
                );
            var notasAgrupadas = CalcularValorTotalPorRemetente(notasPendentes, notas);
            return notasAgrupadas;
        }

        //Calcular quanto o remetente deixou de receber devido ao atraso na entrega
        public async Task<IEnumerable> GetVlTotalEntregaEmAtraso()
        {
            var notas = await _httpClient.GetFromJsonAsync<Nota[]>("notas");
            var notasEntEmAtraso = notas.
                Where
                (
                    x => x.dt_entrega != string.Empty
                    && x.dt_entrega != null
                    && DateTime.Parse(x.dt_entrega) > DateTime.Parse(x.dt_emis).AddDays(2)
                );

            var notasAgrupadas = CalcularValorTotalPorRemetente(notasEntEmAtraso, notas);
            return notasAgrupadas;
        }

        //Calcular o valor total das notas para cada remetente
        public async Task<IEnumerable> GetVlTotalNotasRemetente()
        {
            var notas = await _httpClient.GetFromJsonAsync<Nota[]>("notas");
            var notasAgrupadas = CalcularValorTotalPorRemetente(notas, notas);
            return notasAgrupadas;
        }

        /// <summary>
        /// Calcular os valores e agrupar por remetente contidos na lista de notas disponibilzados pela API externa.
        /// </summary>
        /// <param name="lstNotas">lista de notas que foram modificadas por alguma validação</param>
        /// <param name="notasApi">lista de notas disponibilzados pela API externa </param>
        /// <returns>lista de notas agrupadas por remetente e seus valores totais</returns>
        public IEnumerable CalcularValorTotalPorRemetente(IEnumerable<Nota> lstNotas, Nota[] notasApi)
        {
            var lstRemetentes = new List<Remetente>();

            //carrega todos os remetentes retornados da api de notas.
            foreach (var item in notasApi)
                lstRemetentes.Add(new Remetente { Cnpj = item.cnpj_remete, Nome = item.nome_remete });

            //Faz o agrupamento por rementente e calcula o valor.
            var notasAgrupadas = lstNotas
                    .GroupBy(nota => nota.cnpj_remete)
                    .Select(group => new
                    {
                        Remetente = lstRemetentes.Find(remetente => remetente.Cnpj == group.Key),
                        ValorTotal = group.Sum(x => Convert.ToDecimal(x.Valor))
                    })
                    .ToList();

            return notasAgrupadas;
        }
    }


}
