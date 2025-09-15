//Rafael dos Santos Tavares
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaDoZe.Domain.Enums
{
    public enum ETipoColaboradorEnum
    {
        [Display(Name = "Administrador")]
        Administrador = 1,
        [Display(Name = "Atendente")]
        Atendente = 2,
        [Display(Name = "Instrutor")]
        Instrutor = 3
    }
}
