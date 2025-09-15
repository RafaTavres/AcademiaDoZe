using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Domain.Enums;

namespace AcademiaDoZe.Application.Mappings
{
    public static class MatriculaEnumMappings
    {

        public static EMatriculaRestricoesEnum ToDomain(this EAppMatriculaRestricoes appRestricoes)
        {
            return (EMatriculaRestricoesEnum)appRestricoes;
        }
        public static EAppMatriculaRestricoes ToApp(this EMatriculaRestricoesEnum domainRestricoes)
        {
            return (EAppMatriculaRestricoes)domainRestricoes;
        }
    }
}