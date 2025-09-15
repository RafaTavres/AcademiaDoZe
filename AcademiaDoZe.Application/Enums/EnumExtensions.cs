﻿//Rafael dos Santos Tavares
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AcademiaDoZe.Application.Enums
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());

            var attribute = field?.GetCustomAttribute<DisplayAttribute>();
            return attribute?.Name ?? value.ToString();

        }
    }
    // Console.WriteLine( EMatriculaRestricoes.ProblemasRespiratorios.GetDisplayName() );
    // Exibe: Problemas Respiratórios
}