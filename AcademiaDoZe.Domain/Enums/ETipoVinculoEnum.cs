//Rafael dos Santos Tavares
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaDoZe.Domain.Enums
{
    public enum ETipoVinculoEnum
    {
        [Display(Name = "CLT")]
        CLT,
        [Display(Name = "Estagio")]
        Estagio
    }
}
