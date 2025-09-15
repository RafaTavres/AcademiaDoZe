//Rafael dos Santos Tavares
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaDoZe.Domain.Enums
{
    public enum ELogradouroPaisEnum
    {
        [Display(Name = "Brasil")]
        Brasil = 1,
        [Display(Name = "Espanha")]
        Espanha = 2,
        [Display(Name = "EUA")]
        EUA = 3
    }
}
