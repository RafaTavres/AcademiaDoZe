//Rafael dos Santos Tavares
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Domain.Enums;

namespace AcademiaDoZe.Application.Mappings
{
    public static class ColaboradorEnumMappings
    {
        public static ETipoColaboradorEnum ToDomain(this EAppColaboradorTipo appTipo)
        {
            return (ETipoColaboradorEnum)appTipo;
        }
        public static EAppColaboradorTipo ToApp(this ETipoColaboradorEnum domainTipo)
        {
            return (EAppColaboradorTipo)domainTipo;
        }
        public static ETipoVinculoEnum ToDomain(this EAppColaboradorVinculo appVinculo)
        {
            return (ETipoVinculoEnum)appVinculo;
        }
        public static EAppColaboradorVinculo ToApp(this ETipoVinculoEnum domainVinculo)
        {
            return (EAppColaboradorVinculo)domainVinculo;
        }
    }
}