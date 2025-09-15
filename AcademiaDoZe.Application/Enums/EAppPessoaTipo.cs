//Rafael dos Santos Tavares
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AcademiaDoZe.Application.Enums
{
    public enum EAppPessoaTipo
    {
        [Display(Name = "Colaborador")]
        Colaborador,
        [Display(Name = "Aluno")]
        Aluno
    }
}