using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaDoZe.Application.Mappings
{
    public static class PlanoMappings
    {
        public static PlanoDTO ToDto(this Plano plano)
        {
            return new PlanoDTO
            {
                Id = plano.Id,
                Tipo = plano.Tipo,
                Descricao = plano.Descricao,
                Preco = plano.Preco,
                DuracaoEmDias = plano.DuracaoEmDias,
                Ativo = plano.Ativo,
            };
        }
        public static Plano ToEntity(this PlanoDTO planoDto)
        {
            return Plano.Criar(
                planoDto.Id,
                planoDto.Tipo,
                planoDto.Descricao,
                planoDto.Preco,
                planoDto.DuracaoEmDias
            );
        }
        public static Plano UpdateFromDto(this Plano plano, PlanoDTO planoDto)
        {
            // Cria uma nova instância do Logradouro com os valores atualizados

            return Plano.Criar(
                planoDto.Id,
                planoDto.Tipo ?? plano.Tipo,
                planoDto.Descricao ?? plano.Descricao,
                planoDto.Preco != default ? planoDto.Preco : plano.Preco,
                planoDto.DuracaoEmDias != default ? planoDto.DuracaoEmDias : plano.DuracaoEmDias
                );
        }
    }
}
