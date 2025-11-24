using System.Collections.Generic;
using System.Linq;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Domain.Enums;

namespace AcademiaDoZe.Application.Mappings
{
    public static class MatriculaEnumMappings
    {
        public static EMatriculaRestricoesEnum ToDomain(this List<EAppMatriculaRestricoes> appRestricoes)
        {
            if (appRestricoes == null || !appRestricoes.Any())
            {
                return EMatriculaRestricoesEnum.None;
            }

            int combinedValue = appRestricoes.Sum(r => (int)r);

            return (EMatriculaRestricoesEnum)combinedValue;
        }

        public static List<EAppMatriculaRestricoes> ToApp(this EMatriculaRestricoesEnum domainRestricoes)
        {
            var appRestricoes = new List<EAppMatriculaRestricoes>();

            foreach (EAppMatriculaRestricoes appEnumValue in Enum.GetValues(typeof(EAppMatriculaRestricoes)))
            {
                if (appEnumValue == EAppMatriculaRestricoes.None) continue;

                if (domainRestricoes.HasFlag((EMatriculaRestricoesEnum)appEnumValue))
                {
                    appRestricoes.Add(appEnumValue);
                }
            }

            return appRestricoes;
        }
    }
}