//Rafael dos Santos Tavares
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AcademiaDoZe.Application.Enums
{
    public enum EAppColaboradorTipo
    {
        [Display(Name = "Administrador")]
        Administrador = 1,
        [Display(Name = "Atendente")]
        Atendente = 2,
        [Display(Name = "Instrutor")]
        Instrutor = 3
    }
}